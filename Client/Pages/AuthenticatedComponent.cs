using BlazorApp.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Client.Pages
{
    public class AuthenticatedComponent : ComponentBase
    {
        [Inject]
        private IJSRuntime JsRuntime { get; set; }
        [Inject]
        private IGitHubInfo GitHubInfo { get; set; }

        private async Task<string> GetCookie(string cookieName)
        {
            return await JsRuntime.InvokeAsync<string>("InteropFunctions.ReadCookie", cookieName);
        }

        public async Task<bool> IsAuthenticated()
        {
            return !string.IsNullOrWhiteSpace(await GetCookie(GitHubInfo.GitHubAuthCookie));
        }
    }
}
