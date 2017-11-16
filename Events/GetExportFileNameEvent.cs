
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace ProjectEstimationTool.Events
{
    /// <summary>
    /// Event used to signal the UI to show a dialog box where the user enters the
    /// name of the export file.
    /// </summary>
    class GetExportFileNameEvent : PubSubEvent
    {
    } // class GetExportFileNameEvent
} // namespace ProjectEstimationTool.Events
