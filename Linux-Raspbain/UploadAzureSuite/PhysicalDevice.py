import os
import glob
import time
import datetime
import json
import requests
import RPi.GPIO as GPIO    # GPIO
import DeviceClient    # DeviceClient

"""
    Physical Device
"""

os.system('modprobe w1-gpio')
os.system('modprobe w1-therm')

# Device File
BASE_DIR = '/sys/bus/w1/devices/'
FOLDER_NAME = '28*'
DEVICE_FOLDER = glob.glob(BASE_DIR + FOLDER_NAME)[0]
DEVICE_FILE = DEVICE_FOLDER +'/w1_slave'

# Initial GPIO
GPIO_PIN = 26    # GPIO Pin Number
GPIO.setmode(GPIO.BCM)
GPIO.setup(GPIO_PIN, GPIO.OUT)

#**************WorkShop1**************#
### Azure IoT Hub Setting
##IOT_HUB = "IOT Hub Name";    # Note：Delete ".azure-devices.net"
##DEVICE_ID = "Your Device ID";
##DEVICE_KEY = "Your Device Key";


"""
    Read Device File
"""
def read_temperature_raw():
        f = open(DEVICE_FILE,'r')
        lines = f.readlines()
        f.close()
        return lines


"""
    Read Temperature
"""
def read_temperature():
        # Read Device File
        lines = read_temperature_raw()
        while lines[0].strip()[-3:] != 'YES':
                time.sleep(0.2)
                lines = read_temperature_raw()       

        equals_pos = lines[1].find('t=')
        if equals_pos != -1:
                # Get Temperature
                temp_string = lines[1][equals_pos+2:]
                temp_c = float(temp_string) / 1000.0    # Centigrade
                temp_f = temp_c * 9.0 / 5.0 + 32.0    # Fahrenheit
                
                # Display Temperature
                print('Temp: %0.2f C  %0.2f F' % (temp_c, temp_f))
                
                # Toggle LED
                is_over_heat(temp_c)
                
                # Send Temperature Data To Azure IoT Hub
                send_temperature_data(temp_c, temp_f)
        return


"""
    temp_c - Temperature (Centigrade)

    Toggle LED
"""
def is_over_heat(temp_c):
        # Condition
        condition = temp_c > 25

        # Toggle LED
        if condition:
                GPIO.output(GPIO_PIN, GPIO.HIGH)
        else:
                GPIO.output(GPIO_PIN, GPIO.LOW)
        return


"""
    temp_c - Temperature (Centigrade)
    temp_f - Temperature (Fahrenheit)

    Send Temperature Data To Azure IoT Hub
"""
def send_temperature_data(temp_c, temp_f):
        #****************WorkShop3****************#
##        # Create Azure Device Client
##        device = DeviceClient.DeviceClient(IOT_HUB, DEVICE_ID, DEVICE_KEY)
##        device.create_sas(600)
##
##        # Create Message
##        json_object = {
##            'DeviceId': DEVICE_ID,
##            'Temperature': temp_c,
##            'Humidity': 0,
##            'ExternalTemperature': temp_f
##        }
##        
##        # Send Temperature Data To Azure IoT Hub
##        message = json.dumps(json_object)
##        message_bytes = message.encode(encoding='UTF-8')
##        response = device.send(message_bytes)
        return


"""
      Update Device Info
"""
def update_device_info():
        # Create Azure Device Client
        device = DeviceClient.DeviceClient(IOT_HUB, DEVICE_ID, DEVICE_KEY)
        device.create_sas(600)
        
##        #**************WorkShop2**************# 
##        # Create Message
##        json_object = {
##            'DeviceProperties': {
##                'DeviceID': DEVICE_ID,
##                'HubEnabledState': 1,
##                'DeviceState': "normal",
##                'UpdatedTime': datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
##                'Manufacturer': "Raspberry Pi Org",
##                'ModelNumber': "Raspberry Pi 2 B+",
##                'SerialNumber': "A9090",
##                'FirmwareVersion': "8.0",
##                'Platform': "Raspbian",
##                'Processor': "ARM",
##                'Latitude': 23.583234,    # Latitude
##                'Longitude': 120.5825975    # Longitude
##            },
##            'Commands': [],
##            'IsSimulatedDevice': 1,
##            'ObjectType': "DeviceInfo",
##            'Version': "1.0"
##        }
##        # Send Device Info To Azure IoT Hub
##        message = json.dumps(json_object)
##        message_bytes = message.encode(encoding='UTF-8')
##        response = device.send(message_bytes)
##        print('Update Device Info; Response Code: %s' % (response))
        return


# Update Device Info
update_device_info()

try:
# Start
    while True:
        read_temperature()
        time.sleep(2)    # 2 seconds
except KeyboardInterrupt:
    GPIO.cleanup()
