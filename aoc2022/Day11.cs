using System.Reflection.Emit;

namespace aoc2022;

public class Day11 : IAocRunner
{
    public enum OperationKind
    {
        ADD,
        MULTIPLY
    }

    public enum OperandType
    {
        SELF,
        NUMBER
    }

    public class Operation
    {
        public OperationKind Kind { get; set; }
        public OperandType Operand { get; set; }
        public long Amount { get; set; }
        public Operation(OperationKind kind, OperandType operand, long amount = 0)
        {
            Kind = kind;
            Operand = operand;
            Amount = amount;
        }
        public override string ToString()
        {
            return $"Operation {(Kind == OperationKind.ADD ? '+' : '*')} {(Operand == OperandType.SELF ? 'o' : Amount.ToString())}";
        }
    }

    public class Test
    {
        public long Divisor { get; set; }
        public int OnTrue { get; set; }
        public int OnFalse { get; set; }
        public Test(long divisor)
        {
            Divisor = divisor;
        }
    }

    public class Monkey
    {
        public int Id { get; set; }
        public List<long> Items { get; set; } = new();
        public Operation Operation { get; set; }
        public Monkey(int id) { Id = id; }
        public Test Test { get; set; }
        public int Inspected { get; set; }
        public override string ToString()
        {
            return $"Monkey {Id} {Inspected} [{string.Join(",", Items)}]";
        }
    }

    public override Task Run(string variant)
    {
        Monkey? currentMonkey = null;
        List<Monkey> monkeys = new();
        int NUM_ROUNDS = /* 20 */ 10000;

        foreach (var line in System.IO.File.ReadLines($"day11/{variant}.txt"))
        {
            var l = line;

            if (l.Length != 0)
            {
                if (l.StartsWith("Monkey"))
                {
                    if (currentMonkey != null)
                    {
                        monkeys.Add(currentMonkey);
                    }
                    currentMonkey = new Monkey(int.Parse(l.Replace(":", "").Split(" ")[1]));
                }
                else if (l.Contains("Starting"))
                {
                    currentMonkey.Items = l.Split(": ")[1]
                        .Split(",", StringSplitOptions.TrimEntries)
                        .Select(x => long.Parse(x))
                        .ToList();
                }
                else if (l.Contains("Operation"))
                {
                    var parts = l.Split(": new = old ")[1].Split(" ", StringSplitOptions.TrimEntries);
                    var opType = parts[1] == "old" ? OperandType.SELF : OperandType.NUMBER;
                    currentMonkey.Operation = new Operation(
                        parts[0] == "*" ? OperationKind.MULTIPLY : OperationKind.ADD,
                        opType,
                        opType == OperandType.NUMBER ? int.Parse(parts[1]) : 0);
                }
                else if (l.Contains("Test"))
                {
                    currentMonkey.Test = new Test(int.Parse(l.Split(" ").Last()));
                }
                else if (l.Contains("If true"))
                {
                    currentMonkey.Test.OnTrue = int.Parse(l.Split(" ").Last());
                }
                else if (l.Contains("If false"))
                {
                    currentMonkey.Test.OnFalse = int.Parse(l.Split(" ").Last());
                    Console.WriteLine(currentMonkey.Operation.ToString());
                }
            }
        }

        monkeys.Add(currentMonkey);

        var divisor = monkeys.Select(x => x.Test.Divisor).Aggregate(1L, (a, b) => a * b);

        for (var round = 0; round < NUM_ROUNDS; ++round)
        {
            foreach (var monkey in monkeys)
            {
                foreach (var item in monkey.Items)
                {
                    var operand = monkey.Operation.Operand == OperandType.SELF ? item : monkey.Operation.Amount;
                    //Console.WriteLine($" -- before {item} {operand}");
                    var newLevel = monkey.Operation.Kind == OperationKind.MULTIPLY ?
                        item * operand : item + operand;

                    // Part 1
                    //newLevel = (int)Math.Floor((decimal)(newLevel / 3));
                    // Part 2
                    newLevel %= divisor;

                    monkey.Inspected++;
                    var testResult = newLevel % monkey.Test.Divisor == 0;
                    var targetId = testResult ? monkey.Test.OnTrue : monkey.Test.OnFalse;
                    var targetMonkey = monkeys.First(x => x.Id == targetId);
                    targetMonkey.Items = targetMonkey.Items.Concat(new List<long>() { newLevel }).ToList();
                    //Console.WriteLine($"  {monkey.Id} {newLevel} {testResult} {targetId} : [{string.Join(",", targetMonkey.Items)}]");
                }
                monkey.Items = new List<long>();
            }
            //Console.WriteLine($"Round {round + 1}");
            /*foreach (var monkey in monkeys)
            {
                Console.WriteLine($"{monkey}");
            }*/
        }

        foreach (var monkey in monkeys)
        {
            Console.WriteLine($"{monkey}");
        }

        long monkeyBusiness = monkeys.Select(x => x.Inspected).OrderByDescending(x => x).Take(2).Aggregate(1L, (a, b) => a * b);
        Console.WriteLine(monkeyBusiness);

        return Task.CompletedTask;
    }
}