using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEstimationTool.ViewModels
{
    /// <summary>
    ///     Base class for view models.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region Events
        /// <summary>
        ///     Event that is fired when a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Private data members
        #endregion Private data members

        #region Construction
        /// <summary>
        ///     Construct a view model object.
        /// </summary>
        /// <param name="container">
        ///     Reference to a unity container object.
        /// </param>
        /// <param name="eventAggregator">
        ///     Reference to an event aggregator object.
        /// </param>
        protected ViewModelBase
        (
        )
        {
        }
        #endregion // Construction

        #region Protected properties
        #endregion Protected properties

        #region Helper methods
        /// <summary>
        ///     Helper to change the value of a field and raise 
        /// </summary>
        /// <typeparam name="T">
        ///     Data type of the value that must be set.
        /// </typeparam>
        /// <param name="storage">
        ///     Reference to a variable or object to set.
        /// </param>
        /// <param name="value">
        ///     New value of the variable or object.
        /// </param>
        /// <param name="propertyName">
        ///     Name of the property.
        /// </param>
        /// <returns>
        ///     Returns <c>true</c> if the property changed and the <see cref="PropertyChanged"/> event was raised. Returns
        ///     <c>false</c> if the new value is the same as the old value.
        /// </returns>
        protected bool SetProperty<T>
        (
            ref T storage, 
            T value,
            [CallerMemberName] string propertyName = null
        )
        {
            // Only change if the new value is different from the old value
            if (!Object.Equals(storage, value))
            {
                storage = value;
                OnPropertyChanged(propertyName);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Helper to check if the <see cref="PropertyChanged"/> event is hooked and invoke it
        ///     if a property changes.
        /// </summary>
        /// <param name="propertyName">
        ///     Name of the property that changed state.
        /// </param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion Helper methods.
    } // class ViewModelBase
} //namespace ProjectEstimationTool.ViewModels
