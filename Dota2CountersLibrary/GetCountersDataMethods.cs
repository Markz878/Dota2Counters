using Microsoft.Playwright;
using System.Text.RegularExpressions;

namespace Dota2CountersLibrary;
public class GetCountersDataMethods
{
    public static async Task<List<HeroCounterData>> ParseHeroCounters(bool addBadAgainstToGoodAgainstCounters, string avatarsLocation, IProgress<string>? progress = null, IProgress<string>? error = null, CancellationToken token = default)
    {
        using IPlaywright playwright = await Playwright.CreateAsync();
        await using IBrowser browser = await playwright.Chromium.LaunchAsync();
        IPage page = await browser.NewPageAsync();
        List<HeroCounterData> heroCounters = new();
        foreach (string avatarPath in Directory.EnumerateFiles(avatarsLocation).Where(x => !x.EndsWith("Name.png")))
        {
            token.ThrowIfCancellationRequested();
            string hero = Path.GetFileNameWithoutExtension(avatarPath).Replace("_", " ");
            progress?.Report(hero);
            IResponse? response = await page.GotoAsync($"https://dota2.fandom.com/wiki/{hero.Replace(" ", "_")}/Counters");
            ArgumentNullException.ThrowIfNull(response);
            if (!response.Ok)
            {
                error?.Report(hero);
                continue;
            }
            await ParseHeroCountersFromPage(page, heroCounters, hero);
        }
        if (addBadAgainstToGoodAgainstCounters)
        {
            foreach (HeroCounterData hero in heroCounters)
            {
                foreach (string badAgainst in hero.BadAgainst)
                {
                    HeroCounterData? badAgainstHero = heroCounters.FirstOrDefault(x => x.Hero == badAgainst);
                    badAgainstHero?.GoodAgainst.Add(hero.Hero);
                }
            }
        }
        return heroCounters;
    }

    private static async Task ParseHeroCountersFromPage(IPage page, List<HeroCounterData> heroCounters, string hero)
    {
        string html = await page.InnerHTMLAsync(".mw-parser-output");
        int badAgainstIndex = html.LastIndexOf("Bad against...");
        int goodAgainstIndex = html.LastIndexOf("Good against...", StringComparison.InvariantCultureIgnoreCase);
        int worksWellIndex = html.LastIndexOf("Works well with...");
        string badAgainstArea = html[badAgainstIndex..goodAgainstIndex];
        string goodAgainstArea = html[goodAgainstIndex..worksWellIndex];
        string worksWellWithArea = html[worksWellIndex..];
        MatchCollection badAgainstMatches = Regex.Matches(badAgainstArea, @"div style=""margin-bottom:5px; box-shadow:0px 0px 2px 4px red;""><a href=""/wiki/([\w-]+)"" title=""([a-zA-Z-\s]+)"">");
        MatchCollection goodAgainstMatches = Regex.Matches(goodAgainstArea, @"div style=""margin-bottom:5px; box-shadow:0px 0px 2px 4px chartreuse;""><a href=""/wiki/([\w-]+)"" title=""([a-zA-Z-\s]+)"">");
        MatchCollection worksWellWithMatches = Regex.Matches(worksWellWithArea, @"div style=""margin-bottom:5px; box-shadow:0px 0px 2px 4px skyblue;""><a href=""/wiki/([\w-]+)"" title=""([a-zA-Z-\s]+)"">");
        heroCounters.Add(new HeroCounterData(hero,
            new SortedSet<string>(badAgainstMatches.Select(x => x.Groups[2].Value)),
            new SortedSet<string>(goodAgainstMatches.Select(x => x.Groups[2].Value)),
            new SortedSet<string>(worksWellWithMatches.Select(x => x.Groups[2].Value))));
    }

    private static async Task ParseHeroCountersFromPage2(IPage page, List<HeroCounterData> heroCounters, string hero)
    {
        ILocator badAgainstElements = page.Locator(@"div[style=""margin-bottom:5px; box-shadow:0px 0px 2px 4px red;""] > a");
        SortedSet<string> badAgainstHeroes = new();
        foreach (IElementHandle item in await badAgainstElements.ElementHandlesAsync())
        {
            string? heroName = await item.GetAttributeAsync("title");
            ArgumentNullException.ThrowIfNull(heroName);
            badAgainstHeroes.Add(heroName);
        }
        ILocator goodAgainstElements = page.Locator(@"div[style=""margin-bottom:5px; box-shadow:0px 0px 2px 4px chartreuse;""] > a");
        SortedSet<string> goodAgainstHeroes = new();
        foreach (IElementHandle item in await goodAgainstElements.ElementHandlesAsync())
        {
            string? heroName = await item.GetAttributeAsync("title");
            ArgumentNullException.ThrowIfNull(heroName);
            goodAgainstHeroes.Add(heroName);
        }
        ILocator goodWithElements = page.Locator(@"div[style=""margin-bottom:5px; box-shadow:0px 0px 2px 4px skyblue;""] > a");
        SortedSet<string> goodWithHeroes = new();
        foreach (IElementHandle item in await goodWithElements.ElementHandlesAsync())
        {
            string? heroName = await item.GetAttributeAsync("title");
            ArgumentNullException.ThrowIfNull(heroName);
            goodWithHeroes.Add(heroName);
        }
        heroCounters.Add(new HeroCounterData(hero, badAgainstHeroes, goodAgainstHeroes, goodWithHeroes));
    }
}
