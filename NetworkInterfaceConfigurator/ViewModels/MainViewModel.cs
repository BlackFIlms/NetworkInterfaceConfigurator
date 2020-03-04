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
        private int _Clicks;
        
        public int Clicks
        {
            get { return _Clicks; }

            set
            {
                _Clicks = value;
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
