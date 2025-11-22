using System;
using System.IO;
using System.Reflection;
using EasyBrailleEdit.Common.Config;

namespace EasyBrailleEdit.Common
{
    /// <summary>
    /// 提供應用程式全域會用到的變數、常數和公用函式。
    /// </summary>
    public static class AppGlobals
    {
        /// <summary>
        /// 取得應用程式組態設定。
        /// </summary>
        public static AppConfig Config { get; } = AppConfig.GetInstance();

        /// <summary>
        /// 應用程式執行路徑
        /// </summary>
        public static string AppPath { get; set; } = null!;
        
        /// <summary>
        /// 暫存目錄路徑
        /// </summary>
        public static string TempPath { get; } = GetTempPath();

        // Class constructor.
        static AppGlobals()
        {
        }

        /// <summary>
        /// 計算總頁數
        /// </summary>
        /// <param name="totalLines">總列數。</param>
        /// <param name="linesPerPage">每頁可印幾列。</param>
        /// <param name="printPageFoot">是否印頁尾。</param>
        /// <returns></returns>
        public static int CalcTotalPages(int totalLines, int linesPerPage, bool printPageFoot)
        {
            if (printPageFoot)
            {
                linesPerPage--;
            }

            int totalPages = totalLines / linesPerPage;
            if (totalLines % linesPerPage > 0)
            {
                totalPages++;
            }
            return totalPages;
        }

        /// <summary>
        /// 計算指定的列號位於第幾頁。注意：第一頁是傳回 0。
        /// </summary>
        /// <param name="lineNumer">列號，從 0 開始。</param>
        /// <param name="linesPerPage"></param>
        /// <param name="printPageFoot"></param>
        /// <returns>頁號，0-based。</returns>
        public static int CalcCurrentPage(int lineNumer, int linesPerPage, bool printPageFoot)
        {
            if (printPageFoot)
            {
                linesPerPage--;
            }

            int page = lineNumer / linesPerPage;
            return page;
        }

        /// <summary>
        /// 取得暫存目錄路徑
        /// </summary>
        /// <returns></returns>
		public static string GetTempPath()
		{
            Assembly asmb = Assembly.GetExecutingAssembly();
            if (asmb == null)
            {
                throw new Exception("Assembly.GetExecutingAssembly() 無法取得組件!");
            }

            string dirName = Path.GetDirectoryName(asmb.Location);
            string path = Path.Join(dirName, @"Temp\");
           
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			return path;
		}
    }
}
