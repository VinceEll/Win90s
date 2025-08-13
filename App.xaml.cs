using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace Win90s
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            var mainWindow = new MainWindow();
            mainWindow.CommandBindings.Add
                (
                new CommandBinding(ApplicationCommands.Close, 
                CloseCommand_Executed, 
                CanExecuteCommand
                ));
            
            mainWindow.Show();

        }
        

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Current?.Windows.Count > 0)
            {
                var window = Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
                window?.Close();

            }
        }

        private void CanExecuteCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Current?.Windows.OfType<Window>().Any(w => w.IsActive) ?? false;
        }
      

    }

    

}
