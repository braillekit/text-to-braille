using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using BrailleToolkit;

namespace EasyBrailleEdit.Controls
{
    public class PreviewPanel : UserControl
    {
        private WebBrowser webBrowser1 = null!;

        public PreviewPanel()
        {
            InitializeComponent();
            UpdatePreview(null);
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

        public void UpdatePreview(List<BrailleLine>? lines)
        {
            if (lines == null || lines.Count == 0)
            {
                webBrowser1.DocumentText = @"
<html>
<head>
<style>
    body { font-family: 'Microsoft JhengHei', sans-serif; padding: 20px; color: #666; line-height: 1.6; }
    .info { background-color: #f9f9f9; border: 1px solid #ddd; padding: 15px; border-radius: 5px; }
    .note { color: #d9534f; font-weight: bold; margin-top: 10px; }
</style>
</head>
<body>
    <div class='info'>
        <p>這是顯示即時轉換點字結果的預覽面板。當你在左邊文字編輯區有修改內容，停止敲鍵盤約 1.5 秒之後，這裡就會顯示游標所在位置附近文字的點字轉換結果。</p>
        <p class='note'>注意：不是全文轉換，而僅針對游標所在位置附近的文字進行轉換。</p>
    </div>
</body>
</html>";
                return;
            }

            var sb = new StringBuilder();
            sb.Append("<html><head><style>");
            sb.Append("body { font-family: 'Microsoft JhengHei', sans-serif; padding: 10px; }");
            sb.Append("table { border-collapse: collapse; width: 100%; margin-bottom: 0px; }");
            sb.Append(".separator { height: 10px; }");
            sb.Append(".blank-td { height: 15px; background-color: #f5f5f5; padding: 0; }");
            sb.Append("td { border: 1px solid #ddd; padding: 8px; vertical-align: top; }");
            sb.Append(".text { font-weight: bold; color: #333; margin-bottom: 4px; }");
            sb.Append(".phonetic { color: #666; font-size: 0.9em; margin-bottom: 4px; }");
            sb.Append(".braille { color: #0000FF; font-family: 'Consolas', monospace; font-size: 1.2em; }");
            sb.Append("</style></head><body>");

            foreach (var line in lines)
            {
                // Check if line is blank
                bool isBlank = false;
                if (line.Words.Count == 0) isBlank = true;
                else if (line.Words.Count == 1 && BrailleWord.IsBlank(line.Words[0])) isBlank = true;

                if (isBlank)
                {
                    sb.Append("<table><tr><td class='blank-td'>（空行）</td></tr></table>");
                    sb.Append("<div class='separator'></div>");
                    continue;
                }

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
                sb.Append("<div class='separator'></div>");
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
