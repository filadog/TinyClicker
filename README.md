## TinyClicker

A simple background autoclicker for the Tiny Tower game tailored for the greedy Golden Tickets collectors.\
TinyClicker implements image pattern matching and OCR to help you automate the grind of rebuilding your Tiny Tower.

## 

![tinyclicker_png2](https://user-images.githubusercontent.com/51026900/174921574-5e6b74a7-d3b1-4d8e-a21f-95d65431b792.png)

##

#### Platform: Windows 7 (x86/x64) or later versions
### Supported Emulators:
- LDPlayer 4.x
- BlueStacks 5.x

## Features

- 100% autonomous work * 
- Works in background and does not steal the cursor
- Tower rebuilding at desired floor and completion of the tutorial
- TinyClicker will watch all advertisements to maximize coins and bux profit
- Automatic participation in the hourly raffle
- Easy configuration 
- Rebuilding statistics
- Science points collection

**only with VIP package version at this time*

![](https://github.com/filadog/TinyClicker/blob/master/gif.gif)


## Before running

Make sure to run only one instance of android emulator. Do not build floors manually if the clicker is running.
Emulator window with the game can be covered with other windows, but it should not be hidden in the system tray or be partially out of the screen bounds. 
Player interactions with the game are possible but not recommended. 

By default the tower will rebuild as soon as it hits 50th floor. If you have more floors make sure to manually transform your bitizens to the best hotels available before launching the clicker.

TinyClicker will generate a log entry inside Stats.txt file each time it rebuilds the tower. The file will be created once the tower rebuilds in the main clicker folder.

#### The program is in the phase of development and some bugs may occur.

## Setup

#### LDPlayer
- Prior to launching the clicker, set the LDPlayer resolution setting to Customize, with <code>Width = 333</code>, <code>Height = 592</code>, <code>DPI = 150</code>. It is also recommended to set the "Fixed window size" to "Enable" at the "Other settings" tab in order to prevent accidental change of the resolution.

#### BlueStacks
- Navigate to settings and create a custom screen resolution under the "Screen" tab. Set <code>Width to 666 px</code> and <code>Height to 1184 px</code>, <code>DPI to 240 or 320</code>. Save and apply the settings. Emulator window can be reasonably resized afterwards by dragging the window edge before the start of TinyClicker. Important part is the screen aspect ratio and it shouldn't be changed. Do not resize the window if the clicker has already started.

#### General setup
- Launch TinyClicker, navigate to the Settings window (a cog button) and set the <code>Current floor</code> number to the last built floor of your tower. <code>Rebuild at</code> sets the floor at which the clicker will rebuild the tower. <code>Watch Ads From</code> sets the floor number after which the ads will be watched, as they become more time-efficient. Set the value higher the more golden tickets you already have. Rule of thumb is 100 GT = 25th floor, 200 = 35th floor and so on until 50. If you have more than 500 GT ads will only waste time. Unchecked <code>Watch Bux Ads</code> will reject all ads with bux reward, as they are time-inefficient. 

- It is strongly advisable to disable the web browser and play market apps inside the simulated device since some ads will try to open them and cause the Clicker to stall.

- It is necessary to swith the regular video offers gift (FREE gift that appears every 4-5 minutes) to only offer you bux, TinyClicker will collect them.

- When the setup is done, press Save and Exit buttons in settings, select your emulator in the main window, launch the game and start the TinyClicker. Your settings will be saved and current floor will be automatically incremented. Next time, just select the emulator and start the Clicker.

## Information

### Collecting coins for upgrades with science points

If you wish to collect a big amount of coins without TinyClicker trying to spend them, set the <code>Current floor</code> setting to at least 200 before starting the clicker and set the <code>Rebuild at</code> setting to an even higer value. That way the clicker will assume it needs a lot of coins for a new floor and will farm them indefinitely. Science upgrades are not automated and should be performed manually, TinyClicker will collect science points for you though.


### For the iOS players

If you have a Windows PC it is possible to use Tiny Tower cloud save inside the Android version of Tiny Tower inside android emulator of your choice, VIP package will be transformed as well. 


### Cloud sync

Avoid launching the game on a mobile device when it's already launched inside emulator somewhere else. In-game cloud sync may confuse the most recent version of the game and some progress can be lost. In order to avoid this make sure to run only one instance of the game at all times. 

It's also possible to sync the game manually. To do so, navigate to the <code>CLOUD</code> page inside the game's main menu and click on your email. The game will briefly show "Loading" window which will indicate that cloud sync was successful. In case of an error, check the network.
