﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrailleToolkit;
using EasyBrailleEdit.Common;
using EasyBrailleEdit.Printing;
using Huanlin.Common.Helpers;
using Huanlin.Http;
using Huanlin.Sys;
using Huanlin.Windows.Forms;
using Serilog;

namespace EasyBrailleEdit
{
    public partial class MainForm : Form
    {
        string m_FileName;
        bool m_Modified;	// 檔案內容是否有修改過。

        private InvalidCharForm m_InvalidCharForm;
        private BusyForm m_BusyForm;

        private ConvertionDialog m_ConvertDialog;
        
        private FileRunner m_FileRunner;

        public MainForm()
        {
            InitializeComponent();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .WriteTo.RollingFile(@"Logs\log-main-{Date}.txt")
                .CreateLogger();

            m_FileRunner = new FileRunner();

            m_Modified = false;
            NewFile();
        }

        #region 方法

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
            rtbOrg.Clear();
            rtbOrg.ClearUndo();
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
                Filter = "文字檔(*.txt)|*.txt|雙視檔案(*.btx)|*.btx|所有檔案|*.*"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Modified = false;
                if (dlg.FileName.ToLower().EndsWith(".btx"))
                {
                    string s = dlg.FileName.Replace(".btx", ".txt");
                    if (File.Exists(s))
                    {
                        this.FileName = s;
                        if (FileHelper.IsUTF8Encoded(FileName))
                        {
                            rtbOrg.Text = File.ReadAllText(FileName, Encoding.UTF8);
                        }
                        rtbOrg.LoadFile(FileName, RichTextBoxStreamType.PlainText);
                    }

                    OpenBrailleFileInEditor(dlg.FileName);
                }
                else
                {
                    this.FileName = dlg.FileName;
                    if (FileHelper.IsUTF8Encoded(FileName))
                    {
                        rtbOrg.Text = File.ReadAllText(FileName, Encoding.UTF8);
                    }
                    else
                    {
                        rtbOrg.LoadFile(FileName, RichTextBoxStreamType.PlainText);
                    }
                }
            }
        }

        /// <summary>
        /// 載入雙視檔案，並開啟雙視編輯視窗。
        /// </summary>
        /// <param name="filename">點字檔名。</param>
        private void OpenBrailleFileInEditor(string filename)
        {
            m_BusyForm = new BusyForm
            {
                Message = "正在載入點字資料..."
            };
            m_BusyForm.Show();
            m_BusyForm.UseWaitCursor = true;
            this.Enabled = false;

            DualEditForm frm = null;
            try
            {
                // 直接開啟雙視編輯視窗
                frm = new DualEditForm(filename);
            }
            finally
            {
                this.Enabled = true;
                m_BusyForm.Hide();
                m_BusyForm.Close();
            }

            this.ShowInTaskbar = false;
            try
            {
                frm.ShowDialog();
            }
            finally
            {
                this.ShowInTaskbar = true;
                this.Show();
                this.BringToFront();
                this.Activate();
            }
        }

        private bool SaveFile()
        {
            if (String.IsNullOrEmpty(m_FileName))
            {
                return SaveFileAs();
            }
            File.WriteAllText(m_FileName, rtbOrg.Text, Encoding.UTF8);
            Modified = false;
            StatusText = "檔案儲存成功。";
            return true;
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
                this.FileName = dlg.FileName;
                SaveFile();
            }
            return false;
        }

        private void Quit()
        {
            Close();
        }


        /// <summary>
        /// 呼叫 Txt2Brl.exe 進行轉檔。
        /// </summary>
        /// <param name="inFileName">輸入檔名。</param>
        /// <param name="outFileName">輸出檔名。</param>
        private void InvokeTxt2Brl(string inFileName, string outFileName)
        {
            StringBuilder arg = new StringBuilder();

            // switches
            arg.Append("-C" + AppGlobals.Config.Braille.CellsPerLine);

            // input file name
            arg.Append(" ");
            arg.Append(inFileName);

            // output file name
            arg.Append(" ");
            arg.Append(outFileName);

            m_FileRunner.NeedWait = true;		// 不要等待程式結束，立刻返回
            m_FileRunner.ShowWindow = true;
            m_FileRunner.UseShellExecute = false;
            m_FileRunner.RedirectStandardOutput = false;

            string cmd = Application.StartupPath + @"\txt2brl.exe";
            try
            {
                if (m_FileRunner.Run(cmd, arg.ToString()))
                {
                    // 等待執行結束
                    while (m_FileRunner.Running)
                    {
                        Application.DoEvents();
                    }
                    m_FileRunner.KillProcess();
                }
                else
                {
                    throw new Exception("轉點字過程發生錯誤!\r\n" + m_FileRunner.ErrorMsg);
                }
            }
            finally
            {
            }
        }

        /// <summary>
        /// 把輸入的文字存成暫存檔，並呼叫 Txt2Brl.exe 進行轉檔。
        /// </summary>
        private string DoConvert(string content)
        {
            string inFileName = Path.Combine(AppGlobals.TempPath, Constant.Files.CvtInputTempFileName);
            string outFileName = Path.Combine(AppGlobals.TempPath, Constant.Files.CvtOutputTempFileName);
            string phraseFileName = Path.Combine(AppGlobals.TempPath, Constant.Files.CvtInputPhraseListFileName);

            // 建立輸入檔案
            File.WriteAllText(inFileName, content, Encoding.UTF8);	

            // 建立輸入的詞庫設定檔
            string[] fileNames = m_ConvertDialog.SelectedPhraseFileNames;
            File.WriteAllLines(phraseFileName, fileNames, Encoding.UTF8);

            // 刪除輸出檔案
            if (File.Exists(outFileName))	
            {
                File.Delete(outFileName);	
            }
            
            InvokeTxt2Brl("\"" + inFileName + "\"", "\"" + outFileName + "\"");

            return outFileName;
        }

        private void PrepareForConvertion()
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
        private void ConvertAndShowEditor()
        {
            if (m_ConvertDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            PrepareForConvertion();

            // 決定要轉換的輸入文字
            string content;

            if (rtbOrg.SelectionLength > 1) // 若有選取文字，就只轉換選取的部份。
            {
                content = rtbOrg.SelectedText;
            }
            else
            {
                // 若已存在雙視檔案，則詢問是否直接載入。
                string brljFileName = FileName.Replace(".txt", ".brlj");
                string btxFileName = FileName.Replace(".txt", ".btx");
                if (File.Exists(brljFileName) || File.Exists(btxFileName))
                {
                    string s = "雙視檔案已經存在，是否重新轉點字?\n[是]: 執行點字轉換\n[否]: 直接載入既有的雙視資料";
                    switch (MsgBoxHelper.ShowYesNoCancel(s))
                    {
                        case DialogResult.Yes:
                            content = rtbOrg.Text;
                            break;
                        case DialogResult.No:
                            if (File.Exists(brljFileName))
                            {
                                OpenBrailleFileInEditor(brljFileName);
                            }
                            else
                            {
                                OpenBrailleFileInEditor(btxFileName);
                            }
                            return;
                        default:
                            this.Enabled = true;
                            return;
                    }
                }
                else
                {
                    content = rtbOrg.Text;
                }
            }

            this.Enabled = false;
            txtErrors.Visible = true;

            // 執行轉換
            string outFileName = DoConvert(content);

            this.Enabled = true;

            if (!HandleConvertionError())
            {
                return;
            }

            OpenBrailleFileInEditor(outFileName);
        }

        private bool SetDefaultPreviewPrinter()
        {
            var form = new ConfigTextPrinterForm();
            return form.ShowDialog() == System.Windows.Forms.DialogResult.OK;
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
        private void ConvertAndPrintPreview()
        {
            if (!HasDefaultTextPrinter())
            {
                MessageBox.Show("尚未選擇預設印表機!");

                if (!SetDefaultPreviewPrinter())
                {
                    return;
                }                
            }

            string brlFileName = null;

            if (!ConvertTextToBraille(rtbOrg.Text, out brlFileName))
                return;

            m_BusyForm = new BusyForm
            {
                Message = "正在載入點字資料..."
            };
            m_BusyForm.Show();
            m_BusyForm.UseWaitCursor = true;
            this.Enabled = false;

            DualPrintDialog dlg = null;
            try
            {
                dlg = new DualPrintDialog(brlFileName);
            }
            finally
            {
                this.Enabled = true;
                m_BusyForm.Hide();
                m_BusyForm.Close();
            }

            try
            {
                // 一定要讓 main form 變成作用中視窗，否則預覽列印對話窗不會變成 top-level window!!
                this.Activate();
                dlg.PreviewText();
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
        private bool HandleConvertionError()
        {
            // 取得錯誤資訊
            List<CharPosition> invalidChars = new List<CharPosition>();
            string errMsg = "";
            bool hasError = GetCvtErrors(ref errMsg, ref invalidChars);

            // 處理錯誤
            if (hasError)	// 若轉換過程中發生錯誤
            {
                if (invalidChars.Count > 0)		// 無效的字元
                {
                    int cnt = 0;
                    foreach (CharPosition charPos in invalidChars)
                    {
                        m_InvalidCharForm.Add(charPos);
                        cnt++;
                        if (cnt >= 100)	// 最多顯示 100 個錯誤字元。
                            break;
                    }

                    ShowInvlaidCharForm(invalidChars.Count);
                }
                else if (!String.IsNullOrEmpty(errMsg))	// 錯誤訊息
                {
                    txtErrors.Text = errMsg;
                    txtErrors.Visible = true;
                    MsgBoxHelper.ShowError("轉換過程中發生錯誤!\r\n請檢視並修正錯誤（顯示於編輯區域下方），再執行轉換程序。");
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// 取得 Txt2Brl.exe 轉點字時輸出的錯誤資訊。
        /// </summary>
        /// <param name="errMsg"></param>
        /// <param name="invalidChars"></param>
        /// <returns></returns>
        private bool GetCvtErrors(ref string errMsg, ref List<CharPosition> invalidChars)
        {
            bool hasError = false;
            string fname;

            errMsg = "";
            invalidChars.Clear();

            // 取得結果旗號以及錯誤訊息（如果有的話）
            fname = AppGlobals.GetTempPath() + Constant.Files.CvtResultFileName;

            if (!File.Exists(fname))
                return false;

            using (StreamReader sr = new StreamReader(fname, Encoding.Default))
            {
                string errFlag = sr.ReadLine();
                if (String.IsNullOrEmpty(errFlag))
                {
                    return false;
                }
                if (errFlag.Equals("1"))
                {
                    hasError = true;
                    errMsg = sr.ReadToEnd();
                }
                sr.Close();
            }

            if (!hasError)
                return false;

            // 取得所有轉換失敗的字元
            fname = AppGlobals.GetTempPath() + Constant.Files.CvtErrorCharFileName;

            if (!File.Exists(fname))
                return hasError;

            using (StreamReader sr = new StreamReader(fname, Encoding.Default))
            {
                string s;
                string[] parts;

                while (true)
                {
                    s = sr.ReadLine();
                    if (String.IsNullOrEmpty(s))
                    {
                        break;
                    }
                    parts = s.Split(' ');
                    if (parts.Length != 3) 
                    {
                        throw new Exception("檔案格式不正確: " + fname);
                    }
                    CharPosition ch = new CharPosition
                    {
                        LineNumber = Convert.ToInt32(parts[0]),
                        CharIndex = Convert.ToInt32(parts[1]),
                        CharValue = parts[2][0]
                    };

                    invalidChars.Add(ch);
                }
                sr.Close();
            }

            return hasError;
        }

        /// <summary>
        /// 顯示無法轉換的字元。
        /// </summary>
        private void ShowInvlaidCharForm(int errorCount)
        {
            MsgBoxHelper.ShowError("共有 " + errorCount.ToString() + " 個字元無法轉換!\r\n" +
                "請逐一修正後再執行轉換程序。");

            m_InvalidCharForm.Show();
            m_InvalidCharForm.Left = this.Left + this.Width - m_InvalidCharForm.Width - 4;
            m_InvalidCharForm.Top = rtbOrg.PointToScreen(new Point(0, 0)).Y;
            m_InvalidCharForm.Height = 400;
            if (m_InvalidCharForm.Bottom > (this.Bottom - 30))
            {
                m_InvalidCharForm.Height = this.Bottom - 30 - m_InvalidCharForm.Top;
            }
        }

        private bool ConvertTextToBraille(string content, out string outFileName)
        {
            PrepareForConvertion();

            this.Enabled = false;
            txtErrors.Visible = true;

            // 執行轉換
            outFileName = DoConvert(content);

            this.Enabled = true;

            return HandleConvertionError();
        }

        private void ShowOptionsDialog()
        {
        }

        #endregion

        #region 屬性

        public string FileName
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
                sb.Append(m_FileName);
            }
            if (Modified)
            {
                sb.Append('*');
            }
            Text = sb.ToString();
        }

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

        public string StatusText
        {
            get { return statusStrip1.Items[0].Text; }
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
            if (!AppGlobals.Config.AutoUpdate)
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
            HttpUpdater updater = new HttpUpdater(Log.Logger)
            {
                ClientPath = Application.StartupPath,
                ServerUri = AppGlobals.Config.AutoUpdateFilesUrl,
                ChangeLogFileName = "ChangeLog.txt"
            };

            // debug using local update feed.
            //updater.ServerUri = "http://localhost/ebeupdate/";

            try
            {                
                await updater.GetUpdateListAsync();
            }
            catch (Exception ex)
            {
                // 無法取得檔案更新清單（可能是網際網路無法連線）
                string msg = "無法取得檔案更新清單: " + ex.Message;
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

        private async void MainForm_Load(object sender, EventArgs e)
        {
            Width = Convert.ToInt32(Screen.PrimaryScreen.WorkingArea.Width * 0.9);
            Height = Convert.ToInt32(Screen.PrimaryScreen.WorkingArea.Height * 0.9);
            CenterToScreen();

            StatusText = "";

            // 自動檢查更新
            if (await AutoUpdateAsync())
            {
                Process.Start(Application.ExecutablePath);

                // 用記事本開啟 ChangeLog.
                string changeLogFileName = Path.GetDirectoryName(Application.ExecutablePath) + @"\ChangeLog.txt";
                if (File.Exists(changeLogFileName))
                {
                    Process.Start("NotePad.exe", changeLogFileName);
                }

                Application.Exit();
                return;
            }
/* 新版本有內建注音字根查詢功能，不再需要注音輸入法!
            // 檢查注音與新注音輸入法（在自動更新之後才檢查）
            if (!ImmHelper.ZhuyinImeInstalled || !ImmHelper.NewZhuyinImeInstalled)
            {
                StatusText = "注意：未偵測到微軟注音或新注音輸入法!";
                // Close(); 不要結束程式, 因為偵測注音輸入法的函式在 Windows 2008 會失效。
                // return;
            }
*/
            Application.DoEvents();

            txtErrors.Visible = false;

            m_ConvertDialog = new ConvertionDialog();

            m_InvalidCharForm = new InvalidCharForm(this);

            rtbOrg.BringToFront();
        }        

/*
        void BrailleProcessor_ConvertionFailed(object sender, ConvertionFailedEventArgs args)
        {
            if (m_BusyForm != null)
            {
                m_BusyForm.AddInvalidChar(args.InvalidChar.CharValue);
                Application.DoEvents();
            }			
        }
*/		
        private void miFileClicked(object sender, EventArgs e)
        {
            ToolStripItem obj = (ToolStripItem) sender;
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

        private void rtbOrg_TextChanged(object sender, EventArgs e)
        {
            Modified = true;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
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

        private void miEditClick(object sender, EventArgs e)
        {
            ToolStripItem obj = (ToolStripItem)sender;
            switch (obj.Tag.ToString())
            {
                case "EditUndo":
                    rtbOrg.Undo();
                    break;
                case "EditRedo":
                    rtbOrg.Redo();
                    break;
                case "EditCut":
                    rtbOrg.Cut();
                    break;
                case "EditCopy":
                    rtbOrg.Copy();
                    break;
                case "EditPaste":
                    rtbOrg.Paste();
                    break;
                case "EditSelectAll":
                    rtbOrg.SelectAll();
                    break;
                default:
                    break;
            }
        }

        private void btnSymbolClick(object sender, EventArgs e)
        {
            // 插入符號

            ToolStripItem obj = (ToolStripItem)sender;
            if (obj.Tag == null)
            {
                rtbOrg.SelectedText = obj.Text;
                return;
            }
            string s = obj.Tag.ToString();
            if (String.IsNullOrEmpty(s))
            {
                rtbOrg.SelectedText = s;
                return;
            }

            // 處理換行符號
            s = s.Replace(@"\n", "\n");

            int i = s.IndexOf('|');
            if (i >= 0)
            {
                rtbOrg.SelectedText = s.Remove(i, 1);
                rtbOrg.Update();
                Application.DoEvents();
                rtbOrg.SelectionStart -= (s.Length - i - 1);
            }
            else
            {
                rtbOrg.SelectedText = s;
            }
        }

        private void miToolsClick(object sender, EventArgs e)
        {
            ToolStripItem obj = (ToolStripItem)sender;
            switch (obj.Tag.ToString())
            {
                case "Convert":
                    ConvertAndShowEditor();
                    break;
                case "Options":
                    ShowOptionsDialog();
                    break;
                default:
                    break;
            }
        }

        private void clearStatusTimer_Tick(object sender, EventArgs e)
        {
            StatusText = "";
            clearStatusTimer.Enabled = false;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_FileRunner != null)
            {
                m_FileRunner.Dispose();
            }
        }

        private async void miHelpClick(object sender, EventArgs e)
        {
            ToolStripItem obj = (ToolStripItem)sender;
            switch (obj.Tag.ToString())
            {
                case "About":
                    AboutForm form = new AboutForm();
                    form.ShowDialog();
                    break;
                case "CheckUpdate":
                    await CheckUpdateAsync();
                    break;
                case "Options":
                    ShowOptionsDialog();
                    break;
                default:
                    break;
            }
        }

        private void miInsertClick(object sender, EventArgs e)
        {
            ToolStripItem obj = (ToolStripItem)sender;
            switch (obj.Tag.ToString())
            {
                case "Tbl2x2":
                    InsertTable(2, 2);
                    break;
                case "Phonetic":
                    InsertPhonetic();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 插入表格。
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void InsertTable(int row, int col)
        {
            char leftTop = '┌';
            char rightTop = '┐';
//            char leftBottom = '└';
//            char rightBottom = '┘';
            char topMiddle = '┬';
//            char bottomMiddle = '⊥';
//            char rightMiddle = '┤';
//            char leftMiddle = '├';
            char line = '─';
//            char stick = '│';

            StringBuilder sb = new StringBuilder();

            // 頂部列
            sb.Append(leftTop);
            for (int i = 0; i < col; i++)
            {
                sb.Append(line);
                if (i == col - 1)
                {
                    sb.Append(rightTop);
                }
                else 
                {
                    sb.Append(topMiddle);
                }
            }

            sb.Append("\n尚未實作完成!");

            rtbOrg.SelectedText = sb.ToString();
        }

        private void InsertPhonetic()
        {
            PhoneticForm fm = new PhoneticForm();
            if (fm.ShowDialog() == DialogResult.OK)
            {
                rtbOrg.SelectedText = "<音標>" + fm.Phonetic + "</音標>";
            }
        }

        private void UpdateCaretPosition()
        {
            int col = rtbOrg.SelectionStart;
            int line = rtbOrg.GetLineFromCharIndex(col);
            statLabelCaretPos.Text = String.Format("列:{0}, 行:{1}", line+1, col+1);
        }

        private void rtbOrg_SelectionChanged(object sender, EventArgs e)
        {
            UpdateCaretPosition();
        }

        /// <summary>
        /// 選取指定位置的字元。
        /// </summary>
        /// <param name="lineIdx">第幾列。</param>
        /// <param name="charIdx">該列的第幾個字元。</param>
        public void SelectChar(int lineIdx, int charIdx)
        {
            if (lineIdx >= rtbOrg.Lines.Length)
                return;

            int i = 0;
            int charCnt = 0;

            // 先算出目標列之前總共有幾個字元（因為 SelectionStart 屬性是整段內容的字元索引）。
            while (i < lineIdx && i < rtbOrg.Lines.Length)
            {
                charCnt += rtbOrg.Lines[i].Length + 1;	// 要多算一個換行符號。
                i++;
            }

            bool charIdxValid = true;	// 指定的字元索引是否有效。

            // 避免文字因為修改過了，導致要選取的字元超過該列的字元長度。此處做修正。
            if (charIdx >= rtbOrg.Lines[lineIdx].Length)
            {				
                charIdx = rtbOrg.Lines[lineIdx].Length - 1;
                charIdxValid = false;
            }

            charIdx += charCnt;
    
            rtbOrg.SelectionStart = charIdx;

            // 唯有當指定的字元索引有效，才選取該字元。
            if (charIdxValid)
            {
                rtbOrg.Select(charIdx, 1);
            }
            else
            {
                rtbOrg.SelectionLength = 0;
            }
        }
    }
}