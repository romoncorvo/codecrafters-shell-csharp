internal static class Program
{
    public static void Main(string[] args)
    {
        while (true)
        {
            Console.Write("$ ");

            var command = Console.ReadLine();
            Console.WriteLine($"{command}: command not found");
        }
    }
}