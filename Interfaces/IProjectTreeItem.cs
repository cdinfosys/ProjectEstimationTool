using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEstimationTool.Interfaces
{
    public interface IProjectTreeItem<T> : INotifyCollectionChanged
    {
        IProjectTreeItem<T> Chilldren { get; set; }
    } // interface IProjectTreeItem
} // namespace ProjectEstimationTool.Interfaces
