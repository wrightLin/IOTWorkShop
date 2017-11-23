using System;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace PhysicalDevice.WireLib
{
    public class OneWire
    {
        private SerialDevice SerialPort = null;     // Serial Port
        private DataWriter DataWriteObject = null;  // 寫入
        private DataReader DataReaderObject = null; // 讀取

        /// <summary>
        /// 關閉
        /// </summary>
        public void Shutdown()
        {
            if (SerialPort != null)
            {
                SerialPort.Dispose();
                SerialPort = null;
            }
        }

        /// <summary>
        /// 重設
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        async Task<bool> OnewireReset(string deviceId)
        {
            try
            {
                if (SerialPort != null) SerialPort.Dispose();

                SerialPort = await SerialDevice.FromIdAsync(deviceId);

                // Configure serial settings
                SerialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
                SerialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                SerialPort.BaudRate = 9600;
                SerialPort.Parity = SerialParity.None;
                SerialPort.StopBits = SerialStopBitCount.One;
                SerialPort.DataBits = 8;
                SerialPort.Handshake = SerialHandshake.None;

                DataWriteObject = new DataWriter(SerialPort.OutputStream);
                DataWriteObject.WriteByte(0xF0);
                await DataWriteObject.StoreAsync();

                DataReaderObject = new DataReader(SerialPort.InputStream);
                await DataReaderObject.LoadAsync(1);
                byte resp = DataReaderObject.ReadByte();
                if (resp == 0xFF)
                {
                    System.Diagnostics.Debug.WriteLine("Nothing connected to UART");
                    return false;
                }
                else if (resp == 0xF0)
                {
                    System.Diagnostics.Debug.WriteLine("No 1-wire devices are present");
                    return false;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Response: " + resp);
                    SerialPort.Dispose();
                    SerialPort = await SerialDevice.FromIdAsync(deviceId);

                    // Configure serial settings
                    SerialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
                    SerialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                    SerialPort.BaudRate = 115200;
                    SerialPort.Parity = SerialParity.None;
                    SerialPort.StopBits = SerialStopBitCount.One;
                    SerialPort.DataBits = 8;
                    SerialPort.Handshake = SerialHandshake.None;
                    DataWriteObject = new DataWriter(SerialPort.OutputStream);
                    DataReaderObject = new DataReader(SerialPort.InputStream);
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 寫入
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public async Task OnewireWriteByte(byte b)
        {
            for (byte i = 0; i < 8; i++, b = (byte)(b >> 1))
            {
                // Run through the bits in the byte, extracting the
                // LSB (bit 0) and sending it to the bus
                await OnewireBit((byte)(b & 0x01));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        async Task<byte> OnewireBit(byte b)
        {
            var bit = b > 0 ? 0xFF : 0x00;
            DataWriteObject.WriteByte((byte)bit);
            await DataWriteObject.StoreAsync();
            await DataReaderObject.LoadAsync(1);
            var data = DataReaderObject.ReadByte();
            return (byte)(data & 0xFF);
        }

        /// <summary>
        /// 讀取
        /// </summary>
        /// <returns></returns>
        async Task<byte> OnewireReadByte()
        {
            byte b = 0;
            for (byte i = 0; i < 8; i++)
            {
                // Build up byte bit by bit, LSB first
                b = (byte)((b >> 1) + 0x80 * await OnewireBit(1));
            }
            System.Diagnostics.Debug.WriteLine("onewireReadByte result: " + b);
            return b;
        }

        /// <summary>
        /// 取資料
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<double> GetTemperature(string deviceId)
        {
            double tempCelsius = -200;

            if (await OnewireReset(deviceId))
            {
                await OnewireWriteByte(0xCC); //1-Wire SKIP ROM command (ignore device id)
                await OnewireWriteByte(0x44); //DS18B20 convert T command 
                                              // (initiate single temperature conversion)
                                              // thermal data is stored in 2-byte temperature 
                                              // register in scratchpad memory

                // Wait for at least 750ms for data to be collated
                await Task.Delay(750);

                // Get the data
                await OnewireReset(deviceId);
                await OnewireWriteByte(0xCC); //1-Wire Skip ROM command (ignore device id)
                await OnewireWriteByte(0xBE); //DS18B20 read scratchpad command
                                              // DS18B20 will transmit 9 bytes to master (us)
                                              // starting with the LSB

                byte tempLSB = await OnewireReadByte(); //read lsb
                byte tempMSB = await OnewireReadByte(); //read msb

                // Reset bus to stop sensor sending unwanted data
                await OnewireReset(deviceId);

                // Log the Celsius temperature
                tempCelsius = ((tempMSB * 256) + tempLSB) / 16.0;
                var temp2 = ((tempMSB << 8) + tempLSB) * 0.0625; //just another way of calculating it

                System.Diagnostics.Debug.WriteLine("Temperature: " + tempCelsius + " degrees C " + temp2);
            }
            return tempCelsius;
        }
    }
}
