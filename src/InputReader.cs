internal class InputReader
{
    private string _input = "";
    public string AutocompleteCommand(List<string> wordsToAutoComplete)
    {
        _input = "";
        wordsToAutoComplete.AddRange(CommandHandler.GetExecutableFileNames());

        bool isFirstTabPress = true;
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Tab)
            {
                string[] completeWords = wordsToAutoComplete.Where(w => w.StartsWith(_input, StringComparison.OrdinalIgnoreCase)).ToArray();
                Array.Sort(completeWords);

                if (completeWords.Length == 1)
                {
                    while (_input.Length > 0)
                    {
                        Console.Write("\b \b");
                        _input = _input.Substring(0, _input.Length - 1);
                    }
                    var completeWord = completeWords[0] + " ";

                    _input += completeWord;
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
                _input = _input.Substring(0, _input.Length - 1);
                Console.Write("\b \b");
            }
            else if (!char.IsControl(key.KeyChar))
            {
                _input += key.KeyChar;
                Console.Write(key.KeyChar);
            }
        } while (key.Key != ConsoleKey.Enter);

        System.Console.WriteLine();
        return _input;
    }
}

