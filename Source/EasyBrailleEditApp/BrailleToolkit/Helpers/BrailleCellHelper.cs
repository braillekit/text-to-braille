using Huanlin.Common.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleToolkit.Helpers
{
    public static class BrailleCellHelper
    {
        public static string ByteToHexString(byte code)
        {
            return code.ToString("X2");
        }

        public static byte HexStringToByte(string hexStr)
        {
            return StrHelper.HexStrToByte(hexStr);
        }

        public static string ByteToPositionNumberString(byte code)
        {
            var sb = new StringBuilder();
            byte x = code;
            int dot = 1;
            while (dot <= 6)
            {
                if ((x & 1) == 1)
                {
                    sb.Append(dot.ToString());
                }
                x = (byte)(x >> 1);
                dot++;
            }
            return sb.ToString();
        }

        public static string HexStringToPositionNmberString(string hexStr)
        {
            return ByteToPositionNumberString(HexStringToByte(hexStr));
        }

        /// <summary>
        /// 將點位轉換成 byte 值。
        /// </summary>
        /// <param name="posNumberString">一方點字的點位。例如 "1356"。</param>
        /// <returns></returns>
        public static byte PositionNumberStringToByte(string posNumberString)
        {
            var bits = new BitArray(8, false);

            for (int i = 0; i < posNumberString.Length; i++)
            {
                int posNum = StrHelper.ToInteger(posNumberString[i].ToString(), 0);
                if (posNum < 1 || posNum > 6)
                    throw new ArgumentException($"參數錯誤：'{posNumberString}'。點位必須為 1～6 點!");
                bits[posNum - 1] = true;
            }

            return ConvertHelper.BitsToByte(bits);
        }

        /// <summary>
        /// 把陣列中的點位字串轉換成十六進制代碼。
        /// </summary>
        /// <param name="positionNumbers"></param>
        /// <returns>範例：{"1C", "22"}</returns>
        public static string PositionNumbersToHexString(string[] positionNumbers)
        {
            var result = new StringBuilder();
            for (int i = 0; i < positionNumbers.Length; i++)
            {
                var byteValue = PositionNumberStringToByte(positionNumbers[i]);
                result.Append(ByteToHexString(byteValue));
            }
            return result.ToString();
        }


        /// <summary>
        /// 把陣列中的點位字串轉換成十六進制代碼。
        /// </summary>
        /// <param name="positionNumbers"></param>
        /// <returns>範例：{"1C", "22"}</returns>
        public static string[] PositionNumbersToHexStringArray(string[] positionNumbers)
        {
            var result = new string[positionNumbers.Length];
            for (int i = 0; i < positionNumbers.Length; i++)
            {
                var byteValue = PositionNumberStringToByte(positionNumbers[i]);
                result[i] = ByteToHexString(byteValue);
            }
            return result;
        }

    }
}
