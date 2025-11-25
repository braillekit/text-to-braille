using BrailleToolkit;
using BrailleToolkit.Helpers;
using EasyBrailleEdit.Common;
using EasyBrailleEdit.Forms;
using EasyBrailleEdit.Printing;
using EasyBrailleEdit.Common.Utilities.Windows;
using Huanlin.Common.Helpers;
using Huanlin.Windows.Forms;
using ScintillaNET;
using ScintillaNET_FindReplaceDialog;
using Serilog;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Text;
using EasyBrailleEdit.Common.Utilities.Http;
using EasyBrailleEdit.Services;

namespace EasyBrailleEdit
{
    public partial class MainForm : Form
    {
        string? m_FileName = null;
        bool m_Modified;	// 檔案內容是否有修改過。        

        private Scintilla m_TextArea = null!;

        private InvalidCharForm m_InvalidCharForm = null!;

        private ConversionDialog m_ConvertDialog = null!;

        private PreviewConversionForm m_PreviewConversionForm = null!;

        private FindReplace FindReplaceDialog = null!;
        
        public MainForm()
        {
            InitializeComponent();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .WriteTo.File(@"Logs\log-main-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            m_Modified = false;
        }

        #region 方法


        private void InitFindReplaceDialog(Scintilla scintilla)
        {
            FindReplaceDialog = new FindReplace();
            FindReplaceDialog.Scintilla = scintilla;
            FindReplaceDialog.FindAllResults += FindReplaceDialog_FindAllResults;
            FindReplaceDialog.KeyPressed += TextArea_KeyDown;

            FindReplaceDialog.Window.StartPosition = FormStartPosition.CenterParent;
            FindReplaceDialog.Window.ShowMarkAtMatchedLine = true;
            FindReplaceDialog.Window.HighlightMatches = true;
        }

        private void FindReplaceDialog_FindAllResults(object? sender, FindResultsEventArgs FindAllResults)
        {
            // 沒有用單獨 panel 顯示搜尋結果也沒關係，因為搜尋結果可以直接標示在編輯器中。
            // Pass on find results
            //findAllResultsPanel1.UpdateFindAllResults(FindAllResults.FindReplace, FindAllResults.FindAllResults);
        }

        private void InitTextArea()
        {
            m_TextArea = new Scintilla();
            panFill.Controls.Add(m_TextArea);

            m_TextArea.Dock = DockStyle.Fill;
            m_TextArea.ImeMode = ImeMode.On;
            m_TextArea.WrapMode = WrapMode.Char;
            m_TextArea.Margin = new Padding(3);

            // Line numbers
            m_TextArea.Margins[0].Type = MarginType.Number;
            m_TextArea.Margins[0].Width = 35;

            // Styling
            m_TextArea.Lexer = Lexer.Xml;
            foreach (var style in m_TextArea.Styles)
            {
                style.Font = "微軟正黑體";
                style.SizeF = panFill.Font.Size;
            };
            m_TextArea.Styles[Style.Xml.Tag].Font = "標楷體";
            m_TextArea.Styles[Style.Xml.Tag].Bold = true;
            m_TextArea.Styles[Style.Xml.Tag].ForeColor = Color.Maroon;
            m_TextArea.Styles[Style.Xml.TagEnd].Font = "標楷體";
            m_TextArea.Styles[Style.Xml.TagEnd].Bold = true;
            m_TextArea.Styles[Style.Xml.TagEnd].ForeColor = Color.Maroon;

            m_TextArea.CaretWidth = 3;
            m_TextArea.CaretForeColor = Color.Blue;

            // Events
            m_TextArea.TextChanged += TextArea_TextChanged;
            m_TextArea.UpdateUI += TextArea_UpdateUI;
            m_TextArea.KeyDown += TextArea_KeyDown;


            InitFindReplaceDialog(m_TextArea);
        }

        private void TextArea_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                FindReplaceDialog.ShowFind();
                e.SuppressKeyPress = true;
            }
            else if (e.Shift && e.KeyCode == Keys.F3)
            {
                FindReplaceDialog.Window.FindPrevious();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.F3)
            {
                FindReplaceDialog.Window.FindNext();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.H)
            {
                FindReplaceDialog.ShowReplace();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.I)
            {
                FindReplaceDialog.ShowIncrementalSearch();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.G)
            {
                var scintilla = sender as Scintilla;
                if (scintilla != null)
                {
                    GoTo MyGoTo = new GoTo(scintilla);
                    MyGoTo.ShowGoToDialog();
                    e.SuppressKeyPress = true;
                }
            }

        }

