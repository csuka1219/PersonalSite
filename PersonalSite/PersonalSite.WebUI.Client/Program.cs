using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PersonalSite.WebUI.Client.Authentication;
using PersonalSite.WebUI.Client.Interfaces;
using PersonalSite.WebUI.Client.Services;
using Radzen;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace PersonalSite.WebUI.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddRadzenComponents();

            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(builder.Configuration["ApiUrl"]!)
            }
            .EnableIntercept(sp));

            builder.Services.AddHttpClientInterceptor();

            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IHttpService, HttpService>();
            builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            builder.Services.AddScoped<IFileSystemService, FileSystemService>();
            builder.Services.AddScoped<HttpInterceptorService>();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddBlazoredLocalStorage();

            await builder.Build().RunAsync();
        }
    }
}
