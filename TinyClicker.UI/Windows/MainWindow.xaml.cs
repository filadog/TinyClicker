using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.ComponentModel;
using TinyClicker.Core.Logic;
using TinyClicker.Core.Logging;
using TinyClicker.Core.Services;

namespace TinyClicker.UI;

public partial class MainWindow : Window, IMainWindow
{
    private readonly BackgroundWorker _backgroundWorker;
    private readonly SettingsWindow _settingsWindow;
    private readonly TinyClickerApp _tinyClickerApp;
    private readonly IConfigService _configService;
    private readonly ILogger _logger;

    public bool _isBluestacks = false;
    public bool _isLDPlayer = false;
    public bool _settingsOpened = false;

    public MainWindow(
        BackgroundWorker backgroundWorker,
        SettingsWindow settingsWindow,
        TinyClickerApp tinyClickerApp,
        IConfigService configService,
        ILogger logger)
    {
        _backgroundWorker = backgroundWorker;
        _settingsWindow = settingsWindow;
        _tinyClickerApp = tinyClickerApp;
        _configService = configService;
        _logger = logger;

        _logger.SetMainWindow(this);

        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        InitializeComponent();
    }

    private void Startup()
    {
        if (_isLDPlayer ^ _isBluestacks)
        {
            _configService.Config.IsBluestacks = _isBluestacks;
            _tinyClickerApp.StartInBackground();

            Log("Started!");
            ShowStartedButton();
            HideCheckboxes();
        }
        else
        {
            Log("Error: Select the emulator");
        }
    }

    private void MainWindowMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        DisableSettingsButton();
        Startup();
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        _backgroundWorker.CancelAsync();
        Log("Stopped!");
        ShowExitButton();
        ShowCheckboxes();
        EnableSettingsButton();
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void DisableSettingsButton()
    {
        SettingsButton.IsEnabled = false;
    }
    private void EnableSettingsButton()
    {
        SettingsButton.IsEnabled = true;
    }

    public void Log(string msg)
    {
        Dispatcher.Invoke(() =>
        {
            TextBoxLog.Text = msg;

            // todo add logging enabled parameter
            //msg += "\n";
            //var time = DateTime.Now.ToString();
            //File.AppendAllText(@"./log.txt", time + " " + msg);
        });
    }

    private void HideCheckboxes()
    {
        BlueStacksCheckbox.IsEnabled = false;
        LDPlayerCheckbox.IsEnabled = false;
    }

    private void ShowCheckboxes()
    {
        BlueStacksCheckbox.IsEnabled = true;
        LDPlayerCheckbox.IsEnabled = true;
    }

    private void ShowExitButton()
    {
        StartButton.Visibility = Visibility.Visible;
        StartedImage.Visibility = Visibility.Hidden;
        StopButton.Visibility = Visibility.Hidden;
        ExitButton.Visibility = Visibility.Visible;
    }

    private void ShowStartedButton()
    {
        StartButton.Visibility = Visibility.Hidden;
        StartedImage.Visibility = Visibility.Visible;
        ExitButton.Visibility = Visibility.Hidden;
        StopButton.Visibility = Visibility.Visible;
    }


    private void IsBluestacksCheckboxChecked(object sender, RoutedEventArgs e)
    {
        if (LDPlayerCheckbox != null)
        {
            LDPlayerCheckbox.IsHitTestVisible = false;
            LDPlayerCheckbox.Focusable = false;
            _isBluestacks = true;
        }
    }

    private void IsLDPlayerCheckboxChecked(object sender, RoutedEventArgs e)
    {
        BlueStacksCheckbox.IsHitTestVisible = false;
        BlueStacksCheckbox.Focusable = false;
        _isLDPlayer = true;
    }

    private void BlueStacksCheckbox_Unchecked(object sender, RoutedEventArgs e)
    {
        LDPlayerCheckbox.IsHitTestVisible = true;
        LDPlayerCheckbox.Focusable = true;
        _isBluestacks = false;
    }

    private void LDPlayerCheckbox_Unchecked(object sender, RoutedEventArgs e)
    {
        BlueStacksCheckbox.IsHitTestVisible = true;
        BlueStacksCheckbox.Focusable = true;
        _isLDPlayer = false;
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_settingsOpened)
        {
            _settingsOpened = true;
            _settingsWindow.Show(this);
        }
        else
        {
            _settingsOpened = false;
            _settingsWindow!.Hide();
        }
    }
}
