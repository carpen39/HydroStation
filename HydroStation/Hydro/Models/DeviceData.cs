using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HydroStation.Hydro.Models
{
    public class DeviceData
    {
        [Key]
        public Guid Id { get; set; }
        public string COMPort { get; set; }
        public DateTime DateCreated { get; set; }
        public float PHValue { get; set; }
        public float WaterTemperature { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
    }
}
