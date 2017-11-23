using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace SimulatedDevice
{
    /// <summary>
    /// 裝置的屬性
    /// </summary>
    public class DeviceProperty
    {
        // Device Property
        public string DeviceID = string.Empty;
        public int HubEnabledState = 1;
        public string DeviceState = "normal";
        public string Manufacturer = "Raspberry Pi Org";
        public string ModelNumber = "Raspberry Pi 2 B+";
        public string SerialNumber = "A9090";
        public string FirmwareVersion = "10.0586";
        public string Platform = "Windows10 IoT Core";
        public string Processor = "ARM";
        public double Latitude = 23.583234;
        public double Longitude = 120.5825975;
        public DateTime CreatedTime = DateTime.Now;
        public DateTime UpdatedTime = DateTime.Now;

        // Device
        public int IsSimulatedDevice = 0;
        public string ObjectType = "DeviceInfo";
        public string Version = "1.0";

        /// <summary>
        /// 建構
        /// </summary>
        public DeviceProperty()
        {
        }

        /// <summary>
        /// 建構
        /// </summary>
        /// <param name="deviceId"></param>
        public DeviceProperty(string deviceId)
        {
            this.DeviceID = deviceId;
        }

        /// <summary>
        /// 取得裝置更新資料
        /// </summary>
        /// <returns></returns>
        public string GetMessageString()
        {
            // TODO: 更新裝置資料

            JObject deviceProps = new JObject();
            deviceProps.Add("DeviceID", DeviceID);
            deviceProps.Add("HubEnabledState", HubEnabledState);
            //deviceProps.Add("CreatedTime", CreatedTime);
            deviceProps.Add("DeviceState", DeviceState);
            deviceProps.Add("UpdatedTime", UpdatedTime);
            deviceProps.Add("Manufacturer", Manufacturer);
            deviceProps.Add("ModelNumber", ModelNumber);
            deviceProps.Add("SerialNumber", SerialNumber);
            deviceProps.Add("FirmwareVersion", FirmwareVersion);
            deviceProps.Add("Platform", Platform);
            deviceProps.Add("Processor", Processor);
            deviceProps.Add("Latitude", Latitude);
            deviceProps.Add("Longitude", Longitude);

            JObject device = new JObject();
            device.Add("DeviceProperties", deviceProps);
            device.Add("Commands", new JArray());
            device.Add("IsSimulatedDevice", IsSimulatedDevice);
            device.Add("ObjectType", ObjectType);
            device.Add("Version", Version);

            var messageString = JsonConvert.SerializeObject(device);

            return messageString;
        }

    }
}
