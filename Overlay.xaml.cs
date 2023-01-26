using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Web.WebView2.Core;

namespace SpotifyOverlay
{
    public partial class Overlay : Window
    {
        private const int HotkeyId = 9000;
        //Modifiers:
        private const uint ModNone = 0x0000; //[NONE]
        private const uint ModAlt = 0x0001; //ALT
        private const uint ModControl = 0x0002; //CTRL
        private const uint ModShift = 0x0004; //SHIFT
        private const uint ModWin = 0x0008; //WINDOWS
        //Keys
        private const int VkS = 0x053; // S
        private HwndSource? _source;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
 
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            
            var handle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(handle)!;
            _source.AddHook(HwndHook);

            RegisterHotKey(handle, HotkeyId, ModAlt, VkS);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int wmHotkey = 0x0312; 
            var vkey = (((int)lParam >> 16) & 0xFFFF);
            switch (msg)
            {
                case wmHotkey:
                switch (wParam.ToInt32())
                {
                    case HotkeyId:
                        if (vkey == VkS)
                        {
                           SwitchVisibility();
                        }
                        handled = true;
                        break;
                }
                break;
            }
            return IntPtr.Zero;
        }

        public static readonly RoutedCommand ExitCommand = new RoutedCommand();
        private bool _visible = false;
        
        public Overlay()
        {
            ExitCommand.InputGestures.Add(new KeyGesture(Key.Escape));
            InitializeComponent();
        }

        private void ExitCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SwitchVisibility();
        }

        private void WebviewInitialized(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            var settings = SpotifyWebview.CoreWebView2.Settings;
            settings.IsStatusBarEnabled = false;
            settings.AreDevToolsEnabled = false;
            settings.AreDefaultContextMenusEnabled = false;

        }
        
        private void SwitchVisibility()
        {
            _visible = !_visible;
            this.WindowState = _visible ? WindowState.Maximized : WindowState.Normal;
            this.Topmost = _visible;
        }
    }
}