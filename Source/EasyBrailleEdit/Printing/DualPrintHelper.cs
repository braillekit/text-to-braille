using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using BrailleToolkit;
using EasyBrailleEdit.Common;

namespace EasyBrailleEdit
{
    /// <summary>
    /// 雙視列印。
    /// </summary>
    public partial class DualPrintHelper
    {
        public const string PaperName = "點字紙";
        public const int PaperWidth = 1300;        // 紙張寬度 13 inchess * 100.
        public const int PaperHeight = 1100;       // 紙張高度 11 inches * 100.        
        const double BrailleCellWidth = 24; // 一方點字的寬度
        const int PageNumberRightOffset = 120;  // 頁碼距離右邊界的偏移點數
        const int PageNumberBottomOffset = 25;  // 頁碼距離上邊界的偏移點數

        private BrailleDocument m_BrDoc;
        private PrintOptions m_PrintOptions;
        private string m_BeginOrgPageNumber;		// 起始原書頁碼
        private string m_EndOrgPageNumber;			// 終止原書頁碼

        private bool m_PreviewOnly;				// 是否只預覽，不列印

        private int m_PrintedPageCount;			// 已列印的頁數

        // 列印明眼字時需要的變數
        private int m_TotalPages;       // 實際資料的總頁數（不是 user 指定列印範圍的頁數）。
        private int m_PageNum;			// 程式內部的頁碼。
        private int m_DisplayedPageNum;	// 實際輸出的頁碼。
        private SolidBrush m_TextBrush;
        private Font m_TextFont;
        private double m_TextHeight;
        private double m_LineHeight;

        private PrintDocument m_PrintDoc;

        public PrinterSettings PrinterSettings
        {
            get { return m_PrintDoc.PrinterSettings; }
            set { m_PrintDoc.PrinterSettings = value; }
        }

        public DualPrintHelper(BrailleDocument brDoc, PrintOptions prnOpt)
        {
            m_BrDoc = brDoc;
            m_PrintOptions = prnOpt;

            m_PrintDoc = new PrintDocument();
            m_PrintDoc.BeginPrint += new PrintEventHandler(BrailleText_BeginPrint);
            m_PrintDoc.QueryPageSettings += new QueryPageSettingsEventHandler(BrailleText_QueryPageSettings);
            m_PrintDoc.PrintPage += new PrintPageEventHandler(BrailleText_PrintPage);
            m_PrintDoc.EndPrint += new PrintEventHandler(BrailleText_EndPrint);

            m_PreviewOnly = false;
        }

        /// <summary>
        /// 在執行列印前必須進行的初始化參數工作。明眼字及點字於列印之前都必須呼叫此方法。
        /// </summary>
        private void InitializePrintParameters()
        {
            m_TotalPages = AppGlobals.CalcTotalPages(m_BrDoc.Lines.Count, m_PrintOptions.LinesPerPage, m_PrintOptions.PrintPageFoot);

            if (m_PrintOptions.AllPages)    // 列印全部?
            {
                m_PrintOptions.FromPage = 1;
                m_PrintOptions.ToPage = m_TotalPages;
                m_PageNum = 0;
            }
            else
            {
                // 只列印特定頁次範圍
                m_PageNum = m_PrintOptions.FromPage - 1;

                // 修正終止頁次
                if (m_PrintOptions.ToPage > m_TotalPages)
                {
                    m_PrintOptions.ToPage = m_TotalPages;
                }
            }

            // 起始頁碼
            if (m_PrintOptions.ReassignStartPageNumber)
            {
                m_DisplayedPageNum = m_PrintOptions.StartPageNumber;
            }
            else
            {
                m_DisplayedPageNum = m_PageNum + 1;
            }

            m_BeginOrgPageNumber = null;   // 表示沒有指定原書頁碼。
            m_EndOrgPageNumber = null;   // 表示沒有指定原書頁碼。

            m_PrintedPageCount = 0;
        }


