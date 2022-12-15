using System.Text.RegularExpressions;

namespace aoc2022;

public class Day15 : IAocRunner
{
    private List<List<(int, int)>> _input = new();
    private HashSet<(int, int)> _covered = new();
    private Dictionary<(int, int, int, int), int> _mdCache = new();

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

        foreach (var pair in _input)
        {
            var sensorX = pair[0].Item1;
            var sensorY = pair[0].Item2;
            var beaconX = pair[1].Item1;
            var beaconY = pair[1].Item2;
            var md = GetManhattanDistance(pair);
            //Console.WriteLine($"Processing {pair[0]} {pair[1]} md {md}");
            // outside corners of the "coverage diamond"
            var corners = new List<(int, int)>() {
                (sensorX, sensorY - md - 1),
                (sensorX + md + 1, sensorY),
                (sensorX, sensorY + md + 1),
                (sensorX - md - 1, sensorY)
            };
            for (var i = 0; i < corners.Count; ++i)
            {
                var from = corners[i != 0 ? i - 1 : corners.Count - 1];
                var to = corners[i];
                var dx = to.Item1 - from.Item1;
                var dy = to.Item2 - from.Item2;
                var distance = Math.Max(Math.Abs(dx), Math.Abs(dy));
                var sx = Math.Sign(dx);
                var sy = Math.Sign(dy);
                for (var j = 0; j < distance; ++j)
                {
                    var x = from.Item1 + sx * j;
                    var y = from.Item2 + sy * j;
                    var cand = (x, y);
                    if (x >= 0 && y >= 0 && x <= maxCoord && y <= maxCoord && !beacons.Contains(cand))
                    {
                        var containedInOther = false;
                        foreach (var innerPair in _input)
                        {
                            if (pair[0] == innerPair[0] && pair[1] == innerPair[1])
                            {
                                continue;
                            }
                            if (Contains(innerPair, cand))
                            {
                                containedInOther = true;
                            }
                        }
                        if (!containedInOther)
                        {
                            Console.WriteLine($"  Adding candidate ({x},{y})");
                            candidates.Add(cand);
                            // could break when first candidate found, it's the answer
                        }
                    }
                }
            }
        }

        var result = candidates.First();
        var part2 = result.Item1 * 4000000 + result.Item2;
        Console.WriteLine($"candidates {candidates.Count} {string.Join("|", candidates)} {part2}");

        return Task.CompletedTask;
    }

    private bool Contains(List<(int, int)> pair, (int, int) point)
    {
        var x0 = pair[0].Item1;
        var y0 = pair[0].Item2;
        var x = point.Item1;
        var y = point.Item2;
        var md = GetManhattanDistance(pair);
        var width = md - Math.Abs(y0 - y);
        var height = md - Math.Abs(x0 - x);
        return x >= x0 - width && x <= x0 + width && y >= y0 - height && y <= y0 + height;
    }

    private int GetManhattanDistance(List<(int, int)> pair)
    {
        var sensorX = pair[0].Item1;
        var sensorY = pair[0].Item2;
        var beaconX = pair[1].Item1;
        var beaconY = pair[1].Item2;
        var key = (sensorX, sensorY, beaconX, beaconY);
        var success = _mdCache.TryGetValue(key, out var fromCache);
        if (success)
        {
            return fromCache;
        }
        var md = Math.Abs(sensorX - beaconX) + Math.Abs(sensorY - beaconY);
        _mdCache.Add(key, md);
        return md;
    }

    private int GetCoverageCountAt(int y)
    {
        GetCoverageAt(y);
        return _covered.Count;
    }

    private void GetCoverageAt(int y)
    {
        foreach (var pair in _input)
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
                if (beaconY == y && beaconX == x)
                {
                    // don't count beacon position
                    continue;
                }
                _covered.Add((x, y));
            }
        }
        return;
    }
}