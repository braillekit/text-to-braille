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
        <p>預覽結果會根據版面設定來自動排版與斷行。自動斷行的依據為每列點字方數（顯示於此視窗的底部狀態列）。</p>
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

        /// <summary>
        /// 顯示錯誤訊息
        /// </summary>
        /// <param name="errorMessage">錯誤訊息</param>
        /// <param name="invalidChars">無法轉換的字元清單（選填）</param>
        public void ShowError(string errorMessage, List<CharPosition>? invalidChars = null)
        {
            var sb = new StringBuilder();
            sb.Append(@"
<html>
<head>
<style>
    body { 
        font-family: 'Microsoft JhengHei', sans-serif; 
        padding: 20px; 
        color: #333; 
        line-height: 1.6; 
    }
    .error-container { 
        background-color: #fff3cd; 
        border: 2px solid #ffc107; 
        border-radius: 8px; 
        padding: 20px; 
        margin: 10px 0;
    }
    .error-header { 
        display: flex;
        align-items: center;
        margin-bottom: 15px;
    }
    .error-icon {
        font-size: 32px;
        margin-right: 12px;
    }
    .error-title { 
        color: #d9534f; 
        font-weight: bold; 
        font-size: 1.2em;
        margin: 0;
    }
    .error-message { 
        background-color: #ffffff; 
        border-left: 4px solid #d9534f; 
        padding: 12px; 
        margin: 15px 0;
        border-radius: 4px;
    }
    .invalid-chars-section {
        margin-top: 15px;
    }
    .invalid-chars-title {
        font-weight: bold;
        color: #856404;
        margin-bottom: 10px;
    }
    .invalid-char-item {
        background-color: #ffffff;
        border-left: 3px solid #ffc107;
        padding: 8px 12px;
        margin: 8px 0;
        border-radius: 4px;
    }
    .char-value {
        font-family: 'Microsoft JhengHei', monospace;
        font-size: 1.1em;
        font-weight: bold;
        color: #d9534f;
        background-color: #f8f9fa;
        padding: 2px 6px;
        border-radius: 3px;
    }
    .char-position {
        color: #666;
        font-size: 0.9em;
        margin-left: 8px;
    }
    .suggestion {
        background-color: #d1ecf1;
        border-left: 4px solid #17a2b8;
        padding: 12px;
        margin-top: 15px;
        border-radius: 4px;
    }
    .suggestion-title {
        font-weight: bold;
        color: #0c5460;
        margin-bottom: 5px;
    }
</style>
</head>
<body>
    <div class='error-container'>
        <div class='error-header'>
            <div class='error-icon'>⚠️</div>
            <h2 class='error-title'>即時點字轉換失敗</h2>
        </div>");

            // 顯示錯誤訊息
            if (!string.IsNullOrEmpty(errorMessage))
            {
                sb.Append("<div class='error-message'>");
                sb.Append(System.Net.WebUtility.HtmlEncode(errorMessage));
                sb.Append("</div>");
            }

            // 顯示無法轉換的字元清單
            if (invalidChars != null && invalidChars.Count > 0)
            {
                sb.Append("<div class='invalid-chars-section'>");
                sb.Append("<div class='invalid-chars-title'>不支援的字元或符號：</div>");
                
                // 最多顯示前 10 個無法轉換的字元
                int displayCount = Math.Min(invalidChars.Count, 10);
                for (int i = 0; i < displayCount; i++)
                {
                    var charPos = invalidChars[i];
                    sb.Append("<div class='invalid-char-item'>");
                    sb.Append("• 字元 <span class='char-value'>");
                    sb.Append(System.Net.WebUtility.HtmlEncode(charPos.CharValue.ToString()));
                    sb.Append("</span>");
                    sb.Append("<span class='char-position'>");
                    sb.Append($"(位置：第 {charPos.LineNumber} 行，第 {charPos.CharIndex} 個字)");
                    sb.Append("</span>");
                    sb.Append("</div>");
                }
                
                if (invalidChars.Count > displayCount)
                {
                    sb.Append("<div class='invalid-char-item'>");
                    sb.Append($"... 還有 {invalidChars.Count - displayCount} 個字元未顯示");
                    sb.Append("</div>");
                }
                
                sb.Append("</div>");
            }

            // 顯示建議
            sb.Append(@"
        <div class='suggestion'>
            <div class='suggestion-title'>💡 建議：</div>
            請移除或替換這些不支援的字元。若您認為這些字元應該被支援，請聯繫開發者。
        </div>
    </div>
</body>
</html>");

            webBrowser1.DocumentText = sb.ToString();
        }

        private string GetBrailleUnicode(byte value)

        {
            int code = 0x2800 + value;
            return ((char)code).ToString();
        }
    }
}
