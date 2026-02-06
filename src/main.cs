class Program
{
    static void Main()
    {
        while (true)
        {
            Console.Write("$ ");
            string? command = Console.ReadLine();
            System.Console.WriteLine($"{command}: command not found");
        }
    }
}
