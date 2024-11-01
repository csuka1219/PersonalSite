using Microsoft.AspNetCore.Components;
using PersonalSite.Application.DTOs.Account;
using Radzen;

namespace PersonalSite.WebUI.Client.Pages;

public partial class Login
{
    [Inject]
    private NavigationManager _navigationManager { get; set; }
    async void OnLogin(LoginArgs args)
    {
        AuthenticationRequest authenticationRequest = new AuthenticationRequest
        {
            Email = args.Username,
            Password = args.Password
        };
        await AuthenticationService.Login(authenticationRequest);
        _navigationManager.NavigateTo("/");
    }
}
