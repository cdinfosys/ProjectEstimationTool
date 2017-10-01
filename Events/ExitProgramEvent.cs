using System;
using Prism.Events;

namespace ProjectEstimationTool.Events
{
    /// <summary>
    ///     Event sent to the main window to close the program. The payload is the program exit code.
    /// </summary>
    public class ExitProgramEvent : PubSubEvent<Int32>
    {
    }
}
