using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using BrailleToolkit.Helpers;
using Huanlin.Common.Helpers;

namespace BrailleToolkit
{
    /// <summary>
    /// 點字碼列舉常數。
    /// </summary>
    public enum BrailleCellCode
    {
        /// <summary>
        /// 空方的點字碼的十六進位字串。
        /// </summary>
        Blank = 0x00,
        
        /// <summary>
        /// 大寫
        /// </summary>
        Capital = 0x20,
        
        /// <summary>
        /// 數字
        /// </summary>
        Digit = 0x3C,
        
        /// <summary>
        /// 斜體
        /// </summary>
        Italic = 0x28,
        
        /// <summary>
        /// 連字號 '-'
        /// </summary>
        Hyphen = 0x24
    }

    /// <summary>
    /// 代表點字的一方 (Braille Cell Dimension)。
    /// </summary>
    [Serializable]
    [DataContract]
    public readonly record struct BrailleCell
    {
        private static readonly BrailleCell[] s_AllCells = CreateAllCells();

        private static BrailleCell[] CreateAllCells()
        {
            // 建立好 256 個 BrailleCell 值。
            // NOTE: 其實只用到前面 64 個，因為只有六點，用到的位元為 0..5，範圍是
            //       00..3F。考慮到未來支援八點的點字，故仍保留 256 個入口以維持 API 相容性。
            var allCells = new BrailleCell[256];
            for (int i = 0; i < allCells.Length; i++)
            {
                allCells[i] = new BrailleCell((byte)i);
            }
            return allCells;
        }

        /// <summary>
        /// Gets a BrailleCell instance for the specified braille cell code.
        /// </summary>
        /// <param name="code">The braille cell code.</param>
        /// <returns>The BrailleCell instance.</returns>
        public static BrailleCell GetInstance(BrailleCellCode code)
        {
            return s_AllCells[(int)code];
        }

        /// <summary>
        /// Gets a BrailleCell instance at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The BrailleCell instance.</returns>
        public static BrailleCell GetInstance(int index)
        {
            if (index < 0 || index >= s_AllCells.Length)
                throw new IndexOutOfRangeException("傳入 BrailleCell.GetInstance() 的索引超出範圍!");
            return s_AllCells[index];
        }

        /// <summary>
        /// Gets a BrailleCell instance from a hexadecimal string.
        /// </summary>
        /// <param name="hexStr">The hexadecimal string.</param>
        /// <returns>The BrailleCell instance.</returns>
        public static BrailleCell GetInstance(string hexStr)
        {
            if (String.IsNullOrEmpty(hexStr) || hexStr.Length > 2)
                throw new ArgumentException("參數錯誤: 不是有效的十六進位字串值!");
            return GetInstance(StrHelper.HexStrToByte(hexStr));
        }

        /// <summary>
        /// 以指定點位的方式傳回 BrailleCell 物件。
        /// </summary>
        /// <param name="positionNumbers">點位陣列，例如：3、6 點則為 new int[] {3, 6}。</param>
        /// <returns></returns>
        public static BrailleCell GetInstance(int[] positionNumbers)
        {
            return GetInstance(PositionNumbersToByte(positionNumbers));
        }

        /// <summary>
        /// Gets a BrailleCell instance from a position number string.
        /// </summary>
        /// <param name="positionNumberString">The position number string (e.g., "1356").</param>
        /// <returns>The BrailleCell instance.</returns>
        public static BrailleCell GetInstanceFromPositionNumberString(string positionNumberString)
        {
            return GetInstance(PositionNumberStringToByte(positionNumberString));
        }

        /// <summary>
        /// 將點位轉換成 byte 值。
        /// </summary>
        /// <param name="posNumbers">點位</param>
        /// <returns></returns>
        public static byte PositionNumbersToByte(params int[] posNumbers)
        {
            BitArray bits = new BitArray(8, false);

            foreach (int posNum in posNumbers)
            {
                if (posNum < 1 || posNum > 6)
                    throw new ArgumentException("參數錯誤：{posNum}。點位必須為 1～6 點!");
                bits[posNum - 1] = true;
            }

            return ConvertHelper.BitsToByte(bits);
        }

        /// <summary>
        /// 將點位轉換成 byte 值。
        /// </summary>
        /// <param name="posNumberString">一方點字的點位。例如 "1356"。</param>
        /// <returns></returns>
        public static byte PositionNumberStringToByte(string posNumberString)
        {
            BitArray bits = new BitArray(8, false);

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
        /// <returns></returns>
        public static string PositionNumbersToHexString(string[] positionNumbers)
        {
            var result = new StringBuilder();
            foreach (var posNum in positionNumbers)
            {
                var byteValue = PositionNumberStringToByte(posNum);
                result.Append(byteValue.ToString("X2"));
            }
            return result.ToString();
        }

        /// <summary>
        /// 初始化 BrailleCell 的新執行個體。
        /// </summary>
        /// <param name="value">點字方的位元組值。</param>
        public BrailleCell(byte value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets or sets the value of this braille cell.
        /// </summary>
        [DataMember]
        public byte Value { get; init; }

        /// <summary>
        /// Gets a blank braille cell.
        /// </summary>
        public static BrailleCell Blank
        {
            get
            {
                return GetInstance(BrailleCellCode.Blank);
            }
        }

        /// <summary>
        /// Gets a capital braille cell.
        /// </summary>
        public static BrailleCell Capital
        {
            get
            {
                return GetInstance(BrailleCellCode.Capital);
            }
        }


        /// <summary>
        /// Returns a string representation of this braille cell.
        /// </summary>
        /// <returns>The hexadecimal string.</returns>
        public override string ToString()
        {
            return ToHexString();
        }

        /// <summary>
        /// Converts this braille cell to a hexadecimal string.
        /// </summary>
        /// <returns>The hexadecimal string.</returns>
        public string ToHexString()
        {
            return BrailleCellHelper.ByteToHexString(Value);
        }

        /// <summary>
        /// Converts this braille cell to a position number string.
        /// </summary>
        /// <returns>The position number string.</returns>
        public string ToPositionNumberString()
        {
            return BrailleCellHelper.ByteToPositionNumberString(Value);
        }

        /// <summary>
        /// Converts this braille cell to an array of position numbers.
        /// </summary>
        /// <returns>The position number array.</returns>
        public int[] ToPositionNumberArray()
        {
            return BrailleCellHelper.HexStringToPositionNumbers(ToHexString());
        }
    }
}
