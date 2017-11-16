using Prism.Events;

namespace ProjectEstimationTool.Events
{
    /// <summary>
    ///     Enumeration that identifies the state of the project model.
    /// </summary>
    public enum ProjectModelState
    {
        NoProject,
        Open,
        Modified
    }

    /// <summary>
    ///     Event fired by the model to let the viewmodel know that the model has been closed or loaded.
    /// </summary>
    class ProjectModelChangedEvent : PubSubEvent<ProjectModelState>
    {
    }
} // namespace ProjectEstimationTool.Events
