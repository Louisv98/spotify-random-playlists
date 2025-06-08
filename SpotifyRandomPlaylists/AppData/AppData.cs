using System.Text.Json;
using Swan.Formatters;

namespace AppData;

public class AppData
{
    public string? ClientId { get; set; }
    public string? BaseUri { get; set; }
    public string? RedirectUri { get; set; }
    public string? Verifier { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        var appDataObj = obj as AppData;
        return appDataObj != null
               && ClientId == appDataObj.ClientId
               && BaseUri == appDataObj.BaseUri
               && RedirectUri == appDataObj.RedirectUri
               && Verifier == appDataObj.Verifier
               && AccessToken == appDataObj.AccessToken
               && RefreshToken == appDataObj.RefreshToken;
    }

    protected bool Equals(AppData other)
    {
        return ClientId == other.ClientId 
               && BaseUri == other.BaseUri 
               && RedirectUri == other.RedirectUri 
               && Verifier == other.Verifier 
               && AccessToken == other.AccessToken
               && RefreshToken == other.RefreshToken;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ClientId, BaseUri, RedirectUri, Verifier, AccessToken, RefreshToken);
    }
}
