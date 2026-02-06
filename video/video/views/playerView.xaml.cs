using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
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

        private readonly DispatcherTimer _hideCursorTimer = new()  // Timer para esconder o cursor, não implementado ainda.
        {
            Interval = TimeSpan.FromSeconds(2)
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

            _hideCursorTimer.Tick += HideCursorTimer_Tick;
        }

        // Variáveis de estado
        private bool _wasPlayingBeforeDrag;
        private bool _isFullscreen;
        private bool _isUIHidden;

        private WindowState _prevState;
        private WindowStyle _prevStyle;
        private ResizeMode _prevResize;

        private Point _lastMousePosition;

        // Método para definir o valor do slider com base na posição do mouse
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

        private void HideCursorTimer_Tick(object? sender, EventArgs e)
        {
            if (_isFullscreen && !_isUIHidden)
            {
                HideUi();
            }

            _hideCursorTimer.Stop();
        }

        private void ToggleFullscreen()
        {
            if (!_isFullscreen)
            {
                _prevState = WindowState;
                _prevStyle = WindowStyle;
                _prevResize = ResizeMode;

                WindowStyle = WindowStyle.None;
                ResizeMode = ResizeMode.NoResize;
                WindowState = WindowState.Maximized;

                _isFullscreen = true;

                _hideCursorTimer.Stop();
                _hideCursorTimer.Start();
            }
            else
            {
                WindowStyle = _prevStyle;
                ResizeMode = _prevResize;
                WindowState = _prevState;

                _isFullscreen = false;

                ShowUi();
            }
        }

        private void ShowUi()
        {
            TopBar.Visibility = Visibility.Visible;
            BottomBar.Visibility = Visibility.Visible;
            _isUIHidden = false;
            Mouse.OverrideCursor = null;
        }

        private void HideUi()
        {
            TopBar.Visibility = Visibility.Collapsed;
            BottomBar.Visibility = Visibility.Collapsed;
            _isUIHidden = true;
            Mouse.OverrideCursor = Cursors.None;
        }

        // interações do usuário T&M
        private void Window_LeftMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!VM.IsPlaying)
                return;

            ToggleFullscreen();
        }

        private void Fullscreen_Click(object sender, RoutedEventArgs e)
        {
            if (!VM.IsPlaying)
                return;

            ToggleFullscreen();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && _isFullscreen)
            {
                ToggleFullscreen();
                return;
            }

            if (e.Key == Key.Space)
            {
                VM.PlayPauseCommand.Execute(null);
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Delete)
            {
                ToggleFullscreen();
                return;
            }
        }

        private void OnMouseMoved(object sender, MouseEventArgs e)
        {
            if (!_isFullscreen)
                return;

            var currentPos = e.GetPosition(this);

            if (currentPos == _lastMousePosition)
                return;

            _lastMousePosition = currentPos;

            if (_isUIHidden)
                ShowUi();

            _hideCursorTimer.Stop();
            _hideCursorTimer.Start();
        }
    }
}
