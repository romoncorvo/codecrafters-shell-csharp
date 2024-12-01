internal static class Program
{
    public static void Main()
    {
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
        {
            Exit(command);
        }
        else if (builtin == "echo")
        {
            if (command.Length > 1)
                Console.WriteLine(string.Join(' ', command[1..]));
        }
        else
        {
            Console.WriteLine($"{userInput}: command not found");
        }
    }

    private static void Exit(string[] command)
    {
        if (command.Length > 1 && int.TryParse(command[1], out var n) && n is >= 0 and <= 255)
            Environment.Exit(n);

        Environment.Exit(0);
    }
}