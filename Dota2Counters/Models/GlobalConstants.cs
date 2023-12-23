namespace Dota2Counters.Models;
public static class GlobalConstants
{
    public static readonly string AvatarsLocation = Path.Combine(AppContext.BaseDirectory, "wwwroot", "Avatars");
    public static readonly string HeroCountersDataLocation = Path.Combine(AppContext.BaseDirectory, "heroCounters.json");
}
