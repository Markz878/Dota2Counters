using Dota2Counters.Shared;
using Dota2CountersLibrary;
using Microsoft.AspNetCore.Components;

namespace Dota2Counters.Components;

public partial class EditHeroCountersCard
{
    [Parameter][EditorRequired] public HeroCounterData? Hero { get; set; }
    [Parameter][EditorRequired] public IEnumerable<string>? AllHeroesList { get; set; }
    [Inject] public required AvatarCache AvatarCache { get; set; }
}