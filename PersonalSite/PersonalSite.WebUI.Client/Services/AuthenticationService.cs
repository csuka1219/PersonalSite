using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using PersonalSite.Application.DTOs.Account;
using PersonalSite.WebUI.Client.Interfaces;

namespace PersonalSite.WebUI.Client.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IHttpService _httpService;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider AuthStateProvider;
    private readonly NavigationManager _navigationManager;

    public AuthenticationService(IHttpService httpService, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider, NavigationManager navigationManager)
    {
        _httpService = httpService;
        _localStorage = localStorage;
        AuthStateProvider = authStateProvider;
        _navigationManager = navigationManager;
    }

    public async Task Login(AuthenticationRequest authenticationRequest)
    {
        AuthenticationResponse authenticationResponse = await _httpService.Post<AuthenticationResponse>("Account/authenticate", authenticationRequest);
        await _localStorage.SetItemAsync("authToken", authenticationResponse.JWToken);
        await _localStorage.SetItemAsync("refreshToken", authenticationResponse.RefreshToken);
        await AuthStateProvider.GetAuthenticationStateAsync();
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("refreshToken");
        _navigationManager.NavigateTo("login");
    }
    public async Task Test()
    {
        string res = await _httpService.Post<string>("Account/test", null);
    }

    public async Task<string> RefreshToken()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        var refreshToken = await _localStorage.GetItemAsync<string>("refreshToken");

        var tokenDto = new RefreshTokenDto { Token = token, RefreshToken = refreshToken };
        //var bodyContent = new StringContent(tokenDto, Encoding.UTF8, "application/json");

        var result = await _httpService.Post<AuthenticationResponse>("token/refresh", tokenDto);

        await _localStorage.SetItemAsync("authToken", result.JWToken);
        await _localStorage.SetItemAsync("refreshToken", result.RefreshToken);

        return result.JWToken;
    }
}
