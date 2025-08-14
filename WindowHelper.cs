
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
namespace Win90s
{
    class WindowHelper
    {
        private const int BORDER_SIZE = 5;


        private static bool _isResizing;
        private static ResizeDirection _resizeDir;
        private static Point _startMouse;
        private static Rect _startBounds;
        private static Window _overlay;

        public static void EnableDragAndResize(Window window, UIElement dragElement, FrameworkElement resizeBorder)
        {
            //TODO : Add snapping to grid
            // --- Enable Drag ---
            dragElement.MouseLeftButtonDown += (sender, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                    window.DragMove();
            };

            // --- Enable Resize ---

            resizeBorder.MouseMove += (s, e) =>
            {
                if (_isResizing) return; // Prevent cursor change during resizing

                Point pos = e.GetPosition(window);
                window.Cursor = GetResizeCursor(window, pos, BORDER_SIZE);
            };

            resizeBorder.MouseLeftButtonDown += (s, e) =>
            {
                if (e.LeftButton != MouseButtonState.Pressed) return;

                Point pos = e.GetPosition(window);
                _resizeDir = GetDirection(window, pos, BORDER_SIZE);

                if (_resizeDir != ResizeDirection.None)
                {
                    _startMouse = window.PointToScreen(pos);
                    _startBounds = new Rect(window.Left, window.Top, window.Width, window.Height);
                    CreateOverlay(_startBounds);
                    _isResizing = true;
                    resizeBorder.CaptureMouse();
                }
            };

            resizeBorder.MouseMove += (s, e) =>
            {
                if (_isResizing)
                {
                    var currentMouse = window.PointToScreen(e.GetPosition(window));
                    var newBounds = GetNewBounds(_startBounds, _startMouse, currentMouse, _resizeDir);
                    UpdateOverlay(newBounds);
                }
            };

            resizeBorder.MouseLeftButtonUp += (s, e) =>
            {
                if (_isResizing)
                {
                    window.Left = _overlay.Left;
                    window.Top = _overlay.Top;
                    window.Width = _overlay.Width;
                    window.Height = _overlay.Height;

                    _overlay.Close();
                    _overlay = null;
                    _isResizing = false;
                    resizeBorder.ReleaseMouseCapture();
                }
            };

        }

        private static void CreateOverlay(Rect bounds)
        {
            _overlay = new Window
            {
                Left = bounds.Left,
                Top = bounds.Top,
                Width = bounds.Width,
                Height = bounds.Height,
                AllowsTransparency = true,
                Background = Brushes.Transparent,
                WindowStyle = WindowStyle.None,
                Topmost = true,
                ShowInTaskbar = false
            };

            var rect = new Rectangle
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection { 2, 2 }
            };
            _overlay.Content = rect;
            _overlay.Show();
        }

        private static void UpdateOverlay(Rect bounds)
        {
            _overlay.Left = bounds.Left;
            _overlay.Top = bounds.Top;
            _overlay.Width = bounds.Width;
            _overlay.Height = bounds.Height;

        }

        // Direction + cursor handling
        private static Cursor GetResizeCursor(Window window, Point pos, int thickness)
        {
            var dir = GetDirection(window, pos, thickness);
            return dir switch
            {
                ResizeDirection.TopLeft or ResizeDirection.BottomRight => Cursors.SizeNWSE,
                ResizeDirection.TopRight or ResizeDirection.BottomLeft => Cursors.SizeNESW,
                ResizeDirection.Left or ResizeDirection.Right => Cursors.SizeWE,
                ResizeDirection.Top or ResizeDirection.Bottom => Cursors.SizeNS,
                _ => Cursors.Arrow
            };
        }

        private static ResizeDirection GetDirection(Window window, Point pos, int thickness)
        {
            bool left = pos.X <= thickness;
            bool right = pos.X >= window.ActualWidth - thickness;
            bool top = pos.Y <= thickness;
            bool bottom = pos.Y >= window.ActualHeight - thickness;

            if (top && left) return ResizeDirection.TopLeft;
            if (top && right) return ResizeDirection.TopRight;
            if (bottom && left) return ResizeDirection.BottomLeft;
            if (bottom && right) return ResizeDirection.BottomRight;
            if (left) return ResizeDirection.Left;
            if (right) return ResizeDirection.Right;
            if (top) return ResizeDirection.Top;
            if (bottom) return ResizeDirection.Bottom;

            return ResizeDirection.None;
        }

        private static double snap(double value,double gridsize = 8) =>
            Math.Round(value / gridsize) * gridsize;

        private static Rect GetNewBounds(Rect start, Point startMouse, Point currentMouse, ResizeDirection dir)
        {
            var dx = currentMouse.X - startMouse.X;
            var dy = currentMouse.Y - startMouse.Y;

            double left = start.Left;
            double top = start.Top;
            double width = start.Width;
            double height = start.Height;
            
            if (dir.HasFlag(ResizeDirection.Left))
            {
                left += dx;
                width -= dx;
            }
            if (dir.HasFlag(ResizeDirection.Right))
            {
                width += dx;
            }
            if (dir.HasFlag(ResizeDirection.Top))
            {
                top += dy;
                height -= dy;
            }
            if (dir.HasFlag(ResizeDirection.Bottom))
            {
                height += dy;
            }
            left = snap(left);
            top = snap(top);
            width = snap(width);
            height = snap(height);
            return new Rect(left, top, width, height);
        }

        [Flags]
        private enum ResizeDirection
        {
            None = 0,
            Left = 1,
            Right = 2,
            Top = 4,
            Bottom = 8,
            TopLeft = Top | Left,
            TopRight = Top | Right,
            BottomLeft = Bottom | Left,
            BottomRight = Bottom | Right
        }

        private static int GetHitTest(Window window, Point pos, int thickness)
        {
            if (pos.X <= thickness && pos.Y <= thickness) return HTTOPLEFT;
            if (pos.X >= window.ActualWidth - thickness && pos.Y <= thickness) return HTTOPRIGHT;
            if (pos.X <= thickness && pos.Y >= window.ActualHeight - thickness) return HTBOTTOMLEFT;
            if (pos.X >= window.ActualWidth - thickness && pos.Y >= window.ActualHeight - thickness) return HTBOTTOMRIGHT;
            if (pos.X <= thickness) return HTLEFT;
            if (pos.X >= window.ActualWidth - thickness) return HTRIGHT;
            if (pos.Y <= thickness) return HTTOP;
            if (pos.Y >= window.ActualHeight - thickness) return HTBOTTOM;
            return -1;
        }

        // --- WinAPI ---
        private const int WM_NCLBUTTONDOWN = 0x00A1;
        private const int HTLEFT = 10;
        private const int HTRIGHT = 11;
        private const int HTTOP = 12;
        private const int HTTOPLEFT = 13;
        private const int HTTOPRIGHT = 14;
        private const int HTBOTTOM = 15;
        private const int HTBOTTOMLEFT = 16;
        private const int HTBOTTOMRIGHT = 17;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
    }


}

