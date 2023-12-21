using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TinyClicker.Core.Logging;
using TinyClicker.Core.Logic;
using TinyClicker.Core.Services;

namespace TinyClicker.UI.Windows;

public partial class MainWindow : IMainWindow
{
    private readonly BackgroundWorker _backgroundWorker;
    private readonly SettingsWindow _settingsWindow;
    private readonly TinyClickerApp _tinyClickerApp;
    private readonly IUserConfiguration _userConfiguration;

    private bool _isLDPlayer;

    public bool IsBluestacks;
    public bool SettingsOpened;

    public MainWindow(
        BackgroundWorker backgroundWorker,
        SettingsWindow settingsWindow,
        TinyClickerApp tinyClickerApp,
        IUserConfiguration userConfiguration,
        ILogger logger)
    {
        _backgroundWorker = backgroundWorker;
        _settingsWindow = settingsWindow;
        _tinyClickerApp = tinyClickerApp;
        _userConfiguration = userConfiguration;

        logger.SetMainWindow(this);

        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        InitializeComponent();
    }

    public void Log(string msg)
    {
        Dispatcher.Invoke(
            () =>
            {
                TextBoxLog.Text = msg;

                // todo add logging enabled parameter
                //msg += "\n";
                //var time = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                //File.AppendAllText(@"./log.txt", time + " " + msg);
            });
    }

    private void Startup()
    {
        if (_isLDPlayer ^ IsBluestacks)
        {
            _userConfiguration.SaveLastUsedEmulator(IsBluestacks);
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

    private void HideCheckboxes()
    {
        BlueStacksCheckbox.IsEnabled = false;
        LdPlayerCheckbox.IsEnabled = false;
    }

    private void ShowCheckboxes()
    {
        BlueStacksCheckbox.IsEnabled = true;
        LdPlayerCheckbox.IsEnabled = true;
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
        if (LdPlayerCheckbox == null)
        {
            return;
        }

        LdPlayerCheckbox.IsHitTestVisible = false;
        LdPlayerCheckbox.Focusable = false;
        IsBluestacks = true;
    }

    private void IsLDPlayerCheckboxChecked(object sender, RoutedEventArgs e)
    {
        BlueStacksCheckbox.IsHitTestVisible = false;
        BlueStacksCheckbox.Focusable = false;
        _isLDPlayer = true;
    }

    private void BlueStacksCheckbox_Unchecked(object sender, RoutedEventArgs e)
    {
        LdPlayerCheckbox.IsHitTestVisible = true;
        LdPlayerCheckbox.Focusable = true;
        IsBluestacks = false;
    }

    private void LDPlayerCheckbox_Unchecked(object sender, RoutedEventArgs e)
    {
        BlueStacksCheckbox.IsHitTestVisible = true;
        BlueStacksCheckbox.Focusable = true;
        _isLDPlayer = false;
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        if (!SettingsOpened)
        {
            SettingsOpened = true;
            _settingsWindow.Show(this);
        }
        else
        {
            SettingsOpened = false;
            _settingsWindow.Hide();
        }
    }
}
