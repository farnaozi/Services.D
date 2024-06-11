using System.Security.Claims;

namespace Services.D.Core.Interfaces
{
    public interface IJwtFactory
    {
        ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}
