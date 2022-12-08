namespace aoc2022;

public class Day08 : IAocRunner
{
    private List<List<int>> _treeMap = new();
    private Point _size = new Point(0, 0);
    private static Point LEFT = new Point(-1, 0);
    private static Point RIGHT = new Point(1, 0);
    private static Point UP = new Point(0, -1);
    private static Point DOWN = new Point(0, 1);

    private class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }

    public override Task Run(string variant)
    {
        foreach (var line in System.IO.File.ReadLines($"day08/{variant}.txt"))
        {
            var l = line.Trim();

            if (l.Length != 0)
            {
                _treeMap.Add(l
                    .ToCharArray()
                    .Select(x => x - '0')
                    .ToList());
            }
        }

        _size.Y = _treeMap.Count;
        _size.X = _treeMap[0].Count;


        var visible = 0;
        var part2 = 0;

        for (var y = 0; y < _size.Y; ++y)
        {
            for (var x = 0; x < _size.X; ++x)
            {
                var p = new Point(x, y);
                var current = _treeMap[y][x];

                // Part 1
                if (x == 0 || y == 0 || x == _size.X - 1 || y == _size.Y - 1)
                {
                    // edges, no need to calculate
                    visible++;
                }
                else
                {
                    visible += GetAllTrees(p)
                        .Any(trees => trees
                            .All(height => height < _treeMap[y][x])
                        ) ? 1 : 0;
                }

                // Part 2
                var score = GetAllTrees(p)
                    .Select(trees => UpToHeight(trees, current).Count())
                    .Aggregate(1, (a, b) => a * b);
                if (score > part2)
                {
                    part2 = score;
                }
            }
        }

        Console.WriteLine($"visible {visible} part2 {part2}");

        return Task.CompletedTask;
    }

    private static IEnumerable<int> UpToHeight(IEnumerable<int> enumerable, int height)
    {
        var list = enumerable.ToList();
        var index = list.FindIndex(item => item >= height);
        if (index == -1)
        {
            return list;
        }
        return list.GetRange(0, index + 1);
    }

    private IEnumerable<List<int>> GetAllTrees(Point p)
    {
        return new List<List<Point>>()
            {
                new List<Point>() { LEFT, p },
                new List<Point>() { RIGHT, p },
                new List<Point>() { UP, p },
                new List<Point>() { DOWN, p },

            }
            .Select(l => GetTrees(l[0], l[1]));
    }

    private List<int> GetTrees(Point direction, Point point)
    {
        //Console.WriteLine($"GetTrees {direction} {point} size {_size}");
        List<int> ret;
        if (direction.X != 0)
        {
            var count = direction.X > 0 ? _size.X - (point.X + 1) : point.X;
            if (count == 0)
            {
                return new List<int>();
            }
            ret = _treeMap[point.Y]
                .GetRange(direction.X > 0 ? point.X + 1 : 0, count);
        }
        else
        {
            var count = direction.Y > 0 ? _size.Y - (point.Y + 1) : point.Y;
            if (count == 0)
            {
                return new List<int>();
            }
            ret = _treeMap
                .Select(row => row[point.X])
                .ToList()
                .GetRange(direction.Y > 0 ? point.Y + 1 : 0, count);
        }

        if (direction.X < 0 || direction.Y < 0)
        {
            ret.Reverse();
        }

        return ret;
    }
}