        private void TextArea_TextChanged(object? sender, EventArgs e)
        {
            Modified = true;
        }

        private void TextArea_UpdateUI(object? sender, UpdateUIEventArgs e)
        {
            UpdateCaretPosition();
        }


        private void NewFile()
        {
            if (Modified)
            {
                DialogResult result = MsgBoxHelper.ShowYesNoCancel("目前的文件尚未儲存，是否儲存？");
                if (result == DialogResult.Cancel)
                    return;
                if (result == DialogResult.Yes)
                {
                    SaveFile();
                }
            }
            ResetFileInfo();
        }

        private void ResetFileInfo()
        {
            m_TextArea.ClearAll();
            m_TextArea.EmptyUndoBuffer();
            Modified = false;
            FileName = "";
        }

        private void OpenFile()
        {
            if (Modified)
            {
                DialogResult result = MsgBoxHelper.ShowYesNoCancel("目前的文件尚未儲存，是否儲存？");
                if (result == DialogResult.Cancel)
                    return;
                if (result == DialogResult.Yes)
                {
                    SaveFile();
                }
            }

            var dlg = new OpenFileDialog
            {
                Filter = $"文字檔(*.txt)|*.txt|雙視檔案 YAML 格式 (*{Constant.Files.YamlBrailleFileExt})|*{Constant.Files.YamlBrailleFileExt}|雙視檔案 JSON 格式 (*{Constant.Files.JsonBrailleFileExt})|*{Constant.Files.JsonBrailleFileExt}|所有檔案|*.*"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Modified = false;
                string ext = Path.GetExtension(dlg.FileName);
                if (ext.Equals(Constant.Files.JsonBrailleFileExt, StringComparison.OrdinalIgnoreCase) ||
                    ext.Equals(Constant.Files.YamlBrailleFileExt, StringComparison.OrdinalIgnoreCase))
                {                    
                    OpenBrailleFileInEditor(dlg.FileName);
                }
                else
                {
                    FileName = dlg.FileName;
                    m_TextArea.Text = File.ReadAllText(FileName, Encoding.UTF8);
                    /*
                                        if (FileHelper.IsUTF8Encoded(FileName))
                                        {
                                            m_TextArea.Text = File.ReadAllText(FileName, Encoding.UTF8);
                                        }
                                        else
                                        {
                                            m_TextArea.Text = File.ReadAllText(FileName, Encoding.Default);
                                        }
                    */
                }
                Modified = false; // 必須再清除一次，因為編輯區域載入新內容時會由 event handler 設定為 true。
            }
        }

