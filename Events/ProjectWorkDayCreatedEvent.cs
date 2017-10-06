using System;
using Prism.Events;

namespace ProjectEstimationTool.Events
{
    /// <summary>
    ///     Event fired when a new work day is created.
    /// </summary>
    public class ProjectWorkDayCreatedEvent : PubSubEvent<Int32>
    {
    }
}
