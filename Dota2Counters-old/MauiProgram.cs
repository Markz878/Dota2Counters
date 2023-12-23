using Dota2Counters.Shared;
using System.Text.Json;

namespace Dota2Counters;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddSingleton<AnimationStateIndicator>();
        builder.Services.AddSingleton(s =>
        {
            using Stream avatarsStream = FileSystem.OpenAppPackageFileAsync("Avatars.txt").Result;
            Dictionary<string, string>? heroAvatarCache = JsonSerializer.Deserialize<Dictionary<string, string>>(avatarsStream);
            using Stream avatarsLargeStream = FileSystem.OpenAppPackageFileAsync("AvatarsLarge.txt").Result;
            Dictionary<string, string>? heroAvatarLargeCache = JsonSerializer.Deserialize<Dictionary<string, string>>(avatarsLargeStream);
            ArgumentNullException.ThrowIfNull(heroAvatarCache);
            ArgumentNullException.ThrowIfNull(heroAvatarLargeCache);
            return new AvatarCache() { Avatars = heroAvatarCache, LargeAvatars = heroAvatarLargeCache };
        });
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif
        return builder.Build();
    }
}
