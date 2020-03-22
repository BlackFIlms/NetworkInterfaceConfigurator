using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using Microsoft.Win32;
using System.Management;
using System.ComponentModel;
using System.Collections;

namespace NetworkInterfaceConfigurator.Models
{
    /// <summary>
    /// Operations with adapters via WMI & NetworkInformation.
    /// </summary>
    class NetworkInterfaceLib : ProperyChanged, INotifyDataErrorInfo
    {
        /*//Defining variables
        public static List<AdapterWMI> adaptersWMI = new List<AdapterWMI>();
        public static List<AdapterReg> adaptersReg = new List<AdapterReg>();

        static void Main(string[] args)
        {
            //It's validation check.
            string bkey = AdapterReg.GetBaseKey().ToString();
            string check64 = AdapterReg.InternalCheckIsWow64().ToString();
            
            //Console out.
            string output = bkey + " $$$ " + check64;
            Console.WriteLine(output);
            
            //Debug for AdapterWMI.
            Console.WriteLine("Do you want see adapter list via WMI? y/n");
            Console.Beep();
            char conKey1 = Console.ReadKey().KeyChar;
            Console.WriteLine("\r\n");
            switch (conKey1)
            {
                case 'y':
                    foreach (string adapterIndex in AdapterWMI.GetNicIndexes())
                    {
                        if (AdapterWMI.GetMAC(adapterIndex) != "Empty")
                        {
                            adaptersWMI.Add(new AdapterWMI { NicIndex = adapterIndex, NicID = AdapterWMI.GetNicID(adapterIndex), NicName = AdapterWMI.GetNicName(adapterIndex),
                            NetName = AdapterWMI.GetNetName(adapterIndex), IPEnabled = AdapterWMI.IsIpEnabled(adapterIndex), IP = AdapterWMI.GetIP(adapterIndex),
                            Subnet = AdapterWMI.GetSubnet(adapterIndex), Gateway = AdapterWMI.GetGateway(adapterIndex), DNS = AdapterWMI.GetDNS(adapterIndex),
                            MAC = AdapterWMI.GetMAC(adapterIndex) });
                        }
                    }

                    foreach (AdapterWMI item in adaptersWMI)
                    {
                        Console.WriteLine("Network adapter RegKey:              " + item.NicIndex);
                        Console.WriteLine("Network adapter index from Registry: " + item.NicID);
                        Console.WriteLine("Network adapter name:                " + item.NicName);
                        Console.WriteLine("Network name:                        " + item.NetName);
                        Console.WriteLine("Static IP:                           " + item.IPEnabled);
                        Console.WriteLine("IP:                                  " + item.IP);
                        Console.WriteLine("Subnet:                              " + item.Subnet);
                        Console.WriteLine("Gateway:                             " + item.Gateway);
                        foreach (string dns in item.DNS)
                        {
                            Console.WriteLine("DNS:                                 " + dns);
                        }
                        Console.WriteLine("MAC:                                 " + item.MAC);
                        Console.WriteLine("\r\n");
                    }
                    Console.ReadKey();
                    break;
                case 'n':
                    break;
                default:
                    break;
            }

            //Debug for AdapterReg.
            Console.WriteLine("Do you want see adapter list in Registry? y/n");
            Console.Beep();
            conKey1 = Console.ReadKey().KeyChar;
            Console.WriteLine("\r\n");
            switch (conKey1)
            {
                case 'y':
                    foreach (string adapter in AdapterReg.GetNicIds())
                    {
                        string regKey;
                        adaptersReg.Add(new AdapterReg() { NicID = adapter, NicName = AdapterReg.GetNicName(adapter, out regKey), NetName = AdapterReg.GetNetName(regKey) });
                    }
                    foreach (AdapterReg item in adaptersReg)
                    {
                        Console.WriteLine("Network adapter ID in Registry: " + item.NicID + "\r\n");
                        Console.WriteLine("Network adapter name: " + item.NicName + "\r\n");
                        Console.WriteLine("Network name: " + item.NetName + "\r\n");
                        Console.WriteLine("\r\n");
                    }
                    Console.ReadKey();
                    break;
                case 'n':
                    break;
                default:
                    break;
            }
        }*/

