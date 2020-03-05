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

        private RelayCommand closeWin;
        public RelayCommand CloseWin
        {
            get
            {
                return closeWin ??
                  (closeWin = new RelayCommand(obj =>
                  {
                      MainWindow closeWindowButton = obj as MainWindow;

                      if (closeWindowButton != null)
                      {
                          Debug = closeWindowButton.Name + closeWindowButton.Width.ToString();
                          closeWindowButton.Close();
                      }
                  }));
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