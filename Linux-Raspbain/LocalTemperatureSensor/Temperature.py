import os
import glob
import time
import datetime
import requests
import RPi.GPIO as GPIO    # GPIO

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
        return


"""
    temp_c - Temperature (Centigrade)

    Toggle LED
"""
def is_over_heat(temp_c):
        #**********************WorkShop-2**************************#
        
##        # Condition
##        condition = temp_c > 25
##
##        # Toggle LED
##        if condition:
##                GPIO.output(GPIO_PIN, GPIO.HIGH)
##        else:
##                GPIO.output(GPIO_PIN, GPIO.LOW)

        #**********************************************************#
        return

try:		
#**********************WorkShop-1**************************#
				
### Start
##    while True:
##        read_temperature()
##        time.sleep(1)    # 1 seconds

#**********************************************************#
except KeyboardInterrupt:
    GPIO.cleanup()