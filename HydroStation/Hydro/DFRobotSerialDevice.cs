using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HydroStation.Hydro.Models;
using HydroStation.Storage;

namespace HydroStation.Hydro
{
    public class DFRobotSerialDevice : ISerialDevice
    {
        private SerialPort SerialPort;
        private List<DeviceData> Data = new List<DeviceData>();

        public DFRobotSerialDevice(string comPort)
        {
            SerialPort = new SerialPort(comPort, 115200, Parity.None, 8, StopBits.Two);
            SerialPort.Open();

            StartReader();
        }

        private void StartReader()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var updateTask = Repeat.Interval(TimeSpan.FromSeconds(1),
                    () =>
                    {
                        DeviceData data = GetCurrentSerialDeviceData();
                        this.Data.Add(data);

                        this.Data = this.Data.OrderByDescending(x => x.DateCreated).Take(10).ToList();

                    }, cancellationTokenSource.Token);

            var saveTask = Repeat.Interval(TimeSpan.FromMinutes(1),
                    () =>
                    {
                        if (this.Data.Count > 0)
                        {
                            using (HydroContext context = new HydroContext())
                            {
                                context.DeviceData.Add(this.Data.First());
                                context.SaveChanges();
                            }
                        }

                    }, cancellationTokenSource.Token);
        }

        private List<string> DoPhCalibration()
        {
            List<string> messages = new List<string>();

            SerialPort.WriteLine("enterph");

            int count = 0;
            while(count <= 5)
            {
                messages.Add(SerialPort.ReadLine());
                Thread.Sleep(1000);
                count++;
            }

            SerialPort.WriteLine("calph");

            count = 0;
            while (count <= 5)
            {
                messages.Add(SerialPort.ReadLine());
                Thread.Sleep(1000);
                count++;
            }

            SerialPort.WriteLine("exitph");

            messages.Add(SerialPort.ReadLine());

            return messages;
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            Console.WriteLine("Data Received:");
            Console.Write(indata);
        }

        public DeviceData GetCurrentSerialDeviceData()
        {
            int retryCount = 0;

            while (retryCount < 5)
            {
                try
                {
                    string serialLine = GetSerialLine();

                    return new DeviceData()
                    {
                        COMPort = SerialPort.PortName,
                        DateCreated = DateTime.Now,
                        WaterTemperature = ParseTemperature(serialLine),
                        PHValue = ParsePH(serialLine)
                    };
                }
                catch (Exception ex)
                {

                }

                retryCount++;
            }

            throw new Exception("Couldn't read device data.");
        }

        private float ParsePH(string serialLine)
        {
            return float.Parse(serialLine.Split("pH:")[1]);
        }

        private float ParseTemperature(string serialLine)
        {
            string temp = serialLine.Replace("temperature:", "");
            temp = temp.Split("^C")[0];

            return float.Parse(temp);
        }

        public string GetSerialLine()
        {
            return SerialPort.ReadLine();
        }

        public List<DeviceData> GetLoggedSerialDeviceData()
        {
            return this.Data;
        }

        public string GetDeviceId()
        {
            return this.SerialPort.PortName;
        }
    }
}
