using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HydroStation.Hydro
{
    public interface IDeviceManager
    {
        List<ISerialDevice> GetDeviceReaders();
    }
}
