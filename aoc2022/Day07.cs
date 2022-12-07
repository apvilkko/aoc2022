namespace aoc2022;

public class Day07 : IAocRunner
{
    private string _currentFullPath = "";

    public override Task Run(string variant)
    {
        var currentDir = new Stack<string>();
        var dirSizes = new Dictionary<string, int>();

        foreach (var line in System.IO.File.ReadLines($"day07/{variant}.txt"))
        {
            var l = line.Trim();

            if (l.Length != 0)
            {
                if (l.StartsWith("$"))
                {
                    var command = l.Substring(2);
                    if (command.StartsWith("cd"))
                    {
                        var dir = command.Substring(3);
                        //Console.WriteLine($"cd '{dir}'");
                        if (dir == "/")
                        {
                            currentDir.Clear();
                        }
                        else if (dir == "..")
                        {
                            currentDir.Pop();
                        }
                        else
                        {
                            currentDir.Push(dir);
                        }
                    }
                    else if (command.StartsWith("ls"))
                    {
                        UpdateFullPath(currentDir);
                        //Console.WriteLine($"ls in {_currentFullPath}");
                    }
                    else
                    {
                        throw new Exception("unknown command");
                    }
                }
                else
                {
                    // Listing item
                    if (l.StartsWith("dir "))
                    {
                        if (!dirSizes.ContainsKey(_currentFullPath))
                        {
                            dirSizes[_currentFullPath] = 0;
                        }
                    }
                    else
                    {
                        var parts = l.Split(" ");
                        var filesize = int.Parse(parts[0]);
                        var filename = parts[1];
                        if (!dirSizes.ContainsKey(_currentFullPath))
                        {
                            dirSizes[_currentFullPath] = 0;
                        }
                        dirSizes[_currentFullPath] += filesize;
                    }
                }
            }
        }

        /*foreach (var key in dirSizes.Keys)
        {
            Console.WriteLine($"{key}: {dirSizes[key]}");
        }*/

        // Get all directories and their complete sizes
        var allDirNames = dirSizes.Keys;
        Console.WriteLine($"all dir names: {string.Join(",", allDirNames)}");

        var fullSizes = new Dictionary<string, int>();
        foreach (var dirName in allDirNames)
        {
            var keys = dirSizes.Keys.Where(k => k.Length > 0 && dirName.Length > 0 && k.StartsWith(dirName));
            var dirFullSize = keys.Sum(k => dirSizes[k]);
            //Console.WriteLine($"dirName: {dirName}, keys: {string.Join(",", keys)}, size: {dirFullSize}");

            fullSizes[dirName] = dirFullSize;
        }

        var part1 = fullSizes.Values.Where(x => x <= 100000).Sum();
        Console.WriteLine($"folders at most 100000 sum {part1}");

        // Part 2
        var TOTAL_DISK_SPACE = 70000000;
        var NEED_SPACE = 30000000;

        // fullSizes["/"] contains only root level direct files' sizes now
        var rootKeys = fullSizes.Keys.Where(x => x == "/" || !x.Contains("/"));
        var rootSize = rootKeys.Sum(x => fullSizes[x]);
        var freeSpace = TOTAL_DISK_SPACE - rootSize;
        var spaceRequired = NEED_SPACE - freeSpace;
        Console.WriteLine($"root size {rootSize}, free space {freeSpace}, need {spaceRequired} more for update");

        var sorted = fullSizes.ToList().OrderByDescending(x => x.Value);
        var bigger = sorted.Where(x => x.Value > spaceRequired);
        var deletionCandidate = bigger.Last();
        Console.WriteLine($"Should delete folder {deletionCandidate.Key} with size {deletionCandidate.Value}");

        return Task.CompletedTask;
    }

    private void UpdateFullPath(Stack<string> currentDir)
    {
        var fullDir = currentDir.ToList();
        fullDir.Reverse();
        _currentFullPath = string.Join("/", fullDir);
        if (_currentFullPath.Length == 0) {
            _currentFullPath = "/";
        }
    }
}