        /// <summary>
        /// 列印明眼字。
        /// </summary>
        public void PrintText(bool previewOnly)
        {
            m_PreviewOnly = previewOnly;

            PrintPreviewDialog previewDlg = new PrintPreviewDialog
            {
                Document = m_PrintDoc
            };
            ((Form)previewDlg).WindowState = FormWindowState.Maximized;

            // 避免預覽對話窗 show 不出來。
            if (Form.ActiveForm == null)
            {
                ((Form)previewDlg).TopMost = true;
            }

            if (previewOnly)
            {
                ((Form)previewDlg).Text = "預覽（不可列印）";
            }

            previewDlg.ShowDialog(Form.ActiveForm);	// 一定要傳入 owner window，否則某些情況會 show 不出來!!
        }

        void BrailleText_BeginPrint(object sender, PrintEventArgs e)
        {
            // 注意: 預覽時以及列印至印表機時都會觸發此事件，這裡應避免重複配置大量資源。

            if (m_PreviewOnly && e.PrintAction != PrintAction.PrintToPreview)
            {
                MessageBox.Show("此功能僅供預覽列印結果，不可直接列印。");
                e.Cancel = true;
                return;
            }

            InitializePrintParameters();	// 初始化列印的共用參數（both 明眼字 and 點字）

            m_PrintDoc.PrinterSettings.PrinterName = m_PrintOptions.PrinterName;
            m_PrintDoc.DocumentName = "易點雙視文件";
            if (!String.IsNullOrEmpty(m_BrDoc.FileName))
            {
                m_PrintDoc.DocumentName = m_BrDoc.FileName;
            }

            // 設定紙張大小
            if (String.IsNullOrEmpty(m_PrintOptions.PaperName))
            {
                // 由程式自動設定紙張
                m_PrintDoc.DefaultPageSettings.PaperSize = new PaperSize("custom", PaperWidth, PaperHeight);
            }
            else
            {
                // 使用先前設定的紙張
                foreach (PaperSize paperSize in m_PrintDoc.PrinterSettings.PaperSizes)
                {
                    if (paperSize.PaperName.Equals(m_PrintOptions.PaperName))
                    {
                        m_PrintDoc.DefaultPageSettings.PaperSize = paperSize;
                        break;
                    }
                }
            }

            // 設定紙張來源
            foreach (PaperSource paperSrc in m_PrintDoc.PrinterSettings.PaperSources)
            {
                if (paperSrc.SourceName.Equals(m_PrintOptions.PaperSourceName))
                {
                    m_PrintDoc.DefaultPageSettings.PaperSource = paperSrc;
                    break;
                }
            }

            // 初始化列印明眼字所需之物件
            m_TextBrush = new SolidBrush(Color.Black);
            m_TextFont = new Font("新細明體", (float)AppGlobals.Config.Printing.PrintTextFontSize);
            m_TextHeight = m_TextFont.GetHeight();
            m_LineHeight = AppGlobals.Config.Printing.PrintTextLineHeight;
        }

        void BrailleText_EndPrint(object sender, PrintEventArgs e)
        {
            if (m_TextBrush != null)
            {
                m_TextBrush.Dispose();
                m_TextBrush = null;
            }
            if (m_TextFont != null)
            {
                m_TextFont.Dispose();
                m_TextFont = null;
            }
        }

        /// <summary>
        /// 傳回目前列印的頁次是否為奇數頁。
        /// </summary>
        /// <returns></returns>
        private bool IsPrintingOddPage()
        {
            if ((m_PageNum + 1) % 2 != 0)
                return true;
            return false;
        }


        private void SetOddPageMargins(QueryPageSettingsEventArgs e)
        {			
            e.PageSettings.Margins.Left = m_PrintOptions.OddPageMargins.Left;
            e.PageSettings.Margins.Top = m_PrintOptions.OddPageMargins.Top;
            e.PageSettings.Margins.Right = m_PrintOptions.OddPageMargins.Right;
            e.PageSettings.Margins.Bottom = m_PrintOptions.OddPageMargins.Bottom;
        }

