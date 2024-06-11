using System;
using System.Text;
using System.Threading.Tasks;
using Services.D.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Services.D.Core.Helpers
{
    public static class JWTExtensions
    {
        public static JwtSettings ToJWTSettings(this IConfigurationSection jwtSection)
        {
            var sekretKey = jwtSection[nameof(JwtSettings.SecretKey)];
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(sekretKey));

            return new JwtSettings()
            {
                SecretKey = sekretKey,
                Issuer = jwtSection[nameof(JwtSettings.Issuer)],
                AccessTokenExpireMinute = int.Parse(jwtSection[nameof(JwtSettings.AccessTokenExpireMinute)]),
                SessionTokenExopireMinute = int.Parse(jwtSection[nameof(JwtSettings.SessionTokenExopireMinute)]),
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                SigningKey = signingKey
            };
        }
        public static void ConfigureJwtSettings(this IServiceCollection services, JwtSettings settings)
        {
            services.Configure<JwtSettings>(config =>
            {
                config.Issuer = settings.Issuer;
                config.SigningCredentials = settings.SigningCredentials;
                config.AccessTokenExpireMinute = settings.AccessTokenExpireMinute;
                config.SessionTokenExopireMinute = settings.SessionTokenExopireMinute;
                config.SecretKey = settings.SecretKey;
            });
        }
        public static void AddJWTAuthentication(this IServiceCollection services, JwtSettings settings)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = settings.SigningKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                //ValidIssuer = settings.Issuer,
                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(config =>
            {
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.ClaimsIssuer = settings.Issuer;
                options.TokenValidationParameters = tokenValidationParameters;
                options.SaveToken = true;

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Access-Control-Allow-Headers", "*");
                            context.Response.Headers.Add("Access-Control-Expose-Headers", "*");
                            context.Response.Headers.Add("Access-Token-Expired", "true");
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}
