using PhysicalDevice.Model;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//空白頁項目範本收錄在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PhysicalDevice
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Device PhysicalDevice;      // 裝置

        private DispatcherTimer Timer;              // Timer
        private const int INTERVAL_TIME = 2000;     // Interval Time

        private bool InProg = false;

        public MainPage()
        {
            this.InitializeComponent();

            // 初始化Device
            PhysicalDevice = new Device();
            PhysicalDevice.TempModel = new TemperatureModel();
            PhysicalDevice.InitDevice();
            this.DataContext = PhysicalDevice.TempModel;

            // 設定Timer
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(INTERVAL_TIME);
            Timer.Tick += Timer_Tick;
        }

        /// <summary>
        /// 點擊啟動事件處理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            string uartId = await PhysicalDevice.GetFirstSerialPort();

            if (uartId != string.Empty)
            {
                Timer.Start();
                PhysicalDevice.StartDevice();
            }
        }

        /// <summary>
        /// 啟動後，持續觸發事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, object e)
        {
            if (!InProg)
            {
                InProg = true;
                PhysicalDevice.ProcessDevice();
                InProg = false;
            }
        }

        /// <summary>
        /// 點擊停止事件處理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            Timer.Stop();
            PhysicalDevice.StopDevice();
        }
    }
}
