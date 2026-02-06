class Program
{
    static void Main()
    {
        while (true)
        {
            Console.Write("$ ");

            string? command = Console.ReadLine();
            string[] commandSplit = command.Split(" ");

            switch (commandSplit[0])
            {
                case "exit":
                    return;
                case "echo":
                    
                    System.Console.WriteLine(string.Join(" ", commandSplit[1..]));
                    break;
                default:
                    System.Console.WriteLine($"{command}: command not found");
                    break;
            }

        }
    }
}