        private void SetEvenPageMargins(QueryPageSettingsEventArgs e)
        {
            e.PageSettings.Margins.Left = m_PrintOptions.EvenPageMargins.Left;
            e.PageSettings.Margins.Top = m_PrintOptions.EvenPageMargins.Top;
            e.PageSettings.Margins.Right = m_PrintOptions.EvenPageMargins.Right;
            e.PageSettings.Margins.Bottom = m_PrintOptions.EvenPageMargins.Bottom;
        }

        void BrailleText_QueryPageSettings(object sender, QueryPageSettingsEventArgs e)
        {
            //e.PageSettings.PaperSize = m_PaperSize;
            
            // 設定邊界、處理只印奇數頁或偶數頁的情況。
            if (m_PrintOptions.DoubleSideEffect == DoubleSideEffect.OddPages)
            {
                SetOddPageMargins(e);	// 只印奇數頁，邊界固定設定成奇數頁的邊界。
            }
            else if (m_PrintOptions.DoubleSideEffect == DoubleSideEffect.EvenPages)
            {
                SetEvenPageMargins(e);	// 只印偶數頁，邊界固定設定成奇數頁的邊界。
            }
            else
            {
                // 奇數頁偶數頁全部都印。
                if (IsPrintingOddPage())
                {
                    SetOddPageMargins(e);
                }
                else
                {
                    SetEvenPageMargins(e);
                }
            }
        }

        /// <summary>
        /// 計算指定頁次的起始列索引。
        /// </summary>
        /// <param name="pageNum">頁次（zero-based）。</param>
        /// <returns></returns>
        private int CalcTextLineIndex(int pageNum)
        {
            int linesPerPage = m_PrintOptions.LinesPerPage;
            if (m_PrintOptions.PrintPageFoot)
            {
                linesPerPage--;
            }

            return pageNum * linesPerPage;
        }

        /// <summary>
        /// 略過目前的頁。
        /// 注意：雖然目前這頁不印出，但仍要處理其內容，因為其中還要讀取原書頁碼。
        /// </summary>
        private void SkipCurrentPage(PrintPageEventArgs e)
        {
            // 計算起始列索引
            int lineIdx = CalcTextLineIndex(m_PageNum);

            BrailleLine brLine;
            int lineCnt = 0;

            while (lineCnt < m_PrintOptions.LinesPerPage)
            {
                if (lineIdx >= m_BrDoc.Lines.Count)
                {
                    e.HasMorePages = false;
                    break;
                }

                brLine = m_BrDoc.Lines[lineIdx];

                SetOrgPageNumber(brLine, (lineCnt == 0));

                lineIdx++;
                lineCnt++;

                // 是否該印頁尾了？
                if (m_PrintOptions.PrintPageFoot && lineCnt == (m_PrintOptions.LinesPerPage - 1))
                {
                    lineCnt++;
                }
            }

            m_PageNum++;
            m_DisplayedPageNum++;
        }

        /// <summary>
        /// 為每一個新的列印頁初始化起始原書頁碼。
        /// </summary>
        private void SetBeginOrgPageNumberForNewPage()
        {
            // 每一頁開始列印時，都要把上一頁的終止原書頁碼指定給本頁的起始原書頁碼。
            if (!String.IsNullOrEmpty(m_EndOrgPageNumber))
            {
                m_BeginOrgPageNumber = m_EndOrgPageNumber;
            }
        }

        // 每次列印一頁時觸發此事件。
        void BrailleText_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.HasMorePages = true;

            // 每一頁開始列印時，都要把上一頁的終止原書頁碼指定給本頁的起始原書頁碼。
            SetBeginOrgPageNumberForNewPage();

            if (IsPrintingOddPage())	// 正在列印奇數頁?
            {
                if (m_PrintOptions.DoubleSideEffect == DoubleSideEffect.EvenPages)
                {
                    SkipCurrentPage(e);	// 略過奇數頁					
                    SetBeginOrgPageNumberForNewPage();	// 每當略過一頁，要為下一個新頁初始化原書頁碼。
                }
            }
            else
            {
                if (m_PrintOptions.DoubleSideEffect == DoubleSideEffect.OddPages)
                {
                    SkipCurrentPage(e);	// 略過偶數頁
                    SetBeginOrgPageNumberForNewPage();	// 每當略過一頁，要為下一個新頁初始化原書頁碼。
                }
            }

