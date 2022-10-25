using System;


namespace EasyBrailleEdit.Common
{
    public static class ProductVersionType
    {
        public const int Professional = 0;
        public const int Invalid = 1;
        public const int Home = 2;

        public static bool IsHomeVersion(int flag) => flag == Home;
        public static bool IsValid(int flag) => flag == Professional || flag == Home;
    }
}
