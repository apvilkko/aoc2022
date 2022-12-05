using System.Text.RegularExpressions;

namespace aoc2022;

public class Day05 : IAocRunner
{
    private readonly List<Stack<char>> stacks = new();

    public override Task Run(string variant)
    {
        var crates = new List<List<char>>();
        var moves = new List<List<int>>();

        foreach (var line in System.IO.File.ReadLines($"day05/{variant}.txt"))
        {
            var l = line;

            if (l.Length != 0)
            {
                if (l.StartsWith("m"))
                {
                    var move = Regex.Replace(l, "[^0-9]", " ")
                        .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => int.Parse(x))
                        .ToList();
                    moves.Add(move);
                }
                else if (l.Contains("["))
                {
                    // crates
                    var c = new List<char>();
                    for (var i = 0; i < 10; i++)
                    {
                        try
                        {
                            var ci = l[CratePosition(i)];
                            c.Add(ci);
                        }
                        catch (IndexOutOfRangeException)
                        {
                            break;
                        }
                    }
                    crates.Add(c);
                }
                else
                {
                    // This is the crate numbers line

                    // This could be read from current line also
                    var numCrates = crates.Max(c => c.Count);

                    /*foreach (var crate in crates)
                    {
                        System.Console.WriteLine(string.Join("-", crate));
                    }*/

                    // Convert to stacks
                    for (var i = 0; i < numCrates; ++i)
                    {
                        stacks.Add(new Stack<char>());
                        for (var x = crates.Count - 1; x >= 0; --x)
                        {
                            if (crates[x][i] == ' ')
                            {
                                break;
                            }
                            stacks[i].Push(crates[x][i]);
                        }
                    }

                    /*foreach (var stack in stacks)
                    {
                        Console.WriteLine($"{stack.Peek()} {stack.Count}");
                    }*/
                }
            }
        }

        foreach(var move in moves)
        {
            ApplyMove(move);
        }

        System.Console.WriteLine($"top crates: {string.Join("", stacks.Select(s => s.Peek()))}");

        return Task.CompletedTask;
    }

    private void ApplyMove(List<int> move)
    {
        var amount = move[0];
        var from = move[1] - 1;
        var to = move[2] - 1;
        
        // Part 1
        /*for (var i = 0; i < amount; ++i)
        {
            var crate = stacks[from].Pop();
            stacks[to].Push(crate);
        }*/

        // Part 2
        var temp = new List<char>();
        for (var i = 0; i < amount; ++i)
        {
            var crate = stacks[from].Pop();
            temp.Add(crate);
        }
        for (var i = temp.Count -1; i >= 0; --i)
        {
            stacks[to].Push(temp[i]);
        }
    }

    private static int CratePosition(int index) => 1 + index * 4;
}