using BlazorApp.Shared;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorApp.Client
{
    public class CustomDelegatingHandler : DelegatingHandler
    {
        private readonly IJSRuntime JsRuntime;
        private readonly IGitHubInfo GitHubInfo;

        public CustomDelegatingHandler(IJSRuntime jSRuntime, IGitHubInfo gitHubInfo) : base()
        {
            JsRuntime = jSRuntime;
            GitHubInfo = gitHubInfo;
        }
        private async Task<string> GetCookie(string cookieName)
        {
            return await JsRuntime.InvokeAsync<string>("InteropFunctions.ReadCookie", cookieName);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await GetCookie(GitHubInfo.GitHubAuthCookie);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
