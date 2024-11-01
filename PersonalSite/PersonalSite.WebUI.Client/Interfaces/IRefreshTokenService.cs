namespace PersonalSite.WebUI.Client.Interfaces;

public interface IRefreshTokenService
{
    Task<string> TryRefreshToken();
}
