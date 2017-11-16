using System.Collections.Specialized;

namespace ProjectEstimationTool.Interfaces
{
    interface IProjectTreeItem<T> : INotifyCollectionChanged
    {
        IProjectTreeItem<T> Children { get; set; }
    } // interface IProjectTreeItem
} // namespace ProjectEstimationTool.Interfaces
