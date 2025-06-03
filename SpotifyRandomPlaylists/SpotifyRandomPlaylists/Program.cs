using SpotifyRandomPlaylists.TTY;

namespace SpotifyRandomPlaylists;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var consolePrompt = new ConsolePrompt();
        await consolePrompt.Start();
    }
}
