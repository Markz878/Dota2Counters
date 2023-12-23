using Dota2Counters.Models;
using Dota2CountersLibrary;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.AspNetCore.Components;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Drawing;
using System.Text.Json;
using static Dota2CountersLibrary.GetHeroCounterMethods;
using static Dota2CountersLibrary.ImageMethods;

namespace Dota2Counters.Pages;
public partial class Index
{
    private string? screenshot;
    private string searchText = "";
    private string progress = "";
    private List<HeroCounterData>? heroCounters;
    private List<HeroCounterResult>? heroCounterResults;
    private bool isBusy;
    private bool animateEnter;
    private bool animateExit;
    [Inject] public required NavigationManager Navigation { get; set; }
    [Inject] public required AnimationStateIndicator AnimationState { get; set; }
    [Inject] public required AvatarCache AvatarCache { get; set; }

    protected override async Task OnInitializedAsync()
    {
        animateEnter = AnimationState.Animate;
        AnimationState.Animate = false;
        await Task.Delay(1000);
        animateEnter = false;
    }

    private async Task GetRecommendedHeroes()
    {
        isBusy = true;
        Bitmap? bitmap = null;
        Image<Bgr, byte>? source = null;
        progress = "";
        StateHasChanged();
        await Task.Delay(10);
        int retryCount = 0;
        int? playerPosition = null;
        try
        {
            while (playerPosition == null && retryCount < 5)
            {
                bitmap = TakeScreenshotBitmap();
                source = bitmap.ToImage<Bgr, byte>();
                playerPosition = GetPlayerPosition(source, GlobalConstants.AvatarsLocation);
                if (playerPosition == null)
                {
                    if (retryCount == 4)
                    {
                        UpdateImageSource(bitmap);
                    }
                    source.Dispose();
                    bitmap.Dispose();
                    retryCount++;
                    progress = "Retry " + retryCount;
                    StateHasChanged();
                    await Task.Delay(100);
                }
            }
            ArgumentNullException.ThrowIfNull(bitmap);
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(playerPosition);
            string[] heroes = await Task.Run(() => NamePickedHeroes(bitmap, source, GlobalConstants.AvatarsLocation));
            UpdateImageSource(bitmap);
            (List<string> allies, List<string> enemies) = GetAlliesAndEnemies(playerPosition.Value, heroes);
            if (heroCounters is null)
            {
                try
                {
                    using FileStream stream = File.OpenRead(GlobalConstants.HeroCountersDataLocation);
                    heroCounters = await JsonSerializer.DeserializeAsync<List<HeroCounterData>>(stream);
                }
                catch (FileNotFoundException)
                {
                    throw new Exception("Could not find hero counters data file, go edit hero counter data and try again.");
                }

            }
            ArgumentNullException.ThrowIfNull(heroCounters);
            heroCounterResults = GetHeroCounterResults(heroCounters, allies, enemies);
            progress = "Counters loaded";
        }
        catch (ArgumentNullException)
        {
            progress = "Could not load counter info from screen.";
        }
        catch (Exception ex)
        {
            progress = ex.Message;
        }
        finally
        {
            bitmap?.Dispose();
            source?.Dispose();
            isBusy = false;
        }
    }

    private void UpdateImageSource(Bitmap bitmap)
    {
        using MemoryStream ms = new();
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        BitmapImage image = new();
        screenshot = $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}";
    }

    private async Task EditCounterData()
    {
        animateExit = true;
        AnimationState.Animate = true;
        StateHasChanged();
        await Task.Delay(800);
        Navigation.NavigateTo("/editcounterdata");
    }
}
