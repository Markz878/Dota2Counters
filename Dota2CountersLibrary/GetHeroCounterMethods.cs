using System.Diagnostics;

namespace Dota2CountersLibrary;
public static class GetHeroCounterMethods
{
    public static (List<string> allies, List<string> enemies) GetAlliesAndEnemies(int playerPosition, string[] heroes)
    {
        List<string> allies;
        List<string> enemies;
        if (playerPosition < 5)
        {
            allies = new List<string>(heroes[0..5]);
            enemies = new List<string>(heroes[5..10]);
            allies.RemoveAt(playerPosition);
        }
        else
        {
            allies = new List<string>(heroes[5..10]);
            enemies = new List<string>(heroes[0..5]);
            allies.RemoveAt(playerPosition - 5);
        }
        allies.RemoveAll(x => x == null);
        enemies.RemoveAll(x => x == null);
        return (allies, enemies);
    }

    public static List<HeroCounterResult> GetHeroCounterResults(List<HeroCounterData> heroCounters, List<string> allies, List<string> enemies)
    {
        List<HeroCounterResult> counterResults = [];
        foreach (HeroCounterData hero in heroCounters.Where(x => !allies.Contains(x.Hero) && !enemies.Contains(x.Hero)))
        {
            HeroCounterResult result = new(hero.Hero);
            foreach (string enemy in enemies)
            {
                if (hero.BadAgainst.Contains(enemy))
                {
                    result.Score -= 2;
                    result.BadAgainst.Add(enemy);
                }
                if (hero.GoodAgainst.Contains(enemy))
                {
                    result.Score += 3;
                    result.GoodAgainst.Add(enemy);
                }
            }
            foreach (string ally in allies)
            {
                if (hero.GoodWith.Contains(ally))
                {
                    result.Score++;
                    result.WorksWellWith.Add(ally);
                }
            }
            counterResults.Add(result);
        }
        return counterResults.OrderByDescending(x => x.Score).ThenBy(x => x.Hero).ToList();
    }

    public static void ShowHeroDetails(string hero)
    {
        using Process p = Process.Start("cmd", $"/C start firefox https://dota2.fandom.com/wiki/{hero.Replace(" ", "_")}/Counters");
    }

}
