using System;
using System.Management;

namespace ITToolKit_3
{
    class GetRegistryKeys
    {
        /// <summary>
        /// 古い方法
        /// </summary>

        public string GetHardwareModelName()
        {
            ManagementScope scope = new ManagementScope("root\\cimv2");
            scope.Connect();

            ObjectQuery q = new ObjectQuery("select Manufacturer,Model from Win32_ComputerSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, q);
            ManagementObjectCollection collection = searcher.Get();

            foreach (ManagementObject o in collection)
                return Convert.ToString(o.GetPropertyValue("Model"));
            return null;
        }

        public string GetHardwareVendorName()
        {
            ManagementScope scope = new ManagementScope("root\\cimv2");
            scope.Connect();

            ObjectQuery q = new ObjectQuery("select Manufacturer,Model from Win32_ComputerSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, q);
            ManagementObjectCollection collection = searcher.Get();

            foreach (ManagementObject o in collection)
                return Convert.ToString(o.GetPropertyValue("Manufacturer"));
            return null;
        }

        public string GetOSFullName()
        {
            string result = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select Caption from Win32_OperatingSystem");
            foreach (ManagementObject os in searcher.Get())
            {
                result = os["Caption"].ToString();
                break;
            }
            return result;
        }
    }
}
