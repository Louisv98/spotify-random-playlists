using System.Text.Json;
using Swan.Formatters;

namespace AppData;

public class AppDataManager
{
    private string _jsonPath;
    private JsonSerializerOptions _options;
    public AppData AppData { get; set; }

    public AppDataManager(string solutionRootPath)
    {
        AppData = new AppData();
        Initialize(solutionRootPath);
        InitFromJson();
    }
    
    private void Initialize(string solutionRootPath)
    {
        _jsonPath = Path.Combine(solutionRootPath, "SharedData", "data.json");
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
        AppData.RedirectUri = data.RedirectUri;
        AppData.Verifier = data.Verifier;
        AppData.AccessToken = data.AccessToken;
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