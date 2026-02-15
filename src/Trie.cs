internal class Trie
{
    private class TrieNode
    {
        public Dictionary<char, TrieNode> Children { get; }
        public bool IsEndOfWord { get; set; }
        public TrieNode()
        {
            Children = new Dictionary<char, TrieNode>();
            IsEndOfWord = false;
        }
    }
    private TrieNode _root;
    public Trie(string[] words)
    {
        _root = new TrieNode();

        foreach(var word in words)
            Insert(word);
    }
    public void Insert(string word)
    {
        var currentNode = _root;
        foreach (var ch in word)
        {
            if(!currentNode.Children.ContainsKey(ch))
                currentNode.Children[ch] = new TrieNode();
            currentNode = currentNode.Children[ch];
        }
        currentNode.IsEndOfWord = true;
    }
    public bool Autocomplete(string prefix, out string completeWord)
    {
        var currentNode = _root;
        completeWord = "";
        foreach (var ch in prefix)
        {
            if(!currentNode.Children.ContainsKey(ch))
                return false;
            
            currentNode = currentNode.Children[ch];
        }

        while(!currentNode.IsEndOfWord)
        {
            var firstChar = currentNode.Children.Keys.First();
            completeWord += firstChar;
            currentNode = currentNode.Children[firstChar];
        }

        return true;
    }
}