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
    private readonly AppDataManager _appDataManager;

    public SpotifyAuth()
    {
        var solutionRootDir = GetSolutionRootPath();
        _appDataManager = new AppDataManager(solutionRootDir);
    }

    public void Login()
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
    
    private static string GetSolutionRootPath()
    {
        string currentDirectory = AppContext.BaseDirectory;
        // On monte dans l'arborescence jusqu'à trouver un dossier qui contient un fichier .sln
        // Ou un marqueur connu de la racine de la solution (ex: un dossier "SharedData")
        while (!string.IsNullOrEmpty(currentDirectory))
        {
            if (Directory.GetFiles(currentDirectory, "*.sln").Any() ||
                Directory.Exists(Path.Combine(currentDirectory, "SharedData")))
            {
                return currentDirectory;
            }
            DirectoryInfo parent = Directory.GetParent(currentDirectory);
            if (parent == null) break; // Sortir si plus de parent
            currentDirectory = parent.FullName;
        }
        // Fallback si la racine de la solution n'est pas trouvée (moins robuste)
        return Path.Combine(AppContext.BaseDirectory, "../../../../"); // Exemple pour remonter 4 niveaux
    }
}