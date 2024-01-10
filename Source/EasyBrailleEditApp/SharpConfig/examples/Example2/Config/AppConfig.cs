using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using SharpConfig;
using Example2.Config.Sections;

namespace Example2.Config
{
    /// <summary>
    /// Load and save configuration.
    /// </summary>
    public class AppConfig
    {
        private const string IniFileName = "AppConfig.ini";

        private static AppConfig _instance;

        public static AppConfig GetInstance()
        {
            if (_instance == null)
            {
                _instance = new AppConfig();
            }

            return _instance;
        }

        private string _configFileName;
        private Configuration _config;

        public GeneralSection General { get; private set; }
        public PrintingSection Printing { get; private set; }

        public AppConfig()
        {
            Assembly asmb = Assembly.GetExecutingAssembly();
            string path = Path.GetDirectoryName(asmb.Location);
            _configFileName = Path.Combine(path, IniFileName);

            // Create section objects for default configuration values.
            General = new GeneralSection();
            Printing = new PrintingSection();

            Load();
        }

        public void Load()
        {
            Configuration.IgnoreInlineComments = false;
            _config = Configuration.LoadFromFile(_configFileName, Encoding.UTF8);

            General = _config[GeneralSection.Name].ToObject<GeneralSection>();
            Printing = _config[PrintingSection.Name].ToObject<PrintingSection>();
        }

        public void Save()
        {
            _config[GeneralSection.Name].GetValuesFrom(General);
            _config[PrintingSection.Name].GetValuesFrom(Printing);
            _config.SaveToFile(_configFileName, Encoding.UTF8);
        }
    }

}
