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
    private int _floorCostDecrease;

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
        ExitSettingsButton.Click += ExitSettingsButton_Click;
        SaveSettingsButton.Click += SaveSettingsButton_Click;

        TextBoxCurrentFloor.Text = _configService.Config.CurrentFloor.ToString();
        TextBoxFloorToRebuildAt.Text = _configService.Config.RebuildAtFloor.ToString();
        TextBoxWatchAdsFrom.Text = _configService.Config.WatchAdsFromFloor.ToString();
        CheckboxWatchBuxAds.IsChecked = _configService.Config.WatchBuxAds;
        CheckboxVipPackage.IsChecked = _configService.Config.VipPackage;
        BuildFloors.IsChecked = _configService.Config.BuildFloors;
        FloorCostDecreaseTextBox.Text = _configService.Config.FloorCostDecrease.ToString();

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
        if (ValidateNumberField(TextBoxFloorToRebuildAt))
        {
            _rebuildAtFloor = int.Parse(TextBoxFloorToRebuildAt.Text);
        }
    }

    private void TextBoxWatchAdsFrom_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ValidateNumberField(TextBoxWatchAdsFrom))
        {
            _watchAdsFromFloor = int.Parse(TextBoxWatchAdsFrom.Text);
        }
    }

    private void TextBoxCurrentFloor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ValidateNumberField(TextBoxCurrentFloor))
        {
            _currentFloor = int.Parse(TextBoxCurrentFloor.Text);
        }
    }

    private bool ValidateNumberField(TextBox textBox)
    {
        var parsed = int.TryParse(textBox.Text, out _);
        MainWindow?.Log(!parsed ? "Invalid input value" : $"{textBox.Name} value set");

        return parsed;
    }

    private void ExitSettingsButton_Click(object sender, RoutedEventArgs e)
    {
        if (MainWindow == null)
        {
            throw new InvalidOperationException("Main window is null");
        }

        MainWindow.SettingsOpened = false;
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
            MainWindow.IsBluestacks,
            0,
            _floorCostDecrease);

        _configService.SaveConfig(config);
        MainWindow?.Log("Saved settings");
    }

    private static string GetVersionInfo()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;

        return $"v{version?.Major}.{version?.Minor}";
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

    private void FloorCostDecreaseTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var parsed = int.TryParse(FloorCostDecreaseTextBox.Text, out var value);

        if (!parsed || ( value < 0 || value > 10))
        {
            MainWindow!.Log("Value should be between 0 and 10");
        }
        else
        {
            MainWindow!.Log($"{FloorCostDecreaseTextBox.Name} value set");
            _floorCostDecrease = value;
        }
    }
}
