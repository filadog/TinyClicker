## TinyClicker

A simple autoclicker for the Tiny Tower game. TinyClicker implements pattern matching and OCR to help you automate the grind of rebuilding your Tiny Tower on desktop via LDPlayer. Work in progress



![TinyClicker](https://user-images.githubusercontent.com/51026900/140165371-999ee88c-6d1e-44ef-bd62-c5ede942049b.png)

<code>Platform: Windows</code>\
<code>Recommended version of LDPlayer: 4.x</code>

## Features

- 100% autonomous work * 
- Tower rebuilding at desired floor and completion of the tutorial
- TinyClicker will watch all advertisements to maximize coins and bux profit
- Automatic participation in the hourly raffle
- Configure once, launch and forget it

**only with VIP package version at this time*

![](https://github.com/filadog/TinyClicker/blob/master/gif.gif)

## Libraries in use

[shimat/opencvsharp](https://github.com/shimat/opencvsharp)\
[charlesw/tesseract](https://github.com/charlesw/tesseract)


## Before running

Make sure to run only one instance of LDPlayer. 
LDPlayer window with the game can be covered with other windows, but do not minimize it to the tray. 
Player interactions with the game are possible but aren't recommended.\
Avoid checking the tower on mobile device while the clicker is running, in-game cloud sync may confuse the most recent version of the game and some progress can be lost.


## Setup

- Prior to launching the clicker, set the LDPlayer resolution setting to Customize, with <code>Width = 333</code>, <code>Height = 592</code>, <code>DPI = 150</code>. It is also recommended to set the "Fixed window size" to "Enable" at the "Other settings" tab in order to prevent accidental change of the resolution. This is important and TinyClicker won't work without the correct resolution.

- In case there is no VIP package, open the file Config.txt, located inside the TinyClicker folder with notepad. Locate the "VipPackage": setting and change it to "false" without quotes. If you have the VIP package leave the setting at true. You should also provide the number of the last built floor (e.g. 14 or 27 etc.) for automatic tower rebuilding and the elevator speed (e.g. 7.25 or 6 etc.). Only use the format provided by examples in brackets.\
The setup is done once unless some of the parameters such as elevator speed or VIP status change. In case the config was edited, close TinyClicker before and restart it after the new config was saved.

- It is advisable to disable the web browser app inside the emulator since some ads will try to open it and cause the clicker to bug, as th program cannot navigate through anything but the Tiny Tower app.

- In case the VIP package is present, it is recommended to swith the regular video offers gift (FREE gift that appears every 4-5 minutes) to only offer you bux, TinyClicker will collect them.

- When the setup is done, LDPlayer is launched first, then TinyClicker. To start the clicker press the Start button, TinyClicker will find the game if it's on the screen and will start its job. 

Disclaimer: The program is in the phase of active development and bugs may occur. Use at your own risk.

## For the iOS players

If you have a Windows PC it is possible to use TinyTower cloud save inside the Android version of Tiny Tower in LDPlayer, VIP package will be transformed as well. Avoid checking the tower on your mobile device while the clicker is running because in-game cloud sync may confuse the most recent version of the game and you may lose some progress. To avoid this wait at least 30 minutes before launching the game on your iOS device after TinyClicker is stopped.
