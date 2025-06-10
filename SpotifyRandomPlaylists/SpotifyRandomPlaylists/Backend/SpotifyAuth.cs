using System.Buffers.Text;
using System.Diagnostics;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography;
using System.Text;
using AppData;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using Swan.Formatters;

namespace SpotifyRandomPlaylists.Backend;


/// <summary>
/// Spotify API auth management.
///
/// For now only works with one user (me, the developer) since
/// we are in dev mode. Must be in extended quota mode to work with other users without hassle.
///
/// Taken from https://johnnycrazy.github.io/SpotifyAPI-NET/docs/authorization_code
/// </summary>
public class SpotifyAuth
{
    private readonly AppDataManager _appDataManager = new();

    public async Task Login()
    {
        // Generates a secure random verifier of length 120 and its challenge
        var (verifier, challenge) = PKCEUtil.GenerateCodes(120);
        
        // Make sure "http://localhost:5189/callback" is in your applications redirect URIs!
        var loginRequest = new LoginRequest(
            new Uri(_appDataManager.AppData.RedirectUri),
            _appDataManager.AppData.ClientId,
            LoginRequest.ResponseType.Code
        )
        {
            CodeChallengeMethod = "S256",
            CodeChallenge = challenge,
            Scope = [Scopes.PlaylistReadPrivate, Scopes.PlaylistReadCollaborative]
        };
        _appDataManager.AppData.Verifier = verifier;
        _appDataManager.UpdateJson();
        
        // Send verifier to local server
        var httpClient = new HttpClient();
        var verifierUri = new Uri($"{_appDataManager.AppData.BaseUri}/send-verifier?verifier={verifier}");
        await httpClient.PutAsync(
            verifierUri, 
            null);
        
        var uri = loginRequest.ToUri().ToString();
        OpenUriInDefaultBrowser(uri);
    }
    
    private static void OpenUriInDefaultBrowser(string uri)
    {
        if (string.IsNullOrWhiteSpace(uri))
        {
            Console.WriteLine("Error: URI cannot be null or empty.");
            return;
        }

        try
        {
            Console.WriteLine($"Attempting to open: {uri}");

            // This is the most common and robust way for .NET Core / .NET 5+
            // It intelligently handles different operating systems.
            Process.Start(new ProcessStartInfo
            {
                FileName = uri,
                UseShellExecute = true // Crucial for opening URLs (and files)
            });
        }
        catch (System.ComponentModel.Win32Exception noBrowser)
        {
            // This exception occurs if no default browser is set or the URI is malformed.
            Console.WriteLine($"Error: No default browser or invalid URI. {noBrowser.Message}");
        }
        catch (Exception ex)
        {
            // Catch any other unexpected errors
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }
    
    public async Task<string> GetAccessToken()
    {
        var httpClient = new HttpClient();
        var uri = new Uri($"{_appDataManager.AppData.BaseUri}/token");
        var response = await httpClient.GetAsync(uri);
        var token = await response.Content.ReadAsStringAsync();
        return token;
    }
}