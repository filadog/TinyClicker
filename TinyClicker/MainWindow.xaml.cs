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

            Clicker.Start();
            
            //Parallel.Invoke(() => Clicker.StartClicker());
            //Clicker clicker = new Clicker();

            //clicker.Start();
            //await Task.Run(async () => { await Clicker.StartClicker(); });
            //Task task = Task.Run((Action)Clicker.StartClicker());
            

            //await Task.Run(() => Clicker.StartClicker);


            //Task t1 = new(Clicker.StartClicker().Start);
            //t1.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Print("Stopped!");
            StartButton.Visibility = Visibility.Visible;
            StartedImage.Visibility = Visibility.Hidden;
            StopButton.Visibility= Visibility.Hidden;
            ExitButton.Visibility= Visibility.Visible;
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
