using Microsoft.AspNetCore.Components;
using PersonalSite.WebUI.Client.Interfaces;
using System.Net;
using System.Net.Http.Headers;
using Toolbelt.Blazor;

namespace PersonalSite.WebUI.Client.Services;

public class HttpInterceptorService
{
    private readonly HttpClientInterceptor _interceptor;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly NavigationManager _navManager;

    public HttpInterceptorService(HttpClientInterceptor interceptor, IRefreshTokenService refreshTokenService, NavigationManager navManager)
    {
        _interceptor = interceptor;
        _refreshTokenService = refreshTokenService;
        _navManager = navManager;
    }

    public void RegisterEvent()
    {
        _interceptor.BeforeSendAsync += InterceptBeforeHttpAsync;
        _interceptor.AfterSend += InterceptResponse!;
    }

    public async Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e)
    {
        var absPath = e.Request.RequestUri!.AbsolutePath;

        if (!absPath.Contains("token") && !absPath.Contains("account"))
        {
            var token = await _refreshTokenService.TryRefreshToken();

            if (!string.IsNullOrEmpty(token))
            {
                e.Request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
            }
        }
    }

    private void InterceptResponse(object sender, HttpClientInterceptorEventArgs e)
    {
        string message = string.Empty;
        if (!e.Response.IsSuccessStatusCode)
        {
            var statusCode = e.Response.StatusCode;
            switch (statusCode)
            {
                case HttpStatusCode.NotFound:
                    _navManager.NavigateTo("/404");
                    message = "The requested resorce was not found.";
                    break;
                case HttpStatusCode.Unauthorized:
                    _navManager.NavigateTo("/unauthorized");
                    message = "User is not authorized";
                    break;
                default:
                    _navManager.NavigateTo("/500");
                    message = "Something went wrong, please contact Administrator";
                    break;
            }
            throw new Exception(message);
        }
    }

    public void DisposeEvent() => _interceptor.BeforeSendAsync -= InterceptBeforeHttpAsync;
}