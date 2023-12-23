using Dota2Counters.Models;
using Dota2CountersLibrary;
using Microsoft.AspNetCore.Components;

namespace Dota2Counters.Pages.Components;

public partial class EditHeroCountersCard
{
    [Parameter][EditorRequired] public HeroCounterData? Hero { get; set; }
    [Parameter][EditorRequired] public IEnumerable<string>? AllHeroesList { get; set; }
    [Inject] public required AvatarCache AvatarCache { get; set; }
}