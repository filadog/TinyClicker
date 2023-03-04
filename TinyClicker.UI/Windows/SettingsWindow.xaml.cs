using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TinyClicker.Core.Logic;

namespace TinyClicker.UI;

public partial class SettingsWindow : Window
{
    public MainWindow? MainWindow { get; private set; }

    private readonly ConfigManager _configManager;
    private float _elevatorSpeed = 10f;
    private int _currentFloor;
    private int _rebuildAtFloor;
    private int _watchAdsFromFloor;
    private bool _watchBuxAds;
    private bool _vipPackage = true;
    private DateTime _lastRebuildTime;
    private bool _buildFloors;
    private DateTime _lastRaffleTime;

    public SettingsWindow(ConfigManager configManager)
    {
        _configManager = configManager;
    }

    public void Show(MainWindow mainWindow)
    {
        if (mainWindow is not null)
        {
            MainWindow = mainWindow;
            Owner = mainWindow;

            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            InitializeComponent();
            InitFields();
            Show();
        }
    }

    private void InitFields()
    {
        TextBoxCurrentFloor.Text = _configManager.CurrentConfig.CurrentFloor.ToString();
        TextBoxFloorToRebuildAt.Text = _configManager.CurrentConfig.RebuildAtFloor.ToString();
        TextBoxWatchAdsFrom.Text = _configManager.CurrentConfig.WatchAdsFromFloor.ToString();
        CheckboxWatchBuxAds.IsChecked = _configManager.CurrentConfig.WatchBuxAds ? true : false;
        CheckboxVipPackage.IsChecked = _configManager.CurrentConfig.VipPackage ? true : false;
        cbBuildFloors.IsChecked = _configManager.CurrentConfig.BuildFloors ? true : false;

        _currentFloor = _configManager.CurrentConfig.CurrentFloor;
        _rebuildAtFloor = _configManager.CurrentConfig.RebuildAtFloor;
        _watchAdsFromFloor = _configManager.CurrentConfig.WatchAdsFromFloor;
        _watchBuxAds = _configManager.CurrentConfig.WatchBuxAds;
        _lastRebuildTime = _configManager.CurrentConfig.LastRebuildTime;
        _buildFloors = _configManager.CurrentConfig.BuildFloors;
        _lastRaffleTime = _configManager.CurrentConfig.LastRaffleTime;

        VersionText.Text = GetVersionInfo();
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
                MainWindow!.Log("Invalid input value");
                _rebuildAtFloor = _configManager.CurrentConfig.RebuildAtFloor;
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
                MainWindow!.Log("Invalid input value");
                _watchAdsFromFloor = _configManager.CurrentConfig.WatchAdsFromFloor;
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
                MainWindow!.Log("Invalid input value");
                _currentFloor = _configManager.CurrentConfig.CurrentFloor;
            }
        }
    }

    private void ExitSettingsButton_Click(object sender, RoutedEventArgs e)
    {
        if (MainWindow is not null)
        {
            MainWindow._settingsOpened = false;
            Hide();
        }
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
        var config = new Config(_vipPackage, _elevatorSpeed, _currentFloor, _rebuildAtFloor, _watchAdsFromFloor, _watchBuxAds, _lastRebuildTime, cbBuildFloors.IsChecked.Value, _lastRaffleTime);
        _configManager.SaveConfig(config);
    }

    private string GetVersionInfo()
    {
        return $"v{Assembly.GetExecutingAssembly().GetName().Version.Major}.{Assembly.GetExecutingAssembly().GetName().Version.Minor}";
    }
}
