using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using video.services;
using video.services.Services;

namespace video.Views
{
    public partial class PlayerView : Window
    {
        private PlayerViewModel VM => (PlayerViewModel)DataContext;
        private readonly DispatcherTimer _timer = new()
        {
            Interval = TimeSpan.FromMilliseconds(200)
        };


        public PlayerView()
        {
            InitializeComponent();

            var dialogService = new MediaDialogService();
            var playerService = new MediaPlayerService(Player);
            var metadataService = new VideoMetadataService();

            DataContext = new PlayerViewModel(
                dialogService,
                playerService,
                metadataService
            );


            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private bool _wasPlayingBeforeDrag;

        private void SetSliderValueFromMouse(MouseEventArgs e)
        {
            var pos = e.GetPosition(ProgressSlider).X;
            var ratio = pos / ProgressSlider.ActualWidth;
            ratio = Math.Clamp(ratio, 0, 1);

            ProgressSlider.Value =
                ratio * ProgressSlider.Maximum;
        }

        // Evento de mouse para o slider de progresso
        private void ProgressSlider_MouseDown(object sender, MouseButtonEventArgs e)
        {

            _wasPlayingBeforeDrag = VM.IsPlaying;
            VM.IsUserDraggingSlider = true;

            ProgressSlider.CaptureMouse();

            SetSliderValueFromMouse(e);
            VM.Seek(TimeSpan.FromSeconds(ProgressSlider.Value));

            if (_wasPlayingBeforeDrag)
                VM.PlayPauseCommand.Execute(null);
        }

        private void ProgressSlider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            VM.IsUserDraggingSlider = false;
            ProgressSlider.ReleaseMouseCapture();

            SetSliderValueFromMouse(e);
            VM.Seek(TimeSpan.FromSeconds(ProgressSlider.Value));

            if (_wasPlayingBeforeDrag)
                VM.PlayPauseCommand.Execute(null);
        }

        private void ProgressSlider_MouseMove(object sender, MouseEventArgs e)
        {
            
            if (!VM.IsUserDraggingSlider)
                return;

            SetSliderValueFromMouse(e);
            VM.Seek(TimeSpan.FromSeconds(ProgressSlider.Value));

        }

        // Timer para atualizar o slider de progresso
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (VM.IsUserDraggingSlider)
                return;

            if (Player.Source == null)
                return;

            if (!Player.NaturalDuration.HasTimeSpan)
                return;

            ProgressSlider.Maximum =
                Player.NaturalDuration.TimeSpan.TotalSeconds;

            ProgressSlider.Value =
                Player.Position.TotalSeconds;
        }


    }
}
