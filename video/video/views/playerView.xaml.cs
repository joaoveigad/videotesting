using System.Windows;
using System.Windows.Threading;
using video.services;
using video.services.Services;

namespace video.Views
{
    /// <summary>
    /// Interaction logic for playerView.xaml
    /// </summary>
    public partial class PlayerView : Window
    {

        // STATE
        private bool IsUserDraggingSlider = false;

        // SERVICES / HELPERS

        private readonly DispatcherTimer Timer = new()
        {
            Interval = TimeSpan.FromSeconds(0.1)
        };

        // CONSTRUCTOR

        public PlayerView()
        {
            InitializeComponent();
            var dialogService = new MediaDialogService();
            var playerService = new MediaPlayerService(Player);
            var metadataService = new VideoMetadataService();

            DataContext = new PlayerViewModel(dialogService,playerService, metadataService);

            Timer.Tick += Timer_Tick;
            Timer.Start();
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