        /// <summary>
        /// 載入雙視檔案，並開啟雙視編輯視窗。
        /// </summary>
        /// <param name="filename">點字檔名。有可能是 cvt_out.tmp 或任何 .brx 檔案。</param>
        private void OpenBrailleFileInEditor(string filename)
        {            
            string ext = Path.GetExtension(filename);
            if (ext.Equals(Constant.Files.JsonBrailleFileExt, StringComparison.OrdinalIgnoreCase) ||
                ext.Equals(Constant.Files.YamlBrailleFileExt, StringComparison.OrdinalIgnoreCase))
            {
                if (Modified)
                {
                    MsgBoxHelper.ShowInfo("編輯區內的文件尚未儲存，不自動載入新的明眼字檔案，而只載入雙視檔案。");
                }
                else
                {
                    // 自動載入相同主檔名的明眼字檔案（如果存在的話）
                    string s = Path.ChangeExtension(filename, ".txt");
                    if (File.Exists(s))
                    {
                        FileName = s;
                        if (FileHelper.IsUTF8Encoded(FileName))
                        {
                            m_TextArea.Text = File.ReadAllText(FileName, Encoding.UTF8);
                        }
                        else
                        {
                            m_TextArea.Text = File.ReadAllText(FileName, Encoding.Default);
                        }
                    }
                }
            }

            var busyForm = new BusyForm
            {
                Message = "正在載入點字資料..."
            };
            busyForm.Show();
            busyForm.UseWaitCursor = true;
            Enabled = false;

            DualEditForm? frm = null;
            try
            {
                // 直接開啟雙視編輯視窗
                frm = new DualEditForm(filename);
            }
            finally
            {
                Enabled = true;
                busyForm.Close();
            }

            m_TextArea.Enabled = false;
            Enabled = false;
            try
            {
                frm.ShowDialog();
            }
            finally
            {
                Enabled = true;
                m_TextArea.Enabled = true;
                Show();
                BringToFront();
                Activate();
            }
        }

        private bool SaveFile()
        {
            if (String.IsNullOrEmpty(m_FileName))
            {
                return SaveFileAs();
            }
            File.WriteAllText(m_FileName, m_TextArea.Text, Encoding.UTF8);
            Modified = false;
            StatusText = "檔案儲存成功。";
            
            // 觸發即時預覽
            if (m_PreviewConversionForm != null && m_PreviewConversionForm.Visible)
            {
                UpdatePreviewAsync();
            }

            return true;
        }

