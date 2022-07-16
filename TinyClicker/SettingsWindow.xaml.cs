using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TinyClicker;

public partial class SettingsWindow : Window
{
    readonly ConfigManager _configManager;
    readonly MainWindow _mainWindow;

    private float _elevatorSpeed = 10f;
    private int _currentFloor;
    private int _rebuildAtFloor;
    private int _watchAdsFromFloor;
    private bool _watchBuxAds;
    private bool _vipPackage = true;
    private DateTime _lastRebuildTime;

    public SettingsWindow(MainWindow mainWindow)
    {
        InitializeComponent();
        _mainWindow = mainWindow;
        _configManager = new ConfigManager();
        TextBoxCurrentFloor.Text = _configManager.curConfig.CurrentFloor.ToString();
        TextBoxFloorToRebuildAt.Text = _configManager.curConfig.RebuildAtFloor.ToString();
        TextBoxWatchAdsFrom.Text = _configManager.curConfig.WatchAdsFromFloor.ToString();
        CheckboxWatchBuxAds.IsChecked = _configManager.curConfig.WatchBuxAds ? true : false;
        CheckboxVipPackage.IsChecked = _configManager.curConfig.VipPackage ? true : false;

        _currentFloor = _configManager.curConfig.CurrentFloor;
        _rebuildAtFloor = _configManager.curConfig.RebuildAtFloor;
        _watchAdsFromFloor = _configManager.curConfig.WatchAdsFromFloor;
        _watchBuxAds = _configManager.curConfig.WatchBuxAds;
        _lastRebuildTime = _configManager.curConfig.LastRebuildTime;
        VersionText.Text = $"v{Assembly.GetExecutingAssembly().GetName().Version.Major}.{Assembly.GetExecutingAssembly().GetName().Version.Minor}";
    }

    private void SettingsWindow_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }

    private void TextBoxFloorToRebuildAt_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_configManager != null)
        {
            try
            {
                string text = TextBoxFloorToRebuildAt.Text;
                int value = int.Parse(text);
                if (value != 0)
                {
                    _rebuildAtFloor = value;
                }
            }
            catch (FormatException)
            {
                _mainWindow.Log("Invalid input value");
                _rebuildAtFloor = _configManager.curConfig.RebuildAtFloor;
            }
        }
    }

    private void TextBoxWatchAdsFrom_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_configManager != null)
        {
            try
            {
                string text = TextBoxWatchAdsFrom.Text;
                int value = int.Parse(text);
                if (value != 0)
                {
                    _watchAdsFromFloor = value;
                }
            }
            catch (FormatException)
            {
                _mainWindow.Log("Invalid input value");
                _watchAdsFromFloor = _configManager.curConfig.WatchAdsFromFloor;
            }
        }
    }

    private void TextBoxCurrentFloor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_configManager != null)
        {
            try
            {
                string text = TextBoxCurrentFloor.Text;
                int value = int.Parse(text);
                if (value != 0)
                {
                    _currentFloor = value;
                }
            }
            catch (FormatException)
            {
                _mainWindow.Log("Invalid input value");
                _currentFloor = _configManager.curConfig.CurrentFloor;
            }
        }
    }

    private void ExitSettingsButton_Click(object sender, RoutedEventArgs e)
    {
        _mainWindow._settingsOpened = false;
        Close();
    }

    private void CheckboxWatchBuxAds_Checked(object sender, RoutedEventArgs e)
    {
        _watchBuxAds = true;
    }

    private void CheckboxWatchBuxAds_Unchecked(object sender, RoutedEventArgs e)
    {
        _watchBuxAds = false;
    }

    private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
    {
        var config = new Config(_vipPackage, _elevatorSpeed, _currentFloor, _rebuildAtFloor, _watchAdsFromFloor, _watchBuxAds, _lastRebuildTime);
        _configManager.SaveConfig(config);
    }
}
