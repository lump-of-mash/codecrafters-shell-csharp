class Program
{
    static void Main()
    {
        while (true)
        {
            Console.Write("$ ");

            string? command = Console.ReadLine();

            if(command == "exit")
            {
                break;
            }

            System.Console.WriteLine($"{command}: command not found");
        }
    }
}
