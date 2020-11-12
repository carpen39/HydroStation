using HydroStation.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HydroStation.Hydro
{
    public class DeviceManager : IDeviceManager
    {
        private List<ISerialDevice> DeviceReaders = new List<ISerialDevice>();
        public DeviceManager()
        {
            StartReader();
        }

        private void StartReader()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var task = Repeat.Interval(TimeSpan.FromSeconds(15),
                    () =>
                    {
                        for (int i = 0; i < 9; i++) 
                        {
                            try
                            {
                                DFRobotSerialDevice reader = new DFRobotSerialDevice($"COM{i}");
                                reader.GetCurrentSerialDeviceData();

                                DeviceReaders.Add(reader);
                            } catch (Exception ex)
                            {

                            }
                        }
                    }, cancellationTokenSource.Token);
        }

        public List<ISerialDevice> GetDeviceReaders()
        {
            return DeviceReaders;
        }
    }
}
