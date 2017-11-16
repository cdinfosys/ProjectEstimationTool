using System;
using System.Windows;
using NLog;
using ProjectEstimationTool.Utilities;

namespace ProjectEstimationTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    partial class App : Application
    {
        public App()
        {
            Application.Current.DispatcherUnhandledException += OnUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
        }

        private void OnAppDomainUnhandledException(Object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                Utility.Logger.Log(LogLevel.Fatal, ex);
            }
            else
            {
                Utility.Logger.Log(LogLevel.Fatal, e.ExceptionObject.ToString());
            }
        }

        private void OnUnhandledException(Object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Utility.Logger.Log(LogLevel.Fatal, e.Exception);
        }
    }
}
