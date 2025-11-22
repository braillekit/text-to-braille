using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using SharpConfig;
using EasyBrailleEdit.Common.Config.Sections;

namespace EasyBrailleEdit.Common.Config
{
    /// <summary>
    /// 此類別提供了應用程式組態檔的載入與儲存。
    /// </summary>
    public sealed class AppConfig
    {
        /// <summary>
        /// 組態檔名稱。
        /// </summary>
        public const string IniFileName = "AppConfig.ini";
        
        /// <summary>
        /// 預設組態檔名稱。
        /// </summary>
        public const string IniFileNameDefault = "AppConfig.Default.ini";

        private static AppConfig _instance;

        /// <summary>
        /// 取得 AppConfig 的唯一實體。
        /// </summary>
        /// <returns></returns>
        public static AppConfig GetInstance()
        {
            _instance ??= new AppConfig();

            return _instance;
        }

        private string _configFileName;
        private Configuration _config;        

        /// <summary>
        /// 一般設定區段。
        /// </summary>
        public GeneralSection General { get; private set; }
        
        /// <summary>
        /// 點字設定區段。
        /// </summary>
        public BrailleSection Braille { get; private set; }  // 會自動視為區段 [Braille] 的設定
        
        /// <summary>
        /// 點字編輯器設定區段。
        /// </summary>
        public BrailleEditorSection BrailleEditor { get; private set; }  // 會自動視為區段 [BrailleEditor] 的設定
        
        /// <summary>
        /// 列印設定區段。
        /// </summary>
        public PrintingSection Printing { get; private set; } // 會自動視為區段 [Printing] 的設定

        /// <summary>
        /// 建構函式。
        /// </summary>
        public AppConfig()
        {
            General = new GeneralSection();
            Braille = new BrailleSection();
            BrailleEditor = new BrailleEditorSection();
            Printing = new PrintingSection();

            Load();
        }

        private static void CreateDefaultConfigFile(string filename)
        {
            StringBuilder sb = new();
            sb.AppendLine(";應用程式組態檔");
            sb.AppendLine("[General}]");
            sb.AppendLine($"AutoUpdate=true");
            sb.AppendLine($"AutoUpdateFilesUrl={Constant.DefaultAutoUpdateFilesUrl}");

            File.WriteAllText(filename, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// 載入組態檔。
        /// </summary>
        public void Load()
        {
            Assembly asmb = Assembly.GetExecutingAssembly() ?? throw new Exception("Assembly.GetExecutingAssembly() 無法取得組件!");

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
                    CreateDefaultConfigFile(filename);
                }
            }

            _configFileName = filename;

            Configuration.IgnoreInlineComments = false;
            Configuration.SupressArrayParsing = true;
            _config = Configuration.LoadFromFile(_configFileName, Encoding.UTF8);

            General = _config[GeneralSection.Name].ToObject<GeneralSection>();
            Braille = _config[BrailleSection.Name].ToObject<BrailleSection>();
            BrailleEditor = _config[BrailleEditorSection.Name].ToObject<BrailleEditorSection>();
            Printing = _config[PrintingSection.Name].ToObject<PrintingSection>();
        }

        /// <summary>
        /// 儲存組態檔。
        /// </summary>
        public void Save()
        {
            _config[GeneralSection.Name].GetValuesFrom(General);
            _config[BrailleSection.Name].GetValuesFrom(Braille);
            _config[BrailleEditorSection.Name].GetValuesFrom(BrailleEditor);
            _config[PrintingSection.Name].GetValuesFrom(Printing);
            _config.SaveToFile(_configFileName, Encoding.UTF8);
        }
    }

}
