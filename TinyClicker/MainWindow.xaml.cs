using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.ComponentModel;

namespace TinyClickerUI
{
    public partial class MainWindow : Window
    {
        private BackgroundWorker worker = new BackgroundWorker();

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
            TinyClicker.stopped = false;
            worker.WorkerSupportsCancellation = true;
            TinyClicker.StartInBackground(worker);
            
            Print("Started!");
            ShowStartedButton();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            worker.CancelAsync();
            Print("Stopped!");
            ShowExitButton();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void Print(string msg)
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
    }
}
