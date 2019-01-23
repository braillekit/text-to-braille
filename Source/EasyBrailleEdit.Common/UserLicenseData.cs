using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBrailleEdit.Common
{
    public class UserLicenseData
    {
        public bool IsActive { get; set; }
        public string CustomerName { get; set; }
        public string SerialNumber { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public bool IsEmpty()
        {
            if (string.IsNullOrEmpty(SerialNumber) || string.IsNullOrEmpty(CustomerName))
            {
                return true;
            }
            return false;
        }


        public bool IsExpired()
        {
            if (ExpiredDate.HasValue)
            {
                return (DateTime.Now > ExpiredDate);
            }
            return false;
        }

        public bool IsValid()
        {
            return IsActive && !IsEmpty() && !IsExpired();
        }

    }
}
