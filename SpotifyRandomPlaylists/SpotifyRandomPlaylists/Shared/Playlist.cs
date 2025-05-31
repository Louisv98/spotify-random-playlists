namespace SpotifyRandomPlaylists.Shared;

public class Playlist
{
    public string Name { get; set; }

    public List<string> Genres { get; set; }

    public int SongNumber { get; set; }

    public Playlist()
    {
        var curDate = DateTime.UtcNow;
        Name = $"Random Playlist {curDate.Year}/{curDate.Month}/{curDate.Day}";
        Genres = ["metal"];
        SongNumber = 50;
    }
}