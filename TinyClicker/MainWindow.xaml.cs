using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.ComponentModel;

namespace TinyClickerUI
{
    public partial class MainWindow : Window
    {
        private readonly BackgroundWorker worker = new BackgroundWorker();

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
            Print("Started!");
            StartButton.Visibility = Visibility.Hidden;
            StartedImage.Visibility = Visibility.Visible;
            ExitButton.Visibility = Visibility.Hidden;
            StopButton.Visibility = Visibility.Visible;

            Clicker.stopped = false;
            Clicker.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Print("Stopped!");
            StartButton.Visibility = Visibility.Visible;
            StartedImage.Visibility = Visibility.Hidden;
            StopButton.Visibility= Visibility.Hidden;
            ExitButton.Visibility= Visibility.Visible;

            Clicker.stopped = true;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void Print(string msg)
        {
            TextBoxLog.Text = msg;
        }
    }
}
