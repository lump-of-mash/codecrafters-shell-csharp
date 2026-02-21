internal class InputReader
{
    private string _input = "";
    public string AutocompleteCommand(List<string> wordsToAutoComplete, List<string> history)
    {
        _input = "";
        wordsToAutoComplete.AddRange(CommandHandler.GetExecutableFileNames());

        int currentHistory = history.Count;
        bool isFirstTabPress = true;
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Tab)
            {
                string[] completeWords = wordsToAutoComplete.Where(w => w.StartsWith(_input, StringComparison.OrdinalIgnoreCase)).Distinct().ToArray();
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
                        string commonPrefix = GetLongestCommonPrefix(completeWords);

                        if (commonPrefix.Length == 0)
                        {
                            Console.Write("\a");
                            isFirstTabPress = false;
                            continue;
                        }
                    }
                }
                else
                {
                    Console.Write("\a");
                }

                isFirstTabPress = true;
            }
            else if (key.Key == ConsoleKey.UpArrow && currentHistory > 0)
            {
                while (_input.Length > 0)
                {
                    Console.Write("\b \b");
                    _input = _input.Substring(0, _input.Length - 1);
                }

                _input = history[--currentHistory];
                Console.Write(history[currentHistory]);
                //currentHistory = Math.Max(currentHistory - 1, 0);
            }
            else if (key.Key == ConsoleKey.DownArrow && currentHistory < history.Count - 1)
            {
                while (_input.Length > 0)
                {
                    Console.Write("\b \b");
                    _input = _input.Substring(0, _input.Length - 1);
                }

                _input = history[++currentHistory];
                Console.Write(history[currentHistory]);
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

        string GetLongestCommonPrefix(string[] completeWords)
        {
            var longestWord = completeWords.OrderByDescending(w => w.Length).First();
            string commonPrefix = "";
            for (int i = _input.Length; i < longestWord.Length; i++)
            {
                char currentChar = longestWord[i];
                if (completeWords.All(w => w.Length > i && char.ToLower(w[i]) == char.ToLower(currentChar)))
                    commonPrefix += currentChar;
                else
                    break;
            }

            foreach (var ch in commonPrefix)
            {
                Console.Write(ch);
                _input += ch;
            }

            return commonPrefix;
        }
    }
}

