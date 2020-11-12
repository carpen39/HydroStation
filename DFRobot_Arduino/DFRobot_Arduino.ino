#include "DFRobot_PH.h"
#include <EEPROM.h>

#define PH_PIN A0
float voltage,phValue,temperature = 21;
DFRobot_PH ph;

void setup()
{
  Serial.begin(115200);
  ph.begin();
}

void loop()
{
    static unsigned long timepoint = millis();
    if(millis()-timepoint>1000U)  //time interval: 1s
    {
      timepoint = millis();
      voltage = analogRead(PH_PIN)/1024.0*5000;  // read the voltage
      //temperature = readTemperature();  // read your temperature sensor to execute temperature compensation
      phValue = ph.readPH(voltage,temperature);  // convert voltage to pH with temperature compensation
      Serial.print("temperature:");
      Serial.print(temperature,1);
      Serial.print("^C  pH:");
      Serial.println(phValue,2);
    }
    ph.calibration(voltage,temperature);  // calibration process by Serail CMD
}

float readTemperature()
{
  //add your code here to get the temperature from your temperature sensor
}
