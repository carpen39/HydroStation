using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using HydroStation.Hydro;
using HydroStation.Hydro.Models;
using HydroStation.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HydroStation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly ILogger<DeviceController> _logger;
        private readonly IDeviceManager _deviceManager;

        public DeviceController(IDeviceManager deviceManager, ILogger<DeviceController> logger)
        {
            _logger = logger;
            _deviceManager = deviceManager;
        }

        [HttpGet]
        public List<DeviceInfo> Get()
        {
            using (HydroContext context = new HydroContext())
            {
                return _deviceManager.GetDeviceReaders().Select(x => new DeviceInfo()
                {
                    DeviceConfig = context.DeviceConfig.SingleOrDefault(),
                    DeviceData = x.GetLoggedSerialDeviceData().FirstOrDefault()
                }).ToList();
            }
        }

        [HttpGet]
        [Route("{deviceId}")]
        public DeviceInfo Get(string deviceId)
        {
            using (HydroContext context = new HydroContext())
            {
                return new DeviceInfo()
                {
                    DeviceConfig = context.DeviceConfig.SingleOrDefault(x => x.COMPort == deviceId),
                    DeviceData = _deviceManager.GetDeviceReaders().FirstOrDefault(x => x.GetDeviceId() == deviceId).GetLoggedSerialDeviceData().FirstOrDefault()
                };
            }
        }

        [HttpGet]
        [Route("{deviceId}/calibrateph")]
        public List<string> CalibratePH(string deviceId)
        {
            var reader = _deviceManager.GetDeviceReaders().FirstOrDefault(x => x.GetDeviceId() == deviceId);
            return reader.DoPhCalibration();
        }

        [HttpPost]
        [Route("{deviceId}")]
        public DeviceConfig Post(string deviceId, [FromBody] DeviceConfig deviceConfig)
        {
            using (HydroContext context = new HydroContext())
            {
                var config = context.DeviceConfig.SingleOrDefault(x => x.COMPort == deviceId);

                if (config != null)
                {
                    config.FriendlyName = deviceConfig.FriendlyName;
                    config.DosingConfig = deviceConfig.DosingConfig;

                }
                else
                {
                    config = new DeviceConfig()
                    {
                        COMPort = deviceId,
                        DosingConfig = deviceConfig.DosingConfig,
                        FriendlyName = deviceConfig.FriendlyName
                    };

                    context.DeviceConfig.Add(config);
                }

                context.SaveChanges();

                return config;
            }
        }

        [HttpGet]
        [Route("GetHistorical")]
        public List<DeviceData> GetHistorical()
        {
            using (HydroContext context = new HydroContext())
            {
                return context.DeviceData.OrderByDescending(x => x.DateCreated).Take(10).ToList();
            }
        }

        [HttpGet]
        [Route("GetHourlyHistorical")]
        public Dictionary<string, DeviceData> GetHourlyHistorical(int count = 5)
        {
            using (HydroContext context = new HydroContext())
            {
                var startHour = DateTime.Now.AddHours(-count);
                Dictionary<string, DeviceData> data = new Dictionary<string, DeviceData>();

                for (int i = 0; i < count; i++)
                {
                    DateTime key = startHour.AddHours(i);


                    data[key.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")] = context.DeviceData.Where(x => key <= x.DateCreated && x.DateCreated <= key.AddMinutes(59)).Count() > 0 ? new DeviceData()
                    {
                        DateCreated = key,
                        PHValue = context.DeviceData.Where(x => key <= x.DateCreated && x.DateCreated <= key.AddMinutes(59)).Average(x => x.PHValue),
                        WaterTemperature = context.DeviceData.Where(x => key <= x.DateCreated && x.DateCreated <= key.AddMinutes(59)).Average(x => x.WaterTemperature),
                        Temperature = context.DeviceData.Where(x => key <= x.DateCreated && x.DateCreated <= key.AddMinutes(59)).Average(x => x.Temperature),
                        Humidity = context.DeviceData.Where(x => key <= x.DateCreated && x.DateCreated <= key.AddMinutes(59)).Average(x => x.Humidity),
                    } : null;
                }


                return data;
            }
        }

        [HttpGet]
        [Route("GetDailyHistorical")]
        public Dictionary<string, DeviceData> GetDailyHistorical(int count = 5)
        {
            using (HydroContext context = new HydroContext())
            {
                var startHour = DateTime.Now.AddHours(-count);
                Dictionary<string, DeviceData> data = new Dictionary<string, DeviceData>();

                for (int i = 0; i < count; i++)
                {
                    DateTime key = startHour.AddHours(i);


                    data[key.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")] = context.DeviceData.Where(x => key <= x.DateCreated && x.DateCreated <= key.AddMinutes(59)).Count() > 0 ? new DeviceData()
                    {
                        DateCreated = key,
                        PHValue = context.DeviceData.Where(x => key <= x.DateCreated && x.DateCreated <= key.AddMinutes(59)).Average(x => x.PHValue),
                        WaterTemperature = context.DeviceData.Where(x => key <= x.DateCreated && x.DateCreated <= key.AddMinutes(59)).Average(x => x.WaterTemperature),
                        Temperature = context.DeviceData.Where(x => key <= x.DateCreated && x.DateCreated <= key.AddMinutes(59)).Average(x => x.Temperature),
                        Humidity = context.DeviceData.Where(x => key <= x.DateCreated && x.DateCreated <= key.AddMinutes(59)).Average(x => x.Humidity),
                    } : null;
                }


                return data;
            }
        }

        [HttpGet]
        [Route("GetMinuteHistorical")]
        public Dictionary<string, DeviceData> GetMinuteHistorical(int count = 5)
        {
            using (HydroContext context = new HydroContext())
            {
                var startMinute = DateTime.Now.AddMinutes(-count);
                Dictionary<string, DeviceData> data = new Dictionary<string, DeviceData>();

                for (int i = 0; i < count; i++)
                {
                    DateTime key = startMinute.AddMinutes(i);


                    data[key.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")] = context.DeviceData.Where(x => key <= x.DateCreated && x.DateCreated <= key.AddSeconds(59)).Count() > 0 ? new DeviceData()
                    {
                        DateCreated = key,
                        PHValue = context.DeviceData.Where(x => key <= x.DateCreated && x.DateCreated <= key.AddSeconds(59)).Average(x => x.PHValue),
                        WaterTemperature = context.DeviceData.Where(x => key <= x.DateCreated && x.DateCreated <= key.AddSeconds(59)).Average(x => x.WaterTemperature),
                        Temperature = context.DeviceData.Where(x => key <= x.DateCreated && x.DateCreated <= key.AddSeconds(59)).Average(x => x.Temperature),
                        Humidity = context.DeviceData.Where(x => key <= x.DateCreated && x.DateCreated <= key.AddSeconds(59)).Average(x => x.Humidity),
                    } : null;
                }


                return data;
            }
        }
    }

}
