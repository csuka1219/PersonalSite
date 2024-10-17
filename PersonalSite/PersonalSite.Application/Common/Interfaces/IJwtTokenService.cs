namespace PersonalSite.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(string username, IList<string> roles);
}
