using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using NetworkInterfaceConfigurator.Models;
using NetworkInterfaceConfigurator.Views;

namespace NetworkInterfaceConfigurator.ViewModels
{
    class EditPresetViewModel : PropChanged
    {
        // Constructor.
        public EditPresetViewModel(Preset pr, PresetsDB pDB, int prCount)
        {
            // Get preset without link to object.
            CurrentPreset.ID = pr.ID;
            CurrentPreset.Name = pr.Name;
            CurrentPreset.IP = pr.IP;
            CurrentPreset.Subnet = pr.Subnet;
            CurrentPreset.Gateway = pr.Gateway;
            CurrentPreset.DNS1 = pr.DNS1;
            CurrentPreset.DNS2 = pr.DNS2;
            CurrentPreset.MAC = pr.MAC;
            CurrentPreset.MACR = pr.MACR;

            presetsDB = pDB;

            presetsCount = prCount;
        }

        // Variables, Constants & Properties.
        private Preset currentPreset = new Preset();
        public Preset CurrentPreset
        {
            get { return currentPreset; }
            set
            {
                currentPreset = value;
                OnPropertyChanged("CurrentPreset");
            }
        }
        PresetsDB presetsDB;
        int presetsCount;


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

                          // Write changes to DB.
                          presetsDB.DBinit();
                          presetsDB.EditPreset(CurrentPreset);
                          presetsDB.Disconnect();

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
        private RelayCommand deleteBtn;
        public RelayCommand DeleteBtn
        {
            get
            {
                return deleteBtn ??
                  (deleteBtn = new RelayCommand(obj =>
                  {
                      try
                      {
                          Window w = obj as Window;

                          // Write changes to DB.
                          presetsDB.DBinit();
                          presetsDB.DeletePreset(Convert.ToInt32(CurrentPreset.ID), presetsCount);
                          presetsDB.Disconnect();

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
