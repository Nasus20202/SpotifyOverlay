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
        public static readonly RoutedCommand ExitCommand = new RoutedCommand();
        private KeyboardHandler _keyboardHandler;
        private bool _visible = false;
        
        public Overlay()
        {
            ExitCommand.InputGestures.Add(new KeyGesture(Key.Escape));
            
            _keyboardHandler = new KeyboardHandler();
            const char altVkCode = (char) 0x12, sVkCode = (char) 0x53;
            var keyboardShortcut = new KeyboardShortcut(new List<char>{altVkCode, sVkCode}, SwitchVisibility);
            _keyboardHandler.AddShortcut(keyboardShortcut);
            
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
        
        public void SwitchVisibility()
        {
            _visible = !_visible;
            this.WindowState = _visible ? WindowState.Maximized : WindowState.Normal;
            this.Topmost = _visible;
        }
    }
}