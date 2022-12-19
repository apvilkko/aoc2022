namespace aoc2022;

public class Day19 : IAocRunner
{
    private List<Point> _points = new();
    private Dictionary<Point, int> _surfaces = new();

    private static List<Point> DIRECTIONS = new() {
        new Point(0, 0, 1),
        new Point(0, 1, 0),
        new Point(1, 0, 0),
        new Point(0, 0, -1),
        new Point(0, -1, 0),
        new Point(-1, 0, 0),
    };

    class Point {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public Point(int x, int y, int z) {
            X = x;
            Y = y;
            Z = z;
        }
        public override string ToString()
        {
            return $"({X},{Y},{Z})";
        }
        public bool IsAdjacent(Point other) {
            return (Math.Abs(other.X - X) == 1 && other.Y == Y && other.Z == Z) ||
            (Math.Abs(other.Y - Y) == 1 && other.X == X && other.Z == Z) ||
            (Math.Abs(other.Z - Z) == 1 && other.Y == Y && other.X == X);
        }
        public override bool Equals(object obj) => this.Equals(obj as Point);

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

            return (X == p.X) && (Y == p.Y) && (Z == p.Z);
        }

        public override int GetHashCode() => (X, Y, Z).GetHashCode();

        public static bool operator ==(Point lhs, Point rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }

                return false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Point lhs, Point rhs) => !(lhs == rhs);
        public static Point operator +(Point a, Point b) =>
            new Point(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    private bool IsInterior(Point point) {
            var maxX = _points.Select(p => p.X).Max();
            var maxY = _points.Select(p => p.Y).Max();
            var maxZ = _points.Select(p => p.Z).Max();

            if (_points.Find(p => p == point) != null) {
                return false;
            }

            var blocked = 0;
            foreach (var dir in DIRECTIONS) {
                var p = new Point(point.X, point.Y, point.Z);
                while(true) {
                    p += dir;
                    if (_points.Find(o => o == p) != null) {
                        blocked++;
                        break;
                    }
                    if (p.X > maxX || p.Y > maxY || p.Z > maxZ || p.X < 0 || p.Y < 0 || p.Z < 0) {
                        break;
                    }
                }
            }
            return blocked == 6;
        }

    public override Task Run(string variant)
    {
        foreach (var line in System.IO.File.ReadLines($"day19/{variant}.txt"))
        {
            var l = line.Trim();

            if (l.Length != 0)
            {
                var p = l.Split(",").Select(s => int.Parse(s)).ToList();
                _points.Add(new Point(p[0], p[1], p[2]));
            }
        }

        foreach (var p in _points) {
            _surfaces[p] = 6;
        }

        foreach (var p in _points) {
            _surfaces[p] -= _points.Select(other => other.IsAdjacent(p)).Where(x => x).Count();
        }

        var exposed = _surfaces.Values.Sum();
        Console.WriteLine($"part 1 {exposed}");

        // part 2
        var maxX = _points.Select(p => p.X).Max();
        var maxY = _points.Select(p => p.Y).Max();
        var maxZ = _points.Select(p => p.Z).Max();

        var interiors = new HashSet<Point>();
        for (var x = 0; x < maxX; ++x) {
            for (var y = 0; y < maxY; ++y) {
                for (var z = 0; z < maxZ; ++z) {
                    var p = new Point(x, y, z);
                    if (IsInterior(p)) {
                        interiors.Add(p);
                    }
                }
            }
        }
        Console.WriteLine($"interiors {interiors.Count} {string.Join(",", interiors)}");

        for (var z = 0; z <= maxZ ; ++z) {
        for (var y = 0; y <= maxY; ++y) {
            for (var x = 0; x <= maxX; ++x) {
                var c = new Point(x,y,z);
                var p = _points.FirstOrDefault(o => o == c);
                if (p != null) {
                    Console.Write('C');
                } else {
                    var i = interiors.FirstOrDefault(o => o == c);
                    if (i != null) {
                        Console.Write('i');
                    } else {
                        Console.Write('.');
                    }
                }
            }
            Console.Write('\n');
        }
        Console.Write("--\n");
        }

        foreach (var i in interiors) {
            foreach (var dir in DIRECTIONS) {
                var p = i + dir;
                if (p.IsAdjacent(i) && _surfaces.ContainsKey(p) && _surfaces[p] > 0) {
                    _surfaces[p]--;
                }
            }
        }

        exposed = _surfaces.Values.Sum();
        Console.WriteLine($"part 2 {exposed}");

        // too low


        return Task.CompletedTask;
    }
}