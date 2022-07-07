using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.ComponentModel;

namespace TinyClicker;

public partial class MainWindow : Window
{
    SettingsWindow? _settingsWindow;
    private readonly BackgroundWorker _worker = new();
    bool _isBluestacks = false;
    bool _isLDPlayer = false;
    public bool _settingsOpened = false;

    public MainWindow()
    {
        InitializeComponent();
    }

    void Startup()
    {
        if (_isLDPlayer || _isBluestacks)
        {
            var tinyClicker = new TinyClickerApp(_isBluestacks);
            tinyClicker.StartInBackground(_worker);

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
        _worker.CancelAsync();
        Log("Stopped!");
        ShowExitButton();
        ShowCheckboxes();
        EnableSettingsButton();
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    void DisableSettingsButton()
    {
        SettingsButton.IsEnabled = false;
    }
    void EnableSettingsButton()
    {
        SettingsButton.IsEnabled = true;
    }

    public void Log(string msg)
    {
        Dispatcher.BeginInvoke(() =>
        {
            TextBoxLog.Text = msg;
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
            var settingsWindow = new SettingsWindow(this);
            _settingsWindow = settingsWindow;
        }
        else
        {
            _settingsOpened = false;
            _settingsWindow.Close();
        }
    }
}
