using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using Microsoft.Win32;
using System.Management;

namespace NetworkInterfaceLib
{
    class NetworkInterfaceLib
    {
        //Defining variables
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
        }
    }
    /// <summary>
    /// Operations with adapters via WMI & NetworkInformation.
    /// </summary>
    class AdapterWMI
    {
        //Defining variables for objects.
        /// <summary>
        /// Get or Set Network adapter reg key.
        /// </summary>
        public string NicIndex { get; set; }
        /// <summary>
        /// Get or Set Network adapter id.
        /// </summary>
        public string NicID { get; set; }
        /// <summary>
        /// Get or Set Network adapter Name.
        /// </summary>
        public string NicName { get; set; }
        /// <summary>
        /// Get or Set Network name.
        /// </summary>
        public string NetName { get; set; }
        /// <summary>
        /// Get or Set IPEnabled.
        /// </summary>
        public bool IPEnabled { get; set; }
        /// <summary>
        /// Get or Set IP.
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// Get or Set Subnet.
        /// </summary>
        public string Subnet { get; set; }
        /// <summary>
        /// Get or Set Gateway.
        /// </summary>
        public string Gateway { get; set; }
        /// <summary>
        /// Get or Set DNS.
        /// </summary>
        public string[] DNS { get; set; }
        /// <summary>
        /// Get or Set MAC.
        /// </summary>
        public string MAC { get; set; }

        //Methods.

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
            System.Net.NetworkInformation.NetworkInterface[] netName = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            foreach (System.Net.NetworkInformation.NetworkInterface item in netName)
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
            System.Net.NetworkInformation.NetworkInterface[] netName = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            foreach (System.Net.NetworkInformation.NetworkInterface item in netName)
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

    }
    /// <summary>
    /// Operations with adapters in registry.
    /// </summary>
    class AdapterReg
    {
        //Defining variables for objects.
        /// <summary>
        /// Get or Set Network adapter id.
        /// </summary>
        public string NicID { get; set; }
        /// <summary>
        /// Get or Set Network adapter Name.
        /// </summary>
        public string NicName { get; set; }
        /// <summary>
        /// Get or Set Network name.
        /// </summary>
        public string NetName { get; set; }

        //Set constants for Rgistry.
        private const string netAdaptersReg = @"SYSTEM\CurrentControlSet\Control\Class\{4D36E972-E325-11CE-BFC1-08002bE10318}\";
        private const string netConnNamesReg = @"SYSTEM\ControlSet001\Control\Network\{4D36E972-E325-11CE-BFC1-08002BE10318}\";
        
        //Methods.

        /// </summary>
        /// Set new MAC addres.
        /// </summary>
        public static bool SetMAC(string nicID, string newMAC)
        {
            bool ret = false;
            using (RegistryKey bkey = GetBaseKey())
            using (RegistryKey key = bkey.OpenSubKey(netAdaptersReg + nicID))
            {
                if (key != null)
                {
                    key.SetValue("NetworkAddress", newMAC, RegistryValueKind.String);

                    ManagementObjectSearcher adpters = new ManagementObjectSearcher(
                        new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE Index = " + nicID));

                    foreach (ManagementObject obj in adpters.Get().OfType<ManagementObject>())
                    {
                        obj.InvokeMethod("Disable", null);
                        obj.InvokeMethod("Enable", null);
                        ret = true;
                    }
                }
            }

            return ret;
        }
        /// <summary>
        /// Get physical adapter IDs from registry.
        /// </summary>
        public static IEnumerable<string> GetNicIds()
        {
            using (RegistryKey bkey = GetBaseKey())
            using (RegistryKey key = bkey.OpenSubKey(netAdaptersReg))
            {
                if (key != null)
                {
                    foreach (string name in key.GetSubKeyNames().Where(n => n != "Properties")) //Get all IDs.
                    {
                        using (RegistryKey sub = key.OpenSubKey(name))
                        {
                            if (sub != null)
                            {
                                object busType = sub.GetValue("BusType"); //Filtering not physycal adapters.
                                string busStr = busType != null ? busType.ToString() : string.Empty;
                                if (busStr != string.Empty)
                                {
                                    yield return name;
                                }
                            }
                        }
                    }
                }
                else
                {
                    yield return "Something with registry."; //if we can't find registry branch, we send this string for Error.
                }
            }
        }
        /// <summary>
        /// Get adapter name from registry and it can output registry branch for it's adapter.
        /// </summary>
        public static string GetNicName(string nicID, out string regKey)
        {
            using (RegistryKey bkey = GetBaseKey())
            using (RegistryKey key = bkey.OpenSubKey(netAdaptersReg + nicID))
                if (key != null)
                {
                    regKey = key.GetValue("NetCfgInstanceId").ToString();
                    string name = key.GetValue("DriverDesc").ToString();
                    return name;
                }
                else
                {
                    regKey = null;
                    return "Something with registry."; //if we can't find registry branch, we send this string for Error.
                }
        }
        ///  <summary>
        ///  Get network name from registry.
        ///  </summary>
        public static string GetNetName(string regKey)
        {
            using (RegistryKey bkey = GetBaseKey())
            using (RegistryKey key = bkey.OpenSubKey(netConnNamesReg + regKey + @"\Connection"))
                if (key != null)
                {
                    string name = key.GetValue("Name").ToString();
                    return name;
                }
                else
                {
                    regKey = null;
                    return "Something with registry."; //if we can't find registry branch, we send this string for Error.
                }
        }

        /// <summary>
        /// This functions check system version (64/32bit) and then get corresponding branch of registry.
        /// </summary>
        public static RegistryKey GetBaseKey()
        {
            return RegistryKey.OpenBaseKey(
                RegistryHive.LocalMachine,
                InternalCheckIsWow64() ? RegistryView.Registry64 : RegistryView.Registry32);
        }
        public static bool InternalCheckIsWow64()
        {
            bool b = Environment.Is64BitOperatingSystem;
            return b;
        }
    }
}
