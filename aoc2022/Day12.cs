namespace aoc2022;

public class Day12 : IAocRunner
{
    private HashSet<(int, int)> _explored = new();
    private Dictionary<(int, int), (int, int)> _parents = new();

    private static List<(int, int)> MOVES = new()
    {
        (1,0), (-1,0), (0,1), (0,-1)
    };

    private class Point
    {
        public char Value { get; set; }
        public Point(char value) { Value = value; }
    }

    private class Board
    {
        public List<List<Point>> Map = new();
        public (int, int) Start;
        public (int, int) End;

        public Point GetAt(int x, int y)
        {
            return Map[y][x];
        }

        public Point GetAt((int, int) coord)
        {
            return Map[coord.Item2][coord.Item1];
        }

        public (int, int) Size()
        {
            return (Map[0].Count, Map.Count);
        }
    }

    public override Task Run(string variant)
    {
        var board = new Board();
        var y = 0;
        foreach (var line in System.IO.File.ReadLines($"day12/{variant}.txt"))
        {
            var l = line.Trim();

            if (l.Length != 0)
            {
                board.Map.Add(l.ToCharArray().Select(x => new Point(x)).ToList());
                var startI = l.IndexOf('S');
                if (startI > -1)
                {
                    board.Start = (startI, y);
                }
                var endI = l.IndexOf('E');
                if (endI > -1)
                {
                    board.End = (endI, y);
                }
                y++;
            }
        }

        var result = Solve(board, board.Start);
        Console.WriteLine($"{result} {_explored.Count} {_parents.Count}");

        GetLength(result);

        var startingPoints = new List<(int, int)>();
        for (y = 0; y < board.Size().Item2; ++y)
        {
            for (var x = 0; x < board.Size().Item1; ++x)
            {
                if (board.GetAt(x, y).Value == 'a')
                {
                    startingPoints.Add((x, y));
                }
            }
        }

        var part2 = startingPoints.Min(start =>
        {
            var res = Solve(board, start);
            return GetLength(res);
        });
        Console.WriteLine($"part 2 {part2}");

        return Task.CompletedTask;
    }

    private int GetLength((int, int) result)
    {
        if (result != (-1, -1))
        {
            var child = result;
            var length = 0;
            while (true)
            {
                var parent = _parents.GetValueOrDefault(child, (-1, -1));
                if (parent == (-1, -1))
                {
                    break;
                }
                length++;
                child = parent;
            }
            Console.WriteLine($"length {length}");
            return length;
        }
        return int.MaxValue;
    }

    private (int, int) Solve(Board board, (int, int) pos)
    {
        _parents.Clear();
        _explored.Clear();
        var q = new Queue<(int, int)>();
        q.Enqueue(pos);
        _explored.Add(pos);
        while (q.Count > 0)
        {
            var v = q.Dequeue();
            //Console.WriteLine($"processing {v}");
            if (board.GetAt(v).Value == 'E')
            {
                return v;
            }

            var x = v.Item1;
            var y = v.Item2;
            var possibleMoves = MOVES.Where(move =>
            {
                var newX = x + move.Item1;
                var newY = y + move.Item2;
                var oldValue = board.GetAt(x, y).Value;
                if (oldValue == 'S')
                {
                    oldValue = 'a';
                }
                var withinLimits = newX >= 0 && newY >= 0 && newY < board.Size().Item2 && newX < board.Size().Item1;
                if (!withinLimits)
                {
                    return false;
                }
                var newValue = board.GetAt(newX, newY).Value;
                if (newValue == 'E')
                {
                    newValue = 'z';
                }
                return (newValue - oldValue <= 1);
            }).ToList();

            foreach (var move in possibleMoves)
            {
                var newX = x + move.Item1;
                var newY = y + move.Item2;
                var newPos = (newX, newY);
                //Console.WriteLine($"{x},{y}->{newX},{newY}");
                if (!_explored.Contains(newPos))
                {
                    _explored.Add(newPos);
                    _parents[newPos] = v;
                    q.Enqueue(newPos);
                }
            }
        }
        return (-1, -1);
    }


}