using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBrailleEdit.Common
{
    public class UserLicenseData
    {
        public bool IsValid { get; set; }
        public string CustomerName { get; set; }
        public string SerialNumber { get; set; }
        public int VersionLicense { get; set; }

        public bool IsEmpty()
        {
            if (String.IsNullOrEmpty(SerialNumber) || String.IsNullOrEmpty(CustomerName))
            {
                return true;
            }
            return false;
        }
    }
}
