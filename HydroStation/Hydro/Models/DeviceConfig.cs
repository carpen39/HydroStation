using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HydroStation.Hydro.Models
{
    public class DeviceConfig
    {
        [Key]
        public string COMPort { get; set; }
        public DosingConfig DosingConfig { get; set; }
        public string FriendlyName { get; set; }
    }
}
