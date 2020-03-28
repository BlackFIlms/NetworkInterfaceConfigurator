using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkInterfaceConfigurator.Models
{
    static class RandMAC
    {
        public static string GetRandMAC()
        {
            string MAC = "";
            Random rndGen = new Random();
            string block = "";
            
            for (int i = 0; i < 6; i++)
            {
                block = Convert.ToString(rndGen.Next(250), 16);

                if (Convert.ToInt32(block, 16) < 16)
                {
                    block = "0" + block;
                }

                MAC += block + ":";
            }

            MAC = MAC.ToUpper().Remove(17); // Remove last ":".

            return MAC;
        }
    }
}
