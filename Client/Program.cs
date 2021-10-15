using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorApp.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace BlazorApp.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            var baseAddress = builder.Configuration["BaseAddress"] ?? builder.HostEnvironment.BaseAddress;

            builder.Services.AddSingleton<IState>(new State());

            builder.Services.AddScoped<CustomDelegatingHandler>();

            builder.Services
                .AddHttpClient("Client", c => c.BaseAddress = new Uri(baseAddress))
                .AddHttpMessageHandler<CustomDelegatingHandler>();

            builder.Services.AddTransient(s => {
                var client = s.GetService<IHttpClientFactory>().CreateClient("Client");
                return client;
            });

            builder.Services.AddSingleton<IGitHubInfo>(new GitHubInfo());
            await builder.Build().RunAsync();
        }
    }
}