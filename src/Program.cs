using System.Net;
using System.Net.Sockets;


internal class Program
{
    public static void Main(string[] args)
    {
        Console.Write("$ ");

        var command = Console.ReadLine();
        Console.WriteLine($"{command}: command not found");
    }
}
