using BlazorApp.Shared;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Octokit;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: WebJobsStartup(typeof(BlazorApp.Api.StartUp))]
namespace BlazorApp.Api
{

    public class StartUp : IWebJobsStartup
    {
        private readonly string clientId = Environment.GetEnvironmentVariable("GitHubClientID");
        private readonly string clientSecret = Environment.GetEnvironmentVariable("GitHubClientSecret");

        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.TryAddSingleton<IGitHubInfo>(new GitHubInfo
            {
                ClientID = clientId,
                ClientSecret = clientSecret
            });
        }
    }
}