        private async void UpdatePreviewAsync()
        {
            try
            {
                // 1. 計算範圍 (前後 5 行)
                int currentLine = m_TextArea.CurrentLine;
                int startLine = Math.Max(0, currentLine - 5);
                int endLine = Math.Min(m_TextArea.Lines.Count - 1, currentLine + 5);

                StringBuilder sb = new StringBuilder();
                for (int i = startLine; i <= endLine; i++)
                {
                    sb.AppendLine(m_TextArea.Lines[i].Text);
                }
                string content = sb.ToString();

                // 2. 執行轉換 (不鎖定 UI)
                // 注意：這裡直接呼叫 DoConvertAsync，它會建立新的 Converter 實例
                // 我們需要確保不會干擾主執行緒太久
                var result = await DoConvertAsync(content);

                if (result.Success && !string.IsNullOrEmpty(result.OutputFilePath))
                {
                    // 3. 讀取結果
                    var doc = BrailleDocument.LoadBrailleFile(result.OutputFilePath);
                    
                    // 4. 更新預覽視窗
                    m_PreviewConversionForm.UpdatePreview(doc.Lines);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UpdatePreviewAsync failed");
                // 預覽失敗不應干擾使用者，只記錄 Log
            }
        }

        private bool SaveFileAs()
        {
            SaveFileDialog dlg = new SaveFileDialog
            {
                DefaultExt = ".txt",
                Filter = "文字檔(*.txt)|*.txt|所有檔案|*.*"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                FileName = dlg.FileName;
                SaveFile();
            }
            return false;
        }

        private void Quit()
        {
            Close();
        }




        /// <summary>
        /// 執行點字轉換。
        /// </summary>
        private async Task<BrailleConversionResult> DoConvertAsync(string content)
        {
            using var converter = BrailleConverterFactory.CreateConverter();
            
            string[] phraseFiles = m_ConvertDialog.SelectedPhraseFileNames;
            
            var progress = new Progress<ConversionProgress>(p => 
            {
                // 可以在這裡更新 UI 進度（未來擴充）
            });
            
            return await converter.ConvertAsync(
                content, 
                AppGlobals.Config.Braille.CellsPerLine,
                phraseFiles,
                progress);
        }

        private void PrepareForConversion()
        {
            m_InvalidCharForm.Hide();	// 隱藏轉換失敗字元視窗。
            txtErrors.Visible = false;	// 隱藏轉換時的錯誤訊息面板。

            // 初始化進度相關的資訊
            m_InvalidCharForm.Clear();
            txtErrors.Clear();
        }

        /// <summary>
        /// 轉換點字並開啟編輯器。
        /// </summary>
        private async void ConvertAndShowEditor(bool showDialog)
        {
            if (m_ConvertDialog == null)
            {
                MsgBoxHelper.ShowInfo("應用程式尚未載入完成，請稍後再執行此功能。");
                return;
            }

            m_ConvertDialog.IsConvertingSelectedText = !string.IsNullOrEmpty(m_TextArea.SelectedText);

            if (showDialog)
            {
                if (m_ConvertDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
            }

            PrepareForConversion();

            // 決定要轉換的輸入文字
            string content;

            if (m_ConvertDialog.IsConvertingSelectedText) // 若有選取文字，就只轉換選取的部份。
            {
                content = m_TextArea.SelectedText;
            }
            else
            {
                // 若已存在雙視檔案，則詢問是否直接載入。
                string brxFileName = (FileName ?? "").Replace(".txt", Constant.Files.JsonBrailleFileExt);
                string brljFileName = (FileName ?? "").Replace(".txt", ".brlj");
                if (File.Exists(brxFileName) || File.Exists(brljFileName))
                {
                    string s = "雙視檔案已經存在，是否重新轉點字?\n[是]: 執行點字轉換\n[否]: 直接載入既有的雙視資料";
                    switch (MsgBoxHelper.ShowYesNoCancel(s))
                    {
                        case DialogResult.Yes:
                            content = m_TextArea.Text;
                            break;
                        case DialogResult.No:
                            if (File.Exists(brljFileName))
                            {
                                OpenBrailleFileInEditor(brljFileName);
                            }
                            else
                            {
                                OpenBrailleFileInEditor(brxFileName);
                            }
                            return;
                        default:
                            Enabled = true;
                            return;
                    }
                }
                else
                {
                    content = m_TextArea.Text;
                }
            }

            Enabled = false;
            txtErrors.Visible = true;

            // 執行轉換
            var result = await DoConvertAsync(content);

            Enabled = true;

            if (!HandleConversionResult(result))
            {
                return;
            }

            OpenBrailleFileInEditor(result.OutputFilePath!);
        }

        private bool SetDefaultPreviewPrinter()
        {
            var form = new ConfigTextPrinterForm();
            return form.ShowDialog() == DialogResult.OK;
        }

        private bool HasDefaultTextPrinter()
        {
            string defaultPrinter = AppGlobals.Config.Printing.DefaultTextPrinter;
            if (String.IsNullOrWhiteSpace(defaultPrinter))
            {
                return false;
            }
            var hasDefaultPrinter = PrinterSettings.InstalledPrinters.Cast<string>()
                .Any(printerName => defaultPrinter.Equals(printerName, StringComparison.InvariantCultureIgnoreCase));
            return hasDefaultPrinter;
        }

        /// <summary>
        /// 轉點字並預覽明眼字列印結果（只能預覽，不真的印出）。
        /// </summary>
        private async void ConvertAndPrintPreview()
        {
            if (!HasDefaultTextPrinter())
            {
                MessageBox.Show("尚未選擇預設印表機!");

                if (!SetDefaultPreviewPrinter())
                {
                    return;
                }
            }

            var (success, brlFileName) = await ConvertTextToBrailleAsync(m_TextArea.Text);

            if (!success)
                return;

            var busyForm = new BusyForm
            {
                Message = "正在載入點字資料..."
            };
            busyForm.Show();
            busyForm.UseWaitCursor = true;
            this.Enabled = false;

            DualPrintDialog? dlg = null;
            try
            {
                dlg = new DualPrintDialog(brlFileName);
            }
            finally
            {
                this.Enabled = true;
                busyForm.Close();
            }

            try
            {
                // 一定要讓 main form 變成作用中視窗，否則預覽列印對話窗不會變成 top-level window!!
                this.Activate();
                dlg?.PreviewText();
            }
            finally
            {
                this.Show();
                this.BringToFront();
                this.Activate();
            }
        }


        /// <summary>
        /// 處理／顯示轉換點字時的錯誤。
        /// </summary>
        /// <returns>若轉點字過程有發生錯誤（無法轉換的字元）則傳回 false；若無錯誤則傳回 true。</returns>
        private bool HandleConversionResult(BrailleConversionResult result)
        {
            if (result.HasError)
            {
                if (result.InvalidChars.Count > 0)
                {
                    int cnt = 0;
                    foreach (var charPos in result.InvalidChars)
                    {
                        m_InvalidCharForm.Add(charPos);
                        cnt++;
                        if (cnt >= 100)
                            break;
                    }
                    ShowInvlaidCharForm(result.InvalidChars.Count);
                }
                else if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    txtErrors.Text = result.ErrorMessage;
                    txtErrors.Visible = true;
                    MsgBoxHelper.ShowError("轉換過程中發生錯誤!\r\n請檢視並修正錯誤（顯示於編輯區域下方），再執行轉換程序。");
                }
                return false;
            }
            return true;
        }



        /// <summary>
        /// 顯示無法轉換的字元。
        /// </summary>
        private void ShowInvlaidCharForm(int errorCount)
        {
            BringToFront();
            Activate();

            MsgBoxHelper.ShowError("共有 " + errorCount.ToString() + " 個字元無法轉換!\r\n" +
                "請逐一修正後再執行轉換程序。");

            m_InvalidCharForm.Show();
            m_InvalidCharForm.Left = this.Left + this.Width - m_InvalidCharForm.Width - 4;
            m_InvalidCharForm.Top = m_TextArea.PointToScreen(new Point(0, 0)).Y;
            m_InvalidCharForm.Height = 400;
            if (m_InvalidCharForm.Bottom > (this.Bottom - 30))
            {
                m_InvalidCharForm.Height = this.Bottom - 30 - m_InvalidCharForm.Top;
            }
        }

        private async Task<(bool Success, string OutputFileName)> ConvertTextToBrailleAsync(string content)
        {
            PrepareForConversion();

            this.Enabled = false;
            txtErrors.Visible = true;

            // 執行轉換
            var result = await DoConvertAsync(content);

            this.Enabled = true;

            bool success = HandleConversionResult(result);
            return (success, result.OutputFilePath!);
        }

        private void ShowOptionsDialog()
        {
        }

        #endregion

        #region 屬性

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? FileName
        {
            get
            {
                return m_FileName;
            }
            set
            {
                m_FileName = value;
                UpdateCaption();
            }
        }

        private void UpdateCaption()
        {
            StringBuilder sb = new StringBuilder(Constant.AppName);
            if (String.IsNullOrEmpty(m_FileName))
            {
                sb.Append(" - 未命名");
            }
            else
            {
                sb.Append($"- {m_FileName}");
            }
            if (Modified)
            {
                sb.Append('*');
            }
            Text = sb.ToString();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Modified
        {
            get { return m_Modified; }
            set
            {
                if (m_Modified != value)
                {
                    m_Modified = value;
                    UpdateCaption();
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string StatusText
        {
            get { return statusStrip1.Items[0].Text ?? string.Empty; }
            set
            {
                statusStrip1.Items[0].Text = value;
                statusStrip1.Refresh();
                clearStatusTimer.Interval = 4000;
                clearStatusTimer.Enabled = true;
            }
        }

        #endregion

        /// <summary>
        /// 自動更新。
        /// </summary>
        /// <returns>傳回 true 表示更新成功，必須結束程式。</returns>
        private async Task<bool> AutoUpdateAsync()
        {
            if (!AppGlobals.Config.General.AutoUpdate)
            {
                return false;
            }

            if (SysInfo.IsNetworkConnected())
            {
                return await DoUpdateAsync(true);
            }
            return false;

        }

        private async Task CheckUpdateAsync()
        {
            if (!SysInfo.IsNetworkConnected())
            {
                MsgBoxHelper.ShowInfo("未偵測到網路連線，無法執行線上更新!");
                return;
            }

            if (await DoUpdateAsync(false))
            {
                string msg = "應用程式必須重新啟動才能完成更新程序，是否立即重新啟動？\r\n若您有資料尚未儲存，請選擇【否】。";
                if (MsgBoxHelper.ShowYesNo(msg) == DialogResult.Yes)
                {
                    Process.Start(Application.ExecutablePath);
                    Application.Exit();
                    return;
                }
            }
        }

        private async Task<bool> DoUpdateAsync(bool autoMode)
        {
            var updater = new HttpUpdater()
            {
                ClientPath = Application.StartupPath,
                ServerUri = AppGlobals.Config.General.AutoUpdateFilesUrl,
                ChangeLogFileName = Constant.ChangeLogFileName
            };

            // debug using local update feed.
            // updater.ServerUri = "https://raw.githubusercontent.com/huanlin/easy-braille-edit/test-auto-update-subfolder/UpdateFiles/";

            try
            {
                await updater.GetUpdateListAsync(Constant.DefaultAutoUpdateFileListName);
            }
            catch (Exception ex)
            {
                // 無法取得檔案更新清單（可能是網際網路無法連線）
                string msg = "無法取得檔案更新清單: " + (ex.Message ?? string.Empty); // Fixed CS8600
                if (autoMode)
                {
                    StatusText = msg;
                }
                else
                {
                    MsgBoxHelper.ShowError(msg);
                }
                return false;
            }

            if (updater.HasUpdates())
            {
                if (MsgBoxHelper.ShowYesNo("「易點雙視」有新版本，是否立即更新？") == DialogResult.Yes)
                {
                    UpdateProgressForm updForm = new UpdateProgressForm();
                    updForm.Show();

                    try
                    {
                        updater.FileUpdating += updForm.updator_FileUpdating;
                        updater.FileUpdated += updForm.updator_FileUpdated;
                        updater.DownloadProgressChanged += updForm.updator_DownloadProgressChanged;

                        if (await updater.UpdateAsync() > 0)
                        {
                            updForm.TopMost = false;
                            var fvi = FileVersionInfo.GetVersionInfo(Application.ExecutablePath);

                            MsgBoxHelper.ShowInfo("「易點雙視」更新完成!");
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        updForm.TopMost = false;
                        MsgBoxHelper.ShowError("更新失敗!\r\n" + ex.Message);
                    }
                    finally
                    {
                        updForm.Close();
                        updForm.Dispose();
                    }
                }
            }
            else
            {
                if (!autoMode)
                {
                    MsgBoxHelper.ShowInfo("您使用的已經是最新版，無須更新。");
                }
            }
            return false;
        }

        private async void MainForm_Load(object? sender, EventArgs e)
        {
            AppGlobals.AppPath = Path.GetDirectoryName(Application.ExecutablePath) ?? "";

            if (Screen.PrimaryScreen != null)
            {
                Width = Convert.ToInt32(Screen.PrimaryScreen.WorkingArea.Width * 0.9);
                Height = Convert.ToInt32(Screen.PrimaryScreen.WorkingArea.Height * 0.9);
            }
            CenterToScreen();

            InitTextArea();

            StatusText = "";

            NewFile();


            // 自動檢查更新
            if (await AutoUpdateAsync())
            {
                Process.Start(Application.ExecutablePath);
                Application.Exit();
                return;
            }
            Application.DoEvents();

            txtErrors.Visible = false;

            m_ConvertDialog = new ConversionDialog();

            m_InvalidCharForm = new InvalidCharForm(this);

            EnablePreviewConversion(false);
            
            m_TextArea.BringToFront();

            // 處理來自命令列的參數
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && args[1].EndsWith(Constant.Files.JsonBrailleFileExt))
            {
                if (File.Exists(args[1]))
                {
                    OpenBrailleFileInEditor(args[1]);
                }      
            }
        }

        private void EnablePreviewConversion(bool enable)
        {
            if (enable)
            {
                if (m_PreviewConversionForm == null || m_PreviewConversionForm.IsDisposed)
                {
                    m_PreviewConversionForm = new PreviewConversionForm();
                    m_PreviewConversionForm.TopMost = true;
                }

                m_PreviewConversionForm.Show();
                m_PreviewConversionForm.BringToFront();
                m_PreviewConversionForm.Activate();

                btnPreviewConversion.Text = "關閉即時預覽";
                btnPreviewConversion.ToolTipText = "關閉即時預覽（目前已開啟）";
            }
            else
            {
                // 使用 Hide 來避免 Dispose
                m_PreviewConversionForm?.Hide();
                btnPreviewConversion.Text = "啟用即時預覽";
                btnPreviewConversion.ToolTipText = "啟用即時預覽（目前未開啟）";
            }
        }

        private void miFileClicked(object? sender, EventArgs e)
        {
            var obj = sender as ToolStripItem;
            if (obj?.Tag == null) return;

            switch (obj.Tag.ToString())
            {
                case "FileNew":
                    NewFile();
                    break;
                case "FileOpen":
                    OpenFile();
                    break;
                case "FileSave":
                    SaveFile();
                    break;
                case "FileSaveAs":
                    SaveFileAs();
                    break;
                case "FilePrintPreview":
                    ConvertAndPrintPreview();
                    break;
                case "FileSetPreviewPrinter":
                    SetDefaultPreviewPrinter();
                    break;
                case "FileExit":
                    Quit();
                    break;
            }
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            e.Cancel = false;
            if (Modified)
            {
                DialogResult result = MsgBoxHelper.ShowYesNoCancel("目前的文件尚未儲存，是否儲存？");
                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                if (result == DialogResult.Yes)
                {
                    if (!SaveFile())
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        private void miEditClick(object? sender, EventArgs e)
        {
            var obj = sender as ToolStripItem;
            if (obj?.Tag == null) return;

            switch (obj.Tag.ToString())
            {
                case "EditUndo":
                    m_TextArea.Undo();
                    break;
                case "EditRedo":
                    m_TextArea.Redo();
                    break;
                case "EditCut":
                    m_TextArea.Cut();
                    break;
                case "EditCopy":
                    m_TextArea.Copy();
                    break;
                case "EditPaste":
                    m_TextArea.Paste();
                    break;
                case "EditSelectAll":
                    m_TextArea.SelectAll();
                    break;
                case "EditFind":
                    FindReplaceDialog.ShowFind();
                    break;
                case "EditReplace":
                    FindReplaceDialog.ShowReplace();
                    break;
                default:
                    break;
            }
        }

        private void btnSymbolClick(object? sender, EventArgs e)
        {
            // 插入符號

            var obj = sender as ToolStripItem;
            if (obj == null) return;
            if (obj.Tag == null)
            {
                m_TextArea.ReplaceSelection(obj.Text);
                return;
            }
            string s = obj.Tag.ToString() ?? string.Empty; // Fixed CS8600
            if (String.IsNullOrEmpty(s))
            {
                m_TextArea.ReplaceSelection(s);
                return;
            }

            // 處理換行符號
            s = s.Replace(@"\n", "\n");

            int i = s.IndexOf('|');
            if (i >= 0) // 有包含 '|' 字元者代表成對的標記或符號。
            {
                int selectionLength = m_TextArea.SelectedText.Length;
                var leftPart = s.Substring(0, i);
                var rightPart = s.Substring(i + 1);               
                m_TextArea.ReplaceSelection(leftPart + m_TextArea.SelectedText + rightPart);
                
                m_TextArea.Update();
                Application.DoEvents();

                if (selectionLength < 1)
                {                    
                    m_TextArea.CurrentPosition -= (s.Length - i - 1);
                    m_TextArea.SetEmptySelection(m_TextArea.CurrentPosition);
                }
            }
            else
            {
                m_TextArea.ReplaceSelection(s);
            }
        }

        private void miToolsClick(object? sender, EventArgs e)
        {
            var obj = sender as ToolStripItem;
            if (obj?.Tag == null) return;

            switch (obj.Tag.ToString())
            {
                case "Convert":
                    ConvertAndShowEditor(showDialog: true);
                    break;
                case "QuickConvert":
                    ConvertAndShowEditor(showDialog: false);
                    break;
                case "Options":
                    ShowOptionsDialog();
                    break;
                case "PreviewConversion":
                    PreviewConversion();
                    break; 
            }
        }

        private void clearStatusTimer_Tick(object? sender, EventArgs e)
        {
            StatusText = "";
            clearStatusTimer.Enabled = false;
        }

        private void MainForm_FormClosed(object? sender, FormClosedEventArgs e)
        {
        }

        private async void miHelpClick(object? sender, EventArgs e)
        {
            var obj = sender as ToolStripItem;
            if (obj?.Tag == null) return;

            switch (obj.Tag.ToString())
            {
                case "About":
                    AboutForm form = new AboutForm();
                    form.ShowDialog();
                    break;
                case "CheckUpdate":
                    await CheckUpdateAsync();
                    break;
                case "RevisionHistory":
                    ShowRevisionHistory();
                    break;
            }
        }

        private void PreviewConversion()
        {
            if (btnPreviewConversion.Text!.StartsWith("關閉"))
            {
                EnablePreviewConversion(false);
            }
            else
            {
                EnablePreviewConversion(true);
            }
            
            //m_PreviewConversionForm.SetTextToConvert(m_TextArea.Text);
        }

        private void ShowRevisionHistory()
        {
            // 用記事本開啟 ChangeLog.
            var changeLogFileName = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath)!, Constant.ChangeLogFileName);

            if (File.Exists(changeLogFileName))
            {
                Process.Start("NotePad.exe", changeLogFileName);
            }
            else
            {
                MsgBoxHelper.ShowError($"找不到檔案: {changeLogFileName}");
            }
        }

        private void miInsertClick(object? sender, EventArgs e)
        {
            var obj = sender as ToolStripItem;
            if (obj?.Tag == null) return;

            switch (obj.Tag.ToString())
            {
                case "Table":
                    InsertTable();
                    break;
                case "Phonetic":
                    InsertPhonetic();
                    break;
            }
        }



        private void InsertTable()
        {
            var form = new InsertTableForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                m_TextArea.ReplaceSelection(
                    "<表格>\r\n" + 
                    TextHelper.GenerateTable(form.RowCount, form.ColumnCount, form.CellsPerColumn) + 
                    "</表格>");
            }
            
        }

        private void InsertPhonetic()
        {
            PhoneticForm fm = new PhoneticForm();
            if (fm.ShowDialog() == DialogResult.OK)
            {
                m_TextArea.ReplaceSelection("<音標>" + fm.Phonetic + "</音標>");
            }
        }

        private void UpdateCaretPosition()
        {
            int line = m_TextArea.CurrentLine;
            int col = m_TextArea.CurrentPosition - m_TextArea.Lines[line].Position;
            statLabelCaretPos.Text = $"列:{line+1}, 行:{col+1}";
        }

        /// <summary>
        /// 選取指定位置的字元。
        /// </summary>
        /// <param name="lineIdx">第幾列。</param>
        /// <param name="charIdx">該列的第幾個字元。</param>
        public void SelectChar(int lineIdx, int charIdx)
        {
            if (lineIdx >= m_TextArea.Lines.Count)
                return;


            // 避免文字因為修改過了，導致要選取的字元超過該列的字元長度。此處做修正。
            int lineLength = m_TextArea.Lines[lineIdx].Length;
            if (charIdx >= lineLength)
            {
                charIdx = lineLength - 1;
            }

            int pos = m_TextArea.Lines[lineIdx].Position + charIdx;
            m_TextArea.SetEmptySelection(pos);
        }
    }
}