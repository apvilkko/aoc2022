namespace aoc2022;

public class Day01 : IAocRunner
{
    public override Task Run(string variant)
    {
        var arr = new List<List<int>>();
        var temp = new List<int>();

        foreach (var line in System.IO.File.ReadLines($"day01/{variant}.txt"))
        {
            if (line.Trim().Length == 0 && temp.Count > 0)
            {
                arr.Add(temp);
                temp = new List<int>();
            }
            else {
                var number = int.Parse(line.Trim());
                temp.Add(number);
            }
        }

        var ordered = arr.Select((numbers, i) => (numbers.Sum(), i)).OrderByDescending(x => x.Item1);
        var first = ordered.First();
        var top3Sum = ordered.Take(3).Sum(x => x.Item1);
        System.Console.WriteLine($"max value is {first}, top three sum {top3Sum}");

        return Task.CompletedTask;
    }
}
