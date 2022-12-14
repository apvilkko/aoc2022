using System.Text.RegularExpressions;

namespace aoc2022;

public class Day14 : IAocRunner
{
    private HashSet<(int, int)> _rocks = new();
    private (int, int) SAND_ORIGIN = (500, 0);
    private HashSet<(int, int)> _sand = new();
    private int _abyssY = 0;
    private bool _part2 = true;

    public override Task Run(string variant)
    {
        foreach (var line in System.IO.File.ReadLines($"day14/{variant}.txt"))
        {
            var l = line.Trim();

            if (l.Length != 0)
            {
                var pairs = Regex.Matches(l, @"\d+,\d+")
                    .Select(pair => pair.ToString()
                        .Split(",")
                        .Select(x => int.Parse(x))
                        .ToList()
                    );
                var prev = (-1, -1);
                foreach (var coord in pairs)
                {
                    var c = (coord[0], coord[1]);
                    if (prev != (-1, -1))
                    {
                        if (c.Item1 == prev.Item1)
                        {
                            for (var y = Math.Min(prev.Item2, c.Item2); y <= Math.Max(c.Item2, prev.Item2); ++y)
                            {
                                _rocks.Add((c.Item1, y));
                            }
                        }
                        else if (c.Item2 == prev.Item2)
                        {
                            for (var x = Math.Min(prev.Item1, c.Item1); x <= Math.Max(c.Item1, prev.Item1); ++x)
                            {
                                _rocks.Add((x, c.Item2));
                            }
                        }
                    }
                    prev = c;
                }
            }
        }

        _abyssY = _rocks.Max(c => c.Item2) + (_part2 ? 2 : 1);
        while (AdvanceSand()) { }

        Console.WriteLine($"abyss/floor starts at {_abyssY}, sand count {_sand.Count}");

        return Task.CompletedTask;
    }

    private bool Taken((int, int) pos)
    {
        if (_part2 && pos.Item2 >= _abyssY)
        {
            return true;
        }
        var result = _rocks.Contains(pos) || _sand.Contains(pos);
        return result;
    }

    private bool InAbyss((int, int) pos) => pos.Item2 >= _abyssY;

    private bool AdvanceSand()
    {
        var pos = SAND_ORIGIN;

        while (true)
        {
            var x = pos.Item1;
            var y = pos.Item2;
            var candidates = new List<(int, int)>()
            {
                (x, y + 1),
                (x - 1, y + 1),
                (x + 1, y + 1)
            };

            if (!_part2 && candidates.Any(c => InAbyss(c)))
            {
                return false;
            }

            if (candidates.All(c => Taken(c)))
            {
                _sand.Add((x, y));

                if (_part2 && y == 0)
                {
                    return false;
                }

                return true;
            }

            var free = candidates.FindIndex(c => !Taken(c));
            pos = candidates[free];
        }
    }
}