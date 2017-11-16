using System;
using Prism.Events;

namespace ProjectEstimationTool.Events
{
    /// <summary>
    /// Event raised to start the process of exporting the data after the file name has been selected.
    /// </summary>
    class ExportProjectToExcelEvent : PubSubEvent<String>
    {
    }
}
