using System;
using System.Data;
using System.Configuration;
using System.Net;
using System.Text;

namespace GenerateRegFile
{
    internal static class RegDataIndex
    {
        public const int ProductID = 0;
        public const int LicenseKey = 1;
        public const int CustomerName = 2;
        public const int ContactName = 3;
        public const int Email = 4;
        public const int Tel = 5;
        public const int Address = 6;
        public const int IPAddress = 7;
        public const int Version = 8;
        public const int ExpiredDate = 9;
    }

    /// <summary>
    /// 使用者註冊資料。
    /// </summary>
    public class UserRegData
    {        
        private string m_ProductID;     // 產品編號
        private string m_LicenseKey;    // 序號
		private string m_VersionName;	// 版本名稱
		private DateTime m_ExpiredDate;	// 有效期限

        public string CustomerName;
        public string ContactName;
        public string Email;
        public string Tel;
        public string Address;
        public IPAddress IPAddr;
		

        public static UserRegData Parse(string s)
        {
            UserRegData ur = new UserRegData();

            string[] parts = s.Split(';');
            ur.ProductID = parts[RegDataIndex.ProductID];
            ur.LicenseKey = parts[RegDataIndex.LicenseKey];
            ur.CustomerName = parts[RegDataIndex.CustomerName];

            if (parts.Length <= 8)  // 考慮舊版註冊檔的相容性所加的判斷
            {
                // Note: 舊版的註冊資料的欄位索引跟新版的不同，所以要用數字，不能用常數。
                ur.Tel = parts[3];
                ur.Address = parts[4];
                ur.IPAddr = IPAddress.Parse(parts[5]);

                if (parts.Length >= 7)
                {
                    ur.VersionName = parts[6];
                }
                else
                {
                    ur.VersionName = "";
                }

                if (parts.Length == 8)	// 考慮舊版註冊檔的相容性所加的判斷
                {
                    ur.ExpiredDate = DateTime.Parse(parts[7]);
                }
                else
                {
                    ur.ExpiredDate = new DateTime(3000, 1, 1);
                }
            }
            else
            {
                ur.ContactName = parts[RegDataIndex.ContactName];
                ur.Email = parts[RegDataIndex.Email];
                ur.Tel = parts[RegDataIndex.Tel];
                ur.Address = parts[RegDataIndex.Address];
                ur.IPAddr = IPAddress.Parse(parts[RegDataIndex.IPAddress]);
                ur.VersionName = parts[RegDataIndex.Version];
                ur.ExpiredDate = DateTime.Parse(parts[RegDataIndex.ExpiredDate]);
            }

            return ur;
        }

        /// <summary>
        /// 傳回註冊檔名。此函式故意將檔名常數字串簡易編碼（每個字元後面多安插一個任意字元），
        /// 以免破解者直接用搜尋常數字串的方式找到關鍵函式。
        /// </summary>
        /// <returns></returns>
        public static string GetFileName()
        {
            string s = "uastekruroeggx.oddacts";
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < s.Length; i += 2)
            {
                sb.Append(s[i]);
            }
            return sb.ToString();
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ProductID);
            sb.Append(';');
            sb.Append(LicenseKey);
            sb.Append(';');
            sb.Append(CustomerName);
            sb.Append(';');
            sb.Append(ContactName);
            sb.Append(';');
            sb.Append(Email);
            sb.Append(';');
            sb.Append(Tel);
            sb.Append(';');
            sb.Append(Address);
            sb.Append(';');
            sb.Append(IPAddr.ToString());
			sb.Append(';');
			sb.Append(VersionName);
			sb.Append(';');
			sb.Append(ExpiredDate.ToString("yyyy-MM-dd"));

            return sb.ToString();
        }

        public string ProductID
        {
            get { return m_ProductID; }
            set { m_ProductID = value.ToUpper(); }
        }

        public string LicenseKey
        {
            get { return m_LicenseKey; }
            set { m_LicenseKey = value.ToUpper(); }
        }

		public string VersionName
		{
			get { return m_VersionName; }
			set { m_VersionName = value.ToUpper(); }
		}

		public DateTime ExpiredDate
		{
			get { return m_ExpiredDate; }
			set { m_ExpiredDate = value; }
		}
    }
}
