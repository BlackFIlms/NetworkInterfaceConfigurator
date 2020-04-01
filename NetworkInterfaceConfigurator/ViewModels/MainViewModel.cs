using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
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
            if (!Directory.Exists(AppFolder))
                Directory.CreateDirectory(AppFolder);

            // Settings init.
            settings = new Settings(AppFolder);

            // Log init.
            File.Delete(AppFolder + "Log.txt");
            LogList = new ObservableCollection<LogEntry>();

            // Presets init.
            presetsDB = new PresetsDB(this);
            GetPresets();

            // Adapters init.
            GetAdapters();
        }

        // Variables, Constants & Properties.
        private readonly string appFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NetworkInterfaceConfigurator\\"); // Path for app settings.
        public string AppFolder
        {
            get { return appFolder; }
        }
        private Settings settings;
        public Settings Settings
        {
            get { return settings; }
            set
            {
                settings = value;
                OnPropertyChanged("Settings");
            }
        }

        public ObservableCollection<LogEntry> LogList { get; set; }
        private LogEntry debug;
        public LogEntry Debug
        {
            get { return debug; }

            set
            {
                debug = value;
                debug.Time = debug.DateTime.ToLongTimeString();

                // Add entry to log list.
                LogList.Add(debug);
                // Writes log to file.
                File.AppendAllText(AppFolder + "Log.txt","Time: " + debug.Time + " " + "Index: " + "#" + debug.Index + " " + "Message: " + debug.Message + " " + "Type: " + debug.GetType().ToString().Replace("NetworkInterfaceConfigurator.ViewModels.LogEntry", "") + "\r\n");

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

                // Copy current adapter values, to TextBoxes in gridAdapterSettings.
                tempAdapter.IP = selectedAdapter.IP;
                tempAdapter.Subnet = selectedAdapter.Subnet;
                tempAdapter.Gateway = selectedAdapter.Gateway;
                tempAdapter.DNS1 = selectedAdapter.DNS1;
                tempAdapter.DNS2 = selectedAdapter.DNS2;
                tempAdapter.MAC = selectedAdapter.MAC;

                // Add log entry.
                Debug = new LogEntryMessage()
                {
                    DateTime = DateTime.Now,
                    Index = LogEntry.IndexCount,
                    Message = "Adapter settings initialized."
                };

                OnPropertyChanged("SelectedAdapter");
            }
        }
        // Temp adapter for decrease count of false positives validation.
        private NetworkInterfaceLib tempAdapter = new NetworkInterfaceLib();
        public NetworkInterfaceLib TempAdapter
        {
            get { return tempAdapter; }
            set
            {
                tempAdapter = value;
                OnPropertyChanged("TempAdapter");
            }
        }

        PresetsDB presetsDB;

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
                        // Copy log file, for save it before next start.
                        if (File.Exists(AppFolder + "Log.txt"))
                        {
                            string copyFileName = DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToLongTimeString().Replace(":", "-") + "_" + "Log.txt";
                            if (!Directory.Exists(AppFolder + "OldLogs\\"))
                                Directory.CreateDirectory(AppFolder + "OldLogs\\");
                            File.Copy(AppFolder + "Log.txt", AppFolder + "OldLogs\\" + copyFileName);
                        }

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

        #region Menu
        private RelayCommand openFile;
        public RelayCommand OpenFile
        {
            get
            {
                return openFile ??
                    (openFile = new RelayCommand(obj =>
                    {
                        List<string> adapterSettings = new List<string>();

                        // Open file.
                        OpenFileDialog path = new OpenFileDialog();
                        path.Filter = "Text files(*.txt) | *.txt";
                        path.ShowDialog();
                        
                        try
                        {
                            adapterSettings = File.ReadLines(path.FileName).ToList();
                            
                            // Find settings in list, remove names from settings and apply settings to temp adapter.
                            TempAdapter.IP = adapterSettings.Find(x => x.Contains("IP")).Replace("IP=", "");
                            TempAdapter.Subnet = adapterSettings.Find(x => x.Contains("Subnet")).Replace("Subnet=", "");
                            TempAdapter.Gateway = adapterSettings.Find(x => x.Contains("Gateway")).Replace("Gateway=", "");
                            TempAdapter.DNS1 = adapterSettings.Find(x => x.Contains("DNS1")).Replace("DNS1=", "");
                            TempAdapter.DNS2 = adapterSettings.Find(x => x.Contains("DNS2")).Replace("DNS2=", "");
                            TempAdapter.MAC = adapterSettings.Find(x => x.Contains("MAC")).Replace("MAC=", "");

                            // Add log entry.
                            Debug = new LogEntryMessage()
                            {
                                DateTime = DateTime.Now,
                                Index = LogEntry.IndexCount,
                                Message = "File openned and settings copied to text fields."
                            };
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message, "Error");

                            // Add log entry.
                            Debug = new LogEntryError()
                            {
                                DateTime = DateTime.Now,
                                Index = LogEntry.IndexCount,
                                Message = "Error: " + e.Message + " " + "Source: " + e.Source + " " + "TargetSite: " + e.TargetSite + " " + "StackTrace: " + e.StackTrace // Presentation of the error, variant 1.
                            };
                        }
                    }));
            }
        }
        private RelayCommand saveFile;
        public RelayCommand SaveFile
        {
            get
            {
                return saveFile ??
                    (saveFile = new RelayCommand(obj =>
                    {
                        List<string> adapterSettings = new List<string>();

                        // Save file.
                        SaveFileDialog path = new SaveFileDialog();
                        path.Filter = "Text files(*.txt) | *.txt";
                        path.ShowDialog();

                        try
                        {
                            // Write current settings to list.
                            adapterSettings.Add("IP=" + SelectedAdapter.IP);
                            adapterSettings.Add("Subnet=" + SelectedAdapter.Subnet);
                            adapterSettings.Add("Gateway=" + SelectedAdapter.Gateway);
                            adapterSettings.Add("DNS1=" + SelectedAdapter.DNS1);
                            adapterSettings.Add("DNS2=" + SelectedAdapter.DNS2);
                            adapterSettings.Add("MAC=" + SelectedAdapter.MAC);

                            // Write current settings to file.
                            File.AppendAllLines(path.FileName, adapterSettings.AsEnumerable<string>());
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message, "Error");

                            // Add log entry.
                            Debug = new LogEntryError()
                            {
                                DateTime = DateTime.Now,
                                Index = LogEntry.IndexCount,
                                Message = "Error: " + e
                            };
                        }
                    }));
            }
        }
        private RelayCommand openOptionsWindow;
        public RelayCommand OpenOptionsWindow
        {
            get
            {
                return openOptionsWindow ??
                    (openOptionsWindow = new RelayCommand(obj =>
                    {
                        // Create and open window for edit options.
                        var w = new OptionsWindow();
                        var vm = new OptionsViewModel(AppFolder, Settings);
                        w.DataContext = vm;
                        bool? result = w.ShowDialog();

                        if (result.Value)
                        {
                            // Add log entry.
                            Debug = new LogEntryMessage()
                            {
                                DateTime = DateTime.Now,
                                Index = LogEntry.IndexCount,
                                Message = "Options changed."
                            };
                        }
                        else
                        {
                            // Add log entry.
                            Debug = new LogEntryMessage()
                            {
                                DateTime = DateTime.Now,
                                Index = LogEntry.IndexCount,
                                Message = "Options not changed."
                            };
                        }
                    }));
            }
        }
        private RelayCommand openUpdateLink;
        public RelayCommand OpenUpdateLink
        {
            get
            {
                return openUpdateLink ??
                    (openUpdateLink = new RelayCommand(obj =>
                    {
                        Process.Start("https://github.com/BlackFIlms/NetworkInterfaceConfigurator/releases");
                    }));
            }
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

            // Add log entry.
            Debug = new LogEntryMessage()
            {
                DateTime = DateTime.Now,
                Index = LogEntry.IndexCount,
                Message = "Adapters initialized."
            };
        }
        #endregion

        #region Buttons
        private RelayCommand randomizeMAC;
        public RelayCommand RandomizeMAC
        {
            get
            {
                return randomizeMAC ??
                  (randomizeMAC = new RelayCommand(obj =>
                  {
                      try
                      {
                          TempAdapter.MAC = RandMAC.GetRandMAC();

                          // Add log entry.
                          Debug = new LogEntryMessage()
                          {
                              DateTime = DateTime.Now,
                              Index = LogEntry.IndexCount,
                              Message = "MAC randomized."
                          };
                      }
                      catch (NullReferenceException e)
                      {
                          MessageBox.Show("You did not select adapter!", "Error");

                          // Add log entry.
                          Debug = new LogEntryError()
                          {
                              DateTime = DateTime.Now,
                              Index = LogEntry.IndexCount,
                              Message = "Error(NullReferenceException): " + e
                          };
                      }
                  }));
            }
        }

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

                          foreach (TextBox item in parameters)
                          {
                              // Add log entry.
                              Debug = new LogEntryMessage()
                              {
                                  DateTime = DateTime.Now,
                                  Index = LogEntry.IndexCount,
                                  Message = "Changing settings: " + item.Name + " " + "Value: " + item.Text
                              };

                              bool changingState;
                              switch (item.Name)
                              {
                                  case "adapterSetIP":
                                      SelectedAdapter.IP = item.Text; // Writes ip from TextBox to adapter property.
                                      break;
                                  case "adapterSetSubnet":
                                      SelectedAdapter.Subnet = item.Text; // Writes subnet from TextBox to adapter property.
                                      changingState = NetworkInterfaceLib.SetStatic(SelectedAdapter.NicIndex, SelectedAdapter.IP, SelectedAdapter.Subnet); // Apply new ip and subnet to adapter.
                                      if (changingState)
                                      {
                                          // Add log entry.
                                          Debug = new LogEntryMessage()
                                          {
                                              DateTime = DateTime.Now,
                                              Index = LogEntry.IndexCount,
                                              Message = "Adapter: " + SelectedAdapter.NicName + " " + "Network: " + SelectedAdapter.NetName + " " + "IP and Subnet settings changed."
                                          };
                                      }
                                      else
                                      {
                                          // Add log entry.
                                          Debug = new LogEntryMessage()
                                          {
                                              DateTime = DateTime.Now,
                                              Index = LogEntry.IndexCount,
                                              Message = "Adapter: " + SelectedAdapter.NicName + " " + "Network: " + SelectedAdapter.NetName + " " + "IP and Subnet settings not changed."
                                          };
                                      }
                                      break;
                                  case "adapterSetGateway":
                                      SelectedAdapter.Gateway = item.Text; // Writes gateway from TextBox to adapter property.
                                      changingState = NetworkInterfaceLib.SetGateway(SelectedAdapter.NicIndex, SelectedAdapter.Gateway); // Apply new gateway to adapter.
                                      if (changingState)
                                      {
                                          // Add log entry.
                                          Debug = new LogEntryMessage()
                                          {
                                              DateTime = DateTime.Now,
                                              Index = LogEntry.IndexCount,
                                              Message = "Adapter: " + SelectedAdapter.NicName + " " + "Network: " + SelectedAdapter.NetName + " " + "Gateway settings changed."
                                          };
                                      }
                                      else
                                      {
                                          // Add log entry.
                                          Debug = new LogEntryMessage()
                                          {
                                              DateTime = DateTime.Now,
                                              Index = LogEntry.IndexCount,
                                              Message = "Adapter: " + SelectedAdapter.NicName + " " + "Network: " + SelectedAdapter.NetName + " " + "Gateway settings not changed."
                                          };
                                      }
                                      break;
                                  case "adapterSetDNS2": // Change property setting queue of DNS1 and DNS2.
                                      SelectedAdapter.DNS2 = item.Text; // Writes dns1 from TextBox to adapter property.
                                      break;
                                  case "adapterSetDNS1":
                                      SelectedAdapter.DNS1 = item.Text; // Writes dns2 from TextBox to adapter property.
                                      changingState = NetworkInterfaceLib.SetDNS(SelectedAdapter.NicIndex, SelectedAdapter.DNS1, SelectedAdapter.DNS2); // Apply new dns1 and dns2 to adapter.
                                      if (changingState)
                                      {
                                          // Add log entry.
                                          Debug = new LogEntryMessage()
                                          {
                                              DateTime = DateTime.Now,
                                              Index = LogEntry.IndexCount,
                                              Message = "Adapter: " + SelectedAdapter.NicName + " " + "Network: " + SelectedAdapter.NetName + " " + "DNS settings changed."
                                          };
                                      }
                                      else
                                      {
                                          // Add log entry.
                                          Debug = new LogEntryMessage()
                                          {
                                              DateTime = DateTime.Now,
                                              Index = LogEntry.IndexCount,
                                              Message = "Adapter: " + SelectedAdapter.NicName + " " + "Network: " + SelectedAdapter.NetName + " " + "DNS settings not changed."
                                          };
                                      }
                                      break;
                                  case "adapterSetMAC":
                                      SelectedAdapter.MAC = item.Text; // Writes mac address form TextBox to adapter propery.
                                      changingState = NetworkInterfaceLib.SetMAC(SelectedAdapter.NicID, SelectedAdapter.MAC); // Apply new mac address to adapter.
                                      if (changingState)
                                      {
                                          // Add log entry.
                                          Debug = new LogEntryMessage()
                                          {
                                              DateTime = DateTime.Now,
                                              Index = LogEntry.IndexCount,
                                              Message = "Adapter: " + SelectedAdapter.NicName + " " + "Network: " + SelectedAdapter.NetName + " " + "MAC settings changed."
                                          };
                                      }
                                      else
                                      {
                                          // Add log entry.
                                          Debug = new LogEntryMessage()
                                          {
                                              DateTime = DateTime.Now,
                                              Index = LogEntry.IndexCount,
                                              Message = "Adapter: " + SelectedAdapter.NicName + " " + "Network: " + SelectedAdapter.NetName + " " + "MAC settings not changed."
                                          };
                                      }
                                      break;
                              }
                          }

                          // Add log entry.
                          Debug = new LogEntryMessage()
                          {
                              DateTime = DateTime.Now,
                              Index = LogEntry.IndexCount,
                              Message = "Adapter settings changed."
                          };
                      }
                      catch (NullReferenceException e)
                      {
                          MessageBox.Show("You did not select adapter!", "Error");

                          // Add log entry.
                          Debug = new LogEntryError()
                          {
                              DateTime = DateTime.Now,
                              Index = LogEntry.IndexCount,
                              Message = "Error(NullReferenceException): " + e
                          };
                      }
                      catch (WarningException e)
                      {
                          MessageBox.Show(e.Message, "Warning");

                          // Add log entry.
                          Debug = new LogEntryWarning()
                          {
                              DateTime = DateTime.Now,
                              Index = LogEntry.IndexCount,
                              Message = "Warning: " + e
                          };
                      }
                      catch (Exception e)
                      {
                          MessageBox.Show(e.Message, "Error");

                          // Add log entry.
                          Debug = new LogEntryError()
                          {
                              DateTime = DateTime.Now,
                              Index = LogEntry.IndexCount,
                              Message = "Error: " + e
                          };
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

                      // Add log entry.
                      Debug = new LogEntryMessage()
                      {
                          DateTime = DateTime.Now,
                          Index = LogEntry.IndexCount,
                          Message = "Text fields with settings cleared."
                      };
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

                          // Add log entry.
                          Debug = new LogEntryMessage()
                          {
                              DateTime = DateTime.Now,
                              Index = LogEntry.IndexCount,
                              Message = "Adapter settings updated."
                          };
                      }
                      catch (NullReferenceException e)
                      {
                          MessageBox.Show("You did not select adapter!", "Error");

                          // Add log entry.
                          Debug = new LogEntryError()
                          {
                              DateTime = DateTime.Now,
                              Index = LogEntry.IndexCount,
                              Message = "Error(NullReferenceException): " + e
                          };
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
            presetsDB.DBinit(AppFolder);
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

            // Add log entry.
            Debug = new LogEntryMessage()
            {
                DateTime = DateTime.Now,
                Index = LogEntry.IndexCount,
                Message = "Presets initialized."
            };
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
                            presetsDB.DBinit(AppFolder);
                            presetsDB.AddPreset(pr, out int id);

                            // Set preset id and name.
                            pr.ID = id.ToString();
                            pr.Name = "Preset " + id;

                            // Add to presets collection.
                            Presets.Add(pr);

                            // Update preset name in DB.
                            presetsDB.EditPreset(pr);
                            presetsDB.Disconnect();
                            
                            // Add log entry.
                            Debug = new LogEntryMessage()
                            {
                                DateTime = DateTime.Now,
                                Index = LogEntry.IndexCount,
                                Message = "Preset added."
                            };
                        }
                        else
                        {
                            // Add log entry.
                            Debug = new LogEntryWarning()
                            {
                                DateTime = DateTime.Now,
                                Index = LogEntry.IndexCount,
                                Message = "Warning: " + "The number of presets reached a maximum."
                            };
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
                          TempAdapter.IP = pr.IP;
                          TempAdapter.Subnet = pr.Subnet;
                          TempAdapter.Gateway = pr.Gateway;
                          TempAdapter.DNS1 = pr.DNS1;
                          TempAdapter.DNS2 = pr.DNS2;
                          if (Convert.ToBoolean(pr.MACR))
                          {
                              TempAdapter.MAC = RandMAC.GetRandMAC();
                          }
                          else
                          {
                              TempAdapter.MAC = pr.MAC;
                          }
                          
                          // Add log entry.
                          Debug = new LogEntryMessage()
                          {
                              DateTime = DateTime.Now,
                              Index = LogEntry.IndexCount,
                              Message = "Preset settings copied to text fields."
                          };
                      }
                      catch (NullReferenceException e)
                      {
                          MessageBox.Show("You did not select adapter!", "Error");

                          // Add log entry.
                          Debug = new LogEntryError()
                          {
                              DateTime = DateTime.Now,
                              Index = LogEntry.IndexCount,
                              Message = "Error(NullReferenceException): " + e
                          };
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
                          presetsDB.DBinit(AppFolder);
                          presetsDB.EditPreset(pr);
                          presetsDB.Disconnect();

                          // Add log entry.
                          Debug = new LogEntryMessage()
                          {
                              DateTime = DateTime.Now,
                              Index = LogEntry.IndexCount,
                              Message = "Current adapter settings copied to preset settings."
                          };
                      }
                      catch (NullReferenceException e)
                      {
                          MessageBox.Show("You did not select adapter!" + "\r\n" + e, "Error");

                          // Add log entry.
                          Debug = new LogEntryError()
                          {
                              DateTime = DateTime.Now,
                              Index = LogEntry.IndexCount,
                              Message = "Error(NullReferenceException): " + e
                          };
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
                        var vm = new EditPresetViewModel(pr, presetsDB, Presets.Count, Settings);
                        w.DataContext = vm;
                        bool? result = w.ShowDialog();

                        // Edit Presets collection.
                        if (result.Value)
                        {
                            Presets.Clear();
                            GetPresets();

                            // Add log entry.
                            Debug = new LogEntryMessage()
                            {
                                DateTime = DateTime.Now,
                                Index = LogEntry.IndexCount,
                                Message = "Preset settings edited."
                            };
                        }
                        else
                        {
                            // Add log entry.
                            Debug = new LogEntryMessage()
                            {
                                DateTime = DateTime.Now,
                                Index = LogEntry.IndexCount,
                                Message = "Preset settings not changed."
                            };
                        }
                    }));
            }
        }
        #endregion
    }
}