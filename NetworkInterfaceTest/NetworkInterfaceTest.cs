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
        private const string baseReg = @"SYSTEM\CurrentControlSet\Control\Class\{4D36E972-E325-11CE-BFC1-08002bE10318}\";
        public static string output;

        static void Main(string[] args)
        {
            string bkey = GetBaseKey().ToString();
            string check64 = InternalCheckIsWow64().ToString();

            string adapters = "";
            foreach (string adapter in GetNicIds())
            {
                adapters += adapter + "\r\n";
            }

            output = bkey + " $$$ " + check64;
            Console.WriteLine(output);
            Console.WriteLine("Do you want see adapter list? y/n");
            Console.Beep();
            char conKey1 = Console.ReadKey().KeyChar;
            Console.WriteLine("\r\n");
            switch (conKey1)
            {
                case 'y':
                    Console.WriteLine(adapters);
                    Console.ReadKey();
                    break;
                case 'n':
                    break;
                default:
                    break;
            }
        }

        //Set new MAC addres.
        public static bool SetMAC(string nicID, string newMAC)
        {
            bool ret = false;
            using (RegistryKey bkey = GetBaseKey())
            using (RegistryKey key = bkey.OpenSubKey(baseReg + nicID))
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
        //Get adapter name from registry.
        public static IEnumerable<string> GetNicName(string nicID)
        {
            using (RegistryKey bkey = GetBaseKey())
            using (RegistryKey key = bkey.OpenSubKey(baseReg + nicID))
                return;
        }
        //Get physical adapter IDs from registry.
        public static IEnumerable<string> GetNicIds()
        {
            using (RegistryKey bkey = GetBaseKey())
            using (RegistryKey key = bkey.OpenSubKey(baseReg))
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

        //This functions check system version (64/32bit) and then get corresponding branch of registry.
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
