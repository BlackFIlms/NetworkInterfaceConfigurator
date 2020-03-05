using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DevExpress.Mvvm;

namespace NetworkInterfaceConfigurator.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        private int clicks;
        
        public int Clicks
        {
            get { return clicks; }

            set
            {
                clicks = value;
                RaisePropertyChanged(() => Clicks);
            }
        }


        public ICommand ClickAdd
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Clicks++;
                });
            }
        }
    }
}
