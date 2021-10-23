## Quick info
TinyClicker is an automated clicker for the Tiny Tower mobile game. 
The clicker utilizes computer vision to help you play Tiny Tower via LDPlayer Android mobile emulator.

Recommended version of LDPlayer is 4.x


## Before running
Make sure to run only one instance of LDPlayer. 
You can cover the emulator window with other windows, but do not minimize it to tray. 
Console can be minimized. Player interactions with the game are possible but not recommended. 

## Setup
- Prior to launching the clicker, set the LDPlayer resolution setting to Customize, with Width = 333, Height = 592, DPI = 150. It is also recommended to set the "Fixed window size" to "Enable" at the "Other settings" tab in order to prevent accidental resolution change. This is important and TinyClicker won't work without the correct resolution.

- If you don't have the VIP Tiny Tower package, open the file Config.txt, located inside the TinyClicker folder with notepad. Locate the "VipPackage": setting and change it to "false" without quotes. If you have the VIP package set the setting to true. You can also provide the current number of floors (e.g. 14 or 27 etc) for automatic tower rebuilding and the elevator speed (e.g. 7.25 or 6 etc). Only use the format provided by examples in brackets.
You can skip this step entirely and provide all the information through the console, to do so type "cc" without quotes to the console after launching the clicker and follow the instructions.

- When setup is done, simply launch LDPlayer, then the game and lastly the clicker. Follow the instructions inside the console. This setup is done once unless some of the parameters such as elevator speed or VIP status change.
