using SpotifyAPI.Web;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();
var appDataManager = new AppData.AppDataManager();

// app.UseHttpsRedirection();

app.MapPut("/send-verifier", (string verifier) =>
{
    appDataManager.AppData.Verifier = verifier;
    appDataManager.UpdateJson();
});

app.MapGet("/callback", async (string code) =>
{
    if (appDataManager.AppData.AccessToken is null or "")
    {
        // Note that we use the verifier calculated above!
        var initialResponse = await new OAuthClient().RequestToken(
            new PKCETokenRequest(
                appDataManager.AppData.ClientId, 
                code, 
                new Uri(appDataManager.AppData.RedirectUri), 
                appDataManager.AppData.Verifier)
        );

        appDataManager.AppData.AccessToken = initialResponse.AccessToken;
        appDataManager.AppData.RefreshToken = initialResponse.RefreshToken;
    }
    else
    {
        var response = await new OAuthClient().RequestToken(
            new PKCETokenRefreshRequest(
                appDataManager.AppData.ClientId,
                appDataManager.AppData.RefreshToken)
        );
        
        appDataManager.AppData.AccessToken = response.AccessToken;
    }
    appDataManager.UpdateJson();
});

app.MapGet("/token", () => appDataManager.AppData.AccessToken);

app.Run();