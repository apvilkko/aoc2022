namespace aoc2022;

public class Day10 : IAocRunner
{
    private List<int> _signalStrengths = new();
    private List<List<char>> _crt = new();
    private int _cycle = 0;
    private int ROWS = 6;
    private int COLUMS = 40;

    public override Task Run(string variant)
    {
		for (var y = 0; y < ROWS; ++y)
        {
            var row = new List<char>();
            for (var xx = 0; xx < COLUMS; ++xx)
            {
                row.Add('.');
            }
            _crt.Add(row);
        }
        
        var x = 1;
		
		foreach (var line in System.IO.File.ReadLines($"day10/{variant}.txt"))
        {
            var l = line.Trim();

            if (l.Length != 0)
            {
				if (l.StartsWith("noop"))
				{
                    CheckCycle(x);
                } else if (l.StartsWith("addx"))
				{
                    CheckCycle(x);
                    CheckCycle(x);
                    x += int.Parse(l.Split(" ")[1]);
				}
                
            }
        }

        Console.WriteLine($"signal strength sum {_signalStrengths.Sum()}");

        foreach (var line in _crt)
        {
            Console.WriteLine(string.Join("", line));
        }

        return Task.CompletedTask;
    }

    private void CheckCycle(int x)
    {
        var c = _cycle;
        var crtX = c % COLUMS;
        var crtY = (int)Math.Floor((decimal)(c / COLUMS));
        if (Math.Abs(x - crtX) <= 1)
        {
            _crt[crtY][crtX] = '#';
        }

        _cycle++;
        if ((_cycle + 20) % 40 == 0)
        {
            var strength = _cycle * x;
            _signalStrengths.Add(strength);
        }
    }
}