namespace aoc2022;

public class Day17 : IAocRunner
{
    private static readonly int WIDTH = 7;
    private List<string> _chamber = new();
    private int _rockIndex = -1;
    private int _jetIndex = -1;
    private string _input = "";
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

    private void Cycle() {
        var shape = _shapes[++_rockIndex % _shapes.Count];
        var dir = _input[++_jetIndex];
    }

    private void PrintChamber() {
        foreach(var line in _chamber) {
            Console.WriteLine(line);
        }
    }
    public override Task Run(string variant)
    {
        _input = File.ReadAllText($"day17/{variant}.txt").Trim();

        var i = 0;
        for (; i < 4; ++i) {
            _chamber.Add(new String('.', WIDTH));
        }
        PrintChamber();

        i = 0;
        while (i < 10) {
            Cycle();
            ++i;
        }

        return Task.CompletedTask;
    }
}