using BrailleToolkit.Data;
using Serilog;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace BrailleToolkit.Converters
{
    /// <summary>
    /// Converts English text to BrailleWords based on Unified English Braille (UEB) rules.
    /// This is a prototype implementation focusing on a greedy matching algorithm for Grade 2 contractions.
    /// A full implementation would require a much more sophisticated rule engine.
    /// </summary>
    public class EnglishUebConverter : WordConverter
    {
        private const string UebTableFileName = "BrailleToolkit.Data.EnglishUebBrailleTable.xml";

        // Dictionaries to hold different types of braille symbols
        private readonly Dictionary<string, BrailleWord> _wordSigns = new Dictionary<string, BrailleWord>();
        private readonly Dictionary<string, BrailleWord> _groupSigns = new Dictionary<string, BrailleWord>();
        private readonly Dictionary<string, BrailleWord> _shortForms = new Dictionary<string, BrailleWord>();
        private readonly Dictionary<string, BrailleWord> _letterSigns = new Dictionary<string, BrailleWord>();

        // A combined list, sorted by length descending, for greedy matching
        private readonly List<string> _sortedKeys;

        internal override BrailleTableBase BrailleTable => null; // Not used in this converter

        public EnglishUebConverter()
        {
            LoadTable();

            // Combine all keys and sort them from longest to shortest.
            // This is crucial for the greedy matching algorithm to work correctly.
            // e.g., it ensures we match "about" before we match "a" or "b".
            _sortedKeys = _wordSigns.Keys
                .Concat(_groupSigns.Keys)
                .Concat(_shortForms.Keys)
                .OrderByDescending(k => k.Length)
                .ToList();
        }

        private void LoadTable()
        {
            var assembly = typeof(EnglishUebConverter).Assembly;
            using (var stream = assembly.GetManifestResourceStream(UebTableFileName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException($"Embedded resource {UebTableFileName} not found.");
                }
                using (var reader = new StreamReader(stream))
                {
                    var xdoc = XDocument.Load(reader);
                    foreach (var symbol in xdoc.Descendants("symbol"))
                    {
                        string text = symbol.Attribute("text")?.Value.ToLower();
                        string code = symbol.Attribute("code")?.Value;
                        string type = symbol.Attribute("type")?.Value;

                        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(code) || string.IsNullOrEmpty(type))
                        {
                            continue;
                        }

                        string name = symbol.Attribute("name")?.Value.ToLower();
                        if (string.IsNullOrEmpty(name))
                        {
                            name = text;
                        }

                        var brailleWord = new BrailleWord(name, code);

                        switch (type)
                        {
                            case "Letter":
                                _letterSigns[text] = brailleWord;
                                break;
                            case "WordSign":
                                _wordSigns[text] = brailleWord;
                                break;
                            case "GroupSign":
                                _groupSigns[text] = brailleWord;
                                break;
                            case "ShortForm":
                                _shortForms[name] = brailleWord;
                                break;
                        }
                    }
                }
            }
        }

        public override List<BrailleWord> Convert(Stack<char> charStack, ContextTagManager context)
        {
            if (charStack.Count == 0)
            {
                return null;
            }

            // Handle space character
            if (charStack.Peek() == ' ')
            {
                charStack.Pop();
                return new List<BrailleWord> { BrailleWord.NewBlank() };
            }

            // Create a string from the stack to perform lookaheads without consuming the stack.
            string remainingText = new string(charStack.ToArray());
            Log.Debug("--- Converting: {RemainingText}", remainingText);

            // 1. Greedy search for Grade 2 contractions (WordSigns, GroupSigns, ShortForms)
            foreach (var key in _sortedKeys)
            {
                if (remainingText.StartsWith(key, System.StringComparison.OrdinalIgnoreCase))
                {
                    BrailleWord word = null;
                    bool match = false;

                    // Basic rule: check if the match is a whole word for WordSigns
                    if (_wordSigns.ContainsKey(key))
                    {
                        if (remainingText.Length == key.Length || !char.IsLetter(remainingText[key.Length]))
                        {
                            word = new BrailleWord(key, _wordSigns[key].ToHexSting());
                            match = true;
                        }
                    }
                    else // For GroupSigns and ShortForms, allow partial word match
                    {
                        word = new BrailleWord(key, _groupSigns.ContainsKey(key) ? _groupSigns[key].ToHexSting() : _shortForms[key].ToHexSting());
                        match = true;
                    }

                    if (match)
                    {
                        Log.Debug("Matched Grade 2 key: {Key}", key);
                        // Consume the matched characters from the stack
                        for (int i = 0; i < key.Length; i++)
                        {
                            charStack.Pop();
                        }
                        return new List<BrailleWord> { word };
                    }
                }
            }

            // 2. If no contraction is found, fall back to Grade 1 (single letter)
            string firstCharStr = remainingText.Substring(0, 1).ToLower();
            if (_letterSigns.ContainsKey(firstCharStr))
            {
                Log.Debug("Matched Grade 1 letter: {Letter}", firstCharStr);
                charStack.Pop(); // Consume the character
                var word = new BrailleWord(firstCharStr, _letterSigns[firstCharStr].ToHexSting());
                return new List<BrailleWord> { word };
            }

            // 3. If it's not a recognized English character, we can't handle it.
            Log.Warning("No match found for: {RemainingText}", remainingText);
            return null;
        }
    }
}
