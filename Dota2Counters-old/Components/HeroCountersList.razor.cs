using Dota2Counters.Shared;
using Microsoft.AspNetCore.Components;

namespace Dota2Counters.Components;

public partial class HeroCountersList
{
    private string addHero = "";

    [Inject] public required AvatarCache Avatars { get; set; }

    [Parameter]
    [EditorRequired]
    public string? Header { get; set; }

    [Parameter]
    [EditorRequired]
    public string? HeaderColor { get; set; }

    [Parameter]
    [EditorRequired]
    public SortedSet<string>? HeroList { get; set; }

    [Parameter]
    [EditorRequired]
    public IEnumerable<string>? AllHeroesList { get; set; }

    private void AddHeroToList()
    {
        if (!string.IsNullOrWhiteSpace(addHero) && HeroList is not null && AllHeroesList is not null && AllHeroesList.Contains(addHero))
        {
            HeroList.Add(addHero);
            addHero = "";
        }
    }
}