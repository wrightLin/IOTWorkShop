// Copyright (c) Microsoft. All rights reserved.

using System;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Blinky
{
    public sealed partial class MainPage : Page
    {


        //*******************************WorkShop***************************************//

        //private const int LED_PIN = 26;     // 輸出之GPIO腳位

        //********************************************************************************//

        private GpioPin pin;
        private GpioPinValue pinValue;
        private DispatcherTimer timer;
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);

        public MainPage()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;


            //*******************************WorkShop***************************************//

            //// 初始化GPIO 控制器
            //var gpio = GpioController.GetDefault();

            //// 檢查有無正確連接感測器裝置
            //if (gpio == null)
            //{
            //    pin = null;
            //    GpioStatus.Text = "There is no GPIO controller on this device.";
            //}

            //// 取得GPIO對應之實際pin腳位
            //pin = gpio.OpenPin(LED_PIN);

            //// 設置該GPIO腳位屬性
            //// 1.為輸出腳位
            //// 2.輸出電壓為High
            //pinValue = GpioPinValue.High;
            //pin.Write(pinValue);
            //pin.SetDriveMode(GpioPinDriveMode.Output);

            //GpioStatus.Text = "GPIO pin initialized correctly.";

            //********************************************************************************//



            if (pin != null)
            {
                timer.Start();
            }        
        }

    


        /// <summary>
        ///  計時器事件：變化當前輸出電壓
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, object e)
        {
            if (pinValue == GpioPinValue.High)
            {
                pinValue = GpioPinValue.Low;
                pin.Write(pinValue);
                LED.Fill = redBrush;
            }
            else
            {
                pinValue = GpioPinValue.High;
                pin.Write(pinValue);
                LED.Fill = grayBrush;
            }
        }
             

    }
}