            if (!e.HasMorePages)
            {
                return;
            }

            int lineIdx = 0;
            BrailleLine brLine;
            StringBuilder sb = new StringBuilder();

            // 計算起始列索引
            lineIdx = CalcTextLineIndex(m_PageNum);

            m_PageNum++;
            
            int lineCnt = 0;
            double y = 0;
            int marginLeft = e.MarginBounds.Left;
            int marginTop = e.MarginBounds.Top;            

            // 以下處理已不需要，因為已經增加了偶數頁的列印邊界設定。
            // 若為雙面列印，偶數頁的左邊界要向左偏移，上邊界要向上移。
            //if (m_PrintOptions.DoubleSide && m_PageNum % 2 == 0)
            //{
            //    marginLeft -= 9;
            //    marginTop -= 2;
            //}
           
            while (lineCnt < m_PrintOptions.LinesPerPage)
            {
                if (lineIdx >= m_BrDoc.Lines.Count)
                {
                    e.HasMorePages = false;
                    break;
                }

                brLine = m_BrDoc.Lines[lineIdx];

                SetOrgPageNumber(brLine, (lineCnt == 0));	// 設定原書頁碼

                y = (lineCnt * m_LineHeight) + marginTop; // 計算目前列的 y 軸位置

                PrintLine(brLine, e.Graphics, marginLeft, y, m_BrDoc.CellsPerLine);

                lineCnt++;

                // 是否該印頁尾了？
                if (m_PrintOptions.PrintPageFoot && lineCnt == (m_PrintOptions.LinesPerPage - 1))
                {
                    PrintPageFoot(m_BrDoc.GetPageTitle(lineIdx), m_DisplayedPageNum, 
                        m_BeginOrgPageNumber, m_EndOrgPageNumber, e.Graphics, marginLeft, marginTop);
                    lineCnt++;
                }

                lineIdx++;
            } // end while

            // 補印最後的頁尾
            if (m_PrintOptions.PrintPageFoot && lineIdx == m_BrDoc.Lines.Count)
            {
                if (lineCnt != (m_PrintOptions.LinesPerPage - 1))
                {
                    PrintPageFoot(m_BrDoc.GetPageTitle(lineIdx), m_DisplayedPageNum,
                        m_BeginOrgPageNumber, m_EndOrgPageNumber, e.Graphics, marginLeft, marginTop);
                }
            }

            // 判斷是否為最後一頁
            int stopPageNum = m_PrintOptions.ToPage;

            switch (m_PrintOptions.DoubleSideEffect)   
            {
                case  DoubleSideEffect.OddPages:    // 只印奇數頁
                    if (stopPageNum % 2 == 0)
                    {
                        stopPageNum--;
                    }
                    break;
                case DoubleSideEffect.EvenPages:
                    if (stopPageNum % 2 != 0)
                    {
                        stopPageNum--;
                    }
                    break;
                default:
                    break;
            }

            m_DisplayedPageNum++;

            if (m_PageNum >= stopPageNum)
            {
                e.HasMorePages = false;
            }

            m_PrintedPageCount++;
        }

        /// <summary>
        /// 輸出一列。
        /// </summary>
        /// <param name="brLine">點字列。</param>
        /// <param name="graphics">圖形物件。</param>
        /// <param name="marginLeft">紙張的左邊界。</param>
        /// <param name="y">輸出的 y 軸座標。</param>
        /// <param name="maxCells">最大輸出幾方。</param>
        private void PrintLine(BrailleLine brLine, Graphics graphics, 
            double marginLeft, double y, int maxCells)
        {
            if (brLine == null)
                return;

            BrailleWord brWord;
            int cellCnt = 0;
            double x;

            for (int wordIdx = 0; wordIdx < brLine.WordCount; wordIdx++)
            {
                brWord = brLine.Words[wordIdx];
                if (cellCnt + brWord.CellCount > maxCells)  // 如果目前要印的字將會超過最大方數
                    break;  // 則中止

                x = cellCnt * BrailleCellWidth + marginLeft;
                graphics.DrawString(brWord.Text, m_TextFont, m_TextBrush, (float)x, (float)y);

                cellCnt += brWord.CellCount;
            }
        }

