using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Dota2Counters.Components;

public partial class Dialog
{
    [Parameter][EditorRequired] public bool IsVisible { get; set; }
    [Parameter][EditorRequired] public string? Text { get; set; }
    [Parameter][EditorRequired] public string? Classes { get; set; }
    [Parameter][EditorRequired] public EventCallback<MouseEventArgs> YesCommand { get; set; }
    [Parameter][EditorRequired] public EventCallback<MouseEventArgs> NoCommand { get; set; }
    [Parameter][EditorRequired] public EventCallback<MouseEventArgs> CancelCommand { get; set; }
}