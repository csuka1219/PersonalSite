using Microsoft.AspNetCore.Components;
using PersonalSite.WebUI.Client.Services;

namespace PersonalSite.WebUI.Client.Pages;
public partial class Home : IDisposable
{
    [Inject]
    public HttpInterceptorService Interceptor { get; set; } = default!;

    protected async override Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Interceptor.RegisterEvent();
    }
    public void Dispose() => Interceptor.DisposeEvent();
}
