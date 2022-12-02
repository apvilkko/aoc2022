namespace aoc2022;

public class Day02 : IAocRunner
{
    private static readonly string ROCK = "R";
    private static readonly string PAPER = "P";
    private static readonly string SCISSORS = "S";

    private static readonly int WIN = 6;
    private static readonly int DRAW = 3;
    private static readonly int LOSS = 0;

    private static readonly Dictionary<string, int> VALUES = new() {
        {ROCK, 1 },
        {PAPER, 2},
        {SCISSORS, 3},
    };

    private static readonly List<string> ALL = new() { ROCK, PAPER, SCISSORS };

    public override Task Run(string variant)
    {
        var score = 0;
        var score2 = 0;

        foreach (var line in System.IO.File.ReadLines($"day02/{variant}.txt"))
        {
            var l = line.Trim();
            if (l.Length != 0)
            {
                var parts = l.Split(" ");
                var they = Decode(parts[0]);
                score += GetScore(they, Decode(parts[1]));
                score2 += GetScore(they, DecodePart2(they, parts[1]));
            }
        }

        System.Console.WriteLine($"score {score}");
        System.Console.WriteLine($"score part 2 {score2}");

        return Task.CompletedTask;
    }

    private static bool Wins(string me, string they) =>
        (me == PAPER && they == ROCK) ||
        (me == ROCK && they == SCISSORS) ||
        (me == SCISSORS && they == PAPER);

    private static int GetScore(string they, string me)
    {
        if (they == me)
        {
            return DRAW + VALUES[me];
        }
        return (Wins(me, they) ? WIN : LOSS) + VALUES[me];
    }

    private static string Decode(string input) => input switch
    {
        "A" => ROCK,
        "B" => PAPER,
        "C" => SCISSORS,
        "X" => ROCK,
        "Y" => PAPER,
        "Z" => SCISSORS,
        _ => throw new Exception($"unknown {input}")
    };

    private static string DecodePart2(string they, string me)
    {
        if (me == "X")
        {
            // I need to lose
            return ALL.First(x => !Wins(x, they) && they != x);
        }
        else if (me == "Y")
        {
            // Need a draw
            return ALL.First(x => !Wins(x, they) && !Wins(they, x));
        }
        else if (me == "Z")
        {
            // I need to win
            return ALL.First(x => Wins(x, they));
        }
        throw new Exception($"unknown {me}");
    }
}