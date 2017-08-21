#ifndef KvantoRover_h
#define KvantoRover_h

#include "Dynamixel.h"
#include "Arduino-compatibles.h"
#include "HardwareSerial.h"

#define DXL_WHEEL_LEFT_FRONT    2
#define DXL_WHEEL_LEFT_MIDDLE   8
#define DXL_WHEEL_LEFT_REAR    14
#define DXL_WHEEL_RIGHT_FRONT   4
#define DXL_WHEEL_RIGHT_MIDDLE 10
#define DXL_WHEEL_RIGHT_REAR   16

class KvantoRover {
public:
  KvantoRover();
  void setup(Dynamixel *dxl);
  void loop();
  void processCommand(byte* buffer, byte nCount);
  void printStatus(HardwareSerial *port);
private:
  struct control {
    int x;
    int y;
  };
  control ctrl;
  Dynamixel *dxl;
  void applyCommand(char *cmd, char *val);
  void updateRobot();
};

#endif /*KvantoRover_h*/
