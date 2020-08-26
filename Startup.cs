using Agstt.Repositories;
using Agstt.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Agstt.Startup))]
namespace Agstt
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services
                .AddHttpClient()
                .AddLogging()
                .AddSingleton<IBlobRepository, BlobRepository>()
                .AddSingleton<ITableRepository, TableRepository>()
                .AddScoped<IResponseMetaService, ResponseMetaService>()
                .AddScoped<IResponsePayloadService, ResponsePayloadService>();
        }
    }
}