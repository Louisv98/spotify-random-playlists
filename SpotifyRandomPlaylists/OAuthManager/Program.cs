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
    // Note that we use the verifier calculated above!
    var initialResponse = await new OAuthClient().RequestToken(
        new PKCETokenRequest(
            appDataManager.AppData.ClientId, 
            code, 
            new Uri(appDataManager.AppData.RedirectUri), 
            appDataManager.AppData.Verifier)
    );

    appDataManager.AppData.AccessToken = initialResponse.AccessToken;
    appDataManager.UpdateJson();
});

app.Run();