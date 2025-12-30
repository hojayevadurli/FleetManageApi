using System.ComponentModel.DataAnnotations;

namespace FleetManage.Api.DTOs
{
    public static class AuthDtos
    {
        // Company sign-up (creates tenant + first user)
        public record RegisterCompanyDto(
            [Required, StringLength(128)] string CompanyName,
            [Required, StringLength(128)] string FullName,
            [Required, EmailAddress, StringLength(256)] string Email,
            [Required, MinLength(6), StringLength(128)] string Password
        );

        // Simpler user register (if you also support invite/register user-only)
        public record RegisterDto(
            [Required, EmailAddress, StringLength(256)] string Email,
            [Required, MinLength(6), StringLength(128)] string Password
        );

        public record LoginDto(
            [Required, EmailAddress] string Email,
            [Required] string Password
        );

        public record ForgotPasswordDto(
            [Required, EmailAddress] string Email
        );

        public record ResetPasswordDto(
            [Required, EmailAddress] string Email,
            [Required] string Token,
            [Required, MinLength(6), StringLength(128)] string NewPassword
        );

        // For authenticated “change password”
        public record UpdatePasswordDto(
            [Required] string CurrentPassword,
            [Required, MinLength(6), StringLength(128)] string NewPassword
        );
    }
}
