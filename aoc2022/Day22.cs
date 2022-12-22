namespace aoc2022;
using System.Text.RegularExpressions;

public class Day22 : IAocRunner
{
    private List<List<char>> _map = new();
    private List<(int, string)> _instructions = new();
    private Direction _facing = Direction.Right;
    private (int,int) _pos = (0,0);
    private static char FREE = '.';
    private static char VOID = ' ';
    private static char WALL = '#';

    enum Direction {
        Right = 0,
        Down = 1,
        Left = 2,
        Up = 3,
    }

    private (int,int) ToVector(Direction dir) => dir switch {
        Direction.Right => (1, 0),
        Direction.Down => (0, 1),
        Direction.Left => (-1, 0),
        _ => (0, -1),
    };

    private (char, int) MoveX(int x, int y, int dx) {
        var newX = x + dx;
        var row = _map[y];

        if (newX > row.Count - 1) {
            newX = 0;
        } else if (newX < 0) {
            newX = row.Count - 1;
        }
        Console.WriteLine($"row '{PrintRow(row, newX)}'  - moving X {x}+{dx},{y}, newX {newX}");
        return (row[newX], newX);
    }

    private (char, int) MoveY(int x, int y, int dy) {
        var newY = y + dy;
        if (newY > _map.Count - 1) {
            newY = 0;
        } else if (newY < 0) {
            newY = _map.Count - 1;
        }
        var row = _map[newY];
        while (x > row.Count - 1) {
            newY = (newY + dy) % _map.Count;
            row = _map[newY];
            Console.WriteLine($"  adjusting y to {newY}");
        }
        Console.WriteLine($"row '{PrintRow(row, x)}'  - moving Y {x},{y}+{dy}, newY {newY}");
        return (row[x], newY);
    }

    private string PrintRow(List<char> row, int x) {
        var line = string.Join("", row);
        return line.Substring(0, x) + 'O' + line.Substring(x + 1);
    }

    // true if moved (_pos updated), false if is blocked
    private bool Advance() {
        var x = _pos.Item1;
        var y = _pos.Item2;
        var vect = ToVector(_facing);
        char tile;
        var newX = x;
        var newY = y;
        bool result = false;
        if (vect.Item2 == 0) {
            (tile, newX) = MoveX(x, y, vect.Item1);
            if (tile == FREE) {
                result = true;
            } else if (tile == VOID) {
                var cx = newX;
                while (tile == VOID) {
                    (tile, cx) = MoveX(cx, y, vect.Item1);
                }
                if (tile == FREE) {
                    newX = cx;
                    result = true;
                } else {
                    newX = x;
                }
            }
        } else {
            (tile, newY) = MoveY(x, y, vect.Item2);
            if (tile == FREE) {
                result = true;
            } else if (tile == VOID) {
                var cy = newY;
                while (tile == VOID) {
                    (tile, cy) = MoveY(x, cy, vect.Item2);
                }
                if (tile == FREE) {
                    newY = cy;
                    result = true;
                } else {
                    newY = y;
                }
            }
        }
        if (result) {
            _pos = (newX, newY);
        }
        Console.WriteLine($"  after Advance {_pos} {ToVector(_facing)} '{tile}'");

        return result;
    }

    private void ApplyInstruction((int, string) inst) {
        var dist = inst.Item1;
        Console.WriteLine($"\ninstruction {inst}");
        for (var i = 0; i < dist; ++i) {
            if (!Advance()) {
                break;
            }
        }
        _facing = inst.Item2 switch {
            "R" => (Direction)(((int)_facing + 1) % 4),
            _ => (Direction)(((int)_facing + 3) % 4)
        };
        Console.WriteLine($"new direction is {_facing}");
    }

    public override Task Run(string variant)
    {
        foreach (var line in System.IO.File.ReadLines($"day22/{variant}.txt"))
        {
            var l = line;
            if (l.Contains(FREE) || l.Contains(WALL)) {
                _map.Add(l.TrimEnd().ToCharArray().ToList());
            } else if (l.Contains('R')) {
                _instructions = Regex.Matches(l, @"(\d+)(L|R)")
                    .Select(m => (int.Parse(m.Groups[1].ToString()), m.Groups[2].ToString()))
                    .ToList();
            }
        }

        _pos = (_map[0].IndexOf('.'), 0);
        foreach (var inst in _instructions) {
            ApplyInstruction(inst);
        }

        var part1 = 1000 * (_pos.Item2 + 1) + 4 * (_pos.Item1 + 1) + (int)_facing;
        Console.WriteLine($"part 1: {part1}");

        // too low 89204
        // too high 120212

        return Task.CompletedTask;
    }
}