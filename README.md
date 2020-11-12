# HydroStation
HydroStation is an experimental custom-built alternative to expensive monitoring devices like the Blue Lab Guardian. The main focus of the project was to have constant realtime ph monitoring for hydroponic applications along with the ability to check remotely. The project has since been expended to include monitoring functionality for water temperature, room temperature, and room humidity.    

 - The **device** is an Arduino microcontroller that has various sensors connected. The Arduino runs a custom script that collects this information and sends it over the USB cable ever second.
 - The **software** is a C# web-service that collects the data being sent by the **device** through the USB cable. The web-service hosts a website that allows you see the realtime and historical sensor information using any web-browser. In order to view this site from outside of your home network, you will need to configure port-forwarding on your router. 
   


## Device
### Parts 
**These parts are required.**
- <ins>Arduino Unit</ins>: https://www.dfrobot.com/product-838.html
  - This is the main unit that all of the sensors plug into. This connects to any PC using a USB B cable (printer cable, also linked below) and doesn't require a power cable. 
- <ins>Arduino Shield by DF Robot</ins>: https://www.dfrobot.com/product-1134.html
  - This shield plugs right on top of the Arduino unit. This shield has Gravity connectors on it, which allow any Gravity sensor to plug right into it without soldering anything. All of the sensors this project uses are Gravity sensors, which means they all plug right in.
- <ins>USB Cable A-B</ins>: https://www.dfrobot.com/product-134.html
  - This is used to connect your Arduino to your computer to power it.

**These sensors are optional/modular.** (For example: If you don't wish to monitor room humidity, just skip purchasing that sensor)
- <ins>Industrial PH Probe</ins>: https://www.dfrobot.com/product-2069.html
  - This probe is industrial grade, which means it's safe to keep this submerged in liquid 24/7.
- <ins>Water Temperature Sensor</ins>: https://www.dfrobot.com/product-1354.html
  - This probe is used to check the water temperature of your reservoir. Without this probe, ph calculations will be less accurate.
- <ins>Room Temperature/Room Humidity Sensor</ins>: https://www.dfrobot.com/product-1606.html
  - This sensor monitors both the temperature and humidity of the room.

## Software