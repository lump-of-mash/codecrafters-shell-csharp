using System.Text;
internal class InputReader
{
    private readonly StringBuilder _input = new();
    public string AutocompleteCommand(List<string> wordsToAutoComplete)
    {
        _input.Clear();
        wordsToAutoComplete.AddRange(CommandHandler.GetExecutableFileNames());

        bool isFirstTabPress = true;
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Tab)
            {
                string[] completeWords = wordsToAutoComplete.Where(w => w.StartsWith(_input.ToString(), StringComparison.OrdinalIgnoreCase)).ToArray();
                Array.Sort(completeWords);

                if (completeWords.Length == 1)
                {
                    while (_input.Length > 0)
                    {
                        Console.Write("\b \b");
                        _input.Remove(_input.Length - 1, 1);
                    }
                    var completeWord = completeWords[0] + " ";

                    _input.Append(completeWord);
                    Console.Write(completeWord);
                }
                else if (completeWords.Length > 1)
                {
                    if (isFirstTabPress == false)
                    {
                        Console.WriteLine();
                        System.Console.WriteLine(string.Join("  ", completeWords));
                        Console.Write("$ " + _input);
                    }
                    else
                    {
                        Console.Write("\a");
                        isFirstTabPress = false;
                        continue;
                    }

                }
                else
                {
                    Console.Write("\a");
                }

                isFirstTabPress = true;
            }
            else if (key.Key == ConsoleKey.Backspace && _input.Length > 0)
            {
                _input.Remove(_input.Length - 1, 1);
                Console.Write("\b \b");
            }
            else if (!char.IsControl(key.KeyChar))
            {
                _input.Append(key.KeyChar);
                Console.Write(key.KeyChar);
            }
        } while (key.Key != ConsoleKey.Enter);

        System.Console.WriteLine();
        return _input.ToString();
    }
}

