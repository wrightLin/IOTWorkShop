import random
import time
import datetime
import json
import DeviceClient    # DeviceClient

"""
    Simulated Device
"""

# Azure IoT Hub Setting
IOT_HUB = "IOT Hub Name";    # Note：Delete ".azure-devices.net"
DEVICE_ID = "Your Device ID";
DEVICE_KEY = "Your DeviceKey";


"""
   Read Temperature
"""
def read_temperature():
        # Generator Temperature
        temp_c = random.uniform(5, 35)    # Centigrade
        temp_f = temp_c * 9.0 / 5.0 + 32.0    # Fahrenheit
        
        # Send Temperature Data To Azure IoT Hub
        send_temperature_data(temp_c, temp_f)
        return


"""
    temp_c - Temperature (Centigrade)
    temp_f - Temperature (Fahrenheit)

    Send Temperature Data To Azure IoT Hub
"""
def send_temperature_data(temp_c, temp_f):
        # Create Azure Device Client
        device = DeviceClient.DeviceClient(IOT_HUB, DEVICE_ID, DEVICE_KEY)
        device.create_sas(600)

        # Create Message
        json_object = {
            'DeviceId': DEVICE_ID,
            'Temperature': temp_c,
            'Humidity': 0,
            'ExternalTemperature': temp_f
        }
        
        # Send Temperature Data To Azure IoT Hub
        message = json.dumps(json_object)
        message_bytes = message.encode(encoding='UTF-8')
        response = device.send(message_bytes)
		
		# Display Temperature
        print('Temp: %0.2f C  %0.2f F ; Response Code: %s' % (temp_c, temp_f, response))
        return


"""
      Update Device Info
"""
def update_device_info():
        # Create Azure Device Client
        device = DeviceClient.DeviceClient(IOT_HUB, DEVICE_ID, DEVICE_KEY)
        device.create_sas(600)
        
        # Create Message
        json_object = {
            'DeviceProperties': {
                'DeviceID': DEVICE_ID,
                'HubEnabledState': 1,
                'DeviceState': "normal",
                'UpdatedTime': datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
                'Manufacturer': "Raspberry Pi Org",
                'ModelNumber': "Raspberry Pi 2 B+",
                'SerialNumber': "A9090",
                'FirmwareVersion': "8.0",
                'Platform': "Raspbian",
                'Processor': "ARM",
                'Latitude': 23.583234,    # Latitude
                'Longitude': 120.5825975    # Longitude
            },
            'Commands': [],
            'IsSimulatedDevice': 1,
            'ObjectType': "DeviceInfo",
            'Version': "1.0"
        }
        
        # Send Device Info To Azure IoT Hub
        message = json.dumps(json_object)
        message_bytes = message.encode(encoding='UTF-8')
        response = device.send(message_bytes)
        print('Update Device Info; Response Code: %s' % (response))
        return


# Update Device Info
update_device_info()

# Start
while True:
        read_temperature()
        time.sleep(2)    # 2 seconds