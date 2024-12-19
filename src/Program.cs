using System.Diagnostics;
using System.Text.RegularExpressions;

internal static partial class Program
{
    private static string[] _paths = [];
    private static readonly string HomeDirectory = Environment.GetEnvironmentVariable("HOME")!;

    public static void Main()
    {
        _paths = Environment.GetEnvironmentVariable("PATH")?.Split(Path.PathSeparator) ?? [];
        while (true) Repl();
    }

    private static void Repl()
    {
        Console.Write("$ ");

        var userInput = Console.ReadLine();

        if (userInput == null)
            return;

        var builtin = userInput.Split(' ')[0];

        var quotedCommands = QuoteCapture()
            .Matches(userInput)
            .Select(x => x.Captures[0].Value.Trim('\'').Trim('\"'))
            .ToArray();

        var command = quotedCommands.Length > 0
            ? new[] { builtin }.Concat(quotedCommands).ToArray()
            : userInput.Split(' ')
                .Where(x => x != "")
                .Select(x =>
                {
                    return new[] { "cat", "echo" }.Contains(builtin)
                        ? EscapeCharacter().Replace(x, match => match.Groups[1].Value)
                        : x;
                })
                .ToArray();

        if (builtin == "exit")
            Exit(command);

        else if (builtin == "echo")
            Echo(command);
        else if (builtin == "type")
            Type(command);
        else if (builtin == "pwd")
            PrintWorkingDirectory();
        else if (builtin == "cd")
            ChangeDirectory(command);
        else if (ExecutableInPath(builtin, out var location))
            if (builtin == "cat")
                foreach (var argument in command[1..])
                    Process.Start(location, "\"" + argument + "\"").WaitForExit();
            else
                Process.Start(location, string.Join(' ', command[1..])).WaitForExit();
        else
            Console.WriteLine($"{userInput}: command not found");
    }

    private static void PrintWorkingDirectory()
    {
        Console.WriteLine($"{Environment.CurrentDirectory}");
    }

    private static void ChangeDirectory(string[] command)
    {
        if (command.Length > 1 && command[1].StartsWith('.') &&
            TryNavigateToRelativeDirectory(command[1], out var newDirectory))
            Directory.SetCurrentDirectory(newDirectory);
        else if (command.Length > 1 && command[1].StartsWith('~'))
            Directory.SetCurrentDirectory(HomeDirectory);
        else if (command.Length > 1 && Directory.Exists(command[1]))
            Directory.SetCurrentDirectory(command[1]);
        else
            Console.WriteLine($"cd: {string.Join(' ', command[1..])}: No such file or directory");
    }

    private static bool TryNavigateToRelativeDirectory(string relativePath, out string newDirectory)
    {
        newDirectory =
            Environment.CurrentDirectory.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        var navigationPaths = relativePath.Split(Path.AltDirectorySeparatorChar);

        foreach (var navigation in navigationPaths)
        {
            if (navigation is "" or ".")
                continue;

            if (navigation == "..")
                newDirectory = newDirectory[..newDirectory.LastIndexOf(Path.AltDirectorySeparatorChar)];
            else
                newDirectory += $"{Path.AltDirectorySeparatorChar}{navigation}";
        }

        return Directory.Exists(newDirectory);
    }

    private static void Type(string[] command)
    {
        if (command.Length <= 1)
            return;

        var arguments = string.Join(' ', command[1..]);
        if (arguments == "echo")
            Console.WriteLine("echo is a shell builtin");
        else if (arguments == "exit")
            Console.WriteLine("exit is a shell builtin");
        else if (arguments == "type")
            Console.WriteLine("type is a shell builtin");
        else if (arguments == "pwd")
            Console.WriteLine("pwd is a shell builtin");
        else if (arguments == "cd")
            Console.WriteLine("cd is a shell builtin");
        else if (ExecutableInPath(arguments, out var location))
            Console.WriteLine($"{arguments} is {location}");
        else
            Console.WriteLine($"{arguments}: not found");
    }

    private static bool ExecutableInPath(string arguments, out string location)
    {
        location = "";
        foreach (var path in _paths)
            if (File.Exists($"{path}/{arguments}"))
            {
                location = path + "/" + arguments;
                return true;
            }

        return false;
    }

    private static void Echo(string[] command)
    {
        if (command.Length > 1)
            Console.WriteLine(string.Join(' ', command[1..]));
    }

    private static void Exit(string[] command)
    {
        if (command.Length > 1 && int.TryParse(command[1], out var n) && n is >= 0 and <= 255)
            Environment.Exit(n);

        Environment.Exit(0);
    }

    [GeneratedRegex("'([^']*)'|\"([^\"]*)\"")]
    private static partial Regex QuoteCapture();

    [GeneratedRegex(@"\\(.?)")]
    private static partial Regex EscapeCharacter();
}