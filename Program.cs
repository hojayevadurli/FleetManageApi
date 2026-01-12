using FleetManage.Api.Data;
using FleetManage.Api.Interfaces;
using FleetManage.Api.Services; // Ensure this is present
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on all interfaces
if (builder.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("https://0.0.0.0:7297", "http://0.0.0.0:5139");
}

// ----------------------------
// CORS
// ----------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.SetIsOriginAllowed(origin => true) // Allow any origin
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials() // Allow credentials if needed
              .WithExposedHeaders("Location");
    });
});

// ----------------------------
// Database & Identity
// ----------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// ----------------------------
// Auth (JWT)
// ----------------------------
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var keyBytes = Encoding.UTF8.GetBytes(jwtSection["Key"]!);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSection["Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(2)
    };
});

builder.Services.AddAuthorization();

// ----------------------------
// Services & DI
// ----------------------------
builder.Services.AddScoped<ITenantContext, HttpTenantContext>();
// Email Service Selection
builder.Services.AddScoped<IEmailSender>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    // If we have an SMTP host, use the real sender. Otherwise dev console.
    if (!string.IsNullOrEmpty(config["Email:Host"]))
    {
        return new SmtpEmailSender(config, sp.GetRequiredService<ILogger<SmtpEmailSender>>());
    }
    
    return new ConsoleEmailSender(sp.GetRequiredService<ILogger<ConsoleEmailSender>>());
});
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// -- NEW: NHTSA Recall Service --
builder.Services.AddHttpClient<INhtsaRecallService, NhtsaRecallService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// ----------------------------
// Swagger
// ----------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FleetManage.Api", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Paste JWT token only (no 'Bearer ' prefix).",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

// ----------------------------
// Pipeline
// ----------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();