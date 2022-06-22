## TinyClicker

A simple background autoclicker for the Tiny Tower game. TinyClicker implements pattern matching and OCR to help you automate the grind of rebuilding your Tiny Tower on desktop. Work in progress.

![tinyclicker_png2](https://user-images.githubusercontent.com/51026900/174921574-5e6b74a7-d3b1-4d8e-a21f-95d65431b792.png)

#### Platform: Windows 7 (x64/x86) or later versions
### Supported Emulators:
- LDPlayer 4.x
- BlueStacks 5.x

## Features

- 100% autonomous work * 
- Tower rebuilding at desired floor and completion of the tutorial
- TinyClicker will watch all advertisements to maximize coins and bux profit
- Automatic participation in the hourly raffle
- Configure once, launch and forget it
- Rebuilding statistics

**only with VIP package version at this time*

![](https://github.com/filadog/TinyClicker/blob/master/gif.gif)


## Before running

Make sure to run only one instance of android emulator. 
Emulator window with the game can be covered with other windows, but do not minimize it to the tray. 
Player interactions with the game are possible but not recommended.

By default the tower will rebuild as soon as it hits 50th floor. If you have more floors make sure to manually transform your bitizens elsewhere before launching the clicker.

TinyClicker will generate a log entry inside Stats.txt file each time it rebuilds the tower. The file will be created once the tower rebuilds in the main clicker folder.

#### Disclaimer: The program is in the phase of active development and bugs may occur. Use at your own risk.

## Setup

#### LDPlayer
- Prior to launching the clicker, set the LDPlayer resolution setting to Customize, with <code>Width = 333</code>, <code>Height = 592</code>, <code>DPI = 150</code>. It is also recommended to set the "Fixed window size" to "Enable" at the "Other settings" tab in order to prevent accidental change of the resolution. This is important and TinyClicker won't work without the correct resolution.

#### BlueStacks
- Navigate to settings and create a custom screen resolution under the "Screen" tab. Set <code>Width to 666 px</code> and <code>Height to 1184 px</code>, <code>DPI to 240 or 320</code>. Save and apply the settings. Emulator window can be reasonably resized afterwards by dragging the window edge before the start of TinyClicker. Important part is the screen aspect ratio and it shouldn't be changed. Do not resize the window if the clicker has already started.

#### General setup
- Launch TinyClicker, navigate to the Settings window (a cog button) and set the <code>Current floor</code> number to the last built floor of your tower. <code>Rebuild at</code> sets the floor at which the clicker will rebuild the tower. <code>Watch Ads From</code> sets the floor number after which ads will be watched, as they become more time-efficient. Set the value higher the more gold bux you already have. Rule of thumb is 100 gold bux = 25th floor, 200 = 35th floor and so on until 50. If you have more than 500 gold bux ads will only waste time. Unchecked <code>Watch Bux Ads</code> will reject all ads with bux reward, as they are inefficient. 

- It is advisable to disable the web browser app inside the simulated device since some ads will try to open it and cause the clicker to bug as the program cannot navigate through anything but the Tiny Tower interface.

- It is necessary to swith the regular video offers gift (FREE gift that appears every 4-5 minutes) to only offer you bux, TinyClicker will collect them.

- When the setup is done, press Save and Exit buttons in settings, select your emulator in the main window, launch the game and start the TinyClicker. Your settings will be saved and current floor will be automatically tracked. Next time, just select the emulator and Start the clicker.


## For the iOS players

If you have a Windows PC it is possible to use Tiny Tower cloud save inside the Android version of Tiny Tower inside android emulator, VIP package will be transformed as well. Avoid checking the tower on your mobile device while the clicker is running because in-game cloud sync may confuse the most recent version of the game and you may lose some progress. To avoid this allow for at least 30 minutes to pass before launching the game on your iOS device after TinyClicker is stopped.


## Libraries in use

[shimat/opencvsharp](https://github.com/shimat/opencvsharp)\
[charlesw/tesseract](https://github.com/charlesw/tesseract)
