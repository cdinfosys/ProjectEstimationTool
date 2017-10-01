using System;
using System.Windows;
using NLog;
using ProjectEstimationTool.Utilities;

namespace ProjectEstimationTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Application.Current.DispatcherUnhandledException += OnUnhandledException;
        }

        private void OnUnhandledException(Object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Utility.Logger.Log(LogLevel.Fatal, e.Exception);
        }
    }
}
