using aoc2022;

if (args.Length == 0)
{
    System.Console.WriteLine("Please enter dayNN");
    return 1;
}

var day = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(args[0]);
var variant = args.Length > 1 ? args[1] : "input"; 

System.Console.WriteLine($"Running {day} {variant}");

var type = Type.GetType("aoc2022." + day);
if (type == null)
{
    System.Console.WriteLine($"No runner for {day}");
    return 1;
}
var runner = (IAocRunner)Activator.CreateInstance(type);
if (runner == null)
{
    System.Console.WriteLine($"No instance for {day}");
    return 1;
}

await runner.Run(variant);

return 0;