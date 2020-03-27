using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkInterfaceConfigurator.Models
{
    class Preset : ProperyChanged
    {
        // Constructor.
        public Preset()
        {

        }

        #region Variables, Constants & Properties.
        //Variables.
        private string id;
        private string name;
        private string ip;
        private string subnet;
        private string gateway;
        private string dns1;
        private string dns2;
        private string mac;
        private string macr;

        //Properties.
        /// <summary>
        /// Get or Set preset id.
        /// </summary>
        public string ID
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("ID");
            }
        }
        /// <summary>
        /// Get or Set preset name.
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        /// <summary>
        /// Get or Set preset ip.
        /// </summary>
        public string IP
        {
            get { return ip; }
            set
            {
                ip = value;
                OnPropertyChanged("IP");
            }
        }
        /// <summary>
        /// Get or Set preset subnet.
        /// </summary>
        public string Subnet
        {
            get { return subnet; }
            set
            {
                subnet = value;
                OnPropertyChanged("Subnet");
            }
        }
        /// <summary>
        /// Get or Set preset gateway.
        /// </summary>
        public string Gateway
        {
            get { return gateway; }
            set
            {
                gateway = value;
                OnPropertyChanged("Gateway");
            }
        }
        /// <summary>
        /// Get or Set preset dns1.
        /// </summary>
        public string DNS1
        {
            get { return dns1; }
            set
            {
                dns1 = value;
                OnPropertyChanged("DNS1");
            }
        }
        /// <summary>
        /// Get or Set preset dns2.
        /// </summary>
        public string DNS2
        {
            get { return dns2; }
            set
            {
                dns2 = value;
                OnPropertyChanged("DNS2");
            }
        }
        /// <summary>
        /// Get or Set preset mac.
        /// </summary>
        public string MAC
        {
            get { return mac; }
            set
            {
                mac = value;
                OnPropertyChanged("MAC");
            }
        }
        /// <summary>
        /// Get or Set preset macr.
        /// </summary>
        public string MACR
        {
            get { return macr; }
            set
            {
                macr = value;
                OnPropertyChanged("MACR");
            }
        }
        #endregion

        #region GetMethods
        #endregion

        #region SetMethods
        #endregion
    }
}