namespace aoc2022;

public class Day06 : IAocRunner
{
    public override Task Run(string variant)
    {
        // Part 1
        //var NUM_ITEMS = 4;
        // Part 2
        var NUM_ITEMS = 14;

        foreach (var line in System.IO.File.ReadLines($"day06/{variant}.txt"))
        {
            var l = line.Trim();

            if (l.Length != 0)
            {
                var list = new List<Char>();
                for (var i = 0; i < l.Length; i++)
                {
                    list.Add(l[i]);
                    if (list.Count >= NUM_ITEMS && list.TakeLast(NUM_ITEMS).Distinct().Count() == NUM_ITEMS)
                    {
                        Console.WriteLine($"break at position {i + 1}");
                        break;
                    }
                }
            }
        }

        return Task.CompletedTask;
    }
}