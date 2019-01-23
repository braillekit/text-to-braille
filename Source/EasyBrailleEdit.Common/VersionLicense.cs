using System;


namespace EasyBrailleEdit.Common
{
    public static class VersionLicense
    {
        public const int Professional = 0;
        public const int Invalid = 1;
        public const int Home = 2;

        public static bool IsHomeVersion(int flag) => flag == Home;
        public static bool IsValid(int flag) => flag == Professional || flag == Home;

        public static string GetName(int flag)
        {
            switch (flag)
            {
                case Home:
                    return "家用版";
                case Professional:
                    return "專業版";
                default:
                    return "試用版";
            }
        }

        public static bool IsPrintingEnabled(int flag) => IsValid(flag);

        /// <summary>
        /// 最多能輸出幾頁雙視文件。0 表示無限制。家庭版最多可輸出（存檔） 5 頁。
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static int GetMaxOutputPage(int flag) => (flag == Home ? 5 : 0);
    }
}
