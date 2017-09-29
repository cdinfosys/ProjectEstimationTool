using Prism.Events;

namespace ProjectEstimationTool.Events
{
    /// <summary>
    ///     Event fired by the model to let the viewmodel know that the model has been closed or loaded.
    /// </summary>
    public class ProjectModelChangedEvent : PubSubEvent
    {
    }
} // namespace ProjectEstimationTool.Events
