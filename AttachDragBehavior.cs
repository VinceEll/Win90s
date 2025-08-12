using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace Win90s
{
    class AttachDragBehavior : IMultiValueConverter
    {
        public static AttachDragBehavior Instance { get; } = new AttachDragBehavior();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is Window window && values[1] is UIElement dragElement)
            {
                WindowHelper.EnableDragMove(window, dragElement);
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
