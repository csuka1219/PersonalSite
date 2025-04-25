namespace Microsoft.Extensions.DependencyInjection;
public static class ConfigureServices
{
    public static IServiceCollection AddWebAPIServices(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder => builder
                    .WithOrigins("http://89.116.106.135:8088",
                                 "https://localhost:7080")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
        });

        return services;
    }
}
