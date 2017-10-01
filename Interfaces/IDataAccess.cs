﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectEstimationTool.Classes;

namespace ProjectEstimationTool.Interfaces
{
    public interface IDataAccess : IDisposable
    {
        /// <summary>
        /// Create a new data store.
        /// </summary>
        void CreateNew();

        /// <summary>
        /// Open an existing data store.
        /// </summary>
        void Open();

        /// <summary>
        ///     Set the update time of the data store to the current UTC time
        /// </summary>
        void SetLastUpdateTime();

        /// <summary>
        ///     Create an archive copy of a current task item.
        /// </summary>
        /// <param name="taskItemID">
        ///     ID of the item to archive.
        /// </param>
        void ArchiveTaskItem(Int32 taskItemID);

        /// <summary>
        ///     Mark a task item as deleted
        /// </summary>
        /// <param name="taskItemID">
        ///     ID of the item to mark.
        /// </param>
        /// <param name="isDeleted">
        ///     Turn the deleted flag on or off.
        /// </param>
        void DeleteTaskItem(Int32 taskItemID, Boolean isDeleted);

        /// <summary>
        ///     Insert a new record or update an existing record in the TaskItem table.
        /// </summary>
        /// <param name="taskItemID"></param>
        /// <param name="itemDescription"></param>
        /// <param name="estimatedTimeMinutes"></param>
        /// <param name="minimumTimeMinutes"></param>
        /// <param name="maximumTimeMinutes"></param>
        /// <param name="percentageComplete"></param>
        /// <param name="TimeSpentMinutes"></param>
        void InsertOrUpdateTaskItem
        (
            Int32 taskItemID,
            Int32 parentTaskItemID,
            String itemDescription,
            Int32 estimatedTimeMinutes,
            Int32 minimumTimeMinutes,
            Int32 maximumTimeMinutes,
            Int32 percentageComplete,
            Int32 timeSpentMinutes
        );

        /// <summary>
        ///     Retrieves a list of project items
        /// </summary>
        /// <param name="onlyActiveItems">
        ///     If <c>true</c> only items that were not marked as deleted are retrieved. If <c>false</c> all items are retrieved.
        /// </param>
        /// <returns>
        /// </returns>
        IEnumerable<ProjectTreeItemBase> GetTaskItems(Boolean onlyActiveItems);

        /// <summary>
        ///     Log a work day in the DaysWorked table.
        /// </summary>
        /// <param name="workDayDate">
        ///     Day on which the work was done.
        /// </param>
        void LogWorkDay(DateTime workDayDate);

        /// <summary>
        ///     Gets a list of the days on which work was logged.
        /// </summary>
        /// <returns>
        ///     Returns a list of dates from the DaysWorked table.
        /// </returns>
        IEnumerable<DateTime> GetWorkDays();

        /// <summary>
        /// Creates a new project version record.
        /// </summary>
        void CreateProjectVersion();

        /// <summary>
        /// Returns a collection of project versions.
        /// </summary>
        /// <returns>
        /// Returns a collection of <see cref="ProjectVersionDTO"/> project version records.
        /// </returns>
        IEnumerable<ProjectVersionDTO> GetProjectVersions();

        /// <summary>
        /// Returns the schema version of the data store.
        /// </summary>
        Int32 SchemaVersion { get; }

        /// <summary>
        /// Get the date and time when the store was last updated.
        /// </summary>
        DateTime LastUpdateTime { get; }

        /// <summary>
        /// Gets the version number of the project.
        /// </summary>
        Int32 ProjectVersion { get; }
    } // interface IDataAccess
} // namespace ProjectEstimationTool.Interfaces