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
                if (!req.Query.TryGetValue("_code", out var code) && !req.Query.TryGetValue("code", out code))
                {
                    return new BadRequestObjectResult("no code found");
                }

                if (!code.Any() || string.IsNullOrEmpty(code))
                {
                    return new BadRequestObjectResult("invalid code");
                }

                if (!req.Query.TryGetValue("return_uri", out var return_uri) || !return_uri.Any() || string.IsNullOrEmpty(code))
                {
                    return new BadRequestObjectResult("invalid return URI");
                }

                
                var request = new OauthTokenRequest(_gitHubInfo.ClientID, _gitHubInfo.ClientSecret, code.FirstOrDefault());
                var client = FreshGitHubClient();
                var token = await client.Oauth.CreateAccessToken(request);


                req.HttpContext.Response.Cookies.Append(_gitHubInfo.GitHubAuthCookie, token.AccessToken, new CookieOptions { HttpOnly=true });
                req.HttpContext.Response.Cookies.Append(_gitHubInfo.GitHubAuthScopeCookie, string.Join(" ", token.Scope), new CookieOptions { HttpOnly = true });

                return new RedirectResult(return_uri.FirstOrDefault(), false);
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
            if (!req.Cookies.TryGetValue(_gitHubInfo.GitHubAuthCookie, out string authToken))
            {
                return new OkObjectResult(Enumerable.Empty<Repo>());
            }

            try
            {
                var client = FreshGitHubClient();

                client.Credentials = new Credentials(authToken);
                var allRepos = await client.Repository.GetAllForCurrent();
                return new OkObjectResult(allRepos.Select(r => new Repo
                {
                    Id = r.Id,
                    Name = r.FullName,
                    Description = r.Description,
                    Url = r.Url,
                    StargazersCount = r.StargazersCount,
                    Language = r.Language,
                    UpdatedAt = r.UpdatedAt
                }));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"{ex.Message}\r\n{ex.StackTrace}\r\n{authToken}");
            }
        }
    }
}

