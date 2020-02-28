using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using Microsoft.Win32;
using System.Management;

namespace NetworkInterfaceTest
{
    class NetworkInterfaceTest
    {
        //Defining variables
        public static List<Adapter> adapters = new List<Adapter>();

        static void Main(string[] args)
        {
            //It's validation check.
            string bkey = Adapter.GetBaseKey().ToString();
            string check64 = Adapter.InternalCheckIsWow64().ToString();

            //Get adapters list with ID's, NIC names and Network names.
            /*string adapters = "";
            foreach (string adapter in Adapter.GetNicIds())
            {
                adapters += adapter + "\r\n";
            }*/
            
            foreach (string adapter in Adapter.GetNicIds())
            {
                string regKey;
                adapters.Add(new Adapter() { NicID = adapter, NicName = Adapter.GetNicName(adapter, out regKey), NetName = Adapter.GetNetName(regKey) });
            }

            //Console out.
            string output = bkey + " $$$ " + check64;
            Console.WriteLine(output);
            Console.WriteLine("Do you want see adapter list? y/n");
            Console.Beep();
            char conKey1 = Console.ReadKey().KeyChar;
            Console.WriteLine("\r\n");
            switch (conKey1)
            {
                case 'y':
                    foreach (Adapter item in adapters)
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
    class Adapter
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
