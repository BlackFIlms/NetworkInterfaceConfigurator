using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    class NetworkInterfaceLib : NotifyDataErrorInfoAndPropertyChanged
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

        #region Variables, Constants & Properties
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

        //Constatnts.
        private const string netAdaptersReg = @"SYSTEM\CurrentControlSet\Control\Class\{4D36E972-E325-11CE-BFC1-08002bE10318}\";

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
                if (IsValidIP(value, "IP"))
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
                if (IsValidIP(value, "Subnet"))
                {
                    subnet = value;
                    OnPropertyChanged("Subnet");
                }
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
                if (IsValidIP(value, "Gateway"))
                {
                    gateway = value;
                    OnPropertyChanged("Gateway");
                }
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
                if (IsValidIP(value, "DNS1"))
                {
                    dns1 = value;
                    OnPropertyChanged("DNS1");
                }
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
                if (IsValidIP(value, "DNS2"))
                {
                    dns2 = value;
                    OnPropertyChanged("DNS2");
                }
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
                if (IsValidMAC(value, "MAC"))
                {
                    mac = value;
                    OnPropertyChanged("MAC");
                }
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

                    // Change ID format.
                    if (ID.Length < 4)
                    {
                        for (int i = ID.Length; i < 4; i++)
                        {
                            ID = "0" + ID;
                        }
                    }

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
                    string[] subnetArr = (string[])objMO["IPSubnet"];
                    if (subnetArr != null && subnetArr.Count() > 0)
                    {
                        subnet = subnetArr[0];
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
                    string[] gatewayArr = (string[])objMO["DefaultIPGateway"];
                    if (gatewayArr != null && gatewayArr.Count() > 0)
                    {
                        foreach (string item in gatewayArr)
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
                    string[] dnsArr = (string[])objMO["DNSServerSearchOrder"];
                    if (dnsArr != null && dnsArr.Count() > 0)
                    {
                        dns = new string[dnsArr.Count()];
                        dns = dnsArr;
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

        #region SetMethods
        /// <summary>
        /// Set IP and Subnet mask.
        /// </summary>
        public static bool SetStatic(string index, string new_ip, string new_subnet)
        {
            bool result = false;

            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            foreach (ManagementObject objMO in objMOC)
            {
                if (objMO["SettingID"].ToString() == index)
                {
                    if ((bool)objMO["IPEnabled"])
                    {
                        ManagementBaseObject newMO = objMO.GetMethodParameters("EnableStatic");

                        // Set new values.
                        newMO["IPAddress"] = new string[] { new_ip };
                        newMO["SubnetMask"] = new string[] { new_subnet };
                        objMO.InvokeMethod("EnableStatic", newMO, null);

                        result = true;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Set Gateway.
        /// </summary>
        public static bool SetGateway(string index, string new_gateway)
        {
            bool result = false;

            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            foreach (ManagementObject objMO in objMOC)
            {
                if (objMO["SettingID"].ToString() == index)
                {
                    if ((bool)objMO["IPEnabled"])
                    {
                        ManagementBaseObject newMO = objMO.GetMethodParameters("SetGateways");

                        // Set new value.
                        newMO["DefaultIPGateway"] = new string[] { new_gateway };
                        newMO["GatewayCostMetric"] = new int[] { 1 };
                        objMO.InvokeMethod("SetGateways", newMO, null);

                        result = true;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Set DNS.
        /// </summary>
        public static bool SetDNS(string index, string new_dns1, string new_dns2)
        {
            bool result = false;

            ManagementClass objMC = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMC.GetInstances();
            foreach (ManagementObject objMO in objMOC)
            {
                if (objMO["SettingID"].ToString() == index)
                {
                    if ((bool)objMO["IPEnabled"])
                    {
                        ManagementBaseObject newMO = objMO.GetMethodParameters("SetDNSServerSearchOrder");

                        // Set new value.
                        newMO["DNSServerSearchOrder"] = new string[] { new_dns1, new_dns2 };
                        objMO.InvokeMethod("SetDNSServerSearchOrder", newMO, null);

                        result = true;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Set MAC.
        /// </summary>
        public static bool SetMAC(string nicid, string new_mac)
        {
            bool result = false;
            using (RegistryKey bkey = GetBaseKey())
            using (RegistryKey key = bkey.OpenSubKey(netAdaptersReg + nicid, true))
            {
                if (key != null)
                {
                    string regKey = key.GetValue("NetCfgInstanceId").ToString(); // Get regKey(adapter index in registry).
                    string old_mac = GetMAC(regKey); // Get the old MAC address to check changes.
                    old_mac = old_mac.Replace(":", "");

                    new_mac = new_mac.Replace(":", "");
                    key.SetValue("NetworkAddress", new_mac, RegistryValueKind.String);
                    
                    ManagementObjectSearcher objMOS = new ManagementObjectSearcher(
                        new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE Index = " + nicid));

                    foreach (ManagementObject objMO in objMOS.Get().OfType<ManagementObject>())
                    {
                        bool restartState = RestartAdapter(nicid); // Restarts adapter & check it's state.

                        string installed_mac = GetMAC(regKey); // Get installed MAC address to check changes.
                        installed_mac = installed_mac.Replace(":", "");


                        // Check adapter state and check new mac address value.
                        if (restartState && (new_mac != installed_mac))
                        {
                            result = true;
                            throw new WarningException("MAC address was restored to default value.");
                        }
                        else if (restartState && (new_mac != old_mac))
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                            throw new Exception("Something troubles.");
                        }
                    }
                }
                key.Close();
            }

            return result;
        }
        /// <summary>
        /// This functions check system version (64/32bit) and then get corresponding branch of registry.
        /// </summary>
        private static RegistryKey GetBaseKey()
        {
            return RegistryKey.OpenBaseKey(
                RegistryHive.LocalMachine,
                InternalCheckIsWow64() ? RegistryView.Registry64 : RegistryView.Registry32);
        }
        private static bool InternalCheckIsWow64()
        {
            bool b = Environment.Is64BitOperatingSystem;
            return b;
        }
        #endregion

        #region ResetAdapter
        private static void DisableAdapter(string nicid)
        {
            ManagementObjectSearcher objMOS = new ManagementObjectSearcher(
                        new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE Index = " + nicid));

            foreach (ManagementObject objMO in objMOS.Get().OfType<ManagementObject>())
            {
                objMO.InvokeMethod("Disable", null);
            }
        }
        private static void EnableAdapter(string nicid)
        {
            ManagementObjectSearcher objMOS = new ManagementObjectSearcher(
                        new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE Index = " + nicid));

            foreach (ManagementObject objMO in objMOS.Get().OfType<ManagementObject>())
            {
                objMO.InvokeMethod("Enable", null);
            }
        }
        /// <summary>
        /// Get adapter status.
        /// </summary>
        /// <param name="nicid">Adapter ID.</param>
        /// <returns>
        /// Disconnected (0) Connecting(1) Connected(2) Disconnecting(3) Hardware Not Present(4) Hardware Disabled(5) Hardware Malfunction(6)
        /// Media Disconnected(7) Authenticating(8) Authentication Succeeded(9) Authentication Failed(10) Invalid Address(11) Credentials Required(12)
        /// </returns>
        private static string GetAdapterStatus(string nicid)
        {
            string statusInfo = "";

            ManagementObjectSearcher objMOS = new ManagementObjectSearcher(
                        new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE Index = " + nicid));

            foreach (ManagementObject objMO in objMOS.Get().OfType<ManagementObject>())
            {
                statusInfo = objMO["NetConnectionStatus"].ToString();
            }

            return statusInfo;
        }
        /// <summary>
        /// Restarts adapter, and return: True, if adapter connected to network.
        /// </summary>
        public static bool RestartAdapter(string nicid)
        {
            bool result = false;

            DisableAdapter(nicid);
            
            EnableAdapter(nicid);

            if (GetAdapterStatus(nicid) == "2")
                result = true;

            return result;
        }
        #endregion
    }
}