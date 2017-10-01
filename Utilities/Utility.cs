using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ProjectEstimationTool.Classes;
using NLog;

namespace ProjectEstimationTool.Utilities
{
    public static class Utility
    {
        public static event EventHandler<String> TaskItemChanged;

        #region Private class members
        private static readonly Prism.Events.IEventAggregator mEventAggregator = new Prism.Events.EventAggregator();
        private static Logger mLoggerInstance = LogManager.GetCurrentClassLogger();
        #endregion Private class members

        #region Public properties
        public static Prism.Events.IEventAggregator EventAggregator
        {
            get
            {
                return mEventAggregator;
            }
        }
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
    } // class Utility
} // namespace ProjectEstimationTool.Utilities
