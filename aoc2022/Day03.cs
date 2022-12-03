namespace aoc2022;

public class Day03 : IAocRunner
{
    public override Task Run(string variant)
    {
        var sum = 0;
        var sum2 = 0;
        var rucksacks = new List<string>();

        foreach (var line in System.IO.File.ReadLines($"day03/{variant}.txt"))
        {
            var l = line.Trim();

            if (l.Length != 0)
            {
                // Part 1
                var halfpoint = l.Length / 2;
                var bag1 = l[..halfpoint];
                var bag2 = l[halfpoint..];
                var common = bag1.Intersect(bag2).ToList()[0];
                sum += ToPriority(common);

                // Part 2
                rucksacks.Add(l);
                if (rucksacks.Count == 3)
                {
                    var common3 = rucksacks[0].Intersect(rucksacks[1]).Intersect(rucksacks[2]).ToList()[0];
                    sum2 += ToPriority(common3);
                    rucksacks = new List<string>();
                }
            }
        }

        System.Console.WriteLine($"priorities sum {sum} sum2 {sum2}");

        return Task.CompletedTask;
    }

    private static int ToPriority(char ch) => (int)ch - ((int)ch < 91 ? 38 : 96);
}