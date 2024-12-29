## TinyClicker
[![Tests](https://github.com/filadog/TinyClicker/actions/workflows/run_tests.yml/badge.svg?branch=master)](https://github.com/filadog/TinyClicker/actions/workflows/run_tests.yml)
#### Game version: v.5.0.0-391 - _10.03.2024_


## Состояние проекта
Скорее всего TinyClicker уже не будет получать обновлений от меня в будущем. \
После того как в игре стало невыносимо много рекламы и микротранзакций, в нее стало сложно играть.

Исходный код остается доступен под лицензией MIT, можете использовать его целиком или частично для своих проектов. \
Модули работы с распознаванием текста или изображений (Tesseract и OpenCvSharp) могут оказаться полезны в похожих .NET проектах.


## Current project state
TinyClicker will most likely not receive any updates from me in the future. \
I stopped playing due to the devs making too much in-the-face ads and microtransactions. 

Feel free to fork the repository or reuse the code. The repository is under MIT license. \
Some services, such as image or text recognition (OpenCvSharp and Tesseract) might prove helpful in similar .NET projects.


### [English full readme](#tinyclicker---english)

Автокликер для мобильной игры Tiny Tower, созданный для сбора золотых билетов и автоматизации рутины в игре.\
Кликер использует распознавание текста и изображений для полной автоматизации постройки и перестройки вашего небоскреба при запуске игры в эмуляторе Android.

## 

![tinyclicker_png2](https://user-images.githubusercontent.com/51026900/174921574-5e6b74a7-d3b1-4d8e-a21f-95d65431b792.png)

##

#### Платформа: Windows 7 (x86/x64) или более поздняя
### Поддерживаемые эмуляторы:
- LDPlayer 4.X (поддержка прекращена, по возможности используйте Bluestacks)
- BlueStacks 5.X

## Основной функционал

- 100% автономная работа * 
- Кликер не требует взаимодействия и не крадет курсор во время работы
- Перестройка небоскреба на любом этаже и автоматическое прохождение туториала
- Автоматический просмотр рекламы с целью максимизации прибыли в монетах и баксах
- Автоматическое участие в ежечасном розыгрыше золотых билетов (hourly raffle)
- Простая конфигурация
- Статистика по перестроенным небоскребам
- Сбор очков для исследований

**на данный момент только с приобретенным VIP пакетом*

![](https://github.com/filadog/TinyClicker/blob/master/gif.gif)


## До запуска

Убедитесь, что запущен только один экземпляр эмулятора (Bluestacks или LDPlayer). Не стройте новые этажи, если кликер уже запущен.
Окно с эмулятором может быть накрыто любым другим окном на вашем рабочем столе, но оно не должно быть вынесено за рамки экрана или свернуто.
Взаимодействия с игрой во время работы кликера возможны, но не рекомендуются.

По умолчанию небоскреб будет перестроен как только кликером будет куплен 50й этаж. Если у вас уже построено больше этажей, убедитесь что ваши дорогие жильцы переселены в другие небоскребы, так как после перестойки они будут потеряны, или установите этаж, на котором необходимо перестраивать небоскреб на большее значение в конфигурации.

После первой перестройки небоскреба в корневой папке кликера будет создан текстовый файл Stats.txt, который будет содержать информацию о последней перестройке. Последущие подобные события также будут добавлены в этот файл для статистики.

#### Кликер находится в разработке, так что отсутствие багов не гарантируется. В случае возникновения проблем прошу заводить issue с ее описанием.


## Настройка

#### LDPlayer
- До запуск кликера откройте LDPlayer и перейдите в настройки. Установите разрешение экрана в режим Customize, с <code>шириной = 333</code>, <code>высотой = 592</code>, <code>DPI = 150</code>.

#### BlueStacks
- Перейдите в настройки и создайте новое кастомное разрешение экрана. Установите <code>ширину на 666 px</code> и <code>высоту на 1184 px</code>, <code>DPI на 240 или 320</code>. Сохраните и примените настройки. Окно эмулятора можно перемещать и менять размер, важно не менять отношение высоты и ширины экрана, так как это критично для работы кликера. Не меняйте размер экрана, если кликер уже запущен.

#### Общая настройка
- Запустите кликер, перейдите в окно настроек (шестеренка) и установите в поле <code>Current floor</code> значение на последний построенный в игре этаж (целое число от 1, например 43). В поле <code>Rebuild at</code> укажите этаж, на котором должна происходить перестройка небоскреба (по умочанию на 50м этаже). В поле <code>Watch Ads From</code> устанавливается номер этажа, после которого кликер будет смотреть рекламу, предлагаемую в игре, так как она становится эффективнее на более высоких этажах. Это значение зависит от количества золотых билетов, которые у вас уже есть. Примерные значения: 100 билетов - 25й этаж, 200 - 35й и так далее до 50го этажа. Если у вас больше 500 золотых билетов то реклама будет только тратить время. Не отмеченное поле <code>Watch Bux Ads</code> заставит кликер отказаться от просмотра рекламы за баксы, так как, в общем, награда за просмотр рекламы не стоит потраченного времени на таком уровне.

- Настоятельно рекомендуется отключать браузеры и Play Market на устройстве в эмуляторе, так как некоторые виды рекламы будут пытаться открыть эти приложения автоматически, что может привести к ошибкам работы кликера, который не умеет ориентироваться в других приложениях и не сможет продолжить.

- Необходимо переключить регулярные подарки с баксами, которые появляются каждые 4-5 минут в режим, в котором предлагаются только баксы, которые можно забрать по нажатию одной кнопки. Кликер будет их собирать.

- Когда все предыдущие настройки установлены, нажмите Save и затем Exit в окне настроек, выберите эмулятор в главном окне кликера, запустите игру на эмулируемом устройстве и нажмите Start в окне кликера. Ваши настройки будут сохранены и текущий этаж будет автоматически отслеживаться кликером.
При последующих запусках просто выберите эмулятор и запустите кликер.

## Как обновить

Загрузите [архив с последним релизом](https://github.com/stereostas/TinyClicker/releases).\
Перенесите содержимое архива в корневую папку кликера с заменой всех файлов.

## Справка

### Сбор большого количества монет без постройки этажей

Для этого в настройках кликера потребуется убрать галочку с пункта <code>Build Floors</code>. Будет отключено строительство новых этажей и перестройка башни.

### Для игроков на iOS

Если у вас есть ПК с Windows, вы можете перенести ваш прогресс в игре на любой эмулятор Android на ПК, с сохранением VIP пакета.

### Облачная синхронизация в игре

Не запускайте несколько экземпляров одной и той же игры на разных устройствах одновремененно, функция автоматической синхронизации в игре может привести к утрате прогресса, так как периодически игра не может определиться с тем, какая из версий действительно является актуальной.

<br>
<br>
<br>
<br>

## TinyClicker - English

A simple background autoclicker for the Tiny Tower game tailored for the greedy Golden Tickets collectors.\
TinyClicker implements image pattern matching and OCR to help you automate the grind of rebuilding your Tiny Tower.

## 

![tinyclicker_png2](https://user-images.githubusercontent.com/51026900/174921574-5e6b74a7-d3b1-4d8e-a21f-95d65431b792.png)

##

#### Platform: Windows 7 (x86/x64) or later versions
### Supported Emulators:
- LDPlayer 4.X (being deprecated, use Bluestacks if possible)
- BlueStacks 5.X

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

#### The program is in the phase of development and some bugs may occur, feel free to submit an issue in case help is required.

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

## How to update

Download the [latest release from here](https://github.com/stereostas/TinyClicker/releases).\
Extract archive contents inside existing clicker folder with replacement of all files.

## Information

### Collecting coins for upgrades with science points

If you wish to collect a big amount of coins without TinyClicker trying to spend them, uncheck the <code>Build Floors</code> setting and make sure <code>Current floor</code> value is lower than <code>Rebuild at</code> value. 
Science upgrades are not automated and should be performed manually, TinyClicker will collect science points for you though.


### For the iOS players

If you have a Windows PC it is possible to use Tiny Tower cloud save inside the Android version of Tiny Tower inside android emulator of your choice, VIP package will be transformed as well. 


### Cloud sync

Avoid launching the game on a mobile device when it's already launched inside emulator somewhere else. In-game cloud sync may confuse the most recent version of the game and some progress can be lost. In order to avoid this make sure to run only one instance of the game at all times. 

It's also possible to sync the game manually. To do so, navigate to the <code>CLOUD</code> page inside the game's main menu and click on your email. The game will briefly show "Loading" window which will indicate that cloud sync was successful. In case of an error, check the network.

