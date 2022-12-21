namespace aoc2022;
using System.Collections.Generic;

public class Day20 : IAocRunner
{
    public override Task Run(string variant)
    {
        var input = File.ReadAllText($"day20/{variant}.txt")
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select((x, i) => (int.Parse(x), i))
            .ToList();

        var ll = new LinkedList<(int, int)>(input);

        var size = input.Count;

        for (var i = 0; i < size; ++i) {
            var item = ll.First;
            int currentIndex = item.Value.Item2;
            while (currentIndex != i) {
                item = item.Next;
                currentIndex = item.Value.Item2;
            }
            //Console.WriteLine($"{currentIndex} {item.Value}");
            var current = item;
            var amount = item.Value.Item1;
            if (amount == 0) {
                continue;
            }
            var backwards = amount < 0;
            var repeats = Math.Abs(amount) % size;
            for (var j = 0; j < repeats; ++j) {
                if (backwards) {
                    item = item.Previous;
                    if (item == null) {
                        //Console.WriteLine("  -- wrapped to last");
                        item = ll.Last;
                    }
                } else {
                    item = item.Next;
                    if (item == null) {
                        //Console.WriteLine("  -- wrapped to first");
                        item = ll.First;
                    }
                }
            }

            var value = current.Value;
            //Console.WriteLine($"removing {current.Value}");
            ll.Remove(current);

            //Console.WriteLine($"adding {value} {(backwards ? "before" : "after")} {item.Value}");
            if (backwards) {
                ll.AddBefore(item, value);
                // If it got placed first, it wraps to last
                if (ll.First.Value == value) {
                    ll.RemoveFirst();
                    ll.AddLast(value);
                }
            } else {
                ll.AddAfter(item, value);
            }

            //Console.WriteLine($"  {string.Join(",", ll.Select(x => x.Item1).ToList())}");
            //Console.WriteLine("");
        }

        var resultList = new List<(int, int)>(ll);
        var zeroIndex = resultList.FindIndex(x => x.Item1 == 0);
        var part1 = new List<int>(){1000,2000,3000}.Select(i => {
            var val = resultList[(zeroIndex + i) % resultList.Count].Item1;
            Console.WriteLine($"{val}");
            return val;
        }).Sum();
        Console.WriteLine($"part1 {part1}");

        // 2785 too low

        return Task.CompletedTask;
    }
}