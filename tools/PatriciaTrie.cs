using System.Text;

public class PatriciaTrie
{
    public PatriciaTrie() => Root = new PatriciaTrieNode("");

    public PatriciaTrieNode Root { get; }

    public int? FindValue(string key) => Root.FindValue(key);

    public void Insert(string key, int value)
    {
        if (Root.Childs.Count == 0)
            Root.Childs.Add(key[0], new PatriciaTrieNode(key, value));
        else
            Root.Add(key, value);
    }
    
    public override string ToString() => Walk(Root, 1);

    public static string Walk(PatriciaTrieNode node, int level)
    {
        if (node == null)
            return string.Empty;

        var result = new StringBuilder();

        result.Append($"[{node.Key}");
        if (node.Value.HasValue)
            result.Append($":{node.Value.Value.ToString("X")}");
        foreach (var child in node.Childs)
        {
            result.Append(Walk(child.Value, level + 1));
        }
        result.Append("]");

        return result.ToString();
    }
}

public class PatriciaTrieNode
{
    public PatriciaTrieNode(string key, int? value = null, Dictionary<char, PatriciaTrieNode> childs = null)
    {
        Key = key;
        Value = value;
        Childs = childs ?? new Dictionary<char, PatriciaTrieNode>();
    }

    public Dictionary<char, PatriciaTrieNode> Childs { get; set; }

    public string Key { get; private set; }

    public int? Value { get; private set; }

    public bool Find(string key) => FindValue(key).HasValue;
    
    public int? FindValue(string key) 
    {
        if (!key.StartsWith(Key))
            return null;

        if (key.Length == Key.Length)
            return Value;

        if (Childs.TryGetValue(key[Key.Length], out var child))
        {
            return child.FindValue(key.Substring(Key.Length));
        }
        return null;
    }

    public void Add(string key, int value)
    {
        var i = 0;
        while (i < key.Length && i < Key.Length && Key[i] == key[i])
            i++;

        if (i == key.Length && i == Key.Length)//Equals
        {
            Value = value;
        }
        else if (i == key.Length)//Key contains key
        {
            var oldChilds = new Dictionary<char, PatriciaTrieNode>(Childs);
            Childs.Clear();
            Childs.Add(Key[i], new PatriciaTrieNode(Key.Substring(i), Value, oldChilds));
            Key = key.Substring(0, i);
            Value = value;
        }
        else if (i == Key.Length)//key contains Key
        {
            if (Childs.TryGetValue(key[i], out var child))
            {
                child.Add(key.Substring(i), value);
            }
            else
            {
                Childs.Add(key[i], new PatriciaTrieNode(key.Substring(i), value));
            }
        }
        else//find a char not equals
        {
            if (Childs.TryGetValue(key[i], out var child))
            {
                child.Add(key.Substring(i), value);
            }
            else
            {
                var oldChilds = new Dictionary<char, PatriciaTrieNode>(Childs);

                Childs.Clear();
                Childs.Add(Key[i], new PatriciaTrieNode(Key.Substring(i), Value, oldChilds));
                Childs.Add(key[i], new PatriciaTrieNode(key.Substring(i), value));

                Key = Key.Substring(0, i);
                Value = null;
            }
        }
    }
}