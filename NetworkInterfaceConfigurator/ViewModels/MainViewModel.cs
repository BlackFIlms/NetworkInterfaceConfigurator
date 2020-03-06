using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        //Control logics for window.

        //Define command for minimize Window.
        public RelayCommand MinWin
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Window minWindowButton = obj as Window;

                    if (minWindowButton != null)
                    {
                        void _MinWin(Window w) => SystemCommands.MinimizeWindow(w); //Define a function to send arguments to the object MainWindow.
                        _MinWin(minWindowButton);
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
                    Window maxWindowButton = obj as Window;

                    if (maxWindowButton != null)
                    {
                        void _MaxWin(Window w) //Define a function to send arguments to the object MainWindow.
                        {
                            if (w.WindowState == WindowState.Maximized) SystemCommands.RestoreWindow(w);
                            else SystemCommands.MaximizeWindow(w);
                        }
                        _MaxWin(maxWindowButton);
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
                    Window closeWindowButton = obj as Window;

                    if (closeWindowButton != null)
                    {
                        closeWindowButton.Close();
                    }
                });
            }
        }

        public RelayCommand DragWindow
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Window dragButton = obj as Window;

                    if (dragButton != null)
                    {
                        dragButton.DragMove();
                    }
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}