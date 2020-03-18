using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NetworkInterfaceConfigurator.Models;
using NetworkInterfaceConfigurator.Views;

namespace NetworkInterfaceConfigurator.ViewModels
{
    class MainViewModel : ProperyChanged
    {
        //Constructor.
        public MainViewModel()
        {
            GetAdapters();
        }

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
                OnPropertyChanged("SelectedAdapter");
                InitAdapterProperties(value); //After selection adapter, loading adapter properties.
            }
        }

        #region Control logics for window.

        //Define command for minimize Window.
        public RelayCommand MinWin
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Window minWindowCommand = obj as Window;

                    if (minWindowCommand != null)
                    {
                        void _MinWin(Window w) => SystemCommands.MinimizeWindow(w); //Define a function to send arguments to the object MainWindow.
                        _MinWin(minWindowCommand);
                    }
                });
            }
        }

        //Define command for restore||maximize Window.
        public RelayCommand MaxWin
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Window maxWindowCommand = obj as Window;

                    if (maxWindowCommand != null)
                    {
                        void _MaxWin(Window w) //Define a function to send arguments to the object MainWindow.
                        {
                            if (w.WindowState == WindowState.Maximized) SystemCommands.RestoreWindow(w);
                            else SystemCommands.MaximizeWindow(w);
                        }
                        _MaxWin(maxWindowCommand);
                    }
                });
            }
        }

        //Define command for close Window.
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

        //Define command for drag Window.
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
            //Swap culture.My default culture - "ru-RU".Need culture, for ConvertToDouble en-US.

            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            provider.NumberGroupSeparator = ",";
            provider.NumberGroupSizes = new int[] { 3 };

            double res = ((Convert.ToDouble(GetWindowWidth, provider) / 2) - Convert.ToDouble(GetIconHeaderWidth, provider) - Convert.ToDouble(GetMenuHeaderWidth, provider)) - (Convert.ToDouble(GetTitleHeaderWidth, provider) / 2);
            res = Math.Round(res); //Rounds the result so that the title borders occupy full pixels.
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
                    yield return obj;
                }
            }
        }
        //Adapter properties loading only for selected adapter 
        public void InitAdapterProperties(NetworkInterfaceLib obj)
        {
            obj.IPEnabled = NetworkInterfaceLib.IsIpEnabled(obj.NicIndex);
            obj.IP = NetworkInterfaceLib.GetIP(obj.NicIndex);
            obj.Subnet = NetworkInterfaceLib.GetSubnet(obj.NicIndex);
            obj.Gateway = NetworkInterfaceLib.GetGateway(obj.NicIndex);
            obj.DNS = NetworkInterfaceLib.GetDNS(obj.NicIndex);
            obj.MAC = NetworkInterfaceLib.GetMAC(obj.NicIndex);
        }
        #endregion

        #region GetAdapters
        public void GetAdapters()
        {
            Adapters = new ObservableCollection<NetworkInterfaceLib>(Init());
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