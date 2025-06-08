using SpotifyRandomPlaylists.TTY;

namespace SpotifyRandomPlaylists;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var consolePrompt = new ConsolePrompt();

        switch (args.Length)
        {
            case 1:
            {
                if (args[0] == "login")
                {
                    await ConsolePrompt.Login();
                }

                break;
            }
            case 0:
                await consolePrompt.Start();
                break;
            default:
                Console.WriteLine("Usage: SpotifyRandomPlaylists [login]");
                break;
        }
    }
}
