namespace aoc2022;
using MathNet.Symbolics;
using Expr = MathNet.Symbolics.Expression;

public class Day21 : IAocRunner
{
    private Dictionary<string, OperationOrNumber> _input = new();

    enum OperationType {
        Addition,
        Subtraction,
        Multiplication,
        Division
    }

    record OperationOrNumber {
        public record Operation(OperationType OperationType, List<string> Operands) : OperationOrNumber();
        public record Number(long Value) : OperationOrNumber();
        private OperationOrNumber() {}
    }

    private OperationOrNumber ParseOperation(string s) {
        var parts = s.Split(" ", StringSplitOptions.TrimEntries).ToList();
        var opType = parts[1] switch {
            "+" => OperationType.Addition,
            "-" => OperationType.Subtraction,
            "*" => OperationType.Multiplication,
            "/" => OperationType.Division,
            _ => throw new Exception("unknown operation")
        };
        return new OperationOrNumber.Operation(opType, new List<string>(){ parts[0], parts[2] });;
    }

    private OperationOrNumber ParseValue(string s) {
        var count = s.Split(" ").Count();
        return count switch {
            1 => new OperationOrNumber.Number(long.Parse(s)),
            _ => ParseOperation(s)
        };
    }

    private long ApplyOperation(OperationOrNumber.Operation o, bool part2 = false, bool root = false, long humn = 0) {
        var a = GetResult(o.Operands[0], part2, humn);
        var b = GetResult(o.Operands[1], part2, humn);
        if (root) {
            //Console.WriteLine($"  compare {a} {b}");
            return a == b ? 0 : -1;
        }
        return o.OperationType switch {
            OperationType.Addition => a + b,
            OperationType.Subtraction => a - b,
            OperationType.Multiplication => a * b,
            _ => a / b
        };
    }

    private long GetResult(string from, bool part2 = false, long humn = 0) {
        var item = _input[from];
        return item switch {
            OperationOrNumber.Number n => part2 && from == "humn" ? humn : n.Value,
            OperationOrNumber.Operation o => ApplyOperation(o, part2, from == "root", humn),
            _ => throw new Exception("not valid")
        };
    }

    public override Task Run(string variant)
    {
        foreach (var line in System.IO.File.ReadLines($"day21/{variant}.txt"))
        {
            var l = line.Trim();

            if (l.Length != 0)
            {
                var parts = l.Split(": ");
                _input[parts[0]] = ParseValue(parts[1]);
            }
        }

        Console.WriteLine($"part 1 {GetResult("root")}");

        var lhs = AsExpression(((OperationOrNumber.Operation)_input["root"]).Operands[0]);
        var rhs = AsExpression(((OperationOrNumber.Operation)_input["root"]).Operands[1]);
        var e1 = Infix.ParseOrThrow(lhs);
        var e2 = Infix.ParseOrThrow(rhs);
        var x = Expression.Symbol("x");

        Console.WriteLine($"part 2 solving {lhs} = {rhs}\nanswer: {SolveSimpleRoot(x, e1 - e2)}");

        return Task.CompletedTask;
    }

    private string AsExpression(string from) {
        var item = _input[from];
        return item switch {
            OperationOrNumber.Number n => from == "humn" ? "x" : n.Value.ToString(),
            OperationOrNumber.Operation o => "(" + AsExpression(o.Operands[0]) + (
                o.OperationType == OperationType.Addition ? "+" :
                o.OperationType == OperationType.Subtraction ? "-" :
                o.OperationType == OperationType.Multiplication ? "*" : "/"
            ) + Show(o.Operands[1]) + ")",
            _ => throw new Exception("not valid")
        };
    }

    private Expr SolveSimpleRoot(Expr variable, Expr expr)
    {
        // try to bring expression into polynomial form
        Expr simple = Algebraic.Expand(Rational.Numerator(Rational.Simplify(variable, expr)));

        // extract coefficients, solve known forms of order up to 1
        Expr[] coeff = Polynomial.Coefficients(variable, simple);
        switch (coeff.Length)
        {
            case 1: return Expr.Zero.Equals(coeff[0]) ? variable : Expr.Undefined;
            case 2: return Rational.Simplify(variable, Algebraic.Expand(-coeff[0] / coeff[1]));
            default: return Expr.Undefined;
        }
    }
}