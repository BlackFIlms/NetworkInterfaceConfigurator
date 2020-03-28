using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using NetworkInterfaceConfigurator.Models;
using NetworkInterfaceConfigurator.Views;

namespace NetworkInterfaceConfigurator.ViewModels
{
    class MainViewModel : PropChanged
    {
        // Constructor.
        public MainViewModel()
        {
            // Create directory for app settings if it does not exist.
            if (!Directory.Exists(presetsDB.AppFolder))
                Directory.CreateDirectory(presetsDB.AppFolder);

            // Presets init.
            GetPresets();

            // Adapters init.
            GetAdapters();
        }

        // Variables, Constants & Properties.
        PresetsDB presetsDB = new PresetsDB();


        private string debug;
        public string Debug
        {
            get { return debug; }

            set
            {
                debug = value;
                OnPropertyChanged("Debug");
            }
        }

        public ObservableCollection<NetworkInterfaceLib> Adapters { get; set; }

        private NetworkInterfaceLib selectedAdapter;
        public NetworkInterfaceLib SelectedAdapter
        {
            get { return selectedAdapter; }
            set
            {
                selectedAdapter = value;
                InitAdapterProperties(value); // After selection adapter, loading adapter properties.
                OnPropertyChanged("SelectedAdapter");
            }
        }

        public ObservableCollection<Preset> presets;
        public ObservableCollection<Preset> Presets
        {
            get {return presets; }
            set
            {
                presets = value;
                OnPropertyChanged("Presets");
            }
        }

        #region Control logics for window.

        // Define command for minimize Window.
        public RelayCommand MinWin
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Window minWindowCommand = obj as Window;

