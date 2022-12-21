namespace aoc2022;

public class Day20 : IAocRunner
{
    public class LinkedNode {
        public long Value { get; set; }
        public int OriginalIndex { get; set; }
        public LinkedNode? Prev { get; set; } = null;
        public LinkedNode? Next { get; set; } = null;
        public LinkedNode(long value, int index) {
            Value = value;
            OriginalIndex = index;
        }
        public LinkedNode(LinkedNode other) {
            Value = other.Value;
            OriginalIndex = other.OriginalIndex;
            Prev = other.Prev;
            Next = other.Next;
        }
        public override string ToString()
        {
            return $"[{Value} ({OriginalIndex})]";
        }
        public bool Is(LinkedNode other) =>
            other.Value == Value && other.OriginalIndex == OriginalIndex;
    }

    private static void Print(LinkedNode start) {
        var n = start;
        do {
            Console.Write($"{n.Value} ");
            n = n.Next;
        } while (n != start);
        Console.Write("\n");
    }

    private LinkedNode CreateList(List<(long, int)> input) {
        LinkedNode? root = null;
        LinkedNode? prev = null;
        for (var i = 0; i < input.Count; ++i) {
            var node = new LinkedNode(input[i].Item1, input[i].Item2);
            if (i == 0) {
                root = node;
            }
            if (prev != null) {
                node.Prev = prev;
                node.Prev.Next = node;
            }
            prev = node;
        }

        root.Prev = prev;
        root.Prev.Next = root;

        return root;
    }

    public LinkedNode Mix(LinkedNode root, int size) {
        //Print(root);
        for (var i = 0; i < size; ++i) {
            var c = root;
            int currentIndex = c.OriginalIndex;
            while (currentIndex != i) {
                c = c.Next;
                currentIndex = c.OriginalIndex;
            }
            //Console.WriteLine($"processing {c}");
            var amount = c.Value;
            if (amount == 0) {
                continue;
            }
            var backwards = amount < 0;
            var repeats = Math.Abs(amount) % (size-1);
            for (var j = 0; j < repeats; ++j) {
                var prev = c.Prev;
                var next = c.Next;
                var curr = c;
                //Console.WriteLine($"  -- {c} p {prev} n {next}");
                int newRootIndex = -1;

                if (curr.Is(root)) {
                    //Console.WriteLine("root 1");
                    newRootIndex = c.Next.OriginalIndex;
                }

                if (backwards) {
                    if (prev.Is(root)) {
                        //Console.WriteLine("root prev");
                        newRootIndex = curr.Prev.OriginalIndex;
                    }

                    next.Prev = prev;
                    c.Next = prev;
                    prev.Next = next;
                    prev.Prev.Next = c;
                    c.Prev = prev.Prev;
                    prev.Prev = c;

                    //Console.WriteLine($"  back {c.Prev.Prev} <- {c.Prev} <- {c} -> {c.Next} -> {c.Next.Next}");
                } else {
                    if (next.Next.Is(root)) {
                        //Console.WriteLine("root next");
                        newRootIndex = curr.OriginalIndex;
                    }

                    next.Prev = prev;
                    c.Next = next.Next;
                    prev.Next = next;
                    next.Next.Prev = c;
                    c.Prev = next;
                    next.Next = c;

                    //Console.WriteLine($"  forw {c.Prev.Prev} <- {c.Prev} <- {c} -> {c.Next} -> {c.Next.Next}");
                }
                if (newRootIndex > -1) {
                    var nn = root;
                    int k = nn.OriginalIndex;
                    while (k != newRootIndex) {
                        nn = nn.Next;
                        k = nn.OriginalIndex;
                    }
                    //Console.WriteLine($"  *** setting new root to index {newRootIndex} {nn}");
                    root = nn;
                }
            }
            //Print(root);
        }
        return root;
    }

    private long GetCoord(LinkedNode root) {
        var n = root;
        var l = n.Value;
        while (l != 0) {
            n = n.Next;
            l = n.Value;
        }
        long sum = 0;
        for (var i = 1; i <= 3000; ++i) {
            n = n.Next;
            if (i == 1000 || i == 2000 || i == 3000) {
                Console.WriteLine($"{i} {n}");
                sum += n.Value;
            }
        }
        return sum;
    }

    public override Task Run(string variant)
    {
        var input = File.ReadAllText($"day20/{variant}.txt")
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select((x, i) => (long.Parse(x), i))
            .ToList();

        var root = CreateList(input);
        var size = input.Count;

        root = Mix(root, size);
        var sum = GetCoord(root);

        Console.WriteLine($"part 1: {sum}");

        // part 2
        var key = 811589153;
        var input2 = File.ReadAllText($"day20/{variant}.txt")
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select((x, i) => (long.Parse(x) * key, i))
            .ToList();
        root = CreateList(input2);

        for (var i = 0; i < 10; ++i) {
            root = Mix(root, size);
            Print(root);
        }
        sum = GetCoord(root);

        Console.WriteLine($"part 2: {sum}");

        return Task.CompletedTask;
    }
}