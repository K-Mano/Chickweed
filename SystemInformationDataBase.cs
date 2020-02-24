using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITToolKit_3
{
    public class SystemInformationDataBase
    {
        public string AdapterName { get; set; }
        public string VendorName { get; set; }
        public string[] NAPhysicalNumber { get; set; }
        public string SystemVersionId { get; set; }
        public string SystemMajorVersion { get; set; }
        public string SystemMinorVersion { get; set; }
        public string OSFullName { get; set; }
    }
}
