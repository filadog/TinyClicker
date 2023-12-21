using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TinyClicker.Core.Logic;
using TinyClicker.Core.Services;
using TinyClicker.UI.ViewModels;

namespace TinyClicker.UI.Windows;

public partial class SettingsWindow
{
    private const float ELEVATOR_SPEED = 10f;
    private const bool VIP_PACKAGE = true;

    private readonly IUserConfiguration _userConfiguration;
    private readonly UserSettingsValidator _validator;
    private readonly UserSettingsViewModel _userSettings = new();

    private MainWindow? _mainWindow;

    public SettingsWindow(
        IUserConfiguration userConfiguration,
        UserSettingsValidator validator)
    {
        _userConfiguration = userConfiguration;
        _validator = validator;
    }

    public void Show(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        Owner = mainWindow;

        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        InitializeComponent();
        AddEventHandlers();
        InitFields();
        Show();
    }

    private void AddEventHandlers()
    {
        MouseDown += SettingsWindow_MouseDown;

        ButtonExitSettings.Click += ExitSettings;
        ButtonSaveSettings.Click += SaveSettings;

        TextBoxCurrentFloor.TextChanged += TextBox_CurrentFloor_TextChanged;
        TextBoxFloorToRebuildAt.TextChanged += TextBox_FloorToRebuildAt_TextChanged;
        TextBoxWatchAdsFrom.TextChanged += TextBoxWatch_AdsFrom_TextChanged;
        TextBoxFloorCostDecrease.TextChanged += TextBox_FloorCostDecrease_TextChanged;

        CheckBoxWatchBuxAds.Checked += CheckBox_WatchBuxAds_Checked;
        CheckBoxWatchBuxAds.Unchecked += Checkbox_WatchBuxAds_Unchecked;
        CheckBoxBuildFloors.Checked += CheckBox_BuildFloors_Checked;
        CheckBoxBuildFloors.Unchecked += CheckBox_BuildFloors_Unchecked;
    }

    private void InitFields()
    {
        TextBoxCurrentFloor.Text = _userConfiguration.Configuration.CurrentFloor.ToString();
        TextBoxFloorToRebuildAt.Text = _userConfiguration.Configuration.RebuildAtFloor.ToString();
        TextBoxWatchAdsFrom.Text = _userConfiguration.Configuration.WatchAdsFromFloor.ToString();
        TextBoxFloorCostDecrease.Text = _userConfiguration.Configuration.FloorCostDecrease.ToString();

        CheckBoxWatchBuxAds.IsChecked = _userConfiguration.Configuration.WatchBuxAds;
        CheckboxVipPackage.IsChecked = _userConfiguration.Configuration.VipPackage;
        CheckBoxBuildFloors.IsChecked = _userConfiguration.Configuration.BuildFloors;

        _userSettings.CurrentFloor = _userConfiguration.Configuration.CurrentFloor;
        _userSettings.RebuildAtFloor = _userConfiguration.Configuration.RebuildAtFloor;
        _userSettings.WatchAdsFromFloor = _userConfiguration.Configuration.WatchAdsFromFloor;
        _userSettings.WatchBuxAds = _userConfiguration.Configuration.WatchBuxAds;
        _userSettings.LastRebuildTime = _userConfiguration.Configuration.LastRebuildTime;
        _userSettings.BuildFloors = _userConfiguration.Configuration.BuildFloors;
        _userSettings.LastRaffleTime = _userConfiguration.Configuration.LastRaffleTime;

        TextBlockVersionText.Text = GetVersionInfo();
    }

    private void SettingsWindow_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void TextBox_FloorToRebuildAt_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ValidateNumberField(TextBoxFloorToRebuildAt))
        {
            _userSettings.RebuildAtFloor = int.Parse(TextBoxFloorToRebuildAt.Text);
        }
    }

    private void TextBoxWatch_AdsFrom_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ValidateNumberField(TextBoxWatchAdsFrom))
        {
            _userSettings.WatchAdsFromFloor = int.Parse(TextBoxWatchAdsFrom.Text);
        }
    }

    private void TextBox_CurrentFloor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ValidateNumberField(TextBoxCurrentFloor))
        {
            _userSettings.CurrentFloor = int.Parse(TextBoxCurrentFloor.Text);
        }
    }

    private bool ValidateNumberField(TextBox textBox)
    {
        var parsed = int.TryParse(textBox.Text, out _);
        _mainWindow?.Log(!parsed ? "Invalid input value" : $"{textBox.Name} value set");

        return parsed;
    }

    private void ExitSettings(object sender, RoutedEventArgs e)
    {
        if (_mainWindow == null)
        {
            throw new InvalidOperationException("Main window is null");
        }

        _mainWindow.SettingsOpened = false;
        Hide();
    }

    private void SaveSettings(object sender, RoutedEventArgs e)
    {
        if (_mainWindow == null)
        {
            throw new InvalidOperationException("Main window is null");
        }

        var config = new Configuration(
            VIP_PACKAGE,
            ELEVATOR_SPEED,
            _userSettings.CurrentFloor,
            _userSettings.RebuildAtFloor,
            _userSettings.WatchAdsFromFloor,
            _userSettings.WatchBuxAds,
            _userSettings.LastRebuildTime,
            _userSettings.BuildFloors,
            _userSettings.LastRaffleTime,
            _mainWindow.IsBluestacks,
            default,
            _userSettings.FloorCostDecreasePercent, 
            500); //todo do not forget to implement settings textbox

        _userConfiguration.SaveConfig(config);
        _mainWindow?.Log("Saved settings");
    }

    private static string GetVersionInfo()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;

        return $"v{version?.Major}.{version?.Minor}";
    }

    private void CheckBox_BuildFloors_Checked(object sender, RoutedEventArgs e)
    {
        _userSettings.BuildFloors = true;
    }

    private void CheckBox_BuildFloors_Unchecked(object sender, RoutedEventArgs e)
    {
        _userSettings.BuildFloors = false;
    }

    private void CheckBox_WatchBuxAds_Checked(object sender, RoutedEventArgs e)
    {
        _userSettings.WatchBuxAds = true;
    }

    private void Checkbox_WatchBuxAds_Unchecked(object sender, RoutedEventArgs e)
    {
        _userSettings.WatchBuxAds = false;
    }

    private void TextBox_FloorCostDecrease_TextChanged(object sender, TextChangedEventArgs e)
    {
        var parsed = int.TryParse(TextBoxFloorCostDecrease.Text, out var value);

        if (!parsed || value < 0 || value > 10)
        {
            _mainWindow!.Log("Value should be between 0 and 10");
        }
        else
        {
            _mainWindow!.Log($"{TextBoxFloorCostDecrease.Name} value set");
            _userSettings.FloorCostDecreasePercent = value;
        }
    }
}
