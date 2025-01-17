﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using NitroxModel.Logger;

namespace NitroxLauncher
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Log.Setup();

            // Set default style for all windows to the style with the target type 'Window' (in App.xaml).
            FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window),
                new FrameworkPropertyMetadata
                {
                    DefaultValue = FindResource(typeof(Window))
                });
            FrameworkElement.StyleProperty.OverrideMetadata(typeof(Page),
                new FrameworkPropertyMetadata
                {
                    DefaultValue = FindResource(typeof(Page))
                });

            // Error if running from a temporary directory because Nitrox Launcher won't be able to write files directly to zip/rar
            // Tools like WinRAR do this to support running EXE files while it's still zipped.
            if (Directory.GetCurrentDirectory().StartsWith(Path.GetTempPath(), StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Nitrox launcher should not be executed from a temporary directory. Install Nitrox launcher properly by extracting ALL files and moving these to a dedicated location on your PC.",
                    "Invalid working directory",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Environment.Exit(1);
            }

            base.OnStartup(e);
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // If something went wrong. Close the server if embedded.
            LauncherLogic.Instance.Dispose();
            Log.Error(e.Exception.GetBaseException().ToString()); // Gets the exception that was unhandled, not the "dispatched unhandled" exception.
            MessageBox.Show(GetExceptionError(e.Exception), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private string GetExceptionError(Exception e)
        {
#if RELEASE
            return e.GetBaseException().Message;
#else
            return e.GetBaseException().ToString();
#endif
        }
    }
}
