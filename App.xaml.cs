using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

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
            // Set the application to use a custom theme or style if needed
            // Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/YourTheme.xaml") });
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
