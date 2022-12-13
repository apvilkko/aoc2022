using System.Text.Json.Nodes;

namespace aoc2022;

public class Day13 : IAocRunner
{
    public class ListOrNumber
    {
        public int Number { get; set; } = int.MaxValue;
        public bool Divider { get; set; } = false;
        public List<ListOrNumber>? List { get; set; }
        public ListOrNumber(int value)
        {
            Number = value;
        }
        public ListOrNumber(List<ListOrNumber> list)
        {
            List = list;
        }

        public bool IsNumber() => Number != int.MaxValue;
        public bool IsList() => List != null;

        public override string ToString()
        {
            return IsNumber() ? Number.ToString() : $"[{string.Join(",", List)}]";
        }
    }

    private ListOrNumber Deserialize(string str)
    {
        var arr = JsonNode.Parse(str);
        if (arr.GetType() == typeof(JsonArray))
        {
            return new ListOrNumber(((JsonArray)arr).Select(x => Deserialize(x.ToString())).ToList());
        }
        return new ListOrNumber((int)arr);
    }

    // 1 = right order, -1 = wrong order
    private int Compare(ListOrNumber left, ListOrNumber right)
    {
        if (left.IsNumber() && right.IsNumber())
        {
            return left.Number < right.Number ? 1 :
                left.Number > right.Number ? -1 : 0;
        }
        else if (left.IsList() && right.IsList())
        {
            var maxLen = Math.Max(left.List.Count, right.List.Count);
            for (var i = 0; i < maxLen; ++i)
            {
                if (i > left.List.Count - 1)
                {
                    return 1;
                }
                if (i > right.List.Count - 1)
                {
                    return -1;
                }
                var l = left.List[i];
                var r = right.List[i];
                var result = Compare(l, r);
                if (result != 0)
                {
                    return result;
                }
            }
            return 0;
        }
        else
        {
            var l = left;
            var r = right;
            if (left.IsNumber())
            {
                l = new ListOrNumber(new List<ListOrNumber>() { new ListOrNumber(l.Number) });
            }
            if (right.IsNumber())
            {
                r = new ListOrNumber(new List<ListOrNumber>() { new ListOrNumber(r.Number) });
            }
            return Compare(l, r);
        }
    }

    public override Task Run(string variant)
    {
        var inputFile = File.ReadAllText($"day13/{variant}.txt").Trim();

        // part 1 input
        var input = inputFile
            .Split(Environment.NewLine + Environment.NewLine)
            .Select(pair => pair
                .Split(Environment.NewLine)
                .Select(pairItem => Deserialize(pairItem))
                .ToList()
            )
            .ToList();

        // part 2 input
        var input2 = inputFile
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(item => Deserialize(item))
            .ToList();

        var divider1 = Deserialize("[[2]]");
        var divider2 = Deserialize("[[6]]");
        divider1.Divider = true;
        divider2.Divider = true;
        input2.Add(divider1);
        input2.Add(divider2);

        var part1 = input
            .Select((pair, i) => Compare(pair[0], pair[1]) == 1 ? i + 1 : 0)
            .Sum();

        Console.WriteLine($"right indexes sum {part1}");

        input2.Sort(Compare);
        input2.Reverse();
        var part2 = input2
            .Select((x, i) => (x, i))
            .Where(x => x.Item1.Divider)
            .Select(x => x.Item2 + 1)
            .Aggregate(1, (a, b) => a * b);

        Console.WriteLine($"part2 {part2}");

        return Task.CompletedTask;
    }
}