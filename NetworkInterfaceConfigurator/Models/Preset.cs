using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkInterfaceConfigurator.Models
{
    class Preset : NotifyDataErrorInfoAndPropertyChanged
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
                if (IsValidIP(value, "IP"))
                {
                    ip = value;
                    OnPropertyChanged("IP");
                }
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
                if (IsValidIP(value, "Subnet"))
                {
                    subnet = value;
                    OnPropertyChanged("Subnet");
                }
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
                if (IsValidIP(value, "Gateway"))
                {
                    gateway = value;
                    OnPropertyChanged("Gateway");
                }
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
                if (IsValidDNS(value, "DNS1"))
                {
                    dns1 = value;
                    OnPropertyChanged("DNS1");
                }
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
                if (IsValidDNS(value, "DNS2"))
                {
                    dns2 = value;
                    OnPropertyChanged("DNS2");
                }
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
                if (IsValidMAC(value, "MAC"))
                {
                    mac = value;
                    OnPropertyChanged("MAC");
                }
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

        #region Validation
        /// <summary>
        /// Check ip based values on matching to some conditions.
        /// And after checks, return true if value is valid.
        /// </summary>
        /// <param name="data">Requieries the ip based value.</param>
        /// <param name="key">Requieries name of the property.</param>
        /// <returns>Bool value. True if value is valid.</returns>
        protected bool IsValidIP(string data, string key)
        {
            ClearErrors(key);
            //Variables.
            bool valid = false;
            const int ipGroups = 4;
            const int ipMaxValue = 255;

            //Validation.
            if (string.IsNullOrWhiteSpace(data))
            {
                AddError(key, key + " cannot be empty.");
                return valid;
            }
            if (!ValidateStringFormat(data, @"\A(\d+)\.(\d+)\.(\d+)\.(\d+)\z"))
            {
                AddError(key, "Wrong " + key + " format.");
                return valid;
            }
            if (!ValidateIntMaxValues(data, @"(\d+)", ipGroups, ipMaxValue))
            {
                AddError(key, "Values in " + key + " cannot be greater, than: " + ipMaxValue + ".");
                return valid;
            }

            valid = true;
            return valid;
        }
        /// <summary>
        /// Check dns values on matching to some conditions.
        /// And after checks, return true if value is valid.
        /// </summary>
        /// <param name="data">Requieries the ip based value.</param>
        /// <param name="key">Requieries name of the property.</param>
        /// <returns>Bool value. True if value is valid.</returns>
        protected bool IsValidDNS(string data, string key)
        {
            ClearErrors(key);
            //Variables.
            bool valid = false;
            const int ipGroups = 4;
            const int ipMaxValue = 255;

            //Validation.
            if (string.IsNullOrWhiteSpace(data)) // DNS values can be empty.
            {
                valid = true;
                return valid;
            }
            if (!ValidateStringFormat(data, @"\A(\d+)\.(\d+)\.(\d+)\.(\d+)\z"))
            {
                AddError(key, "Wrong " + key + " format.");
                return valid;
            }
            if (!ValidateIntMaxValues(data, @"(\d+)", ipGroups, ipMaxValue))
            {
                AddError(key, "Values in " + key + " cannot be greater, than: " + ipMaxValue + ".");
                return valid;
            }

            valid = true;
            return valid;
        }
        /// <summary>
        /// Check mac based values on matching to some conditions.
        /// And after checks, return true if value is valid.
        /// </summary>
        /// <param name="data">Requieries the mac based value.</param>
        /// <param name="key">Requieries name of the property.</param>
        /// <returns>Bool value. True if value is valid.</returns>
        protected bool IsValidMAC(string data, string key)
        {
            ClearErrors(key);
            //Variables.
            bool valid = false;
            const int macGroups = 6;
            const int macMaxValue = 255;

            //Validation.
            if (string.IsNullOrWhiteSpace(data))
            {
                AddError(key, key + " cannot be empty.");
                return valid;
            }
            if (!ValidateStringFormat(data, @"\A(((\d+[A-F]*)|([A-F]+\d*)){2}:){5}((\d+[A-F]*)|([A-F]+\d*)){2}\z")) // {2} for single char cases. {5} for 5 times matches this group.
            {
                AddError(key, "Wrong " + key + " format.");
                return valid;
            }
            if (!ValidateHEXMaxValues(data, @"((\d+[A-F]*)|([A-F]+\d*)){2}", macGroups, macMaxValue)) //{2} for A0A or 0A0 cases.
            {
                AddError(key, "Values in " + key + " cannot be greater, than: " + macMaxValue + ".");
                return valid;
            }
            if (ValidateStringFormat(data, @"00(\d+[A-F]*)|00(\d*[A-F]+)")) // For 00x, where x any value.
            {
                AddError(key, "Wrong " + key + " format.");
                return valid;
            }

            valid = true;
            return valid;
        }
        #endregion

        #region GetMethods
        #endregion

        #region SetMethods
        #endregion
    }
}