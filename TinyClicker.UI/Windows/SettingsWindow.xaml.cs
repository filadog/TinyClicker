using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TinyClicker.Core.Logic;
using TinyClicker.Core.Services;

namespace TinyClicker.UI.Windows;

public partial class SettingsWindow
{
    private MainWindow? MainWindow { get; set; }

    private readonly IConfigService _configService;

    private const float ELEVATOR_SPEED = 10f;
    private const bool VIP_PACKAGE = true;

    private int _currentFloor;
    private int _rebuildAtFloor;
    private int _watchAdsFromFloor;
    private bool _watchBuxAds;
    private DateTime _lastRebuildTime;
    private bool _buildFloors;
    private DateTime _lastRaffleTime;

    public SettingsWindow(IConfigService configService)
    {
        _configService = configService;
    }

    public void Show(MainWindow mainWindow)
    {
        MainWindow = mainWindow;
        Owner = mainWindow;

        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        InitializeComponent();
        InitFields();
        Show();
    }

    private void InitFields()
    {
        TextBoxCurrentFloor.Text = _configService.Config.CurrentFloor.ToString();
        TextBoxFloorToRebuildAt.Text = _configService.Config.RebuildAtFloor.ToString();
        TextBoxWatchAdsFrom.Text = _configService.Config.WatchAdsFromFloor.ToString();
        CheckboxWatchBuxAds.IsChecked = _configService.Config.WatchBuxAds;
        CheckboxVipPackage.IsChecked = _configService.Config.VipPackage;
        BuildFloors.IsChecked = _configService.Config.BuildFloors;

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
        if (ValidateNumberField(TextBoxFloorToRebuildAt.Text))
        {
            _rebuildAtFloor = int.Parse(TextBoxFloorToRebuildAt.Text);
        }
    }

    private void TextBoxWatchAdsFrom_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ValidateNumberField(TextBoxWatchAdsFrom.Text))
        {
            _watchAdsFromFloor = int.Parse(TextBoxWatchAdsFrom.Text);
        }
    }

    private void TextBoxCurrentFloor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ValidateNumberField(TextBoxCurrentFloor.Text))
        {
            _currentFloor = int.Parse(TextBoxCurrentFloor.Text);
        }
    }

    private bool ValidateNumberField(string text)
    {
        try
        {
            var value = int.Parse(text);
            if (value != 0)
            {
                _currentFloor = value;
            }

            return true;
        }
        catch (FormatException)
        {
            MainWindow!.Log("Invalid input value");
            return false;
        }
    }

    private void ExitSettingsButton_Click(object sender, RoutedEventArgs e)
    {
        if (MainWindow == null)
        {
            return;
        }

        MainWindow._settingsOpened = false;
        Hide();
    }

    private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
    {
        if (MainWindow == null)
        {
            throw new InvalidOperationException("Main window is null");
        }

        var config = new Config(
            VIP_PACKAGE,
            ELEVATOR_SPEED,
            _currentFloor,
            _rebuildAtFloor,
            _watchAdsFromFloor,
            _watchBuxAds,
            _lastRebuildTime,
            _buildFloors,
            _lastRaffleTime,
            MainWindow._isBluestacks,
            0);

        _configService.SaveConfig(config);
    }

    private static string GetVersionInfo()
    {
        return $"v{Assembly.GetExecutingAssembly().GetName()?.Version?.Major}.{Assembly.GetExecutingAssembly().GetName()?.Version?.Minor}";
    }

    private void BuildFloors_OnChecked(object sender, RoutedEventArgs e)
    {
        _buildFloors = true;
    }

    private void BuildFloors_OnUnchecked(object sender, RoutedEventArgs e)
    {
        _buildFloors = false;
    }

    private void CheckboxWatchBuxAds_Checked(object sender, RoutedEventArgs e)
    {
        _watchBuxAds = true;
    }

    private void CheckboxWatchBuxAds_Unchecked(object sender, RoutedEventArgs e)
    {
        _watchBuxAds = false;
    }
}
