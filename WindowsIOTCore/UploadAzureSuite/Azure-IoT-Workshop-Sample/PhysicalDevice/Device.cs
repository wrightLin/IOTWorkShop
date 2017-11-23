using PhysicalDevice.Model;
using PhysicalDevice.WireLib;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.SerialCommunication;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;

namespace PhysicalDevice
{
    public class Device
    {
        // 裝置資訊
        public string DeviceId = string.Empty;      // DeviceId
        public string DeviceKey = string.Empty;     // SharedAccessKey
        public string HostName = string.Empty;      // HostName

        // Serial Port相關
        private string UartId = string.Empty;
        private OneWire Onewire;

        // GPIO控制相關
        private const int LED_PIN = 26; //pin 37
        private GpioPin LedPin;

        // Azure IoT Hub
        private AzureConnectionHelper AzureHelper;

        // Model
        public TemperatureModel TempModel;


        private DeviceClient Client = null;             // Device Client

        #region 初始化

        /// <summary>
        /// 建構
        /// </summary>
        /// <param name="deviceClient"></param>
        public Device()
        {
            // //************************WorkShop1：填入裝置連線資訊**************************//
            // this.DeviceId = "Your Device ID";
            // this.DeviceKey = "Your Device Key";
            // this.HostName = "IOT Hub Name";

            // // 建立
            // AzureHelper = new AzureConnectionHelper();
            // AzureHelper.HostName = this.HostName;
            // AzureHelper.DeviceId = this.DeviceId;
            // AzureHelper.DeviceKey = this.DeviceKey;

            // // 初始化Device Client
            // Onewire = new OneWire();
            // AzureHelper.InitDeviceClient();
        }

        /// <summary>
        /// 初始化裝置
        /// </summary>
        public void InitDevice()
        {
            // TODO: 設定 LED，一開始不要亮!
            LedPin = GpioController.GetDefault().OpenPin(LED_PIN);
            LedPin.Write(GpioPinValue.Low);
            LedPin.SetDriveMode(GpioPinDriveMode.Output);
        }

        #endregion 初始化


        #region Start Device

        /// <summary>
        /// 啟動
        /// </summary>
        /// <returns></returns>
        public void StartDevice()
        {
            if (UartId == string.Empty) return;

            TempModel.StatusText = "Reading from device: " + UartId;
            TempModel.Started = true;

            try
            {
                // 更新裝置資訊到Azure
                UpdateDeviceInfo();
            }
            catch (Exception ex)
            {
                TempModel.StatusText = "Error: " + ex.ToString();
            }
        }

        /// <summary>
        /// 取得第一個Serial Port
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetFirstSerialPort()
        {
            try
            {
                string aqs = SerialDevice.GetDeviceSelector();
                var dis = await DeviceInformation.FindAllAsync(aqs);
                if (dis.Count > 0)
                {
                    var deviceInfo = dis.First();
                    UartId = deviceInfo.Id;
                }
            }
            catch (Exception ex)
            {
                TempModel.StatusText = "Unable to get serial device: " + ex.Message;
            }
            return UartId;
        }

        /// <summary>
        /// 更新裝置資訊到Azure
        /// </summary>
        public async void UpdateDeviceInfo()
        {
            DeviceProperty deviceProp = new DeviceProperty(AzureHelper.DeviceId);

            // //**************WorkShop2：裝置屬性資訊**************//
            // deviceProp.Latitude = 9.6;       // 緯度
            // deviceProp.Longitude = 53.3;     // 經度
            // deviceProp.SerialNumber = "A9090";
            // deviceProp.UpdatedTime = DateTime.Now;

            await AzureHelper.UpdateDeviceInfo(deviceProp);
        }

        #endregion Start Device


        #region Process Device

        /// <summary>
        /// 主要處理邏輯
        /// </summary>
        public async void ProcessDevice()
        { 
            try
            {
                // 讀取溫度
                double temperature = await Onewire.GetTemperature(UartId);
                TempModel.Temperature = temperature;




                // ////************************WorkShop3：上傳裝置資訊至雲端**************************//
                // // 建立連線
                // string IoTHubConnString = string.Format($"HostName={HostName};DeviceId={DeviceId};SharedAccessKey={DeviceKey}");
                // Client = DeviceClient.CreateFromConnectionString(IoTHubConnString, TransportType.Http1);

                // // 設定資料
                // TelemetryData data = new TelemetryData();
                // data.Temperature = temperature;
                // data.DeviceId = DeviceId;

                // // 傳送訊息
                // var messageString = JsonConvert.SerializeObject(data);
                // var message = new Message(Encoding.UTF8.GetBytes(messageString));
                // await Client.SendEventAsync(message);


                // 從Azure接收Alarm資訊
                string Rmessage = await ReceiveTelemetryData();

                // TODO: 處理從Azure接收Alarm資訊

                //************************WorkShop4：加入溫度警示門檻值**************************//

                ////設定LED亮/暗的條件
                //bool isTurn = (temperature > 26.0);             // 大於26度，就亮燈

                //// 設定LED亮/暗
                //GpioPinValue alert = isTurn ? GpioPinValue.High : GpioPinValue.Low;
                //LedPin.Write(alert);

            }
            catch (Exception ex)
            {
                TempModel.StatusText = "Error: " + ex.ToString();
            }



        }

        /// <summary>
        /// 傳送溫度資料到Azure
        /// </summary>
        /// <param name="temperature"></param>
        /// <returns></returns>
        public async void SendTelemetryData(double temperature)
        {
            await AzureHelper.SendTelemetryData(temperature);
        }

        /// <summary>
        /// 從Azure接收Alarm資訊
        /// </summary>
        public async Task<string> ReceiveTelemetryData()
        {
            string message = await AzureHelper.ReceiveTelemetryData();

            return message;
        }

        #endregion Process Device


        #region Stop Device

        public void StopDevice()
        {
            TempModel.Started = false;
            Onewire.Shutdown();
        }

        #endregion Stop Device
    }
}
