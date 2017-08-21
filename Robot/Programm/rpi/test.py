import serial

print "Open port /dev/serial0"
serialport = serial.Serial(port="/dev/serial0", baudrate=57600, timeout=3)
print "Send command RESET"
serialport.write("RESET\n")
print "Robot answer:"
resp = serialport.readline()
print resp
