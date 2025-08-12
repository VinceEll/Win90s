using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Win90s
{

    public static class WindowAppHelper
    {
        public static readonly DependencyProperty UseNoRoundCornersProperty =
            DependencyProperty.RegisterAttached(
                "UseNoRoundCorners",
                typeof(bool),
                typeof(WindowAppHelper),
                new PropertyMetadata(false, OnUseNoRoundCornersChanged));

        public static void SetUseNoRoundCorners(Window window, bool value)
        {
            window.SetValue(UseNoRoundCornersProperty, value);
        }

        public static bool GetUseNoRoundCorners(Window window)
        {
            return (bool)window.GetValue(UseNoRoundCornersProperty);
        }

        private static void OnUseNoRoundCornersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window && (bool)e.NewValue)
            {
                window.SourceInitialized += (s, args) =>
                {
                    var hwnd = new WindowInteropHelper(window).Handle;
                    const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
                    var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DONOTROUND;
                    DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref preference, sizeof(uint));
                };
            }
        }

        private enum DWM_WINDOW_CORNER_PREFERENCE
        {
            DWMWCP_DEFAULT = 0,
            DWMWCP_DONOTROUND = 1,
            DWMWCP_ROUND = 2,
            DWMWCP_ROUNDSMALL = 3
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(
            IntPtr hwnd,
            int attr,
            ref DWM_WINDOW_CORNER_PREFERENCE attrValue,
            int attrSize);
    }

}
