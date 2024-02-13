using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateYaml
{
    /// <summary>
    /// A BrailleSymbol represents a basic unit of a braille text. It can be a character, a word, or even words.
    /// This class is supposed to support multiple languages, but currently only English and Traditional Chinese are supported.
    /// </summary>
    public class BrailleSymbol
    {
        public string Text { get; set; }
        public List<string> Dots { get; set; } = new List<string>();
        //public string? ShortText { get; set; }
        public string? Description { get; set; }         
    }
}
