using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ITToolKit_3
{
    public class NetworkAdapter
    {
        public NetworkInterface SearchAdapterTypeFromString(NetworkInterfaceType networktype, string contain)
        {
            // Wi-Fiボードの取得
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.NetworkInterfaceType==networktype && adapter.Name.Contains(contain))
                {
                    return adapter;
                }
            }
            return null;
        }
        public string GetAdapterName(NetworkInterface board)
        {
            return board.Name;
        }
        public string GetAdapterVendor(NetworkInterface board)
        {
            return board.Description;
        }
        public string GetMacAddressFromAdapter(NetworkInterface adapter)
        {
            // MACアドレスの取得
            string phy = adapter.GetPhysicalAddress().ToString();
            for (int i = 2; i <= 14; i += 3)
                phy = phy.Insert(i, ":");
            return phy;
        }
    }
}
