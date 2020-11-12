using HydroStation.Hydro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HydroStation.Hydro
{
    public interface ISerialDevice
    {
        string GetDeviceId();
        string GetSerialLine();
        DeviceData GetCurrentSerialDeviceData();
        List<DeviceData> GetLoggedSerialDeviceData();
        List<string> DoPhCalibration();
    }
}
