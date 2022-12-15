using System.Text.RegularExpressions;

namespace aoc2022;

public class Day15 : IAocRunner
{
    private List<List<(int, int)>> _input = new();
    private HashSet<(int, int)> _covered = new();

    public override Task Run(string variant)
    {
        foreach (var line in System.IO.File.ReadLines($"day15/{variant}.txt"))
        {
            var l = line.Trim();

            if (l.Length != 0)
            {
                _input.Add(Regex.Matches(l, @"x=([-\d]+), y=([-\d]+)")
                    .Select(m => (int.Parse(m.Groups[1].ToString()), int.Parse(m.Groups[2].ToString())))
                    .ToList());
            }
        }

        // part 1
        Console.WriteLine(GetCoverageCountAt(10));
        //Console.WriteLine(GetCoverageCountAt(2000000));

        // part 2
        var candidates = new HashSet<(int, int)>();
        var beacons = _input.Select(pair => pair[1]).ToHashSet();
        //var maxCoord = 20;
        var maxCoord = 4000000;
        Random rnd = new Random();
        var maxMd = _input.Select(pair => GetManhattanDistance(pair)).Max();
        Console.WriteLine($"max md {maxMd}");

        /*for (var y = 0; y <= maxCoord; ++y)
        {
            if (y % 1000 == 0)
            {
                Console.WriteLine($"y {y}");
            }
            GetCoverageAt(y);
            for (var x = 0; x <= maxCoord; ++x)
            {
                if (x % 1000 == 0)
                {
                    Console.WriteLine($"y {y} x {x}");
                }
                if (!_covered.Contains((x, y)) && !beacons.Contains((x,y)))
                {
                    candidates.Add((x, y));
                    found = true;
                    break;
                }
                if (found)
                {
                    break;
                }
            }
        }*/

        /*while (true)
        {
            var x = rnd.Next(maxCoord + 1);
            var y = rnd.Next(maxCoord + 1);
            GetCoverageAt(y);
            break; //if (_covered.Contains((x,y)))
        }*/

        var result = candidates.First();
        var part2 = result.Item1 * 4000000 + result.Item2;
        Console.WriteLine($"candidates {candidates.Count} {string.Join("|", candidates)} {part2}");

        return Task.CompletedTask;
    }

    private int GetManhattanDistance(List<(int, int)> pair)
    {
        var sensorX = pair[0].Item1;
        var sensorY = pair[0].Item2;
        var beaconX = pair[1].Item1;
        var beaconY = pair[1].Item2;
        return Math.Abs(sensorX - beaconX) + Math.Abs(sensorY - beaconY);
    }

    private int GetCoverageCountAt(int y)
    {
        GetCoverageAt(y);
        return _covered.Count;
    }

    private void GetCoverageAt(int y)
    {
        foreach(var pair in _input)
        {
            var sensorX = pair[0].Item1;
            var sensorY = pair[0].Item2;
            var beaconX = pair[1].Item1;
            var beaconY = pair[1].Item2;
            var md = GetManhattanDistance(pair);
            var width = md - Math.Abs(sensorY - y);
            //Console.WriteLine($"({sensorX},{sensorY}) ({beaconX},{beaconY}) {md} {width}");
            if (width <= 0)
            {
                continue;
            }
            var minX = sensorX - width;
            var maxX = sensorX + width;
            for (var x = minX; x <= maxX; ++x)
            {
                //Console.WriteLine($"  adding {x}");
                if (beaconY == y && beaconX == x) {
                    // don't count beacon position
                    continue;
                } 
                _covered.Add((x, y));
            }
        }
        return;
    }
}