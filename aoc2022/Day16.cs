using System.Text.RegularExpressions;

namespace aoc2022;

public class Day16 : IAocRunner
{
    private List<Valve> _valves = new();

    public class Valve
    {
        public string Name { get; set; }
        public int FlowRate { get; set; }
        public List<string> Tunnels { get; set; } = new();
        public Valve(string name, int flowRate, List<string> tunnels)
        {
            Name = name;
            FlowRate = flowRate;
            Tunnels = tunnels;
        }
        public override string ToString()
        {
            return $"{Name} {FlowRate} -> {string.Join(",", Tunnels)}";
        }
    }

    public override Task Run(string variant)
    {
        foreach (var line in System.IO.File.ReadLines($"day16/{variant}.txt"))
        {
            var l = line.Trim();

            if (l.Length != 0)
            {
                var m = Regex.Matches(l, @"Valve ([A-Z]+)\s.*flow rate=(\d+);.*valves?\s+(.*)")[0];
                var v = new Valve(m.Groups[1].ToString(),
                    int.Parse(m.Groups[2].ToString()),
                    m.Groups[3].ToString().Split(",", StringSplitOptions.TrimEntries).ToList());
                _valves.Add(v);
            }
        }

        foreach(var v in _valves)
        {
            v.Tunnels = v.Tunnels.OrderByDescending(x => _valves.Find(y => y.Name == x).FlowRate).ToList();
            Console.WriteLine(v);
        }

        var (dist, prev) = GetMaxPressure();

        /*foreach (var d in dist)
        {
            Console.WriteLine($"{d.Key} {d.Value}");
        }
        Console.WriteLine("\n");
        foreach (var p in prev)
        {
            Console.WriteLine($"{p.Key} {p.Value}");
        }*/

        //var goal = GetMaxPressure2(_valves.Find(x => x.Name == "AA"));
        //Console.WriteLine($"goal {goal}");

        foreach (var v in _valves)
        {
            _visited[v.Name] = 0;
        }
        Traverse(_valves.Find(x => x.Name == "AA"), new List<string>() { "AA" });

        return Task.CompletedTask;
    }

    private (Dictionary<string, int>, Dictionary<string, string>) GetMaxPressure()
    {
        var dist = new Dictionary<string, int>();
        var prev = new Dictionary<string, string>();

        var q = new List<Valve>();

        foreach (var v in _valves)
        {
            q.Add(v);
            dist.Add(v.Name, int.MaxValue);
        }

        var source = _valves.Find(v => v.Name == "AA");
        dist[source.Name] = source.FlowRate;

        while (q.Count > 0)
        {
            var u = q.OrderBy(v => dist[v.Name]).First();
            q.Remove(u);
            //Console.WriteLine($"  Inspecting {u}");

            var neighbors = u.Tunnels.Where(t => q.Find(v => v.Name == t) != null);
            foreach (var v in neighbors)
            {
                var vNode = _valves.Find(x => x.Name == v);
                //Console.WriteLine($"    Neighbor {vNode}");
                var alt = dist[u.Name] + vNode.FlowRate; // length?
                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u.Name;
                }
            }
        }

        return (dist, prev);
    }

    private int GetPathLength(Dictionary<string, string> parents, Valve from, Valve to)
    {
        Console.WriteLine($"-> GetPathLength {from} - {to}");
        var x = from.Name;
        var len = 0;
        while (true)
        {
            var ok = parents.TryGetValue(x, out var p);
            if (!ok)
            {
                break;
            }
            x = p;
            len++;
        }
        Console.WriteLine($"<- len {len}");
        return len;
    }

    private Dictionary<string, int> _visited = new();
    private HashSet<string> _opened = new();
    private HashSet<string> _paths = new();
    private int MAX_PATH = 30;

    private bool HasCycle(int cycle, string ps)
    {
        if (ps.Length >= (cycle * 2 * 2))
        {
            var fullCycle = ps.Substring(ps.Length - (cycle * 2)) ==
                ps.Substring(ps.Length - (cycle * 2 * 2), cycle * 2);
            var partialCycle = cycle > 2 &&
                ps.Substring(ps.Length - (cycle * 2) + 2) ==
                ps.Substring(ps.Length - (cycle * 2 * 2) + 2, cycle * 2 - 2);
            return fullCycle || partialCycle;
        }
        return false;
    }

    private void Traverse(Valve v, List<string> path)
    {
        if (v.FlowRate > 0)
        {
            _opened.Add(v.Name);
        }
        _visited[v.Name]++;

        //Console.WriteLine($"{v} path={string.Join(",", path)}");
        var ps = string.Join("", path);
        for (var i = 2; i < 6; ++i)
        {
            if (HasCycle(i, ps))
            {
                //Console.WriteLine($"    has cycle {i}");
                return;
            }
        }

        if (path.Count >= 30)
        {
            //Console.WriteLine($"path reached {string.Join(",", path)}");
            return;
        }

        var tunnels = v.Tunnels.OrderBy(x => _opened.Contains(x)).ToList();
        //Console.WriteLine($"  tunnels {string.Join(",", tunnels)}");

        foreach (var w in v.Tunnels)
        {
            //Console.WriteLine($"   tunnels {v.Name}-{w} {_visited[w]} {_opened.Contains(w)}");
            if (/*_visited[w] <= MAX_PATH &&*/ path.Count(x => x == w) <= MAX_PATH/2)
            {
                var wNode = _valves.Find(x => x.Name == w);
                var newPath = path.Concat(new List<string>() { wNode.Name }).ToList();
                var pathStr = string.Join("", newPath);
                if (!_paths.Contains(pathStr))
                {
                    _paths.Add(pathStr);
                    Traverse(wNode, newPath);
                }
                
            }
            //Console.WriteLine($"   <-- after tunnels {v.Name}-{w} {_visited[w]}");
        }
    }

    private Valve? GetMaxPressure2(Valve source)
    {
        var visited = new Dictionary<string, int>();
        var parents = new Dictionary<string, string>();
        foreach (var v in _valves)
        {
            visited.Add(v.Name, 0);
        }

        var q = new Stack<Valve>();
        //var q = new Queue<Valve>();
        visited[source.Name] = 1;
        q.Push(source);

        while (q.Count > 0)
        {
            var v = q.Pop();
            Console.WriteLine($"Inspect {v} q {q.Count}");

            if (visited[v.Name] <= 1)
            {
                visited[v.Name]++;
                foreach (var w in v.Tunnels)
                {
                    var wNode = _valves.Find(x => x.Name == w);
                    q.Push(wNode);
                }
            }

            /*var pathLength = GetPathLength(parents, v, source);
            if (pathLength >= 30)
            {
                return v;
            }

            foreach (var w in v.Tunnels)
            {
                Console.WriteLine($" visited {w} {visited[w]}");
                if (visited[w] <= 1)
                {
                    visited[w]++;
                    parents[w] = v.Name;
                    var wNode = _valves.Find(x => x.Name == w);
                    q.Enqueue(wNode);
                }
            }*/
        }
        return null;
    }
}