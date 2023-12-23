using Dota2Counters.Models;
using Dota2CountersLibrary;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using static Dota2CountersLibrary.GetCountersDataMethods;

namespace Dota2Counters.Pages;

public sealed partial class EditCounterData : IDisposable
{
    private string searchText = "";
    private string progress = "";
    private List<HeroCounterData>? heroCounters;
    private bool isBusy;
    private bool animateEnter;
    private bool animateExit;
    private bool showCancel;
    private bool showDialog;
    private CancellationTokenSource? cts;
    private HeroCounterData[] filteredHeroCounters = Array.Empty<HeroCounterData>();
    private Debouncer? debouncer;
    [Inject] public required NavigationManager Navigation { get; set; }
    [Inject] public required AnimationStateIndicator AnimationState { get; set; }

    protected override async Task OnInitializedAsync()
    {
        animateEnter = AnimationState.Animate;
        isBusy = true;
        AnimationState.Animate = false;
        try
        {
            using FileStream stream = File.OpenRead(GlobalConstants.HeroCountersDataLocation);
            heroCounters = await JsonSerializer.DeserializeAsync<List<HeroCounterData>>(stream);
            filteredHeroCounters = filteredHeroCounters = heroCounters is null ? Array.Empty<HeroCounterData>() : heroCounters.ToArray();
        }
        catch (FileNotFoundException)
        {
            progress = "Could not find hero counters data file, fetch and save remote data first.";
        }
        catch (Exception ex)
        {
            progress = ex.Message;
        }
        await Task.Delay(1000);
        animateEnter = false;
        isBusy = false;
        debouncer = new(TimeSpan.FromSeconds(1), SetFilteredHeroCounters);
    }

    private void SetSearchText()
    {
        debouncer?.Debounce();
    }

    private async Task SetFilteredHeroCounters()
    {
        await InvokeAsync(() =>
        {
            filteredHeroCounters = heroCounters is null ? Array.Empty<HeroCounterData>() : heroCounters.Where(x => x.Hero.StartsWith(searchText, StringComparison.OrdinalIgnoreCase)).ToArray();
            StateHasChanged();
        });
    }

    private async Task Back()
    {
        animateExit = true;
        AnimationState.Animate = true;
        await Task.Delay(900);
        Navigation.NavigateTo("/");
    }

    private async Task Save()
    {
        isBusy = true;
        progress = "";
        try
        {
            File.Delete(GlobalConstants.HeroCountersDataLocation);
            using FileStream file = File.OpenWrite(GlobalConstants.HeroCountersDataLocation);
            await JsonSerializer.SerializeAsync(file, heroCounters);
            progress = "Counters saved!";
        }
        catch (Exception ex)
        {
            progress = ex.Message;
        }
        finally
        {
            isBusy = false;
        }
    }

    private void ShowFetchDialog()
    {
        isBusy = true;
        showDialog = true;
    }

    private async Task FetchOriginal(bool addBadAgainstCounters)
    {
        cts = new CancellationTokenSource();
        showDialog = false;
        showCancel = true;
        progress = "";
        try
        {
            heroCounters = await ParseHeroCounters(addBadAgainstCounters, GlobalConstants.AvatarsLocation,
                new Progress<string>(x => { progress = "Fetching hero " + x; StateHasChanged(); }),
                new Progress<string>(x => { progress = $"Failed to get page for hero {x}!"; StateHasChanged(); }),
                cts.Token);
        }
        catch (OperationCanceledException)
        {
            progress = "Fetch cancelled by user.";
        }
        catch (Exception ex)
        {
            progress = ex.Message;
        }
        finally
        {
            cts.Dispose();
            isBusy = false;
            showCancel = false;
        }
    }

    private void Cancel()
    {
        isBusy = false;
        showDialog = false;
        cts?.Cancel();
    }

    public void Dispose()
    {
        cts?.Dispose();
        debouncer?.Dispose();
    }
}
