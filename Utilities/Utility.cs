using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ProjectEstimationTool.Classes;
using NLog;
using System.Threading;
using System.Configuration;
using System.Windows;

namespace ProjectEstimationTool.Utilities
{
    [Serializable]
    static class Utility
    {
        public static event EventHandler<String> TaskItemChanged;

        /// <summary>
        /// Object used for thread synching access to the app settings.
        /// </summary>
        public static readonly Object ProgramSettingsSynchLockObject = new Object();

        #region Private class members
        private static readonly Prism.Events.IEventAggregator mEventAggregator = new Prism.Events.EventAggregator();
        private static Logger mLoggerInstance = LogManager.GetCurrentClassLogger();
        private static SynchronizationContext sUISynchronizationContext;
        private static CancellationTokenSource sSaveSettingsCancellationTokenSource;
        #endregion Private class members

        #region Public properties
        public static Prism.Events.IEventAggregator EventAggregator
        {
            get
            {
                return mEventAggregator;
            }
        }

        public static SynchronizationContext UISynchronizationContext => sUISynchronizationContext;
        #endregion Public properties

        public static void NotifyTaskItemChanged
        (
            ProjectTreeItemBase taskItem, 
            [CallerMemberName] string propertyName = null
        )
        {
            TaskItemChanged?.Invoke(taskItem, propertyName);
        }

        public static NLog.ILogger Logger
        {
            get
            {
                return mLoggerInstance;
            }
        }

        /// <summary>
        /// Stores a reference to the UI SynchronizationContext object.
        /// </summary>
        public static void CaptureMainWindowSynchronizationContext()
        {
            sUISynchronizationContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// Save program settings
        /// </summary>
        /// <param name="settings">
        /// Reference to an object that contains program settings.
        /// </param>
        public async static void SaveSettingsAsync(SettingsBase settings)
        {
            // Cancel a pending save operation
            if (sSaveSettingsCancellationTokenSource != null)
            {
                sSaveSettingsCancellationTokenSource.Cancel();
                sSaveSettingsCancellationTokenSource.Dispose();
            }
            sSaveSettingsCancellationTokenSource = new CancellationTokenSource();

            await Task.Run
            (() =>
                {
                    try
                    {
                        lock (ProgramSettingsSynchLockObject)
                        {
                            settings.Save();
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Ignore cancellation exceptions
                    }
                },
                sSaveSettingsCancellationTokenSource.Token
            );
        }
    } // class Utility
} // namespace ProjectEstimationTool.Utilities
