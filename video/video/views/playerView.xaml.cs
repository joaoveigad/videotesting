using System;
using System.Windows;
using System.IO;
using System.Windows.Threading;
using System.Text;
using Microsoft.Win32;

namespace video.Views
{
    /// <summary>
    /// Interaction logic for playerView.xaml
    /// </summary>
    public partial class PlayerView : Window
    {

        // STATE

        private bool isPlaying = false;
        private bool IsUserDraggingSlider = false;

        // SERVICES / HELPERS

        private readonly DispatcherTimer Timer = new()
        {
            Interval = TimeSpan.FromSeconds(0.1)
        };

        private readonly OpenFileDialog MediaOpenDialog = new()
        {
            Title = "Open a media file",
            Filter = "Media Files (*.mp3,*.mp4)|*.mp3;*.mp4"
        };

        private void OpenBtn_click(object sender, RoutedEventArgs e)
        {
            if (MediaOpenDialog.ShowDialog() == true)
            {
                Player.Source = new Uri(MediaOpenDialog.FileName);
                //TitleLbl.Content = Path.GetFileName(MediaOpenDialog.FileName);

                Player.Play();
                isPlaying = true;
            }
        }


        // CONSTRUCTOR

        public PlayerView()
        {
            InitializeComponent();
            DataContext = new PlayerViewModel();

            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        // UI EVENTS
        private void TestBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Clicked here!");
        }
        // TIMER / PLAYER SYNC

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (Player.Source != null &&
                Player.NaturalDuration.HasTimeSpan &&
                !IsUserDraggingSlider)
            {
                ProgressSlider.Maximum =
                    Player.NaturalDuration.TimeSpan.TotalSeconds;

                ProgressSlider.Value =
                    Player.Position.TotalSeconds;
            }
        }


    }
}
