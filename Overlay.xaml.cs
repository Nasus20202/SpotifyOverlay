using System.Collections.Generic;
using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace SpotifyOverlay
{
    public partial class Overlay : Window
    {
        private bool _visible = false;
        
        public Overlay()
        {
            var keyboardHandler = new KeyboardHandler();
            const char altVkCode = (char)0xA4, sVkCode = (char)0x53, escVkCode = (char)0x1B;
            
            var keyboardShortcutMain = new KeyboardShortcut(new List<char>{altVkCode, sVkCode}, SwitchVisibility);
            var escShortcut = new KeyboardShortcut(new List<char>{escVkCode}, EscPressed);
            
            keyboardHandler.AddShortcut(keyboardShortcutMain);
            keyboardHandler.AddShortcut(escShortcut);
            
            InitializeComponent();
        }

        private void EscPressed()
        {
            if (_visible)
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