                    if (minWindowCommand != null)
                    {
                        void _MinWin(Window w) => SystemCommands.MinimizeWindow(w); // Define a function to send arguments to the object MainWindow.
                        _MinWin(minWindowCommand);
                    }
                });
            }
        }

        // Define command for restore||maximize Window.
        public RelayCommand MaxWin
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Window maxWindowCommand = obj as Window;

                    if (maxWindowCommand != null)
                    {
                        void _MaxWin(Window w) // Define a function to send arguments to the object MainWindow.
                        {
                            if (w.WindowState == WindowState.Maximized) SystemCommands.RestoreWindow(w);
                            else SystemCommands.MaximizeWindow(w);
                        }
                        _MaxWin(maxWindowCommand);
                    }
                });
            }
        }

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
        private string iconHeaderWidth;
        public string GetIconHeaderWidth
        {
            get { return iconHeaderWidth; }
            set
            {
                iconHeaderWidth = value;
                OnPropertyChanged("GetIconHeaderWidth");
                CalcMarginTitle();
            }
        }
        private string menuHeaderWidth;
        public string GetMenuHeaderWidth
        {
            get { return menuHeaderWidth; }
            set
            {
                menuHeaderWidth = value;
                OnPropertyChanged("GetMenuHeaderWidth");
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

            double res = ((Convert.ToDouble(GetWindowWidth, provider) / 2) - Convert.ToDouble(GetIconHeaderWidth, provider) - Convert.ToDouble(GetMenuHeaderWidth, provider)) - (Convert.ToDouble(GetTitleHeaderWidth, provider) / 2);
            res = Math.Round(res); // Rounds the result so that the title borders occupy full pixels.
            CenterTitle = res.ToString().Replace(',', '.') + ", 0, 0, 0";
        }
        #endregion

        #region AdaptersInit
        /*
            Manual initialization.
            U can try change this methods on constructor.
            But i don't sure, what this can be work in all cases.

            First step: get index for all adapters.
            Step two: validate adapters.
            Step three: Create new object for each adapter and set base properties.
        */
        public IEnumerable<NetworkInterfaceLib> InitAdapters()
        {
            foreach (string adapterIndex in NetworkInterfaceLib.GetNicIndexes())
            {
                if ((NetworkInterfaceLib.GetMAC(adapterIndex) != "Empty") && (NetworkInterfaceLib.GetNicName(adapterIndex) != null))
                {
                    NetworkInterfaceLib obj = new NetworkInterfaceLib();
                    obj.NicIndex = adapterIndex;
                    obj.NicID = NetworkInterfaceLib.GetNicID(adapterIndex);
                    obj.NicName = NetworkInterfaceLib.GetNicName(adapterIndex);
                    obj.NetName = NetworkInterfaceLib.GetNetName(adapterIndex);
                    //InitAdapterProperties(obj); // Properties init for all adapters, together with adapters init.
                    yield return obj;
                }
            }
        }
        // Adapter properties loading only for selected adapter.
        public void InitAdapterProperties(NetworkInterfaceLib obj)
        {
            obj.IPEnabled = NetworkInterfaceLib.IsIpEnabled(obj.NicIndex);
            obj.IP = NetworkInterfaceLib.GetIP(obj.NicIndex);
            obj.Subnet = NetworkInterfaceLib.GetSubnet(obj.NicIndex);
            obj.Gateway = NetworkInterfaceLib.GetGateway(obj.NicIndex);

            for (int i = 0; i < NetworkInterfaceLib.GetDNS(obj.NicIndex).Length; i++)
            {
                if (i == 0)
                {
                    obj.DNS1 = NetworkInterfaceLib.GetDNS(obj.NicIndex)[0];
                }
                else if (i == 1)
                {
                    obj.DNS2 = NetworkInterfaceLib.GetDNS(obj.NicIndex)[1];
                }
            }

            obj.MAC = NetworkInterfaceLib.GetMAC(obj.NicIndex);
        }
        #endregion

        #region GetAdapters
        public void GetAdapters()
        {
            Adapters = new ObservableCollection<NetworkInterfaceLib>(InitAdapters());
        }
        #endregion

        #region Buttons
        // Apply all properties from MainWindow TextBoxes to adapter.
        private RelayCommand changeSettings;
        public RelayCommand ChangeSettings
        {
            get
            {
                return changeSettings ??
                  (changeSettings = new RelayCommand(obj =>
                  {
                      try
                      {
                          List<object> parameters = obj as List<object>;
                          Debug = parameters.Count.ToString() + " ";

                          foreach (TextBox item in parameters)
                          {
                              Debug += item.Name + "=" + item.Text + " ";
                              switch (item.Name)
                              {
                                  case "adapterSetIP":
                                      SelectedAdapter.IP = item.Text; // Writes ip from TextBox to adapter property.
                                      break;
                                  case "adapterSetSubnet":
                                      SelectedAdapter.Subnet = item.Text; // Writes subnet from TextBox to adapter property.
                                      NetworkInterfaceLib.SetStatic(SelectedAdapter.NicIndex, SelectedAdapter.IP, SelectedAdapter.Subnet); // Apply new ip and subnet to adapter.
                                      break;
                                  case "adapterSetGateway":
                                      SelectedAdapter.Gateway = item.Text; // Writes gateway from TextBox to adapter property.
                                      NetworkInterfaceLib.SetGateway(SelectedAdapter.NicIndex, SelectedAdapter.Gateway); // Apply new gateway to adapter.
                                      break;
                                  case "adapterSetDNS2": // Change property setting queue of DNS1 and DNS2.
                                      SelectedAdapter.DNS2 = item.Text; // Writes dns1 from TextBox to adapter property.
                                      break;
                                  case "adapterSetDNS1":
                                      SelectedAdapter.DNS1 = item.Text; // Writes dns2 from TextBox to adapter property.
                                      NetworkInterfaceLib.SetDNS(SelectedAdapter.NicIndex, SelectedAdapter.DNS1, SelectedAdapter.DNS2); // Apply new dns1 and dns2 to adapter.
                                      break;
                                  case "adapterSetMAC":
                                      SelectedAdapter.MAC = item.Text; // Writes mac address form TextBox to adapter propery.
                                      NetworkInterfaceLib.SetMAC(SelectedAdapter.NicID, SelectedAdapter.MAC); // Apply new mac address to adapter.
                                      break;
                              }
                          }
                      }
                      catch (NullReferenceException e)
                      {
                          MessageBox.Show("You did not select adapter!" + "\r\n" + e, "Error");
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

        private RelayCommand clearSettings;
        public RelayCommand ClearSettings
        {
            get
            {
                return clearSettings ??
                  (clearSettings = new RelayCommand(obj =>
                  {
                      List<object> parameters = obj as List<object>;

                      foreach (TextBox item in parameters)
                      {
                          item.Text = "";
                      }
                  }));
            }
        }

        private RelayCommand updateSettings;
        public RelayCommand UpdateSettings
        {
            get
            {
                return updateSettings ??
                  (updateSettings = new RelayCommand(obj =>
                  {
                      try
                      {
                          InitAdapterProperties(SelectedAdapter);
                      }
                      catch (NullReferenceException e)
                      {
                          MessageBox.Show("You did not select adapter!", "Error");
                      }
                  }));
            }
        }
        #endregion

        #region Presets
        /// <summary>
        /// Presets init. Load all existing presets from DB.
        /// </summary>
        public IEnumerable<Preset> InitPresets()
        {
            presetsDB.DBinit();
            foreach (object[] objArr in presetsDB.Load())
            {
                Preset pr = new Preset();

                pr.ID = objArr[0].ToString();
                pr.Name = objArr[1].ToString();
                pr.IP = objArr[2].ToString();
                pr.Subnet = objArr[3].ToString();
                pr.Gateway = objArr[4].ToString();
                pr.DNS1 = objArr[5].ToString();
                pr.DNS2 = objArr[6].ToString();
                pr.MAC = objArr[7].ToString();
                pr.MACR = objArr[8].ToString();

                yield return pr;
            }
            presetsDB.Disconnect();
        }

        public void GetPresets()
        {
            Presets = new ObservableCollection<Preset>(InitPresets());
        }

        // Preset buttons.
        private RelayCommand addPreset;
        public RelayCommand AddPreset
        {
            get
            {
                return addPreset ??
                    (addPreset = new RelayCommand(obj =>
                    {
                        if (Presets.Count < 8)
                        {
                            Preset pr = new Preset();

                            // Add preset to DB.
                            presetsDB.DBinit();
                            presetsDB.AddPreset(pr, out int id);

                            // Set preset id and name.
                            pr.ID = id.ToString();
                            pr.Name = "Preset " + id;

                            // Add to presets collection.
                            Presets.Add(pr);

                            // Update preset name in DB.
                            presetsDB.EditPreset(pr);
                            presetsDB.Disconnect();
                        }
                        else
                        {
                            // Write error to debug.
                        }
                    }));
            }
        }
        private RelayCommand applyPreset;
        public RelayCommand ApplyPreset
        {
            get
            {
                return applyPreset ??
                  (applyPreset = new RelayCommand(obj =>
                  {
                      Button presetBtn = obj as Button;
                      try
                      {
                          // Get preset settings, from presets collection.
                          int x = 0;
                          while (presetBtn.Tag.ToString() != Presets[x].ID)
                          {
                              x++;
                          }
                          Preset pr = Presets[x];

                          // Write preset settings to SelectedAdapter properties. It does not install in the adapter settings, only in the interface.
                          SelectedAdapter.IP = pr.IP;
                          SelectedAdapter.Subnet = pr.Subnet;
                          SelectedAdapter.Gateway = pr.Gateway;
                          SelectedAdapter.DNS1 = pr.DNS1;
                          SelectedAdapter.DNS2 = pr.DNS2;
                          if (pr.MACR == "true")
                          {
                              // NetworkInterfaceLib.RandMAC();
                          }
                          else
                          {
                              SelectedAdapter.MAC = pr.MAC;
                          }
                      }
                      catch (NullReferenceException e)
                      {
                          MessageBox.Show("You did not select adapter!" + "\r\n" + e, "Error");
                      }
                  }));
            }
        }
        private RelayCommand saveCurrentSettingsToPreset;
        public RelayCommand SaveCurrentSettingsToPreset
        {
            get
            {
                return saveCurrentSettingsToPreset ??
                  (saveCurrentSettingsToPreset = new RelayCommand(obj =>
                  {
                      Button presetBtn = obj as Button;
                      try
                      {
                          // Get preset settings, from presets collection.
                          int x = 0;
                          while (presetBtn.Tag.ToString() != Presets[x].ID)
                          {
                              x++;
                          }
                          Preset pr = Presets[x];

                          // Write SelectedAdapter properties to preset settings.
                          pr.IP = SelectedAdapter.IP;
                          pr.Subnet = SelectedAdapter.Subnet;
                          pr.Gateway = SelectedAdapter.Gateway;
                          pr.DNS1 = SelectedAdapter.DNS1;
                          pr.DNS2 = SelectedAdapter.DNS2;
                          pr.MAC = SelectedAdapter.MAC;

                          // Update preset in DB.
                          presetsDB.DBinit();
                          presetsDB.EditPreset(pr);
                          presetsDB.Disconnect();
                      }
                      catch (NullReferenceException e)
                      {
                          MessageBox.Show("You did not select adapter!" + "\r\n" + e, "Error");
                      }
                  }));
            }
        }
        private RelayCommand editPreset;
        public RelayCommand EditPreset
        {
            get
            {
                return editPreset ??
                    (editPreset = new RelayCommand(obj =>
                    {
                        Button presetBtn = obj as Button;
                        // Get preset settings, from presets collection.
                        int x = 0;
                        while (presetBtn.Tag.ToString() != Presets[x].ID)
                        {
                            x++;
                        }
                        Preset pr = Presets[x];

                        // Create and open window for edit preset.
                        var w = new EditPresetWindow();
                        var vm = new EditPresetViewModel(pr, presetsDB, Presets.Count);
                        w.DataContext = vm;
                        bool? result = w.ShowDialog();

                        // Edit Presets collection.
                        if (result.ToString() == "True")
                        {
                            Presets.Clear();
                            GetPresets();
                        }
                    }));
            }
        }
        #endregion
    }
}