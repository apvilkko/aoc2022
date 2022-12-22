namespace aoc2022;

public class Day17 : IAocRunner
{
    private static readonly int WIDTH = 7;
    private List<List<char>> _chamber = new();
    private int _rockIndex = -1;
    private (int, int) _rockPos = (-1, -1);
    private int _jetIndex = -1;
    private int _stoppedRocks = 0;
    private string _input = "";
    private static List<char> EmptyRow() => new String('.', WIDTH).ToCharArray().ToList();
    private static readonly List<List<string>> _shapes = @"####

.#.
###
.#.

..#
..#
###

#
#
#
#

##
##"
        .Split(Environment.NewLine + Environment.NewLine)
        .Select(x => x.Split(Environment.NewLine).ToList())
        .ToList();

    private List<string> GetCurrentShape() {
        return _shapes[_rockIndex % _shapes.Count];
    }
    private (int, int) GetStartPosition() {
        var shape = GetCurrentShape();
        var x = 2;
        var neededRows = 3 + shape.Count;
        var availableRows = new List<int>();
        for (var y = 0; y < _chamber.Count; ++y) {
            if (_chamber[y].All(x => x == '.')) {
                availableRows.Add(y);
            } else {
                break;
            }
        }
        //Console.WriteLine($"available rows {string.Join(",", availableRows)} needed {neededRows} shape {shape}");
        if (availableRows.Count >= neededRows) {
            var mostBottomEmptyRow = availableRows.Max();
            return (x, mostBottomEmptyRow - (shape.Count - 1) - 3);
        } else {
            // need to add rows
            var amount = neededRows - availableRows.Count;
            //Console.WriteLine($"  adding {amount} rows");
            for (var i = 0; i < amount; ++i) {
                _chamber.Insert(0, EmptyRow());
            }
            return GetStartPosition();
        }
    }

    private bool Collides(int dx, int dy=0) {
        var shape = GetCurrentShape();
        var px = _rockPos.Item1;
        var py = _rockPos.Item2;
        var newPos = (px + dx, py + dy);
        for (var y = 0; y < shape.Count; ++y) {
            for (var x = 0; x < shape[y].Length; ++x) {
                if (shape[y][x] == '.') {
                    continue;
                }
                var cy = newPos.Item2 + y;
                var cx = newPos.Item1 + x;
                char tile;
                try {
                    tile = _chamber[cy][cx];
                } catch (ArgumentOutOfRangeException _) {
                    return true;
                }
                if (tile != '.') {
                    return true;
                }
            }
        }
        return false;
    }

    private void CommitRock() {
        var shape = GetCurrentShape();
        for (var y = 0; y < shape.Count; ++y) {
            for (var x = 0; x < shape[y].Length; ++x) {
                if (shape[y][x] == '.') {
                    continue;
                }
                var cy = _rockPos.Item2 + y;
                var cx = _rockPos.Item1 + x;
                _chamber[cy][cx] = '#';
            }
        }
    }

    public class IterationDoneException : Exception {}

    private void Cycle() {
        if (_rockPos == (-1, -1)) {
            _rockIndex++;
            _rockPos = GetStartPosition();
            //Console.WriteLine($"starts falling: {_rockIndex % _shapes.Count} {_rockPos}");
        }

        var shape = GetCurrentShape();
        var dir = _input[++_jetIndex % _input.Length];
        var dx = dir == '<' ? -1 : 1;
        if (!Collides(dx)) {
            _rockPos.Item1 += dx;
            //Console.WriteLine($"Jet of gas pushes rock {(dx == 1 ? "right" : "left")}");
        } else {
            //Console.WriteLine($"Jet of gas pushes rock {(dx == 1 ? "right" : "left")}, but nothing happens");
        }
        var dy = 1;
        if (!Collides(0, dy)) {
            _rockPos.Item2 += dy;
            //Console.WriteLine("Rock falls");
        } else {
            //Console.WriteLine("Rock comes to rest");
            CommitRock();
            _stoppedRocks++;
            if (_stoppedRocks >= 2022) {
                throw new IterationDoneException();
            }
            _rockPos = (-1, -1);
        }
        //PrintChamber();
    }

    private void PrintChamber() {
        Console.WriteLine();
        foreach(var line in _chamber) {
            Console.WriteLine(string.Join("", line));
        }
        Console.WriteLine();
    }
    public override Task Run(string variant)
    {
        _input = File.ReadAllText($"day17/{variant}.txt").Trim();

        var i = 0;
        for (; i < 4; ++i) {
            _chamber.Add(EmptyRow());
        }
        //PrintChamber();

        i = 0;
        while (true) {
            try {
                Cycle();
            } catch (IterationDoneException _) {
                break;
            }
            ++i;
        }

        //PrintChamber();
        var height = 0;
        for (var y = _chamber.Count - 1; y >= 0; --y) {
            if (_chamber[y].Any(x => x == '#')) {
                height++;
            } else {
                break;
            }
        }

        Console.WriteLine($"height {height}");

        return Task.CompletedTask;
    }
}