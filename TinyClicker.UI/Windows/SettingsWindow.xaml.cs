using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TinyClicker.Core.Logic;
using TinyClicker.Core.Services;

namespace TinyClicker.UI;

public partial class SettingsWindow : Window
{
    public MainWindow? MainWindow { get; private set; }

    private readonly IConfigService _configService;

    private float _elevatorSpeed = 10f;
    private int _currentFloor;
    private int _rebuildAtFloor;
    private int _watchAdsFromFloor;
    private bool _watchBuxAds;
    private bool _vipPackage = true;
    private DateTime _lastRebuildTime;
    private bool _buildFloors;
    private DateTime _lastRaffleTime;

    public SettingsWindow(IConfigService configService)
    {
        _configService = configService;
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
        TextBoxCurrentFloor.Text = _configService.Config.CurrentFloor.ToString();
        TextBoxFloorToRebuildAt.Text = _configService.Config.RebuildAtFloor.ToString();
        TextBoxWatchAdsFrom.Text = _configService.Config.WatchAdsFromFloor.ToString();
        CheckboxWatchBuxAds.IsChecked = _configService.Config.WatchBuxAds ? true : false;
        CheckboxVipPackage.IsChecked = _configService.Config.VipPackage ? true : false;
        cbBuildFloors.IsChecked = _configService.Config.BuildFloors ? true : false;

        _currentFloor = _configService.Config.CurrentFloor;
        _rebuildAtFloor = _configService.Config.RebuildAtFloor;
        _watchAdsFromFloor = _configService.Config.WatchAdsFromFloor;
        _watchBuxAds = _configService.Config.WatchBuxAds;
        _lastRebuildTime = _configService.Config.LastRebuildTime;
        _buildFloors = _configService.Config.BuildFloors;
        _lastRaffleTime = _configService.Config.LastRaffleTime;

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
        if (_configService != null)
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
                _rebuildAtFloor = _configService.Config.RebuildAtFloor;
            }
        }
    }

    private void TextBoxWatchAdsFrom_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_configService != null)
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
                _watchAdsFromFloor = _configService.Config.WatchAdsFromFloor;
            }
        }
    }

    private void TextBoxCurrentFloor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_configService != null)
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
                _currentFloor = _configService.Config.CurrentFloor;
            }
        }
    }

    private void ExitSettingsButton_Click(object sender, RoutedEventArgs e)
    {
        if (MainWindow != null)
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
        var config = new Config(
            _vipPackage,
            _elevatorSpeed,
            _currentFloor,
            _rebuildAtFloor,
            _watchAdsFromFloor,
            _watchBuxAds,
            _lastRebuildTime,
            cbBuildFloors.IsChecked.Value,
            _lastRaffleTime,
            MainWindow._isBluestacks);

        _configService.SaveConfig(config);
    }

    private string GetVersionInfo()
    {
        return $"v{Assembly.GetExecutingAssembly().GetName().Version.Major}.{Assembly.GetExecutingAssembly().GetName().Version.Minor}";
    }
}
