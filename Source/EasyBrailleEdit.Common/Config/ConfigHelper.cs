using System;
using System.IO;
using System.Reflection;
using System.Text;
using Config.Net;

namespace EasyBrailleEdit.Common.Config
{
    /// <summary>
    /// 此類別是用來封裝應用程式組態檔。
    /// </summary>
    public static class ConfigHelper
    {
        private const string IniFileName = "AppConfig.ini";
        private const string IniFileNameDefault = "AppConfig.Default.ini";

        public static IAppConfig CreateConfig()
        {
            Assembly asmb = Assembly.GetExecutingAssembly();
            if (asmb == null)
            {
                throw new Exception("Assembly.GetExecutingAssembly() 無法取得組件!");
            }
            string path = Path.GetDirectoryName(asmb.Location);
            string filename = Path.Combine(path, IniFileName);

            if (!File.Exists(filename))
            {
                string defaultIni = Path.Combine(path, IniFileNameDefault);
                if (File.Exists(defaultIni))
                {
                    File.Copy(defaultIni, filename);
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(";應用程式組態檔");
                    sb.AppendLine("[General}]");
                    sb.AppendLine($"AutoUpdate=true");
                    sb.AppendLine($"AutoUpdateFilesUrl={Constant.DefaultAutoUpdateFilesUrl}");
                    File.WriteAllText(filename, sb.ToString(), Encoding.UTF8);
                }
            }
            // note: parseInlineComments 必須為 true，否則無法讀出正確的設定值，因為 .ini 裡面有用到行內註解。
            var config = new ConfigurationBuilder<IAppConfig>()
                .UseIniFile(filename, parseInlineComments: true) 
                .Build();
            return config;
        }
    }
}
