using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using NetworkInterfaceConfigurator.Models;
using NetworkInterfaceConfigurator.Views;

namespace NetworkInterfaceConfigurator.ViewModels
{
    class AboutViewModel : PropChanged
    {
        // Constructor.
        public AboutViewModel()
        {
            ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
            try
            {
                WebImage = (ImageSource)imageSourceConverter.ConvertFrom("https://avatars0.githubusercontent.com/u/23724134?s=460&u=3c6e8297f90ceea08c74e0f9db93d93c37448d8e&v=4");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error");
            }
        }

        // Variables, Constants & Properties.
        private ImageSource webImage;
        public ImageSource WebImage
        {
            get { return webImage; }
            set
            {
                webImage = value;
                OnPropertyChanged("WebImage");
            }
        }

        private readonly string assemblyInfo = "v. " + typeof(NetworkInterfaceLib).Assembly.GetName().Version.ToString();
        public string AssemblyInfo
        {
            get { return assemblyInfo; }
        }
        private readonly string assemblyCopyright = (Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0] as AssemblyCopyrightAttribute).Copyright;
        public string AssemblyCopyright
        {
            get { return assemblyCopyright; }
        }

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

        // Buttons.
        private RelayCommand openGitHub;
        public RelayCommand OpenGitHub
        {
            get
            {
                return openGitHub ??
                  (openGitHub = new RelayCommand(obj =>
                  {
                      Process.Start("https://github.com/BlackFIlms/");
                  }));
            }
        }
    }
}
