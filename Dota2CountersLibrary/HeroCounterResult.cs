namespace Dota2CountersLibrary;

public class HeroCounterResult
{
    public HeroCounterResult(string hero)
    {
        Hero = hero;
    }
    public string Hero { get; set; }
    public int Score { get; set; }
    public List<string> BadAgainst { get; } = new();
    public List<string> GoodAgainst { get; } = new();
    public List<string> WorksWellWith { get; } = new();
}