        /// <summary>
        /// 判斷傳入的列是否為原書頁碼，如果是，則設定起始或終止原書頁碼。
        /// </summary>
        /// <param name="brLine"></param>
        /// <param name="isFirstLineOfPage">是否為該頁的第一列。</param>
        private void SetOrgPageNumber(BrailleLine brLine, bool isFirstLineOfPage)
        {
            string line = brLine.ToString();

            string orgPageNum = BrailleProcessor.GetOrgPageNumber(line);
            if (String.IsNullOrEmpty(orgPageNum))
            {
                return;
            }

            if (String.IsNullOrEmpty(m_BeginOrgPageNumber))
            {
                m_BeginOrgPageNumber = orgPageNum;
            }
            if (isFirstLineOfPage)	// 如果該頁的第一列就是原書頁碼
            {
                m_BeginOrgPageNumber = orgPageNum;	// 則起始原書頁碼應該直接使用此頁碼。
            }
            m_EndOrgPageNumber = orgPageNum;
        }

        /// <summary>
        /// 列印頁尾。
        /// </summary>
        /// <param name="title">文件標題點字列。</param>
        /// <param name="pageNum">頁碼。</param>
        /// <param name="beginOrgPageNum">起始原書頁碼。</param>
        /// <param name="endOrgPageNum">終止原書頁碼。</param>
        /// <param name="graphics"></param>
        /// <param name="marginLeft"></param>
        /// <param name="marginTop"></param>
        private void PrintPageFoot(BrailleLine title, int pageNum, string beginOrgPageNum, string endOrgPageNum,
            Graphics graphics, double marginLeft, double marginTop)
        {
            int cellCnt;
            double x;
            double y = (m_PrintOptions.LinesPerPage - 1) * m_LineHeight + marginTop;

            // 取得一個數字字元的列印寬度
            float textWidth = graphics.MeasureString("0", m_TextFont).Width;

            // 點字頁碼
            cellCnt = pageNum.ToString().Length;
            x = (m_BrDoc.CellsPerLine - cellCnt) * BrailleCellWidth + marginLeft;
            if (cellCnt < 2)
            {
                x = x - BrailleCellWidth; // textWidth - 1.6;	// 2008-8-26: 修正個位數頁碼位置太靠右邊的問題
            }
            graphics.DrawString(pageNum.ToString(), m_TextFont, m_TextBrush, (float)x, (float)y);

            // 原書頁碼
            if (!String.IsNullOrEmpty(beginOrgPageNum))
            {
                string orgPageNum = "";
                if (String.IsNullOrEmpty(endOrgPageNum))
                {
                    orgPageNum = beginOrgPageNum;
                }
                else
                {
                    if (beginOrgPageNum == endOrgPageNum)
                    {
                        orgPageNum = beginOrgPageNum;
                    }
                    else
                    {
                        orgPageNum = $"{beginOrgPageNum}-{endOrgPageNum}";
                    }
                }

                cellCnt = orgPageNum.Length + cellCnt + 2;  // 額外加點字頁碼的一個數字點位和一個空方。
                x = (m_BrDoc.CellsPerLine - cellCnt) * BrailleCellWidth + marginLeft;
                if (cellCnt < 2)
                {
                    x = x - BrailleCellWidth; // textWidth - 1.6;	// 2008-8-26: 修正個位數頁碼位置太靠右邊的問題
                }
                graphics.DrawString(orgPageNum, m_TextFont, m_TextBrush, (float)x, (float)y);
            }

            // 文件標題
            int maxCells = m_BrDoc.CellsPerLine - cellCnt - 2;    // 最大可印方數。
            PrintLine(title, graphics, marginLeft, y, maxCells);
        }
    }
}
