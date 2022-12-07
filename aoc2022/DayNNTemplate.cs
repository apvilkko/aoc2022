namespace aoc2022;

public class DayNN : IAocRunner
{
    public override Task Run(string variant)
    {
        foreach (var line in System.IO.File.ReadLines($"dayNN/{variant}.txt"))
        {
            var l = line.Trim();

            if (l.Length != 0)
            {
            }
        }

        return Task.CompletedTask;
    }
}