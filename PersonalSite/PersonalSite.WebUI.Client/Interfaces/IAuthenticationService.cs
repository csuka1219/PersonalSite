using PersonalSite.Application.DTOs.Account;

namespace PersonalSite.WebUI.Client.Interfaces;

public interface IAuthenticationService
{
    Task Login(AuthenticationRequest authenticationRequest);
    Task Logout();
    Task<string> RefreshToken();
}