using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.ComponentModel;

namespace TinyClicker;

public partial class MainWindow : Window
{
    private readonly BackgroundWorker _worker = new();

    public MainWindow()
    {
        InitializeComponent();
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
        bool isSelected = TinyClickerApp.IsEmulatorSelected();
        if (isSelected)
        {
            TinyClickerApp.stopped = false;
            _worker.WorkerSupportsCancellation = true;
            TinyClickerApp.StartInBackground(_worker);
            Log("Started!");
            ShowStartedButton();
        }
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        _worker.CancelAsync();
        Log("Stopped!");
        ShowExitButton();
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    public void Log(string msg)
    {
        Dispatcher.Invoke(() =>
        {
            TextBoxLog.Text = msg;
        });
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
            TinyClickerApp.isBluestacks = true;
        }
    }

    private void IsLDPlayerCheckboxChecked(object sender, RoutedEventArgs e)
    {
        BlueStacksCheckbox.IsHitTestVisible = false;
        BlueStacksCheckbox.Focusable = false;
        TinyClickerApp.isLDPlayer = true;
    }

    private void BlueStacksCheckbox_Unchecked(object sender, RoutedEventArgs e)
    {
        LDPlayerCheckbox.IsHitTestVisible = true;
        LDPlayerCheckbox.Focusable = true;
        TinyClickerApp.isBluestacks = false;
    }

    private void LDPlayerCheckbox_Unchecked(object sender, RoutedEventArgs e)
    {
        BlueStacksCheckbox.IsHitTestVisible = true;
        BlueStacksCheckbox.Focusable = true;
        TinyClickerApp.isLDPlayer = false;
    }
}
