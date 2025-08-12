using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Win90s
{
    class WindowHelper
    {
        public static void EnableDragMove(Window window, UIElement dragelement)
        {
            dragelement.MouseLeftButtonDown += (sender, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    window.DragMove();
                }
            };
        }


    }
}
