#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif

namespace BrailleDevTools
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            Window window = base.CreateWindow(activationState);
            window.Activated += Window_Activated;
            return window;
        }

        private async void Window_Activated(object sender, EventArgs e)
        {
        #if WINDOWS
            var window = sender as Window;

            const int DefaultWidth = 1024;
            const int DefaultHeight = 800;

            // 變更視窗大小
            window.Width = DefaultWidth;
            window.Height = DefaultHeight;

            // 給它一點時間來把「變更視窗大小」的操作確實完成。
            await window.Dispatcher.DispatchAsync(() => { });

            // 取得主要顯示器的資訊
            var disp = DeviceDisplay.Current.MainDisplayInfo;

            // 將視窗置於螢幕中央
            window.X = (disp.Width / disp.Density - window.Width) / 2;
            window.Y = (disp.Height / disp.Density - window.Height) / 2;
        #endif
        }
    }
}

