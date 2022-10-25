using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBrailleEdit.Common
{
    public sealed record UserLicenseData
    {
        public bool IsActive { get; set; }
        public string CustomerName { get; set; }
        public string SerialNumber { get; set; }
        public int VersionLicense { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public bool IsEmpty => (string.IsNullOrEmpty(SerialNumber) || string.IsNullOrEmpty(CustomerName));


        public bool IsExpired => ExpiredDate.HasValue ? DateTime.Now.Date > ExpiredDate.Value.Date : false;

        public bool IsValid => IsActive && !IsEmpty && !IsExpired;

        public bool IsTrial => !IsActive || IsEmpty;


        public string GetProductVersionName()
        {
            if (!IsValid)
            {
                return "試用版";
            }

            switch (VersionLicense)
            {
                case ProductVersionType.Home:
                    return "家用版";
                case ProductVersionType.Professional:
                    return "專業版";
                default:
                    return "試用版";
            }
        }

        /// <summary>
        /// 是否接近試用期限？
        /// </summary>
        /// <param name="beforeDays">試用期限之前幾天需要提醒</param>
        /// <returns></returns>
        public bool IsNearExpiration(int beforeDays)
        {
            if (IsValid || IsExpired) return false;
            if (ExpiredDate == null) return false;

            return (DateTime.Now.Date > ExpiredDate.Value.Date.AddDays(0 - beforeDays));
        }


        public int GetMaxPages()
        {
            const int Unlimited = 99999;
            if (IsValid) // 已經註冊
            {
                if (VersionLicense == ProductVersionType.Home)
                {
                    return Constant.HomeVersionMaxPages;
                }
                return Unlimited;

            }

            if (IsExpired) // 已過試用期限
            {
                return Constant.TrialExpiredMaxPages;
            }

            // 還在試用期間。
            return Constant.TrialVersionMaxPages;
        }
    }
}
