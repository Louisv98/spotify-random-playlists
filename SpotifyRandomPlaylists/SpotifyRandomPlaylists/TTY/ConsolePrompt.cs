using System;
using SpotifyAPI.Web;
using SpotifyRandomPlaylists.Backend;
using SpotifyRandomPlaylists.Shared;

namespace SpotifyRandomPlaylists.TTY;

public class ConsolePrompt
{
    private readonly Playlist _playlist = new();

    public void Start()
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
    }

    public void Login()
    {
        var spotifyAuth = new SpotifyAuth();
        spotifyAuth.Login();
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
}