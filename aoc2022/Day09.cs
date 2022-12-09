namespace aoc2022;

public class Day09 : IAocRunner
{
    private static Point LEFT = new Point(-1, 0);
    private static Point RIGHT = new Point(1, 0);
    private static Point UP = new Point(0, -1);
    private static Point DOWN = new Point(0, 1);
    private static Dictionary<string, Point> directionMap = new() {
        { "L", LEFT },
        { "R", RIGHT },
        { "U", UP },
        { "D", DOWN },
    };

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

        public bool Equals(Point p)
        {
            if (p is null)
            {
                return false;
            }
            if (Object.ReferenceEquals(this, p))
            {
                return true;
            }
            if (this.GetType() != p.GetType())
            {
                return false;
            }
            return (X == p.X) && (Y == p.Y);
        }

        public override int GetHashCode() => (X, Y).GetHashCode();

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }
    }
    public override Task Run(string variant)
    {
        var moves = new List<(Point, int)>();
        // Part 1
        //var ROPE_LENGTH = 2;
        // Part 2
        var ROPE_LENGTH = 10;
        var rope = new List<Point>();
        for (var i = 0; i < ROPE_LENGTH; ++i)
        {
            rope.Add(new Point(0, 0));
        }

        var visited = new HashSet<string>();
        visited.Add(rope.Last().ToString());

        foreach (var line in System.IO.File.ReadLines($"day09/{variant}.txt"))
        {
            var l = line.Trim();

            if (l.Length != 0)
            {
                var parts = l.Split(" ");
                var direction = directionMap[parts[0]];
                var amount = int.Parse(parts[1]);
                moves.Add((direction, amount));
            }
        }

        foreach (var move in moves)
        {
            // move rope
            for (var i = 0; i < move.Item2; ++i)
            {
                if (i >= 0)
                {
                    for (var j = 1; j < ROPE_LENGTH; ++j)
                    {
                        rope = ApplyMove(rope, j);
                    }
                    
                    visited.Add(rope.Last().ToString());
                }
                rope[0] += move.Item1;

            }

        }

        // Do the last tail move
        for (var j = 1; j < ROPE_LENGTH; ++j)
        {
            rope = ApplyMove(rope, j);
        }
        visited.Add(rope.Last().ToString());

        Console.WriteLine($"visited total {visited.Count}");

        return Task.CompletedTask;
    }

    private List<Point> ApplyMove(List<Point> rope, int pos)
    {
        var temp = rope[pos];

        var xDiff = rope[pos - 1].X - temp.X;
        var yDiff = rope[pos - 1].Y - temp.Y;

        if (Math.Abs(xDiff) <= 1 && Math.Abs(yDiff) <= 1)
        {
            return rope;
        }

        if (xDiff > 0)
        {
            temp.X++;
        }
        else if (xDiff < 0)
        {
            temp.X--;
        }
        if (yDiff > 0)
        {
            temp.Y++;
        }
        else if (yDiff < 0)
        {
            temp.Y--;
        }

        return rope;
    }
}