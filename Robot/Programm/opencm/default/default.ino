#include "KvantoRover.h"

#define DXL_BUS_SERIAL1 1  //Dynamixel on Serial1(USART1)  <-OpenCM9.04
#define DXL_BUS_SERIAL2 2  //Dynamixel on Serial2(USART2)  <-LN101,BT210
#define DXL_BUS_SERIAL3 3  //Dynamixel on Serial3(USART3)  <-OpenCM 485EXP

Dynamixel Dxl(DXL_BUS_SERIAL3);
KvantoRover Robot;

void setup() {
  SerialUSB.attachInterrupt(usbInterrupt);
  // Dynamixel 2.0 Baudrate -> 0: 9600, 1: 57600, 2: 115200, 3: 1Mbps
  Dxl.begin(3);
  // Initialize robot
  Robot.setup(&Dxl);
}

void usbInterrupt(byte* buffer, byte nCount){
  Robot.processCommand(buffer, nCount);
  Robot.printStatus();
}

void loop() {
  Robot.loop();
}
