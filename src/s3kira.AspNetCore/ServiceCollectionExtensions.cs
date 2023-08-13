using System;
using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace s3kira.AspNetCore;

public static class ServiceCollectionExtensions
{
    private const string DefaultHttpClientName = "s3kira";
        
    public static IHttpClientBuilder AddS3Kira(this IServiceCollection services, S3KiraSettings settings)
    {
        var builder = services.AddHttpClient(DefaultHttpClientName)
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(2)
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
        
        services.AddSingleton(settings);
        services.AddSingleton(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient(DefaultHttpClientName);
            var s3KiraSettings = sp.GetRequiredService<S3KiraSettings>();
            return new S3Kira(s3KiraSettings, client);
        });

        return builder;
    }
}