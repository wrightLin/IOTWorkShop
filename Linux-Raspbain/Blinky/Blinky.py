import os
import glob
##import time
##import RPi.GPIO as GPIO    # GPIO
##
### GPIO Config
##GPIO_PIN = 26    # GPIO Pin
##GPIO.setmode(GPIO.BCM)
##GPIO.setup(GPIO_PIN, GPIO.OUT)
##
### Blink LED
##IS_LED_LIGHT = False
##
##try:
##    while True:
##        # Toggle LED
##        IS_LED_LIGHT = not IS_LED_LIGHT
##
##        if IS_LED_LIGHT:
##            print("Light")
##            GPIO.output(GPIO_PIN, GPIO.HIGH)
##        else:
##            print("Dark")
##            GPIO.output(GPIO_PIN, GPIO.LOW)
##
##        # Timer
##        time.sleep(0.5)    # 0.5 seconds
##except KeyboardInterrupt:
##    GPIO.cleanup()
##	