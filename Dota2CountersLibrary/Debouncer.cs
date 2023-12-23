using System.Timers;

namespace Dota2CountersLibrary;

public sealed class Debouncer : IDisposable
{
    private readonly Func<Task> callback;
    private readonly System.Timers.Timer timer;

    public Debouncer(TimeSpan delay, Func<Task> callback)
    {
        this.callback = callback;
        timer = new(delay);
        timer.Elapsed += Timer_Elapsed;
    }

    private async void Timer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        await callback.Invoke();
        timer.Stop();
    }

    public void Debounce()
    {
        timer.Stop();
        timer.Start();
    }

    public void Dispose()
    {
        timer.Dispose();
    }
}
