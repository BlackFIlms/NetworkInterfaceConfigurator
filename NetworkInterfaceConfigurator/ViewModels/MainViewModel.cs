using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
    class MainViewModel : ProperyChanged
    {
        // Constructor.
        public MainViewModel()
        {
            GetAdapters();
        }

        // Variables, Constants & Properties.
        string tempIP;
        string tempDNS2;


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
        private string gridWidth;
        public string GridWidth
        {
            get { return gridWidth; }
            set
            {
                gridWidth = value;
                OnPropertyChanged("GridWidth");
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

        #region GetAdapters
        public void GetAdapters()
        {
            Adapters = new ObservableCollection<NetworkInterfaceLib>(Init());
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
        public IEnumerable<NetworkInterfaceLib> Init()
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
                else if (i == 1 )
                {
                    obj.DNS2 = NetworkInterfaceLib.GetDNS(obj.NicIndex)[1];
                }
            }

            obj.MAC = NetworkInterfaceLib.GetMAC(obj.NicIndex);
        }
        #endregion

        #region Buttons
        private RelayCommand changeProperties;
        public RelayCommand ChangeProperties
        {
            get
            {
                return changeProperties ??
                  (changeProperties = new RelayCommand(obj =>
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
                                      tempIP = SelectedAdapter.IP; // Writes ip to field, for get it in next case.
                                      SelectedAdapter.IP = item.Text; // Writes ip from TextBox to adapter property.
                                      break;
                                  case "adapterSetSubnet":
                                      string tempSubnet = SelectedAdapter.Subnet;
                                      SelectedAdapter.Subnet = item.Text; // Writes subnet from TextBox to adapter property.
                                      if ((SelectedAdapter.IP != tempIP) || (SelectedAdapter.Subnet != tempSubnet))
                                          NetworkInterfaceLib.SetStatic(SelectedAdapter.NicIndex, SelectedAdapter.IP, SelectedAdapter.Subnet); // Apply new ip and subnet to adapter.
                                      break;
                                  case "adapterSetGateway":
                                      string tempGateway = SelectedAdapter.Gateway;
                                      SelectedAdapter.Gateway = item.Text; // Writes gateway from TextBox to adapter property.
                                      if (SelectedAdapter.Gateway != tempGateway)
                                          NetworkInterfaceLib.SetGateway(SelectedAdapter.NicIndex, SelectedAdapter.Gateway); // Apply new gateway to adapter.
                                      break;
                                  case "adapterSetDNS2": // Change property setting queue of DNS1 and DNS2.
                                      tempDNS2 = SelectedAdapter.DNS2; // Writes dns1 to field, for get it in next case.
                                      SelectedAdapter.DNS2 = item.Text; // Writes dns1 from TextBox to adapter property.
                                      break;
                                  case "adapterSetDNS1":
                                      string tempDNS1 = SelectedAdapter.DNS1;
                                      SelectedAdapter.DNS1 = item.Text; // Writes dns2 from TextBox to adapter property.
                                      if ((SelectedAdapter.DNS1 != tempDNS1) || (SelectedAdapter.DNS2 != tempDNS2))
                                          NetworkInterfaceLib.SetDNS(SelectedAdapter.NicIndex, SelectedAdapter.DNS1, SelectedAdapter.DNS2); // Apply new dns1 and dns2 to adapter.
                                      break;
                                  case "adapterSetMAC":
                                      string tempMAC = SelectedAdapter.MAC;
                                      SelectedAdapter.MAC = item.Text; // Writes mac address form TextBox to adapter propery.
                                      if (SelectedAdapter.MAC != tempMAC)
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
        public RelayCommand EditPreset
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    var w = new EditPresetWindow();
                    var vm = new EditPresetViewModel();
                    w.DataContext = vm;
                    w.ShowDialog();
                });
            }
        }
        #endregion
    }
}