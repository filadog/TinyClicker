## TinyClicker

A simple background autoclicker for the Tiny Tower game. TinyClicker implements pattern matching and OCR to help you automate the grind of rebuilding your Tiny Tower on desktop. Work in progress.



![TinyClicker](https://user-images.githubusercontent.com/51026900/140165371-999ee88c-6d1e-44ef-bd62-c5ede942049b.png)

<code>Platform: Windows 7 and later versions</code>

### Supported Emulators:
- LDPlayer 4.x
- BlueStacks 5.x

## Features

- 100% autonomous work * 
- Tower rebuilding at desired floor and completion of the tutorial
- TinyClicker will watch all advertisements to maximize coins and bux profit
- Automatic participation in the hourly raffle
- Configure once, launch and forget it

**only with VIP package version at this time*

![](https://github.com/filadog/TinyClicker/blob/master/gif.gif)


## Before running

Make sure to run only one instance of emulator. 
Emulator window with the game can be covered with other windows, but do not minimize it to the tray. 
Player interactions with the game are possible but not recommended.\
Avoid checking the tower on synchronized mobile device while the clicker is running, in-game cloud sync may confuse the most recent version of the game and some progress might be lost.

#### Disclaimer: The program is in the phase of active development and bugs may occur. Use at your own risk.

Note: by default the tower will rebuild as soon as it hits 50th floor. If you have more floors make sure to manually transform your bitizens elsewhere before launching the clicker.


## Setup

#### LDPlayer
- Prior to launching the clicker, set the LDPlayer resolution setting to Customize, with <code>Width = 333</code>, <code>Height = 592</code>, <code>DPI = 150</code>. It is also recommended to set the "Fixed window size" to "Enable" at the "Other settings" tab in order to prevent accidental change of the resolution. This is important and TinyClicker won't work without the correct resolution.

#### BlueStacks
- Navigate to settings and create a custom screen resolution under the "Screen" tab. Set <code>Width to 666 px</code> and <code>Height to 1184 px</code>, <code>DPI to 240 or 320</code>. Save and apply the settings. Emulator window can be reasonably resized by dragging the window edge before the start of TinyClicker. Do not resize the window if Clicker has already started.

#### General setup
- If there is no active VIP package, open the file Config.txt, located inside the TinyClicker folder with any text editor. Locate the "VipPackage" setting and change the value after the colon to "false", without quotes. If you have the VIP package leave the setting at true. You should also provide the number of the last built floor (e.g. 14 or 27 etc.) for automatic tower rebuilding. Only use the format provided by examples in brackets.\
The setup is done once unless some of the parameters such as VIP status change. In case the config was edited, close TinyClicker before editing and restart it after the new config was saved.

- It is advisable to disable the web browser app inside the simulated device since some ads will try to open it and cause the clicker to bug as the program cannot navigate through anything but the Tiny Tower interface.

- In case the VIP package is present, it is recommended to swith the regular video offers gift (FREE gift that appears every 4-5 minutes) to only offer you bux, TinyClicker will collect them.

- When the setup is done, emulator is launched first, then TinyClicker is started. To start the clicker, select your emulator and press the Start button, TinyClicker will find the game icon if it's on the screen and will do its job. 



## For the iOS players

If you have a Windows PC it is possible to use Tiny Tower cloud save inside the Android version of Tiny Tower inside android emulator, VIP package will be transformed as well. Avoid checking the tower on your mobile device while the clicker is running because in-game cloud sync may confuse the most recent version of the game and you may lose some progress. To avoid this allow for at least 30 minutes to pass before launching the game on your iOS device after TinyClicker is stopped.


## Libraries in use

[shimat/opencvsharp](https://github.com/shimat/opencvsharp)\
[charlesw/tesseract](https://github.com/charlesw/tesseract)
