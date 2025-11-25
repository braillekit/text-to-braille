using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EasyBrailleEdit
{
    public partial class PreviewConversionForm : Form
    {
        public PreviewConversionForm()
        {
            InitializeComponent();
        }
        public void UpdatePreview(List<BrailleToolkit.BrailleLine> lines)
        {
            if (lines == null || lines.Count == 0)
            {
                webBrowser1.DocumentText = "<html><body></body></html>";
                return;
            }

            var sb = new StringBuilder();
            sb.Append("<html><head><style>");
            sb.Append("body { font-family: 'Microsoft JhengHei', sans-serif; padding: 10px; }");
            sb.Append("table { border-collapse: collapse; width: 100%; margin-bottom: 20px; }");
            sb.Append("td { border: 1px solid #ddd; padding: 8px; vertical-align: top; }");
            sb.Append(".text { font-weight: bold; color: #333; margin-bottom: 4px; }");
            sb.Append(".phonetic { color: #666; font-size: 0.9em; margin-bottom: 4px; }");
            sb.Append(".braille { color: #0000FF; font-family: 'Consolas', monospace; font-size: 1.2em; }");
            sb.Append("</style></head><body>");

            foreach (var line in lines)
            {
                sb.Append("<table>");
                
                // Row 1: Text
                sb.Append("<tr><td><div class='text'>");
                foreach (var word in line.Words)
                {
                    sb.Append(System.Net.WebUtility.HtmlEncode(word.Text));
                }
                sb.Append("</div>");

                // Row 2: Phonetic
                sb.Append("<div class='phonetic'>");
                foreach (var word in line.Words)
                {
                    // Assuming PhoneticCode is available. If not, we might need to adjust.
                    // BrailleWord has PhoneticCode property.
                    string ph = word.PhoneticCode;
                    if (string.IsNullOrEmpty(ph) && word.IsChinese)
                    {
                        // Fallback or empty
                        ph = " "; 
                    }
                    else if (string.IsNullOrEmpty(ph))
                    {
                        ph = " "; // Spacer for alignment if needed, or just empty
                    }
                    // For better alignment, we might need a table per word, but let's start with line-based.
                    // Actually, line-based phonetic string might be hard to align with text if we just dump them.
                    // But the user request asked for "Text, Phonetic, Braille" elements.
                    // Let's try to output them as a sequence.
                    sb.Append(System.Net.WebUtility.HtmlEncode(ph) + " ");
                }
                sb.Append("</div>");

                // Row 3: Braille
                sb.Append("<div class='braille'>");
                foreach (var word in line.Words)
                {
                    // Braille representation. 
                    // We can use the Braille ASCII or Unicode. 
                    // Let's use the hex string or position string for now, or if there is a font.
                    // The requirement says "corresponding Braille". 
                    // Let's use the dots representation (e.g. ⠓⠁⠧⠑) if possible, or just the cell values.
                    // BrailleToolkit doesn't seem to have a direct "ToUnicode" method in BrailleWord based on my read.
                    // But it has `ToPositionNumberString`.
                    // Let's try to construct Unicode braille characters from cell values.
                    
                    foreach (var cell in word.Cells)
                    {
                        sb.Append(GetBrailleUnicode(cell.Value));
                    }
                    sb.Append(" "); // Space between words
                }
                sb.Append("</div></td></tr>");
                sb.Append("</table>");
            }

            sb.Append("</body></html>");
            webBrowser1.DocumentText = sb.ToString();
        }

        private string GetBrailleUnicode(byte value)
        {
            // Braille Unicode block starts at U+2800 (hex)
            // The bit mapping in BrailleToolkit is:
            // Bit 0: Dot 1
            // Bit 1: Dot 2
            // Bit 2: Dot 3
            // Bit 3: Dot 4
            // Bit 4: Dot 5
            // Bit 5: Dot 6
            // 
            // Unicode Braille Pattern dots mapping:
            // U+2800 + 
            // Dot 1: +1
            // Dot 2: +2
            // Dot 3: +4
            // Dot 4: +8
            // Dot 5: +16
            // Dot 6: +32
            // 
            // It seems the mapping is identical!
            
            int code = 0x2800 + value;
            return ((char)code).ToString();
        }
    }
}
