using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.Graphics;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace AndroidScreenMirror
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly SolidColorBrush STATUS_COLOR = new (Color.FromArgb(255, 255, 255, 255));
        private readonly SolidColorBrush ERROR_COLOR = new (Color.FromArgb(255, 246, 99, 109));

        private readonly ScreenCopier _screenCopier;

        public MainWindow()
        {
            this.InitializeComponent();
            _screenCopier = ScreenCopier.Create();

            ConfigureWindow();
        }

        private async void MirrorBtn_Click(object _, RoutedEventArgs e)
        {
            try
            {
                ToggleBtns(false);
                LogStatus("Started mirroring the screen.");

                var processStatus = await _screenCopier.RunAsync();

                if (!string.IsNullOrWhiteSpace(processStatus.Error))
                    LogError(processStatus.Error);
                else
                    LogStatus("Finished screen mirroring.");

            } catch (Exception ex)
            {
                LogError(ex.Message);
            } finally
            {
                ToggleBtns(true);
            }
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            _screenCopier.Dispose();
        }

        private void ConfigureWindow()
        {
            AppWindow.Resize(new SizeInt32(950, 500));

            var presenter = AppWindow.Presenter as OverlappedPresenter;
            presenter.IsResizable = false;
            presenter.IsMaximizable = false;
        }

        private void LogStatus(string text)
        {
            statusTextBlock.Foreground = STATUS_COLOR;
            statusTextBlock.Text = text;
        }

        private void LogError(string error)
        {
            statusTextBlock.Foreground = ERROR_COLOR;
            statusTextBlock.Text = error;
        }

        private void ToggleBtns(bool enableMirrorBtn)
        {
            mirrorBtn.IsEnabled = enableMirrorBtn;
        }
    }
}
