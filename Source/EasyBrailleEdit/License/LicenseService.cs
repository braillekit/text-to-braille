using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyBrailleEdit.Common;
using Huanlin.Windows.Forms;
using Microsoft.Win32;
using Serilog;

namespace EasyBrailleEdit.License
{
    internal static class LicenseService
    {
        private static RegistryKey GetAppRegKey()
        {
            return Registry.CurrentUser.CreateSubKey(@"Software\Michael Tsai\EasyBrailleEdit");
        }

        public static UserLicenseData GetUserLicenseData()
        {
            var regKey = GetAppRegKey();

            var userLic = new UserLicenseData();

            userLic.SerialNumber = Convert.ToString(regKey.GetValue(Constant.SerialNumberRegKey));
            userLic.CustomerName = Convert.ToString(regKey.GetValue(Constant.CustomerNameRegKey));
            return userLic;
        }

        public static void SaveUserLicenseData(UserLicenseData userLic)
        {
            var regKey = GetAppRegKey();
            regKey.SetValue(Constant.SerialNumberRegKey, userLic.SerialNumber, RegistryValueKind.String);
            regKey.SetValue(Constant.CustomerNameRegKey, userLic.CustomerName, RegistryValueKind.String);
            Log.Debug($"成功保存使用者序號 {userLic.SerialNumber} 至 {regKey.Name}");
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static async Task<bool> ValidateUserLicenseAsync(UserLicenseData userLic)
        {
            if (userLic == null || userLic.IsEmpty())
                return false;

            // 下載註冊資訊檔。
            string usersLicFile = Path.Combine(AppGlobals.AppPath, Constant.UsersLicenseFileName);

            await DownloadUserLicenseFileAsync();

            if (!File.Exists(usersLicFile))
            {
                Log.Debug($"沒有發現註冊資料檔 {usersLicFile}");
                return false;
            }

            var usersLicContent = File.ReadAllText(usersLicFile, Encoding.UTF8);
            usersLicContent = Base64Decode(usersLicContent);

            // 比對序號
            foreach (string line in usersLicContent.Split('\n', '\r'))
            {
                var items = line.Split(' ');
                if (items.Length < 3)
                    continue;
                string sn = items[0].Trim().Replace("-", "");
                DateTime expiredDate = Convert.ToDateTime(items[1]);
                int snFlag = Convert.ToInt32(items[2]);

                if (String.IsNullOrWhiteSpace(sn) || sn != userLic.SerialNumber
                    || snFlag != 0 || DateTime.Now > expiredDate)
                    continue;


                // 保存註冊資訊。
                SaveUserLicenseData(userLic);
                AppGlobals.UserLicense.IsValid = true;
                AppGlobals.UserLicense.CustomerName = userLic.CustomerName;
                AppGlobals.UserLicense.SerialNumber = userLic.SerialNumber;
                return true;
            }
            Log.Debug($"使用者輸入的序號無效: {userLic.SerialNumber}");
            return false;

            async Task DownloadUserLicenseFileAsync()
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        // 網址後面加動態參數，以避免快取。
                        string url = Constant.UsersLicenseFileUrl + "?" + DateTime.Now.Ticks;
                        var content = await client.GetStringAsync(url);
                        content = Base64Encode(content);
                        File.WriteAllText(usersLicFile, content, Encoding.UTF8);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"無法下載 {Constant.UsersLicenseFileUrl} : {ex.Message}");
                }
            }
        }

        public static UserLicenseData EnterLicenseData()
        {
            var regForm = new RegForm();
            if (regForm.ShowDialog() != DialogResult.OK)
            {
                return null;
            }

            return new UserLicenseData
            {
                SerialNumber = regForm.LicenseKey,
                CustomerName = regForm.CustomerName
            };
        }
    }
}
