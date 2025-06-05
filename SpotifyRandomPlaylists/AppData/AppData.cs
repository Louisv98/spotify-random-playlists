using System.Text.Json;
using Swan.Formatters;

namespace AppData;

public class AppData
{
    public string? ClientId { get; set; }
    public string? RedirectUri { get; set; }
    public string? Verifier { get; set; }
    public string? AccessToken { get; set; }
}
