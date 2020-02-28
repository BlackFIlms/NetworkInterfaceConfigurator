using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using Microsoft.Win32;

namespace NetworkInterfaceTest
{
    class NetworkInterfaceTest
    {
        public static string output;

        static void Main(string[] args)
        {
            string bkey = GetBaseKey().ToString();
            string check64 = InternalCheckIsWow64().ToString();

            output = bkey + " $$$ " + check64;
            Console.WriteLine(output);
            Console.Beep();
            Console.ReadKey();
        }

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
