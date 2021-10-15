using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Octokit;
using System.Linq;
using BlazorApp.Shared;

namespace BlazorApp.Api
{
    public class GitHub
    {
        private readonly IGitHubInfo _gitHubInfo;

        public GitHub(IGitHubInfo gitHubInfo)
        {
            _gitHubInfo = gitHubInfo;
        }

        private static GitHubClient FreshGitHubClient() => new GitHubClient(new ProductHeaderValue("select-repositories"));

        [FunctionName("Login")]
        public Task<IActionResult> Login(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "auth/login")] HttpRequest req,
            ILogger log)
        {
            var redirectUri = req.Query["redirect_uri"].SingleOrDefault();

            var redirectUriBuilder = new UriBuilder(redirectUri)
            {
                Query = req.QueryString.ToString()
            };

            var request = new OauthLoginRequest(_gitHubInfo.ClientID)
            {
                RedirectUri = redirectUriBuilder.Uri,
                Scopes = { "user", "repo" },
            };

            var client = FreshGitHubClient();
            var loginUrl = client.Oauth.GetGitHubLoginUrl(request);

            return Task.FromResult<IActionResult>(new RedirectResult(loginUrl.ToString(), false));
        }

        [FunctionName("Authorize")]
        public async Task<IActionResult> Authorize(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "auth/authorize")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var code = req.Query["code"].FirstOrDefault();
                var returnUri = req.Query["return_uri"].SingleOrDefault();

                return new OkObjectResult(_gitHubInfo);

                if (string.IsNullOrEmpty(code))
                    return new RedirectResult("/");


                var request = new OauthTokenRequest(_gitHubInfo.ClientID, _gitHubInfo.ClientSecret, code);
                var client = FreshGitHubClient();
                var token = await client.Oauth.CreateAccessToken(request);


                req.HttpContext.Response.Cookies.Append(_gitHubInfo.GitHubAuthCookie, token.AccessToken);
                req.HttpContext.Response.Cookies.Append(_gitHubInfo.GitHubAuthScopeCookie, string.Join(" ", token.Scope));

                return new RedirectResult(returnUri, false);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        [FunctionName("repos")]
        public async Task<IActionResult> Repos(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "github/repos")] HttpRequest req,
            ILogger log)
        {
            var client = FreshGitHubClient();

            if (!req.Headers.TryGetValue("Authorization", out var AuthHeader))
            {
                return new OkObjectResult(Enumerable.Empty<Repo>());
            }

            var authToken = AuthHeader.Single().Split(" ")[1];

            client.Credentials = new Credentials(authToken);
            var allRepos = await client.Repository.GetAllForCurrent();
            return new OkObjectResult(allRepos.Select(r => new Repo { 
                Id = r.Id,
                Name = r.FullName ,
                Description = r.Description,
                Url = r.Url,
                StargazersCount = r.StargazersCount,
                Language = r.Language,
                UpdatedAt = r.UpdatedAt
            }));
        }
    }
}

