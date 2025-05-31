using SpotifyRandomPlaylists.TTY;

namespace SpotifyRandomPlaylists;

public static class Program
{
    public static void Main(string[] args)
    {
        var consolePrompt = new ConsolePrompt();
        consolePrompt.Start();
    }
}
