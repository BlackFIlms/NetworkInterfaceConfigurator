using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkInterfaceConfigurator.Models
{
    class Settings : PropChanged
    {
        // Constructor.
        public Settings(string _appFolder)
        {
            appFolder = _appFolder;
            InitSettingsFile();
        }

        // Variables, Constants & Properties.
        private readonly string appFolder;
        public string AppFolder
        {
            get { return appFolder; }
        }

        private string lang;
        public string Lang
        {
            get { return lang; }
            set
            {
                lang = value;
                OnPropertyChanged("Lang");
            }
        }
        private string allowRandomizeMAC;
        public string AllowRandomizeMAC
        {
            get { return allowRandomizeMAC; }
            set
            {
                allowRandomizeMAC = value;
                OnPropertyChanged("AllowRandomizeMAC");
            }
        }

        // Methods.
        private void InitSettingsFile()
        {
            if (!File.Exists(AppFolder + "AppSettings.ini"))
            {
                // Define default settings.
                List<string> defaultSettings = new List<string>();
                defaultSettings.Add("Lang=RU");
                defaultSettings.Add("AllowRandomizeMAC=False");

                // Write settings to file.
                File.AppendAllLines(AppFolder + "AppSettings.ini", defaultSettings.AsEnumerable<string>());

                // ApplySettings.
                string lang = defaultSettings.Find(x => x.Contains("Lang"));
                lang.Replace("Lang=", "");

                string allowRandomizeMAC = defaultSettings.Find(x => x.Contains("AllowRandomizeMAC"));
                allowRandomizeMAC.Replace("AllowRandomizeMAC=", "");

                ApplySettings(lang, allowRandomizeMAC);
            }
            else
            {
                // Load settings from file.
                List<string> settings = new List<string>();
                settings = File.ReadLines(AppFolder + "AppSettings.ini").ToList();

                // Apply settings.
                string lang = settings.Find(x => x.Contains("Lang"));
                lang.Replace("Lang=", "");

                string allowRandomizeMAC = settings.Find(x => x.Contains("AllowRandomizeMAC"));
                allowRandomizeMAC.Replace("AllowRandomizeMAC=", "");

                ApplySettings(lang, allowRandomizeMAC);
            }
        }
        private void ApplySettings(string _lang, string _allowRandomizeMAC)
        {
            Lang = _lang;
            AllowRandomizeMAC = _allowRandomizeMAC;
        }
        public void SaveSettings()
        {
            // Define default settings.
            List<string> settings = new List<string>();
            settings.Add("Lang=" + Lang);
            settings.Add("AllowRandomizeMAC=" + AllowRandomizeMAC);

            // Write settings to file.
            File.Delete(AppFolder + "AppSettings.ini");
            File.AppendAllLines(AppFolder + "AppSettings.ini", settings.AsEnumerable<string>());
        }
    }
}