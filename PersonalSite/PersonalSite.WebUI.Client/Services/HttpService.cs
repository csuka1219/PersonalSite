using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PersonalSite.WebUI.Client.Interfaces;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace PersonalSite.WebUI.Client.Services;

public class HttpService : IHttpService
{
    private HttpClient _httpClient;
    private NavigationManager _navigationManager;
    private ILocalStorageService _localStorageService;

    public HttpService(
        HttpClient httpClient,
        NavigationManager navigationManager,
        ILocalStorageService localStorageService,
        IConfiguration configuration
    )
    {
        _httpClient = httpClient;
        _navigationManager = navigationManager;
        _localStorageService = localStorageService;
    }

    public async Task<T> Get<T>(string uri)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        return await sendRequest<T>(request);
    }
    public async Task<HttpResponseMessage> GetBasic(string uri)
    {
        return await _httpClient.GetAsync(uri);
    }

    public async Task<T> Post<T>(string uri, object value)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
        return await sendRequest<T>(request);
    }

    public async Task<T> Delete<T>(string uri)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, uri);
        return await sendRequest<T>(request);
    }

    // helper methods

    private async Task<T> sendRequest<T>(HttpRequestMessage request)
    {
        using var response = await _httpClient.SendAsync(request);

        // auto logout on 401 response
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _navigationManager.NavigateTo("logout");
            return default!;
        }

        // throw exception on error response
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            throw new Exception(error!["message"]);
        }

        var res = await response.Content.ReadFromJsonAsync<T>();
        if (res == null)
            return default!;

        return res;

    }
}