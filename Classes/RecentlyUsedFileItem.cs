using CDesignInfoSys.Toolshed.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;
using System.IO;

namespace ProjectEstimationTool.Classes
{
    /// <summary>
    /// Stores an item in the recently used files list.
    /// </summary>
    internal struct RecentlyUsedFileItem
    {
        #region Fields
        /// <summary>
        /// Path to the file
        /// </summary>
        private readonly String mFilePath;
        #endregion // Fields

        #region Construction
        /// <summary>
        /// Construct an instance of the type
        /// </summary>
        /// <param name="filePath">
        /// Path to the file.
        /// </param>
        public RecentlyUsedFileItem(String filePath)
        {
            mFilePath = filePath;
        }

        /// <summary>
        /// Gets the stored file path.
        /// </summary>
        public String FilePath => mFilePath;

        /// <summary>
        /// Gets the file name without the directory path.
        /// </summary>
        public String FileName
        {
            get
            {
                if (String.IsNullOrEmpty(mFilePath)) return null;
                return Path.GetFileName(mFilePath);
            }
        }

        /// <summary>
        /// Gets a flag to indicate if the item is in use.
        /// </summary>
        public Boolean IsUsed => !String.IsNullOrWhiteSpace(mFilePath);
        #endregion // Construction
    } // struct RecentlyUsedFileItem
} // namespace ProjectEstimationTool.Classes
