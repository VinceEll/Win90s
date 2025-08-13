using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace Win90s
{
    public class AttachDragResizeBehavior : IMultiValueConverter
    {
        public static AttachDragResizeBehavior Instance { get; } = new AttachDragResizeBehavior();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 3 &&
                values[0] is Window window &&
                values[1] is UIElement dragElement &&
                values[2] is FrameworkElement resizeBorder)
            {
                WindowHelper.EnableDragAndResize(window, dragElement, resizeBorder);
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

    public static class myCursors
    {
        public static Cursor SizeNWSE { get; } = Cursors.SizeNWSE;
        public static Cursor SizeNESW { get; } = Cursors.SizeNESW;
        public static Cursor SizeWE { get; } = Cursors.SizeWE;
        public static Cursor SizeNS { get; } = Cursors.SizeNS;
        public static Cursor Arrow { get; } = Cursors.Arrow;
    }
    
}