        #region Validation
        private readonly Dictionary<string, List<string>> _errorsByPropertyName = new Dictionary<string, List<string>>();

        public bool HasErrors => _errorsByPropertyName.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            return _errorsByPropertyName.ContainsKey(propertyName) ?
                _errorsByPropertyName[propertyName] : null;
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private bool IsValidIP(string data)
        {
            ClearErrors(nameof(IP));
            bool valid = false;
            if (string.IsNullOrWhiteSpace(data))
            {
                AddError(nameof(IP), "Username cannot be empty.");
                return valid;
            }
            if (string.Equals(data, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                AddError(nameof(IP), "Admin is not valid username.");
                return valid;
            }
            if (data == null || data?.Length <= 5)
            {
                AddError(nameof(IP), "Username must be at least 6 characters long.");
                return valid;
            }
            valid = true;
            return valid;
        }

        private void AddError(string propertyName, string error)
        {
            if (!_errorsByPropertyName.ContainsKey(propertyName))
                _errorsByPropertyName[propertyName] = new List<string>();

            if (!_errorsByPropertyName[propertyName].Contains(error))
            {
                _errorsByPropertyName[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        private void ClearErrors(string propertyName)
        {
            if (_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }
        #endregion

        #region Variables & Properties
        //Variables.
        private string nicIndex;
        private string nicID;
        private string nicName;
        private string netName;
        private bool ipEnabled;
        private string ip;
        private string subnet;
        private string gateway;
        private string dns1;
        private string dns2;
        private string mac;

        //Properties.
        /// <summary>
        /// Get or Set Network adapter reg key.
        /// </summary>
        public string NicIndex
        {
            get { return nicIndex; }

            set
            {
                nicIndex = value;
                OnPropertyChanged("NicIndex");
            }
        }
        /// <summary>
        /// Get or Set Network adapter id.
        /// </summary>
        public string NicID
        {
            get { return nicID; }

            set
            {
                nicID = value;
                OnPropertyChanged("NicID");
            }
        }
        /// <summary>
        /// Get or Set Network adapter Name.
        /// </summary>
        public string NicName
        {
            get { return nicName; }

            set
            {
                nicName = value;
                OnPropertyChanged("NicName");
            }
        }
        /// <summary>
        /// Get or Set Network name.
        /// </summary>
        public string NetName
        {
            get { return netName; }

            set
            {
                netName = value;
                OnPropertyChanged("NetName");
            }
        }
        /// <summary>
        /// Get or Set IPEnabled.
        /// </summary>
        public bool IPEnabled
        {
            get { return ipEnabled; }

            set
            {
                ipEnabled = value;
                OnPropertyChanged("IPEnabled");
            }
        }
        /// <summary>
        /// Get or Set IP.
        /// </summary>
        public string IP
        {
            get { return ip; }

            set
            {
                if (IsValidIP(value))
                {
                    ip = value;
                    OnPropertyChanged("IP");
                }
            }
        }
        /// <summary>
        /// Get or Set Subnet.
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
        /// Get or Set Gateway.
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
        /// Get or Set DNS 1.
        /// </summary>
        public string DNS1
        {
            get { return dns1; }

            set
            {
                dns1 = value;
                OnPropertyChanged("DNS");
            }
        }
        /// <summary>
        /// Get or Set DNS 2.
        /// </summary>
        public string DNS2
        {
            get { return dns2; }

            set
            {
                dns2 = value;
                OnPropertyChanged("DNS");
            }
        }
        /// <summary>
        /// Get or Set MAC.
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
        #endregion
        
        #region GetMethods

        /// <summary>
        /// Get adapter RegKeys.
        /// </summary>
        public static IEnumerable<string> GetNicIndexes()
        {
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            foreach (ManagementObject objMO in objMOC)
            {
                string index = objMO["SettingID"].ToString();
                yield return index;
            }
        }
        /// <summary>
        /// Get adapter ID.
        /// </summary>
        public static string GetNicID(string index)
        {
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            string ID = null;
            foreach (ManagementObject objMO in objMOC)
            {
                if (objMO["SettingID"].ToString() == index)
                {
                    ID = objMO["Index"].ToString();
                    return ID;
                }
            }
            return ID;
        }
        /// <summary>
        /// Get adapter name.
        /// </summary>
        public static string GetNicName(string index)
        {
            NetworkInterface[] netName = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface item in netName)
            {
                if (item.Id == index)
                {
                    return item.Description;
                }
            }
            return null;
        }
        /// <summary>
        /// Get network name.
        /// </summary>
        public static string GetNetName(string index)
        {
            NetworkInterface[] netName = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface item in netName)
            {
                if (item.Id == index)
                {
                    return item.Name;
                }
            }
            return null;
        }
        /// <summary>
        /// Get state of IP configuration.
        /// </summary>
        public static bool IsIpEnabled(string index)
        {
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            bool state = false;
            foreach (ManagementObject objMO in objMOC)
            {
                if (objMO["SettingID"].ToString() == index)
                {
                    state = (bool)objMO["IPEnabled"];
                    return state;
                }
            }
            return state;
        }
        /// <summary>
        /// Get IP.
        /// </summary>
        public static string GetIP(string index)
        {
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            string IP = null;
            foreach (ManagementObject objMO in objMOC)
            {
                if (objMO["SettingID"].ToString() == index)
                {
                    string[] ipaddresses = (string[])objMO["IPAddress"];
                    if (ipaddresses != null && ipaddresses.Count() > 0)
                    {
                        IP = ipaddresses[0];
                    }
                    else
                    {
                        IP = "Empty";
                    }
                }
            }
            return IP;
        }
        /// <summary>
        /// Get Subnet.
        /// </summary>
        public static string GetSubnet(string index)
        {
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            string subnet = null;
            foreach (ManagementObject objMO in objMOC)
            {
                if (objMO["SettingID"].ToString() == index)
                {
                    string[] subnetA = (string[])objMO["IPSubnet"];
                    if (subnetA != null && subnetA.Count() > 0)
                    {
                        subnet = subnetA[0];
                    }
                    else
                    {
                        subnet = "Empty";
                    }
                }
            }
            return subnet;
        }
        /// <summary>
        /// Get Gateway.
        /// </summary>
        public static string GetGateway(string index)
        {
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            string gateway = null;
            foreach (ManagementObject objMO in objMOC)
            {
                if (objMO["SettingID"].ToString() == index)
                {
                    string[] gateWay = (string[])objMO["DefaultIPGateway"];
                    if (gateWay != null && gateWay.Count() > 0)
                    {
                        foreach (string item in gateWay)
                        {
                            gateway = item;
                        }
                    }
                    else
                    {
                        gateway = "Empty";
                    }
                }
            }
            return gateway;
        }
        /// <summary>
        /// Get DNS.
        /// </summary>
        public static string[] GetDNS(string index)
        {
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            string[] dns = null;
            foreach (ManagementObject objMO in objMOC)
            {
                if (objMO["SettingID"].ToString() == index)
                {
                    string[] dnsA = (string[])objMO["DNSServerSearchOrder"];
                    if (dnsA != null && dnsA.Count() > 0)
                    {
                        dns = new string[dnsA.Count()];
                        dns = dnsA;
                    }
                    else
                    {
                        dns = new string[1];
                        dns[0] = "Empty";
                    }
                }
            }
            return dns;
        }
        /// <summary>
        /// Get MAC.
        /// </summary>
        public static string GetMAC(string index)
        {
            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            string MAC = null;
            foreach (ManagementObject objMO in objMOC)
            {
                if (objMO["SettingID"].ToString() == index)
                {
                    if (objMO["MACAddress"] != null)
                    {
                        MAC = objMO["MACAddress"].ToString();
                        return MAC;
                    }
                    else
                    {
                        MAC = "Empty";
                    }
                }
            }
            return MAC;
        }
        #endregion
    }
}
