using Services.D.Core.Interfaces;
using Services.D.Core.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;

namespace Services.D.Core.Repositories
{
    public class JwtFactory : IJwtFactory
    {
        JwtSettings _settings;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;


        ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters)
        {
            var prinipal = _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);



            if (!(securityToken is JwtSecurityToken jwtSecurityToken)
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return null;



            return prinipal;
        }
        public JwtFactory(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }
        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            return ValidateToken(token, new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _settings.SigningCredentials.Key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false
            });
        }

    }
}
