using System;

namespace SimulatedDevice
{
    class Program
    {
        static Device SimulatedDevice = new Device();

        static void Main(string[] args)
        {
            // TODO: Device設定
            SimulatedDevice.HostName = "IOT Hub Name";
            SimulatedDevice.DeviceId = "Your Device ID";
            SimulatedDevice.DeviceKey = "Your Device Key";

            // 初始化裝置
            SimulatedDevice.InitDevice();
            
            // 發送和接收資料
            SimulatedDevice.SendTelemetryData();
            SimulatedDevice.ReceiveTelemetryData();

            Console.ReadLine();
        }
    }
}
