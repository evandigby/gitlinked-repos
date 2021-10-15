namespace BlazorApp.Shared
{
    public interface IGitHubInfo
    {
        string GitHubAuthCookie { get; }
        string GitHubAuthScopeCookie { get; }

        string ClientID { get; set; }
        string ClientSecret { get; set; }
    }
}
