using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using PhysicalDevice.Model;
using System;
using System.Text;
using System.Threading.Tasks;

namespace PhysicalDevice
{
    /// <summary>
    /// 裝置的相關操作
    /// </summary>
    public class AzureConnectionHelper
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
        public void InitDeviceClient()
        {
            // 建立連線
            IoTHubConnString = string.Format($"HostName={HostName};DeviceId={DeviceId};SharedAccessKey={DeviceKey}");
            Client = DeviceClient.CreateFromConnectionString(IoTHubConnString, TransportType.Http1);
        }

        /// <summary>
        /// 更新裝置資訊
        /// </summary>
        public async Task UpdateDeviceInfo(DeviceProperty deviceProp)
        {
            // 傳送更新訊息
            var message = new Message(Encoding.UTF8.GetBytes(deviceProp.GetMessageString()));
            await Client.SendEventAsync(message);
        }

        /// <summary>
        /// 傳送溫度資訊
        /// </summary>
        public async Task SendTelemetryData(double temperature)
        {
            // 設定資料
            TelemetryData data = new TelemetryData();
            data.Temperature = temperature;
            data.DeviceId = DeviceId;
       

            // 傳送訊息
            var messageString = JsonConvert.SerializeObject(data);
            var message = new Message(Encoding.UTF8.GetBytes(messageString));
            await Client.SendEventAsync(message);
        }

        /// <summary>
        /// 接收Alarm資訊
        /// </summary>
        public async Task<string> ReceiveTelemetryData()
        {
            // 接收訊息
            Message receivedMessage = await Client.ReceiveAsync();

            if(receivedMessage == null) return string.Empty;

            // 取得Message
            string message = Encoding.UTF8.GetString(receivedMessage.GetBytes());

            // 刪除Device Queue的訊息
            await Client.CompleteAsync(receivedMessage);

            return message;
        }
    }
}
