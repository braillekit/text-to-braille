﻿using System;
using System.Drawing.Printing;

namespace EasyBrailleEdit
{
    // 雙面列印效果
    public enum DoubleSideEffect
    {
        None,
        OddPages,
        EvenPages
    }

    /// <summary>
    /// 列印選項。
    /// </summary>
    public class PrintOptions
    {
        // 列印範圍
        private bool m_AllPages;    // 列印全部？
        private int m_FromPage;
        private int m_ToPage;


        public PrintOptions() : base()
        {
            m_AllPages = true;
            m_FromPage = 0;
            m_ToPage = 0;

            DoubleSide = false;
            PrintPageFoot = true;
            ReassignStartPageNumber = false;
            StartPageNumber = 1;
            LinesPerPage = AppConst.DefaultLinesPerPage;
            DoubleSideEffect = DoubleSideEffect.None;

            PaperSourceName = "";
            PaperName = "custom";

            OddPageMargins = new Margins();
            EvenPageMargins = new Margins();

            BrUseNewLineForPageBreak = false;
            BrSendPageBreakAtEndOfDoc = false;
        }

        /// <summary>
        /// 起始頁數。
        /// </summary>
        public int FromPage
        {
            get { return m_FromPage; }
            set
            {
                if (value < 1)
                    throw new ArgumentException("起始頁數不可小於 1。");
                m_FromPage = value;
            }
        }

        /// <summary>
        /// 終止頁數。
        /// </summary>
        public int ToPage
        {
            get { return m_ToPage; }
            set
            {
                if (value < 1)
                    throw new ArgumentException("終止頁數不可小於 1。");
                m_ToPage = value;
            }
        }

        /// <summary>
        /// 檢查列印範圍是否有效。
        /// </summary>
        public void CheckRange()
        {
            if (m_AllPages)
                return;
            if (m_ToPage < m_FromPage)
                throw new ArgumentException("終止頁數不可小於起始頁數!");
        }

        public bool AllPages
        {
            get { return m_AllPages; }
            set { m_AllPages = value; }
        }

        public int LinesPerPage { get; set; }

        public bool DoubleSide { get; set; }

        public bool PrintPageFoot { get; set; }

        public int StartPageNumber { get; set; }

        public DoubleSideEffect DoubleSideEffect { get; set; }

        public string PrinterName { get; set; }

        public string PrinterNameForBraille { get; set; }

        public string PaperSourceName { get; set; }

        public string PaperName { get; set; }

        public Margins OddPageMargins { get; set; }

        public Margins EvenPageMargins { get; set; }

        public bool ReassignStartPageNumber { get; set; }

        /// <summary>
        /// 列印點字時，是否在最後輸出一個跳頁符號。default: false。
        /// </summary>
        public bool BrSendPageBreakAtEndOfDoc { get; set; }

        /// <summary>
        /// 列印點字時，是否用一個換行符號來取代跳頁符號（這是舊版的行為，新版本預設都使用跳頁符號）。
        /// 2016-11-02: 新版本的行為是根據 ET Trident 打印機來修改，比較合理。舊版本用換行符號來迫使印表機跳頁的作法不好。
        /// </summary>
        public bool BrUseNewLineForPageBreak { get; set; }
    }
}
