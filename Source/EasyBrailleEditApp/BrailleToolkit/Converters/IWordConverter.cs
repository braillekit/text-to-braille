using System.Collections.Generic;

namespace BrailleToolkit.Converters
{
    /// <summary>
    /// Defines a common interface for all word-to-braille converters.
    /// </summary>
    public interface IWordConverter
    {
        /// <summary>
        /// Tries to convert a segment of text from a character stack into a BrailleWord.
        /// </summary>
        /// <param name="chars">The stack of characters to process. The converter should pop characters if it successfully converts them.</param>
        /// <param name="context">The current context manager to check for active tags (e.g., Math, Table).</param>
        /// <returns>A list of BrailleWord objects if conversion is successful; otherwise, null.</returns>
        List<BrailleWord> Convert(Stack<char> chars, ContextTagManager context);
    }
}
