using System.Reflection;
using System.Text.Json;
using Swan.Formatters;

namespace AppData;

public class AppDataManager
{
    private readonly string _jsonPath;
    private JsonSerializerOptions _options;

    public AppData AppData { get; private init; }

    public AppDataManager()
    {
        _jsonPath = GetJsonPath();
        AppData = new AppData();
        InitFromJson();
    }

    private static string GetJsonPath()
    {
        var assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var buildDirectory = Path.GetDirectoryName(assemblyLocation);
        
        if (buildDirectory is null) throw new ArgumentNullException(nameof(buildDirectory));
        return Path.Combine(buildDirectory, "app_data.json");
    }
    
    private void InitFromJson()
    {
        InitJsonSerializerOptions();
        var jsonString = File.ReadAllText(_jsonPath);
        var data = JsonSerializer.Deserialize<AppData>(jsonString, _options);

        if (data == null)
        {
            throw new NullReferenceException("Could not read AppData");
        }
        AppData.ClientId = data.ClientId;
        AppData.BaseUri = data.BaseUri;
        AppData.RedirectUri = data.RedirectUri;
        AppData.Verifier = data.Verifier;
        AppData.AccessToken = data.AccessToken;
        AppData.RefreshToken = data.RefreshToken;
    }

    private void InitJsonSerializerOptions()
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = true, // Makes the JSON output human-readable (pretty-printed)
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Converts C# PascalCase to JSON camelCase
        };
    }

    public void UpdateJson()
    {
        var jsonString = JsonSerializer.Serialize(AppData, _options);
        File.WriteAllText(_jsonPath, jsonString);
    }
}