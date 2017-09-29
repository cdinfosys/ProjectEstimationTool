using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProjectEstimationTool.Classes
{
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        #region Events
        /// <summary>
        ///     Event that is fired when a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Construction
        /// <summary>
        ///     Construct a view model object.
        /// </summary>
        protected NotifyPropertyChangedBase()
        {
        }
        #endregion // Construction

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
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion Helper methods.
    }
}
