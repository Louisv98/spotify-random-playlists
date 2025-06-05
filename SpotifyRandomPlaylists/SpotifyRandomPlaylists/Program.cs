using SpotifyRandomPlaylists.TTY;

namespace SpotifyRandomPlaylists;

public static class Program
{
    public static void Main(string[] args)
    {
        var consolePrompt = new ConsolePrompt();

        switch (args.Length)
        {
            case 1:
            {
                if (args[0] == "login")
                {
                    consolePrompt.Login();
                }

                break;
            }
            case 0:
                consolePrompt.Start();
                break;
            default:
                Console.WriteLine("Usage: SpotifyRandomPlaylists [login]");
                break;
        }
    }
}
