using System;
using System.IO;
using System.Reflection;
using EasyBrailleEdit.Common.Config;

namespace EasyBrailleEdit.Common
{
    public static class AppGlobals
    {
        public static IAppConfig Config { get; private set; } = ConfigHelper.CreateConfig();
        public static string TempPath { get; private set; } = GetTempPath();

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

		public static string GetTempPath()
		{
            Assembly asmb = Assembly.GetExecutingAssembly();
            if (asmb == null)
            {
                throw new Exception("Assembly.GetExecutingAssembly() 無法取得組件!");
            }

            string path = Path.Combine(Path.GetDirectoryName(asmb.Location), @"\Temp\");
           
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			return path;
		}

        public static bool IsLicensedFor_TaipeiForBlind()
        {
            return Config.AutoUpdateFilesUrl.IndexOf(Constant.ProductBranches.TaipeiForBlind, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }
    }
}
