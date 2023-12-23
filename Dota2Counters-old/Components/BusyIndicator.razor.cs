using Microsoft.AspNetCore.Components;

namespace Dota2Counters.Components;

public partial class BusyIndicator
{
    [Parameter][EditorRequired] public bool IsBusy { get; set; }
    [Parameter] public string? Classes { get; set; } 
}