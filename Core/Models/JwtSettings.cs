using Microsoft.IdentityModel.Tokens;
using System;

namespace Services.D.Core.Models
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public int AccessTokenExpireMinute { get; set; }
        public int SessionTokenExopireMinute { get; set; }
        public SigningCredentials SigningCredentials { get; set; }
        public SymmetricSecurityKey SigningKey { get; set; }
        public DateTime Expiration => IssuedAt.Add(ValidFor);
        public DateTime NotBefore => DateTime.UtcNow;
        public DateTime IssuedAt => DateTime.UtcNow;
        public TimeSpan ValidFor => TimeSpan.FromMinutes(AccessTokenExpireMinute);
    }
}
