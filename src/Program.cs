internal static class Program
{
    public static int Main(string[] args)
    {
        while (true)
        {
            Console.Write("$ ");

            var input = Console.ReadLine();

            if (input != null)
            {
                var inputs = input.Split(' ');
                var command = inputs[0];

                if (command == "exit")
                {
                    if (inputs.Length > 1 && int.TryParse(inputs[1], out var n) && n is >= 0 and <= 255)
                        return n;

                    return 0;
                }

                Console.WriteLine($"{input}: command not found");
            }
        }
    }
}