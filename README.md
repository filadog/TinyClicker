## TinyClicker
TinyClicker is an automated clicker for the Tiny Tower mobile game. 
The clicker utilizes computer vision to help you play Tiny Tower via LDPlayer Android mobile emulator on Windows.

**Platform: Windows**\
**Recommended version of LDPlayer is 4.x**


## Features
- 100% autonomous work
- Fully automated tower rebuilding at level 50 and completion of the obligatory tutorial
- TinyClicker will watch all advertisements to maximize coins and bux profits
- Configure once, launch and forget about it


## Before running
Make sure to run only one instance of LDPlayer. 
Emulator window can be covered with other windows, but do not minimize it to tray. 
Console application can be minimized. Player interactions with the game are possible but not recommended.\
Avoid checking the tower on your mobile device while the clicker is running, in-game cloud sync may confuse the most recent version of the game and some progress may be lost.


## Setup
- Prior to launching the clicker, set the LDPlayer resolution setting to Customize, with Width = 333, Height = 592, DPI = 150. It is also recommended to set the "Fixed window size" to "Enable" at the "Other settings" tab in order to prevent accidental change of the resolution. This is important and TinyClicker won't work without the correct resolution.

- In case there is no VIP package, open the file Config.txt, located inside the TinyClicker folder with notepad. Locate the "VipPackage": setting and change it to "false" without quotes. If you have the VIP package leave the setting at true. You should also provide the number of the last built floor (e.g. 14 or 27 etc) for automatic tower rebuilding and the elevator speed (e.g. 7.25 or 6 etc). Only use the format provided by examples in brackets.\
This step can be skipped entirely with the console, to do so type "cc" without quotes to the console after launching TinyClicker and follow the instructions. Restart the clicker at the end of the setup.\
The setup is done once unless some of the parameters such as elevator speed or VIP status change. In case the config was edited manually, close the clicker before and restart it after the new config was saved.

- When the setup is done, LDPlayer is launched first, then the Clicker. To start the clicker type letter s to the console and press the Enter key, TinyClicker will find the game if it is on the screen and will start working on its own. 

## For iOS players
If you have a Windows PC it is possible to use TinyTower cloud save inside the Android version of Tiny Tower in LDPlayer, VIP package will be transformed as well. Avoid checking the tower on your mobile device while the clicker is running because in-game cloud sync may confuse the most recent version of the game and you may lose some progress. To avoid this wait at least 30 minutes before launching the game on your iOS device after the clicker is stopped.
