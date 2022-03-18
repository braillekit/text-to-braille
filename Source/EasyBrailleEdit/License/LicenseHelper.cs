using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyBrailleEdit.Common;
using Microsoft.Win32;
using Serilog;
using Huanlin.Common.Extensions;

namespace EasyBrailleEdit.License
{
    internal static class LicenseHelper
    {
        private static RegistryKey GetAppRegKey()
        {
            return Registry.CurrentUser.CreateSubKey(Constant.AppRegistryRoot);
        }

        public static UserLicenseData GetUserLicenseData()
        {
            var regKey = GetAppRegKey();

            var userLic = new UserLicenseData();

            try
            {
                userLic.SerialNumber = Convert.ToString(regKey.GetValue(Constant.SerialNumberRegKey));
                userLic.CustomerName = Convert.ToString(regKey.GetValue(Constant.CustomerNameRegKey));
                userLic.VersionLicense = Convert.ToInt32(regKey.GetValue(Constant.VersionLicenseRegKey));
                var expDateStr = Convert.ToString(regKey.GetValue(Constant.ExpiredDateRegKey));
                if (string.IsNullOrWhiteSpace(expDateStr))
                {
                    userLic.ExpiredDate = null;
                }
                else
                {
                    userLic.ExpiredDate = Convert.ToDateTime(expDateStr);
                }
                
            }
            catch
            {
                // ignore error.
            }
            return userLic;
        }

        public static void SaveUserLicenseData(UserLicenseData userLic)
        {
            var regKey = GetAppRegKey();
            regKey.SetValue(Constant.SerialNumberRegKey, userLic.SerialNumber, RegistryValueKind.String);
            regKey.SetValue(Constant.CustomerNameRegKey, userLic.CustomerName, RegistryValueKind.String);
            regKey.SetValue(Constant.VersionLicenseRegKey, userLic.VersionLicense, RegistryValueKind.String);
            regKey.SetValue(Constant.ExpiredDateRegKey, userLic.ExpiredDate?.ToString("yyyy/MM/dd"), RegistryValueKind.String);
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

        /// <summary>
        /// 驗證傳入的使用者授權資料，如果驗證通過，則保存於全域變數 AppGlobals.UserLicense。
        /// </summary>
        /// <param name="userLic"></param>
        /// <returns></returns>
        public static async Task<bool> ValidateAndSaveUseLicenseAsync(UserLicenseData userLic)
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
                int versionFlag = Convert.ToInt32(items[2]);

                if (string.IsNullOrWhiteSpace(sn) || sn != userLic.SerialNumber || DateTime.Now > expiredDate)
                    continue;
                if (!VersionLicense.IsValid(versionFlag))
                    continue;

                // Overwrite 使用期限
                userLic.ExpiredDate = expiredDate;
                userLic.VersionLicense = versionFlag;

                // 保存註冊資訊。
                SaveUserLicenseData(userLic);
                AppGlobals.UserLicense.IsActive = true;
                AppGlobals.UserLicense.CustomerName = userLic.CustomerName;
                AppGlobals.UserLicense.SerialNumber = userLic.SerialNumber;
                AppGlobals.UserLicense.ExpiredDate = userLic.ExpiredDate;
                AppGlobals.UserLicense.VersionLicense = versionFlag;
                return true;
            }
            Log.Debug($"使用者輸入的序號無效: {userLic.SerialNumber}");
            return false;

            async Task DownloadUserLicenseFileAsync()
            {
                string url = Constant.DefaultAutoUpdateRootUrl.EnsureEndWith("/") + Constant.UsersLicenseFileName
                           + "?" + DateTime.Now.Ticks; // 網址後面加動態參數，以避免快取。
                try
                {
                    using (var client = new HttpClient())
                    {                        
                        var content = await client.GetStringAsync(url);
                        content = Base64Encode(content);
                        File.WriteAllText(usersLicFile, content, Encoding.UTF8);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"無法下載 {url} : {ex.Message}");
                }
            }
        }

        public static void SetTrialExpirationDate()
        {
            var regKey = GetAppRegKey();

            var s = Convert.ToString(regKey.GetValue(Constant.ExpiredDateRegKey));
            if (string.IsNullOrWhiteSpace(s))
            {
                var expDate = DateTime.Now.AddDays(30);
                regKey.SetValue(Constant.ExpiredDateRegKey, expDate.ToString("yyyy/MM/dd"), RegistryValueKind.String);
            }
        }

        public static bool IsTrialExpired()
        {
            var regKey = GetAppRegKey();

            try
            {
                var expDate = Convert.ToDateTime(regKey.GetValue(Constant.ExpiredDateRegKey));
                return DateTime.Now > expDate;
            }
            catch
            {
                return true;
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

        public static bool NeedTrackUser()
        {
            var regKey = GetAppRegKey();

            try
            {
                var lastTime = Convert.ToDateTime(regKey.GetValue(Constant.LastTimeTrackingUser));                
                if (lastTime < DateTime.Now.AddDays(-1)) // 每兩天追蹤一次
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return true;
            }
        }

        public static void SaveTrackUserTime()
        {
            var regKey = GetAppRegKey();
            regKey.SetValue(Constant.LastTimeTrackingUser, DateTime.Now.ToString("yyyy/MM/dd"));
        }
    }
}
