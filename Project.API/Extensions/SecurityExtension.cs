using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Project.Domain.Entities.Identity_Auth;
using Project.Infrastructure.Data.Contexts;
using System.Text;

namespace Project.API.Extensions
{
    public static class SecurityExtension
    {
        public static IServiceCollection RegisterSecurityService(this IServiceCollection services, IConfiguration configuration)
        {
            #region Identity Configuration
            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 6;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            #endregion

            #region JWT configuration
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt =>
            {
                byte[] key = Encoding.UTF8.GetBytes(configuration["AppSettings:JwtConfig:Secret"]);
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["AppSettings:JwtConfig:ValidIssuer"],

                    ValidateAudience = true,
                    ValidAudience = configuration["AppSettings:JwtConfig:ValidAudience"],

                    ValidateLifetime = true,
                    RequireExpirationTime = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ClockSkew = TimeSpan.Zero
                };

                jwt.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        var response = new
                        {
                            success = false,
                            message = "You must be logged in to access this resource.",
                            error = new
                            {
                                code = "UNAUTHORIZED_ACCESS",
                                type = "UnauthorizedAccessException"
                            },
                            traceId = context.HttpContext.TraceIdentifier,
                            timestamp = DateTime.UtcNow
                        };

                        return context.Response.WriteAsJsonAsync(response);
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        var response = new
                        {
                            success = false,
                            message = "You do not have permission to access this resource.",
                            error = new
                            {
                                code = "FORBIDDEN_ACCESS",
                                type = "ForbiddenAccessException"
                            },
                            traceId = context.HttpContext.TraceIdentifier,
                            timestamp = DateTime.UtcNow
                        };
                        return context.Response.WriteAsJsonAsync(response);
                    }
                };
            });
            #endregion
            return services;
        }
    }
}