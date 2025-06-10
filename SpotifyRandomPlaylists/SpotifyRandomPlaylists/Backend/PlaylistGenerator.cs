using AppData;
using SpotifyAPI.Web;
using SpotifyRandomPlaylists.Shared;

namespace SpotifyRandomPlaylists.Backend;

public class PlaylistGenerator
{
    private readonly SpotifyAuth _spotifyAuth = new();
    private readonly AppDataManager _appDataManager = new();
    
    public async Task Generate(Playlist playlist)
    {
        var token = await _spotifyAuth.GetAccessToken();
        if (token is null or "")
        {
            Console.WriteLine("No access token provided, Exiting...");
            Environment.Exit(1);
        }

        try
        {
            var api = new SpotifyClient(token);
            var user = await api.UserProfile.Current();
            var createRequest = new PlaylistCreateRequest(playlist.Name);
            var newPlaylist = await api.Playlists.Create(user.Id, createRequest);
            
            // Add random songs per defined genres
            AddSongs(newPlaylist);
        }
        catch (APIException e)
        {
            Console.WriteLine(e.Message);
            Environment.Exit(1);
        }
        catch (Exception e)
        {
            Console.WriteLine("An unexpected error occured");
            Environment.Exit(1);
        }
    }

    private void AddSongs(FullPlaylist newPlaylist)
    {
        
    }

    private async Task GetSongs()
    {
        
    }
}