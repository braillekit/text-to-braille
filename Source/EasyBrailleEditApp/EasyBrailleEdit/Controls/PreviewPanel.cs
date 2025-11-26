using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using BrailleToolkit;

namespace EasyBrailleEdit.Controls
{
    public class PreviewPanel : UserControl
    {
        private WebBrowser webBrowser1;

        public PreviewPanel()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(150, 150);
            this.webBrowser1.TabIndex = 0;
            // 
            // PreviewPanel
            // 
            this.Controls.Add(this.webBrowser1);
            this.Name = "PreviewPanel";
            this.ResumeLayout(false);
        }

        public void UpdatePreview(List<BrailleLine> lines)
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
                sb.Append("<tr>");
                foreach (var word in line.Words)
                {
                    sb.Append("<td><div class='text'>");
                    sb.Append(System.Net.WebUtility.HtmlEncode(word.Text));
                    sb.Append("</div></td>");
                }
                sb.Append("</tr>");

                // Row 2: Phonetic
                sb.Append("<tr>");
                foreach (var word in line.Words)
                {
                    sb.Append("<td><div class='phonetic'>");
                    string ph = word.PhoneticCode;
                    if (string.IsNullOrEmpty(ph) && word.IsChinese)
                    {
                        ph = " "; 
                    }
                    else if (string.IsNullOrEmpty(ph))
                    {
                        ph = " ";
                    }
                    sb.Append(System.Net.WebUtility.HtmlEncode(ph));
                    sb.Append("</div></td>");
                }
                sb.Append("</tr>");

                // Row 3: Braille
                sb.Append("<tr>");
                foreach (var word in line.Words)
                {
                    sb.Append("<td><div class='braille'>");
                    foreach (var cell in word.Cells)
                    {
                        sb.Append(GetBrailleUnicode(cell.Value));
                    }
                    if (word.Cells.Count == 0)
                    {
                         sb.Append("&nbsp;");
                    }
                    sb.Append("</div></td>");
                }
                sb.Append("</tr>");

                sb.Append("</table>");
            }

            sb.Append("</body></html>");
            webBrowser1.DocumentText = sb.ToString();
        }

        private string GetBrailleUnicode(byte value)
        {
            int code = 0x2800 + value;
            return ((char)code).ToString();
        }
    }
}
