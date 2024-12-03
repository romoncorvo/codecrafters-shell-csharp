using System.Diagnostics;

internal static class Program
{
    private static string[] _paths = [];
    private static string _workingDirectory = Environment.CurrentDirectory;

    public static void Main()
    {
        _paths = Environment.GetEnvironmentVariable("PATH")?.Split(':') ?? [];
        while (true) Repl();
    }

    private static void Repl()
    {
        Console.Write("$ ");

        var userInput = Console.ReadLine();

        if (userInput == null)
            return;

        var command = userInput.Split(' ');
        var builtin = command[0];

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
            Process.Start(location, string.Join(' ', command[1..])).WaitForExit();
        else
            Console.WriteLine($"{userInput}: command not found");
    }

    private static void PrintWorkingDirectory()
    {
        Console.WriteLine($"{_workingDirectory}");
    }

    private static void ChangeDirectory(string[] command)
    {
        if (command.Length > 1 && Directory.Exists(command[1]))
            _workingDirectory = command[1];
        else
            Console.WriteLine($"cd: {string.Join(' ', command[1..])}: No such file or directory");
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
}