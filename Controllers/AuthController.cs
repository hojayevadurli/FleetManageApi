using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FleetManage.Api.Data;          // AppDbContext, AppUser, Tenant
using FleetManage.Api.DTOs;          // AuthDtos.*
using FleetManage.Api.Interfaces;    // IEmailSender
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static FleetManage.Api.DTOs.AuthDtos;

namespace FleetManage.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IJwtTokenService _jwt;
        private readonly IEmailSender _email;

        public AuthController(
            AppDbContext db,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IJwtTokenService jwt,
            IEmailSender email)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwt = jwt;
            _email = email;
        }

        // ===================== SIGN UP (Company/Tenant + First Admin User) =====================
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterCompanyDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 0) Check duplicate email
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
                return BadRequest(new { errors = new[] { "Email already in use" } });

            // 1) Create tenant
            var tenant = new Tenant 
            { 
                Name = dto.CompanyName,
                IndustryId = dto.IndustryId,
                Phone = dto.Phone,
                Email = dto.Email // Set company email to admin's email
            };
            _db.Tenants.Add(tenant);
            await _db.SaveChangesAsync();

            // 2) Create first user within that tenant
            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true,
                TenantId = tenant.Id
            };

            var create = await _userManager.CreateAsync(user, dto.Password);
            if (!create.Succeeded)
            {
                // rollback orphan tenant
                _db.Tenants.Remove(tenant);
                await _db.SaveChangesAsync();
                return BadRequest(new { errors = create.Errors.Select(e => e.Description) });
            }

            // Optional: mark first user as Admin via role claim
            await _userManager.AddClaimsAsync(user, new[]
            {
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim("fullName", dto.FullName)
            });

            // 3) Issue JWT
            var token = _jwt.CreateToken(user);

            return Ok(new
            {
                token,
                tenantId = tenant.Id,
                email = user.Email
            });
        }

        // ===================== LOGIN =====================
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null)
                return Unauthorized();

            var ok = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!ok.Succeeded)
                return Unauthorized();

            var token = _jwt.CreateToken(user);

            return Ok(new
            {
                token,
                tenantId = user.TenantId,
                email = user.Email
            });
        }

        // ===================== FORGOT PASSWORD (send reset link/token) =====================
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null)
                return Ok(); // don't reveal existence

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encoded = System.Net.WebUtility.UrlEncode(token);

            // Link to your frontend reset page: /reset?email=...&token=...
            // Use Origin header if present to link back to the correct frontend host
            var baseUrl = Request.Headers.Origin.FirstOrDefault() ?? $"{Request.Scheme}://{Request.Host}";
            var resetUrl = $"{baseUrl}/reset?email={dto.Email}&token={encoded}";

            await _email.SendAsync(
                dto.Email,
                "Reset your FleetManage password",
                $"Click to reset: <a href=\"{resetUrl}\">{resetUrl}</a><br/><br/>" +
                $"(Dev token): <pre>{encoded}</pre>");

            return Ok();
        }

        // ===================== RESET PASSWORD (consume token) =====================
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null)
                return BadRequest("Invalid request");

            // Assuming the token comes in correctly via JSON (already string), NO NEED to decode again if it's identical
            // If the client sent the RAW token (not url encoded), we use it directly.
            // If the client sent the URL ENCODED token, we decode.
            // Usually, standard is to send the raw token via JSON.
            
            // However, if the client took the query param (which is encoded) and sent it directly, it might still be encoded.
            // Let's try direct first. If that fails, we might consider other options. 
            // BUT: The token generated by Identity often contains '+' which UrlDecode turns to space ' '.
            // If we UrlDecode here, and the client ALREADY decoded it (by reading query param), we break it.
            // Safest: Use dto.Token as is.
            
            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok();
        }

        // ===================== UPDATE PASSWORD (authenticated) =====================
        [HttpPost("update-password")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Unauthorized();

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok();
        }

        // ===================== LOGOUT (stateless JWT: no-op) =====================
        [HttpPost("logout")]
        [AllowAnonymous]
        public IActionResult Logout() => NoContent();

        // ===================== ME (protected) =====================
        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var email = User.FindFirstValue(ClaimTypes.Email)
                      ?? User.FindFirstValue(JwtRegisteredClaimNames.Email);

            var tenantId = User.FindFirstValue("tenantId");

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(tenantId))
                return Unauthorized(new { message = "Invalid or missing claims." });

            return Ok(new
            {
                id,
                email,
                tenantId
            });
        }
    }
}
