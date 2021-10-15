namespace BlazorApp.Shared
{
    public class GitHubInfo : IGitHubInfo
    {

        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string GitHubAuthCookie => "x-github-auth";
        public string GitHubAuthScopeCookie => "x-github-claims";
    }
}
