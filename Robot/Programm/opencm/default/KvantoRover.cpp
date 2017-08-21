#include "KvantoRover.h"
#include "Dynamixel.h"
#include "usb_serial.h"
#include "Arduino-compatibles.h"
#include <string.h>
#include "HardwareSerial.h"

#define ID_NUM 1

KvantoRover::KvantoRover() {
  SerialUSB.println("KvantoRover: contructor done");
}

void KvantoRover::setup(Dynamixel *dxl) {
  this->dxl = dxl;

  this->dxl->wheelMode(DXL_WHEEL_LEFT_FRONT);
  this->dxl->wheelMode(DXL_WHEEL_LEFT_MIDDLE);
  this->dxl->wheelMode(DXL_WHEEL_LEFT_REAR);
  this->dxl->wheelMode(DXL_WHEEL_RIGHT_FRONT);
  this->dxl->wheelMode(DXL_WHEEL_RIGHT_MIDDLE);
  this->dxl->wheelMode(DXL_WHEEL_RIGHT_REAR);
  updateRobot();
  SerialUSB.println("KvantoRover: setup done");
}

void KvantoRover::loop() {
}

void KvantoRover::processCommand(byte* buffer, byte nCount){
  SerialUSB.print("KvantoRover Command. Len: ");
  SerialUSB.print(nCount);
  SerialUSB.print("; Cmd: ");
  for(unsigned int i=0; i < nCount;i++)
    SerialUSB.print((char)buffer[i]);
  SerialUSB.println("");

  char cmd[8];
  char val[8];
  byte pos = 0;
  memset(cmd, 0, 8);
  memset(val, 0, 8);
  bool readVal = false;
  char c;
  for(unsigned int i=0; i < nCount;i++) {
    c = (char)buffer[i];
    switch(c) {
      case '\n':
        applyCommand(cmd, val);
        updateRobot();
        pos = 0;
        memset(cmd, 0, 8);
        memset(val, 0, 8);
        break;
      case ':':
        pos = 0;
        readVal = true;
        break;
      case ';':
        applyCommand(cmd, val);
        pos = 0;
        memset(cmd, 0, 8);
        memset(val, 0, 8);
        readVal = false;
        break;
      default:
        if(readVal)
          val[pos] = c;
        else
          cmd[pos] = c;
        pos++;
        break;
    }
  }
}

void KvantoRover::printStatus(HardwareSerial *port) {
  port->write("Robot is OK!\n");
}

void KvantoRover::applyCommand(char *cmd, char *val) {
  if(strcmp(cmd, "X") == 0) {
    ctrl.x = atoi(val);
  } else if(strcmp(cmd, "Y") == 0) {
    ctrl.y = atoi(val);
  } else if(strcmp(cmd, "RESET") == 0) {
    ctrl.x = 0;
    ctrl.y = 0;
  }
}

void KvantoRover::updateRobot() {
  int sp_right = 0;
  int sp_left = 0;
  if(ctrl.x != 0 || ctrl.y != 0) {
    sp_left = ctrl.x + ctrl.y;
    sp_right = -(ctrl.x - ctrl.y);
    if(sp_left > 1000) sp_left = 1000;
    if(sp_left < -1000) sp_left = -1000;
    if(sp_right > 1000) sp_right = 1000;
    if(sp_right < -1000) sp_right = -1000;
  }

  SerialUSB.print("R:");
  SerialUSB.print(sp_right);
  SerialUSB.print("; L:");
  SerialUSB.print(sp_left);
  SerialUSB.println("");

  if(sp_right < 0)
    sp_right = abs(sp_right) | 0x400;
  if(sp_left < 0)
    sp_left = abs(sp_left) | 0x400;

  this->dxl->goalSpeed(DXL_WHEEL_LEFT_FRONT, sp_left);
  this->dxl->goalSpeed(DXL_WHEEL_LEFT_MIDDLE, sp_left);
  this->dxl->goalSpeed(DXL_WHEEL_LEFT_REAR, sp_left);
  this->dxl->goalSpeed(DXL_WHEEL_RIGHT_FRONT, sp_right);
  this->dxl->goalSpeed(DXL_WHEEL_RIGHT_MIDDLE, sp_right);
  this->dxl->goalSpeed(DXL_WHEEL_RIGHT_REAR, sp_right);
}
