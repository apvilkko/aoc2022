namespace aoc2022;

public class Day04 : IAocRunner
{
    public override Task Run(string variant)
    {
        var sum = 0;
        var sum2 = 0;

        foreach (var line in System.IO.File.ReadLines($"day04/{variant}.txt"))
        {
            var l = line.Trim();

            if (l.Length != 0)
            {
                // Part 1
                var ra = l
                    .Split(",")
                    .Select(r => r
                        .Split("-")
                        .Select(x => int.Parse(x))
                        .ToList()
                    ).ToList();
                if (ra[0][0] >= ra[1][0] && ra[0][1] <= ra[1][1] ||
                    ra[1][0] >= ra[0][0] && ra[1][1] <= ra[0][1])
                {
                    sum++;
                }

                // Part 2
                var smaller = ra[0][0] < ra[1][0] ? 0 : 1;
                var other = smaller == 1 ? 0 : 1;
                for (var i = ra[smaller][0]; i <= ra[smaller][1]; i++)
                {
                    if (i >= ra[other][0])
                    {
                        sum2++;
                        break;
                    }
                }
            }
        }

        System.Console.WriteLine($"sum {sum} sum2 {sum2}");

        return Task.CompletedTask;
    }
}