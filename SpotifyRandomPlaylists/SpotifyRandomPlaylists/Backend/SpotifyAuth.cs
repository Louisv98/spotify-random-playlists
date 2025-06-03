using System.Buffers.Text;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography;
using System.Text;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;

namespace SpotifyRandomPlaylists.Backend;


/// <summary>
/// Spotify API auth management.
///
/// For now only works with one user (me, the developer) since
/// we are in dev mode. Must be in extended quota mode to work with other users without hassle.
///
/// Taken from https://johnnycrazy.github.io/SpotifyAPI-NET/docs/authorization_code
/// </summary>
public static class SpotifyAuth
{
    private static EmbedIOAuthServer _server;
    
    private const string RedirectUri = "http://127.0.0.1:5543/callback";
    
    private static string GetClientIdFromFile()
    {
        using var sr = new StreamReader("Backend/client_id");
        return sr.ReadToEnd();
    }

    private static string GetSecretIdFromFile()
    {
        using var sr = new StreamReader("Backend/secret_id");
        return sr.ReadToEnd();
    }

    public static async Task Start()
    {
        // Make sure "http://localhost:5543/callback" is in your spotify application as redirect uri!
        _server = new EmbedIOAuthServer(new Uri(RedirectUri), 5543);
        await _server.Start();

        _server.AuthorizationCodeReceived += OnAuthorizationCodeReceived;
        _server.ErrorReceived += OnErrorReceived;

        var request = new LoginRequest(_server.BaseUri, GetClientIdFromFile(), LoginRequest.ResponseType.Code)
        {
            Scope = new List<string> { Scopes.UserReadEmail }
        };
        BrowserUtil.Open(request.ToUri());
    }

    private static async Task OnAuthorizationCodeReceived(object sender, AuthorizationCodeResponse response)
    {
        await _server.Stop();

        var config = SpotifyClientConfig.CreateDefault();
        var tokenResponse = await new OAuthClient(config).RequestToken(
            new AuthorizationCodeTokenRequest(
                GetClientIdFromFile(), GetSecretIdFromFile(), response.Code, new Uri(RedirectUri)
            )
        );

        await using var sr = new StreamWriter("Backend/access_token");
        await sr.WriteAsync(tokenResponse.AccessToken);
    }

    private static async Task OnErrorReceived(object sender, string error, string state)
    {
        Console.WriteLine($"Aborting authorization, error received: {error}");
        await _server.Stop();
    }
}