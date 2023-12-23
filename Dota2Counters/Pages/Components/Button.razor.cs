using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Dota2Counters.Pages.Components;

public partial class Button
{
    [Parameter] public bool IsBusy { get; set; }
    [Parameter][EditorRequired] public string? Text { get; set; }
    [Parameter] public string? Icon { get; set; }
    [Parameter][EditorRequired] public EventCallback<MouseEventArgs> Command { get; set; }
    [Parameter] public string? Classes { get; set; }
}