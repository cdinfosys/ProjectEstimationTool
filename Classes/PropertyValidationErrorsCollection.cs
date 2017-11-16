using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectEstimationTool.Classes
{
    class PropertyValidationErrorsCollection
    {
        public delegate void CollectionChangedDelegate();

        private Dictionary<String, Boolean> mPropertyErrors = new Dictionary<String, Boolean>(StringComparer.OrdinalIgnoreCase);
        private CollectionChangedDelegate mCollectionChangeCallback;

        public PropertyValidationErrorsCollection(CollectionChangedDelegate collectionChangeCallback)
        {
            mCollectionChangeCallback = collectionChangeCallback;
        }

        public void ClearError(String propertyName)
        {
            if (mPropertyErrors.ContainsKey(propertyName))
            {
                if (mPropertyErrors[propertyName])
                {
                    mPropertyErrors[propertyName] = false;
                    mCollectionChangeCallback?.Invoke();
                }
            }
        }

        public void SetError(String propertyName)
        {
            if (mPropertyErrors.ContainsKey(propertyName))
            {
                if (!mPropertyErrors[propertyName])
                {
                    mPropertyErrors[propertyName] = true;
                    mCollectionChangeCallback?.Invoke();
                }
            }
            else
            {
                mPropertyErrors.Add(propertyName, true);
                mCollectionChangeCallback?.Invoke();
            }
        }

        public Boolean HasError(String propertyName)
        {
            Boolean hasError;
            if (mPropertyErrors.TryGetValue(propertyName, out hasError))
            {
                return hasError;
            }
            return false;
        }

        public Boolean HasErrors => (mPropertyErrors.Values.Any(u => u == true));
     } // class PropertyValidationErrorsCollection
} // namespace ProjectEstimationTool.Classes
