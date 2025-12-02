using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace EasyBrailleEdit.Forms
{
    /// <summary>
    /// 軟體更新通知對話窗。
    /// </summary>
    public class UpdateNotificationDialog : Form
    {
        private Label? lblMessage;
        private LinkLabel? linkReleaseNotes;
        private Button? btnYes;
        private Button? btnNo;

        public UpdateNotificationDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // 設定表單屬性
            Text = "軟體更新通知";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Size = new Size(400, 180);

            // 建立訊息標籤
            lblMessage = new Label
            {
                Text = "「易點雙視」有新版本，是否立即更新？",
                Location = new Point(20, 20),
                Size = new Size(360, 40),
                Font = new Font("Microsoft JhengHei UI", 10F, FontStyle.Regular),
                AutoSize = false
            };

            // 建立版本發布說明連結
            linkReleaseNotes = new LinkLabel
            {
                Text = "查看版本發布說明",
                Location = new Point(20, 65),
                Size = new Size(360, 25),
                Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Regular),
                LinkColor = Color.Blue,
                ActiveLinkColor = Color.Red,
                VisitedLinkColor = Color.Purple,
                AutoSize = false
            };
            linkReleaseNotes.Click += LinkReleaseNotes_Click;

            // 建立「是」按鈕
            btnYes = new Button
            {
                Text = "是(&Y)",
                Location = new Point(105, 105),
                Size = new Size(85, 30),
                DialogResult = DialogResult.Yes,
                Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Regular)
            };

            // 建立「否」按鈕
            btnNo = new Button
            {
                Text = "否(&N)",
                Location = new Point(210, 105),
                Size = new Size(85, 30),
                DialogResult = DialogResult.No,
                Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Regular)
            };

            // 將控制項加入表單
            Controls.Add(lblMessage);
            Controls.Add(linkReleaseNotes);
            Controls.Add(btnYes);
            Controls.Add(btnNo);

            // 設定預設和取消按鈕
            AcceptButton = btnYes;
            CancelButton = btnNo;
        }

        /// <summary>
        /// 處理版本發布說明連結的點擊事件。
        /// </summary>
        private void LinkReleaseNotes_Click(object? sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://braillekit.github.io/text-to-braille/release-notes/",
                    UseShellExecute = true
                });

                // 標記連結已被訪問
                if (sender is LinkLabel link)
                {
                    link.LinkVisited = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"無法開啟瀏覽器：{ex.Message}",
                    "錯誤",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
