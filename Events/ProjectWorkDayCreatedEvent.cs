using System;
using Prism.Events;

namespace ProjectEstimationTool.Events
{
    /// <summary>
    ///     Event fired when a new work day is created.
    /// </summary>
    class ProjectWorkDayCreatedEvent : PubSubEvent<Int32>
    {
    }
}
