# 6 Axis Robotic Arm

![](https://github.com/Juulbl/6AxisRoboticArm/blob/master/media/images/RobotArmStage3-2.jpeg)

## Hardware

### Motors
Servos drive all the joints of the robot. Because all joints carry a different weight, three different kinds of servos are used.

Servos:
- MG996R:<br/>This servo is used at joints two and three, these joints bare the most weight and therefor need high torque servos.
- S3003:<br/>This servo is used at joints one and four, these joint don't carry a lot of weight so they don't need high torque servos.
- SG90:<br/>This servo is used in the last two joints, these joint carry the least amount of weight and needed to be small.

### Drivers

All servos require their own PWM signals. It would take a lot of CPU power if the micro controller needed to handle this by itself. Because of this an external PWM driver is used, in this case the [Adafruit PCA9685](https://www.adafruit.com/product/815). This driver can handle up to 16 servos at 6 Volts. This specific driver was chosen because it could handle enough servos and uses an I2c interface to communicate with the micro controller.

### Micro controllers

The robot was build to interpret incoming(UART) data and convert it to the correct servo angle which it than send to the PWM/Servo driver. This made it also possible to create a front-end which acts like a time line and makes it posible to update the robot's position live when programming its movement.<br/>A micro controller was chosen which could "easily" use UART as well as I2c communication. Because a lot of the intensive calculations are done at the computer running the front-end the micro controller didn't need to have a really fast CPU, therefor the [Arduino Uno](https://store.arduino.cc/arduino-uno-rev3) was chosen.

## C++ / Arduino software



## C# / GTK software