using System;

namespace PhysicalDevice.Model
{
    /// <summary>
    /// 溫度資料
    /// </summary>
    public class TelemetryData
    {
        public TelemetryData()
        {
            //2016/03/03濕度暫時以亂數傳入
            Random r = new Random();
            this.Humidity = r.NextDouble() * 5.00+20.00;
        }
        public string DeviceId { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
    }


}
