using DS18B20_1WireBus.Model;
using DS18B201WireLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Gpio;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DS18B20_1WireBus
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private OneWire onewire;
        private string deviceId = string.Empty;
        private DispatcherTimer timer;
        private bool inprog = false;
        private int ledPin ;
        TemperatureModel tempData;
        double temperatureThreshold;

        private GpioPin pin;
        private GpioPinValue pinValue;

        /// <summary>
        /// 建構式
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            tempData = new TemperatureModel();
            deviceId = string.Empty;
            this.DataContext = tempData;
            timer = new DispatcherTimer();


            //*******************************WorkShop-1***********************************//

            //timer.Interval = TimeSpan.FromMilliseconds(1200);           // 取溫度資料的時間間隔

            //*******************************************************************************//



            //*********************************WorkShop-2***********************************//

            //temperatureThreshold = 25.0;    // 溫度門檻值
            //ledPin = 26;              // 輸出之GPIO腳位

            //********************************************************************************//


            timer.Tick += Timer_Tick;
            onewire = new OneWire();
        }

        /// <summary>
        /// 開始溫度感測
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            // 取得裝置資訊
            await GetFirstSerialPort();

            if (deviceId != string.Empty)
            {
                tempData.StatusText = "Reading from device: " + deviceId;
                tempData.Started = true;
                timer.Start();
            }
        }
        /// <summary>
        /// 計時器函式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Timer_Tick(object sender, object e)
        {

            if (!inprog)
            {
                inprog = true;

                //*******************************WorkShop-1***********************************//

                ////取得溫度資料
                //tempData.Temperature = await onewire.getTemperature(deviceId);

                //********************************************************************************//



                //*********************************WorkShop-2***********************************//

                //// 溫度高於門檻值，閃爍LED指示燈
                //if (tempData.Temperature > temperatureThreshold)
                //{
                //    Blink_LED();
                //}

                //********************************************************************************//


                inprog = false;
            }
        }

        /// <summary>
        /// 停止溫度感測
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            tempData.Started = false;
            onewire.shutdown();
        }


        /// <summary>
        /// 取得連接裝置
        /// </summary>
        /// <returns></returns>
        private async Task GetFirstSerialPort()
        {
            try
            {
                // 初始化GPIO裝置 
                InitGPIODevices();

                string aqs = SerialDevice.GetDeviceSelector();
                var dis = await DeviceInformation.FindAllAsync(aqs);
                if(dis.Count > 0)
                {
                    var deviceInfo = dis.First();
                    deviceId = deviceInfo.Id;
                }
            }
            catch (Exception ex)
            {
                tempData.StatusText = "Unable to get serial device: " + ex.Message;
            }
        }


        /// <summary>
        /// 閃爍LED燈
        /// </summary>
        private void Blink_LED()
        {
            if (pinValue == GpioPinValue.High)
            {
                pinValue = GpioPinValue.Low;
                pin.Write(pinValue);
            }
            else
            {
                pinValue = GpioPinValue.High;
                pin.Write(pinValue);
            }
        }


        /// <summary>
        ///  初始化GPIO裝置 (LED燈)
        /// </summary>
        private void InitGPIODevices()
        {
            // 初始化GPIO 控制器 
            var gpio = GpioController.GetDefault();
            // 檢查有無正確連接感測器裝置
            if (gpio == null)
            {
                pin = null;
            }

            // 取得GPIO對應之實際pin腳位
            pin = gpio.OpenPin(ledPin);

            // 設置該GPIO腳位屬性
            // 1.為輸出腳位
            // 2.輸出電壓為Low
            pinValue = GpioPinValue.Low;
            pin.Write(pinValue);
            pin.SetDriveMode(GpioPinDriveMode.Output);
        }
    }
}
