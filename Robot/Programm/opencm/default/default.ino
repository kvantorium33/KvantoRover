#include "KvantoRover.h"
#include <string.h>

#define DXL_BUS_SERIAL1 1  //Dynamixel on Serial1(USART1)  <-OpenCM9.04
#define DXL_BUS_SERIAL2 2  //Dynamixel on Serial2(USART2)  <-LN101,BT210
#define DXL_BUS_SERIAL3 3  //Dynamixel on Serial3(USART3)  <-OpenCM 485EXP

Dynamixel Dxl(DXL_BUS_SERIAL3);
KvantoRover Robot;
byte cmd[255];
byte len = 0;

void setup() {
  Serial2.begin(57600);
  Serial2.attachInterrupt(serialInterrupt);
  Dxl.begin(3);
  Robot.setup(&Dxl);
}

void serialInterrupt(byte buf){
  if(buf == '\n') {
    SerialUSB.print(buf);
    Robot.processCommand(cmd, len);
    Robot.printStatus(&Serial2);
    memset(cmd, 0, 255);
    len = 0;
  } else {
    cmd[len] = buf;
    len++;
  }
}

void loop() {
  Robot.loop();
}
