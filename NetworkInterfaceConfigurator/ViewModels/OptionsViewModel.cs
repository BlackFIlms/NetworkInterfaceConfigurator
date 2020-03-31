using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NetworkInterfaceConfigurator.Models;
using NetworkInterfaceConfigurator.Views;

namespace NetworkInterfaceConfigurator.ViewModels
{
    class OptionsViewModel : PropChanged
    {
        // Constructor.
        public OptionsViewModel(string _appFolder, Settings _settings)
        {
            settings = _settings;

            appFolder = _appFolder;

            // Get settings without link to object.
            tempSettings = new Settings(AppFolder);
            TempSettings.Lang = _settings.Lang;
            TempSettings.AllowRandomizeMAC = _settings.AllowRandomizeMAC;

            // Define languages.
            Languages = new ObservableCollection<string>();
            Languages.Add("RU");
            Languages.Add("EN");
        }

        // Variables, Constants & Properties.
        private Settings settings; // Field for save link, to origin settings object.
        private readonly string appFolder;
        string AppFolder
        {
            get { return appFolder; }
        }
        private Settings tempSettings;
        public Settings TempSettings
        {
            get { return tempSettings; }
            set
            {
                tempSettings = value;
                OnPropertyChanged("TempSettings");
            }
        }
        public ObservableCollection<string> Languages { get; set; } // List of available langs.

        #region Control logics for window.

        // Define command for close Window.
        public RelayCommand CloseWin
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Window closeWindowCommand = obj as Window;

                    if (closeWindowCommand != null)
                    {
                        closeWindowCommand.Close();
                    }
                });
            }
        }

        // Define command for drag Window.
        public RelayCommand DragWindow
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Window dragCommand = obj as Window;

                    if (dragCommand != null)
                    {
                        dragCommand.DragMove();
                    }
                });
            }
        }
        #endregion

        #region Center header title.
        private string centerTitle;
        public string CenterTitle
        {
            get { return centerTitle; }

            set
            {
                centerTitle = value;
                OnPropertyChanged("CenterTitle");
            }
        }
        private string windowWidth;
        public string GetWindowWidth
        {
            get { return windowWidth; }
            set
            {
                windowWidth = value;
                OnPropertyChanged("GetWindowWidth");
                CalcMarginTitle();
            }
        }
        private string titleHeaderWidth;
        public string GetTitleHeaderWidth
        {
            get { return titleHeaderWidth; }
            set
            {
                titleHeaderWidth = value;
                OnPropertyChanged("GetTitleHeaderWidth");
                CalcMarginTitle();
            }
        }
        public void CalcMarginTitle()
        {
            // Swap culture.My default culture - "ru-RU".Need culture, for ConvertToDouble en-US.

            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            provider.NumberGroupSeparator = ",";
            provider.NumberGroupSizes = new int[] { 3 };

            double res = (Convert.ToDouble(GetWindowWidth, provider) / 2) - (Convert.ToDouble(GetTitleHeaderWidth, provider) / 2);
            res = Math.Round(res); // Rounds the result so that the title borders occupy full pixels.
            CenterTitle = res.ToString().Replace(',', '.') + ", 0, 0, 0";
        }
        #endregion

        #region Buttons
        private RelayCommand okBtn;
        public RelayCommand OkBtn
        {
            get
            {
                return okBtn ??
                  (okBtn = new RelayCommand(obj =>
                  {
                      try
                      {
                          Window w = obj as Window;

                          // Update settings.
                          settings.Lang = TempSettings.Lang;
                          settings.AllowRandomizeMAC = TempSettings.AllowRandomizeMAC;
                          // Save to file.
                          settings.SaveSettings();

                          // Close window.
                          w.DialogResult = true;
                      }
                      catch (WarningException e)
                      {
                          MessageBox.Show(e.Message, "Warning");
                      }
                      catch (Exception e)
                      {
                          MessageBox.Show(e.Message, "Error");
                      }
                  }));
            }
        }
        #endregion
    }
}
