using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using SimulatedDevice.Model;
using System;
using System.Text;

namespace SimulatedDevice
{
    /// <summary>
    /// 裝置的相關操作
    /// </summary>
    public class Device
    {
        public string DeviceId = string.Empty;          // DeviceId
        public string DeviceKey = string.Empty;         // SharedAccessKey
        public string HostName = string.Empty;          // HostName

        private string IoTHubConnString = string.Empty; // Iot Hub Connection String
        private DeviceClient Client = null;             // Device Client

        private const int INTERVAL_TIME = 2000;         // Interval Time

        /// <summary>
        /// 初始化裝置
        /// </summary>
        public void InitDevice()
        {
            // 建立連線
            IoTHubConnString = string.Format($"HostName={HostName};DeviceId={DeviceId};SharedAccessKey={DeviceKey}");
            Client = DeviceClient.CreateFromConnectionString(IoTHubConnString, TransportType.Http1);

            // 更新裝置資訊
            UpdateDeviceInfo();
        }

        /// <summary>
        /// 更新裝置資訊
        /// </summary>
        public async void UpdateDeviceInfo()
        {
            DeviceProperty deviceProp = new DeviceProperty(DeviceId);

            // TODO: 設定Device Info
            deviceProp.Latitude = -23.583234;
            deviceProp.Longitude = 120.5825975;
            deviceProp.SerialNumber = "A9090";
            deviceProp.UpdatedTime = DateTime.Now;

            // 傳送更新訊息
            var message = new Message(Encoding.UTF8.GetBytes(deviceProp.GetMessageString()));
            await Client.SendEventAsync(message);

            // 處理事件
            Console.WriteLine($"Device {DeviceId} Start");
        }

        /// <summary>
        /// 傳送溫度資訊
        /// </summary>
        public async void SendTelemetryData()
        {
            while(true)
            {
                // TODO: 產生隨機資料
                Random rand = new Random();
                int randValue = rand.Next(25);
                TelemetryData data = new TelemetryData();
                data.Temperature = 15 + randValue;
                data.DeviceId = DeviceId;

                // TODO: 處理事件
                Console.WriteLine($"Device Id：{data.DeviceId}；Temperature：{data.Temperature}");

                // 傳送訊息
                var messageString = JsonConvert.SerializeObject(data);
                var message = new Message(Encoding.UTF8.GetBytes(messageString));
                await Client.SendEventAsync(message);

                System.Threading.Thread.Sleep(INTERVAL_TIME);
            }
        }

        /// <summary>
        /// 接收Alarm資訊
        /// </summary>
        public async void ReceiveTelemetryData()
        {
            while (true)
            {
                // 接收訊息
                Message receivedMessage = await Client.ReceiveAsync();

                if(receivedMessage == null) continue;

                // TODO: 處理事件
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Received message: {Encoding.UTF8.GetString(receivedMessage.GetBytes())}");
                Console.ResetColor();

                // 刪除Device Queue的訊息
                await Client.CompleteAsync(receivedMessage);
            }
        }
    }
}
