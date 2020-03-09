using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NetworkInterfaceConfigurator.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
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
            }
        }
        private RelayCommand stableGridWidth;
        public RelayCommand StableGridWidth
        {
            get
            {
                return stableGridWidth ??
                    (stableGridWidth = new RelayCommand(obj =>
                    {
                        Grid grid = obj as Grid;
                        GridWidth = grid.RenderSize.Width.ToString(); //All methods for get Grid width, return old data. And it's affected on CalcMarginTitle method. Need to solution!!!
                    }));
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
            }
        }
        private RelayCommand calcMarginTitle;
        public RelayCommand CalcMarginTitle
        {
            get
            {
                return calcMarginTitle ??
                    (calcMarginTitle = new RelayCommand(obj =>
                    {
                        //Swap culture. My default culture - "ru-RU". Need culture, for ConvertToDouble en-US.
                        NumberFormatInfo provider = new NumberFormatInfo();
                        provider.NumberDecimalSeparator = ".";
                        provider.NumberGroupSeparator = ",";
                        provider.NumberGroupSizes = new int[] { 3 };

                        double res = ((Convert.ToDouble(GetWindowWidth, provider) / 2) - Convert.ToDouble(GetIconHeaderWidth, provider) - Convert.ToDouble(GetMenuHeaderWidth, provider)) - (Convert.ToDouble(GetTitleHeaderWidth, provider) / 2);
                        CenterTitle = res.ToString().Replace(',', '.') + ", 0, 0, 0";
                        Debug = res.ToString().Replace(',', '.') + ", 0, 0, 0" + " " + (Convert.ToDouble(GetWindowWidth, provider) / 2) + " " + Convert.ToDouble(GetIconHeaderWidth, provider) + " " + Convert.ToDouble(GetMenuHeaderWidth, provider) + " " + GridWidth + " " + (Convert.ToDouble(GetTitleHeaderWidth, provider) / 2);
                    }));
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}