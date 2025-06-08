using System;
using AppData;
using SpotifyAPI.Web;
using SpotifyRandomPlaylists.Backend;
using SpotifyRandomPlaylists.Shared;

namespace SpotifyRandomPlaylists.TTY;

public class ConsolePrompt
{
    private readonly Playlist _playlist = new();
    private readonly AppDataManager _appDataManager = new();

    public async Task Start()
    {
        // Take input params
        var nameInput = ValidateInput<string>("Enter playlist name: ");
        var genreInput = ValidateInput<List<string>>("Enter genre(s): ");
        var songNumInput = ValidateInput<int>("Enter song number: ");
        
        // Only assign to props if not empty
        if (nameInput.Length > 0)
        {
            _playlist.Name = nameInput;
        }

        if (genreInput.Count > 0)
        {
            _playlist.Genres = genreInput;
        }

        if (songNumInput > 0)
        {
            _playlist.SongNumber = songNumInput;
        }
        
        // TODO: Talk to api and create playlist
        await GeneratePlaylist(_playlist);
    }

    public static async Task Login()
    {
        var spotifyAuth = new SpotifyAuth();
        await spotifyAuth.Login();
    }

    private static T ValidateInput<T>(string promptText)
    {
        bool validInput;
        T value;
        
        do
        {
            validInput = true;
            value = default(T)!;
            try
            {
                Console.Write(promptText);
                if (typeof(T) == typeof(string))
                {
                    var inputValue = Console.ReadLine();
                    if (inputValue is null)
                    {
                        throw new ArgumentNullException(inputValue, "Cannot convert nulltype to string");
                    }
                    value = (T)Convert.ChangeType(inputValue, typeof(T));
                }
                else if (typeof(T) == typeof(List<string>))
                {
                    var inputValue = Console.ReadLine();
                    if (inputValue is null)
                    {
                        throw new ArgumentNullException(inputValue, "Cannot convert nulltype to string[]");
                    }
                    
                    var inputList = inputValue.Split(",").Select(v => v.Trim()).ToList();
                    value = (T)Convert.ChangeType(inputList, typeof(T));
                }
                else if (typeof(T) == typeof(int))
                {
                    var tempValue = Console.ReadLine();
                    if (tempValue is null)
                    {
                        throw new ArgumentNullException(tempValue, "Cannot convert nulltype to string");
                    }
                    
                    // input can be empty, then do nothing else
                    if (tempValue.Length > 0)
                    {
                        var inputValue = int.Parse(tempValue);
                        
                        // number cannot be below 0
                        if (inputValue < 0)
                        {
                            throw new ArgumentOutOfRangeException(nameof(inputValue), inputValue, "Cannot take a negative number");
                        }
                        value = (T)Convert.ChangeType(inputValue, typeof(T));
                    }
                    else
                    {
                        // we put zero to signify no input was made
                        value = (T)Convert.ChangeType(0, typeof(T));
                    }
                }
                else
                {
                    throw new ArgumentException($"Type {typeof(T)} is not supported");
                }
            }
            catch (Exception e)
            {
                validInput = false;
            }
        } while (!validInput);

        return value;
    }

    private async Task<string> GetAccessToken()
    {
        var httpClient = new HttpClient();
        var uri = new Uri($"{_appDataManager.AppData.BaseUri}/token");
        var response = await httpClient.GetAsync(uri);
        var token = await response.Content.ReadAsStringAsync();
        return token;
    }

    private async Task GeneratePlaylist(Playlist playlist)
    {
        var token = await GetAccessToken();
        if (token is null or "")
        {
            Console.WriteLine("No access token provided, Exiting...");
            Environment.Exit(1);
        }

        try
        {
            var api = new SpotifyClient(token);
            var res = await api.UserProfile.Current();
            Console.WriteLine("My email: " + res.Email);
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
}