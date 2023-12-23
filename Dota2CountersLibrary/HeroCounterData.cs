namespace Dota2CountersLibrary;

public record class HeroCounterData(string Hero, 
    SortedSet<string> BadAgainst,
    SortedSet<string> GoodAgainst,
    SortedSet<string> GoodWith);
