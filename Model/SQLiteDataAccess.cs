using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using ProjectEstimationTool.Classes;
using ProjectEstimationTool.Interfaces;
using ProjectEstimationTool.Properties;
using ProjectEstimationTool.Utilities;

namespace ProjectEstimationTool.Model
{
    public class SQLiteDataAccessExceptionArgs : ExceptionArgs
    {
        public enum ErrorCode
        {
            NoError = 0,
            EmptyFilePath,
            InvalidFilePath,
            DatabaseSchemaVersionNotFound,
            UnsupportedDatabaseSchemaVersion,
            DatabaseLastUpdateDateTimeNotFound,
            ProjectVersionNotFound,
            NoCurrentWorkDaySet,
        }

        private String mFilePath = null;
        private ErrorCode mErrorCode = ErrorCode.NoError;

        public SQLiteDataAccessExceptionArgs(String filePath, ErrorCode errorCode)
        {
            this.mFilePath = filePath;
            this.mErrorCode = errorCode;
        }

        public SQLiteDataAccessExceptionArgs(ErrorCode errorCode)
        {
            this.mErrorCode = errorCode;
        }

        public override String Message
        {
            get
            {
                String errorMessage;
                switch (this.mErrorCode)
                {
                    case ErrorCode.NoError:
                        errorMessage = Resources.NoError;
                        break;

                    case ErrorCode.InvalidFilePath:
                        errorMessage = Resources.SQLiteDataAccess_InvalidFilePath;
                        break;

                    case ErrorCode.DatabaseSchemaVersionNotFound:
                        errorMessage = Resources.SQLiteDataAccess_DatabaseSchemaVersionNotFound;
                        break;

                    case ErrorCode.UnsupportedDatabaseSchemaVersion:
                        errorMessage = Resources.SQLiteDataAccess_UnsupportedDatabaseSchemaVersion;
                        break;

                    case ErrorCode.DatabaseLastUpdateDateTimeNotFound:
                        errorMessage = Resources.SQLiteDataAccess_DatabaseLastUpdateDateTimeNotFound;
                        break;

                    case ErrorCode.ProjectVersionNotFound:
                        errorMessage = Resources.SQLiteDataAccess_ProjectVersionNotFound;
                        break;

                    case ErrorCode.NoCurrentWorkDaySet:
                        errorMessage = Resources.SQLiteDataAccess_NoCurrentWorkDaySet;
                        break;

                    default:
                        errorMessage = Resources.UndefinedError;
                        break;
                }

                return String.Format
                (
                    "{0} {1}",
                    errorMessage,
                    String.IsNullOrEmpty(this.mFilePath) ? String.Empty : String.Format("({0})"), this.mFilePath
                );
            }
        }
    }

    public class SQLiteDataAccess : IDataAccess
    {
        /// <summary>
        ///     ID of the record in the ProjectMetaData table that stores the database schema version ID.
        /// </summary>
        private const Int32 PROJECT_METADATA_ID_SCHEMA_VERSION = 1;

        /// <summary>
        ///     ID of the record in the ProjectMetaData table that stores the date when the database was last updated.
        /// </summary>
        private const Int32 PROJECT_METADATA_ID_LAST_UPDATE_DATE = 2;

        /// <summary>
        ///     ID of the record in the ProjectMetaData table that stores the date when the project started
        /// </summary>
        private const Int32 PROJECT_METADATA_ID_START_DATE = 3;

        /// <summary>
        ///     Number of minutes per work day.
        /// </summary>
        private const Int32 PROJECT_METADATA_ID_MINUTES_PER_WORK_DAY = 4;

        /// <summary>
        ///     ID of the current project database schema version
        /// </summary>
        private const Int32 CURRENT_DATABASE_SCHEMA_VERSION_ID = 1;

        private const String SQL_CREATE_PROJECT_METADATA_TABLE = 
        @"
            CREATE TABLE ProjectMetaData
            (
                ProjectMetaDataID INTEGER NOT NULL PRIMARY KEY,
                IntegralValue INTEGER NULL,
                StringValue INTEGER NULL,
                DateTimeValue DATETIME NULL,
                RealValue REAL NULL
            )
        ";

        private const String SQL_CREATE_DAYS_WORKED_TABLE =
        @"
            CREATE TABLE DaysWorked
            (
                DaysWorkedID INTEGER NOT NULL PRIMARY KEY,
                CalendarDate DATETIME NOT NULL,
                ProjectPercentageComplete INTEGER NOT NULL
            )
        ";

        private const String SQL_CREATE_PROJECT_VERSION_TABLE =
        @"
            CREATE TABLE ProjectVersion
            (
                ProjectVersionID INTEGER NOT NULL PRIMARY KEY,
                VersionDate DATETIME NOT NULL
            )
        ";

        private const String SQL_CREATE_TASK_ITEM_TABLE =
        @"
            CREATE TABLE TaskItem
            (
                TaskItemID INTEGER NOT NULL PRIMARY KEY,
                ParentTaskItemID INTEGER NULL,
                ItemDescription TEXT NOT NULL,
                EstimatedTimeMinutes INTEGER NOT NULL,
                MinimumTimeMinutes INTEGER NOT NULL,
                MaximumTimeMinutes INTEGER NOT NULL,
                PercentageComplete INTEGER NOT NULL,
                TimeSpentMinutes INTEGER NOT NULL,
                IsDeleted INTEGER NULL,
                FOREIGN KEY (ParentTaskItemID) REFERENCES TaskItem(TaskItemID)
            )
        ";

        private const String SQL_CREATE_TASK_ITEM_ARCHIVE_TABLE =
        @"
            CREATE TABLE TaskItemArchive
            (
                TaskItemArchiveID INTEGER NOT NULL PRIMARY KEY,
                ProjectVersionID INTEGER NOT NULL,
                TaskItemID INTEGER NOT NULL,
                ParentTaskItemID INTEGER NULL,
                ItemDescription TEXT NOT NULL,
                EstimatedTimeMinutes INTEGER NOT NULL,
                MinimumTimeMinutes INTEGER NOT NULL,
                MaximumTimeMinutes INTEGER NOT NULL,
                PercentageComplete INTEGER NOT NULL,
                TimeSpentMinutes INTEGER NOT NULL,
                IsDeleted INTEGER NULL,
                ArchiveTime DATETIME NOT NULL,
                FOREIGN KEY (TaskItemID) REFERENCES TaskItem(TaskItemID),
                FOREIGN KEY (ParentTaskItemID) REFERENCES TaskItem(TaskItemID)
                FOREIGN KEY (ProjectVersionID) REFERENCES ProjectVersion(ProjectVersionID)
            )
        ";

        private const String SQL_CREATE_TASK_ITEM_NOTE =
        @"
            CREATE TABLE TaskItemNote
            (
                TaskItemNoteID INTEGER NOT NULL PRIMARY KEY,
                TaskItemID INTEGER NOT NULL,
                IsHandled INTEGER NULL,
                NoteText TEXT NOT NULL,
                FOREIGN KEY (TaskItemID) REFERENCES TaskItem(TaskItemID)
            )
        ";

        private const String SQL_STORE_DATABASE_SCHEMA_VERSION_ID = 
        @"
            INSERT INTO ProjectMetaData
            (
                ProjectMetaDataID,
                IntegralValue
            )
            VALUES
            (
                @projectMetaDataID,
                @integralValue
            )
        ";

        private const String SQL_STORE_MINUTES_PER_WORK_DAY = 
        @"
            INSERT INTO ProjectMetaData
            (
                ProjectMetaDataID,
                IntegralValue
            )
            VALUES
            (
                @projectMetaDataID,
                @integralValue
            )
        ";

        private const String SQL_STORE_DATABASE_LAST_UPDATED_DATE = 
        @"
            INSERT INTO ProjectMetaData
            (
                ProjectMetaDataID,
                DateTimeValue
            )
            VALUES
            (
                @projectMetaDataID,
                @dateTimeValue
            )
        ";

        private const String SQL_STORE_PROJECT_START_DATE = 
        @"
            INSERT INTO ProjectMetaData
            (
                ProjectMetaDataID,
                DateTimeValue
            )
            VALUES
            (
                @projectMetaDataID,
                @dateTimeValue
            )
        ";

        private const String SQL_FETCH_ACTIVE_TASK_ITEMS =
        @"
            SELECT
                TaskItemID,
                ParentTaskItemID,
                ItemDescription,
                EstimatedTimeMinutes,
                MinimumTimeMinutes,
                MaximumTimeMinutes,
                PercentageComplete,
                TimeSpentMinutes
            FROM
                TaskItem
            WHERE
                (IsDeleted IS NULL) OR (IsDeleted = 0)
        ";

        private const String SQL_STORE_UPDATE_TASK_ITEM =
        @"
            INSERT OR REPLACE INTO TaskItem
            (
                TaskItemID,
                ParentTaskItemID,
                ItemDescription,
                EstimatedTimeMinutes,
                MinimumTimeMinutes,
                MaximumTimeMinutes,
                PercentageComplete,
                TimeSpentMinutes
            )
            VALUES
            (
                @taskItemID,
                @parentTaskItemID,
                @itemDescription,
                @estimatedTimeMinutes,
                @minimumTimeMinutes,
                @maximumTimeMinutes,
                @percentageComplete,
                @timeSpentMinutes
            )
        ";

        private const String SQL_DELETE_TASK_ITEM = @"UPDATE TaskItem SET IsDeleted = @isDeleted WHERE TaskItemID = @taskItemID";

        private const String SQL_GET_MAX_TASK_ITEM_ID = @"SELECT MAX(TaskItemID) AS TaskItemID FROM TaskItem";

        private const String SQL_ARCHIVE_TASK_ITEM =
        @"
            INSERT INTO TaskItemArchive
            (
                ProjectVersionID,
                TaskItemID,
                ParentTaskItemID,
                ItemDescription,
                EstimatedTimeMinutes,
                MinimumTimeMinutes,
                MaximumTimeMinutes,
                PercentageComplete,
                TimeSpentMinutes,
                IsDeleted,
                ArchiveTime
            )
            SELECT
                @projectVersionID,
                TaskItemID,
                ParentTaskItemID,
                ItemDescription,
                EstimatedTimeMinutes,
                MinimumTimeMinutes,
                MaximumTimeMinutes,
                PercentageComplete,
                TimeSpentMinutes,
                IsDeleted,
                DATETIME('now')
            FROM
                TaskItem
            WHERE
                TaskItemID = @taskItemID
        ";

        private const String SQL_GET_CURRENT_WORK_DAY = 
        @"
            SELECT 
                DaysWorkedID,
                CalendarDate
            FROM 
                DaysWorked
            ORDER BY
                DaysWorkedID DESC
            LIMIT 1
        ";

        private const String SQL_LOG_WORK_DAY = @"INSERT INTO DaysWorked(CalendarDate, ProjectPercentageComplete) VALUES (@calendarDate, -1)";

        private const String SQL_UPDATE_WORK_DAY_TIME_SPENT = @"UPDATE DaysWorked SET ProjectPercentageComplete = @projectPercentageComplete WHERE DaysWorkedID=@daysWorkedID";

        private const String SQL_GET_WORK_DAYS =
        @"
            SELECT
                DaysWorkedID,
                CalendarDate,
                ProjectPercentageComplete
            FROM
                DaysWorked
        ";

        private const String SQL_READ_DATABASE_SCHEMA_VERSION_ID = @"SELECT IntegralValue FROM ProjectMetaData WHERE ProjectMetaDataID=@projectMetaDataID";

        private const String SQL_READ_WORK_DAY_LENGTH = @"SELECT IntegralValue FROM ProjectMetaData WHERE ProjectMetaDataID=@projectMetaDataID";

        private const String SQL_UPDATE_LAST_UPDATE_TIME = @"INSERT OR REPLACE INTO ProjectMetaData(ProjectMetaDataID, DateTimeValue) VALUES (@projectMetaDataID, DATETIME('now'))";

        private const String SQL_FETCH_LAST_UPDATE_TIME = @"SELECT DateTimeValue FROM ProjectMetaData WHERE ProjectMetaDataID = @projectMetaDataID";

        private const String SQL_GET_CURRENT_PROJECT_VERSION = @"SELECT MAX(ProjectVersionID) AS ProjectVersionID FROM ProjectVersion";

        private const String SQL_CREATE_PROJECT_VERSION = @"INSERT INTO ProjectVersion(VersionDate) VALUES (DATETIME('now'))";

        private const String SQL_GET_ALL_VERSIONS = @"SELECT  ProjectVersionID, VersionDate FROM ProjectVersion";

        #region Private data members
        /// <summary>
        ///     Backing data member for the <see cref="DatabaseFilePath"/> property.
        /// </summary>
        private String mDatabaseFilePath;

        /// <summary>
        ///     Previously created connection object.
        /// </summary>
        private SQLiteConnection mSQLiteConnection;

        /// <summary>
        ///     Backing data member for the <see cref="SchemaVersion"/> property.
        /// </summary>
        private Int32 mSchemaVersion = CURRENT_DATABASE_SCHEMA_VERSION_ID;

        /// <summary>
        ///     Date and time when the database was last updated.
        /// </summary>
        private DateTime mDatabaseLastUpdatedDate = DateTime.UtcNow;

        /// <summary>
        ///     Version of the project.
        /// </summary>
        private Int32 mCurrentProjectVersionID = 0;

        /// <summary>
        ///     ID of the last record in the DaysWorked table.
        /// </summary>
        private Int32 mCurrentWorkDayID = 0;

        /// <summary>
        ///     Number of minutes per work day.
        /// </summary>
        private Int32 mWorkDayLengthMinutes = 450;

        /// <summary>
        ///     Date of the current work day.
        /// </summary>
        private DateTime mCurrentWorkDayDate = DateTime.Now.Date;

        /// <summary>
        ///     Start date of the project.
        /// </summary>
        private DateTime mProjectStartDate;
        #endregion Private data members

        #region IDataAccess implementation
        public void CreateNew()
        {
            if (String.IsNullOrEmpty(DatabaseFilePath))
            {
                throw new Exception<SQLiteDataAccessExceptionArgs>(new SQLiteDataAccessExceptionArgs(SQLiteDataAccessExceptionArgs.ErrorCode.EmptyFilePath));
            }

            // Delete any existing files with the same name
            if (File.Exists(DatabaseFilePath))
            {
                File.Delete(DatabaseFilePath);
            }

            // Create the metadata table.
            SQLiteConnection.CreateFile(DatabaseFilePath);
            SQLiteConnection dbConnection = GetConnection();
            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_CREATE_PROJECT_METADATA_TABLE;
                dbCommand.ExecuteNonQuery();
            }

            // Create the table that stores the number of days that have been booked against the project.
            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_CREATE_DAYS_WORKED_TABLE;
                dbCommand.ExecuteNonQuery();
            }

            // Create the table that stores the project version history.
            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_CREATE_PROJECT_VERSION_TABLE;
                dbCommand.ExecuteNonQuery();
            }

            // Create the table that stores the project tasks
            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_CREATE_TASK_ITEM_TABLE;
                dbCommand.ExecuteNonQuery();
            }

            // Create the table that stores the project task items history
            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_CREATE_TASK_ITEM_ARCHIVE_TABLE;
                dbCommand.ExecuteNonQuery();
            }

            // Create the table that stores notes that can be attached to tasks.
            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_CREATE_TASK_ITEM_NOTE;
                dbCommand.ExecuteNonQuery();
            }

            // Store the value that identifies the schema layout.
            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_STORE_DATABASE_SCHEMA_VERSION_ID;
                dbCommand.Parameters.Add(new SQLiteParameter("@projectMetaDataID", PROJECT_METADATA_ID_SCHEMA_VERSION));
                dbCommand.Parameters.Add(new SQLiteParameter("@integralValue", CURRENT_DATABASE_SCHEMA_VERSION_ID));
                dbCommand.ExecuteNonQuery();
            }

            // Store the number of minutes in a work day
            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_STORE_DATABASE_SCHEMA_VERSION_ID;
                dbCommand.Parameters.Add(new SQLiteParameter("@projectMetaDataID", PROJECT_METADATA_ID_MINUTES_PER_WORK_DAY));
                dbCommand.Parameters.Add(new SQLiteParameter("@integralValue", 450));
                dbCommand.ExecuteNonQuery();
            }

            // Store the value that identifies the date when the project was last updated.
            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_STORE_DATABASE_LAST_UPDATED_DATE;
                dbCommand.Parameters.Add(new SQLiteParameter("@projectMetaDataID", PROJECT_METADATA_ID_LAST_UPDATE_DATE));
                dbCommand.Parameters.Add(new SQLiteParameter("@dateTimeValue", DateTime.UtcNow));
                dbCommand.ExecuteNonQuery();
            }

            // Store the project start date.
            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_STORE_PROJECT_START_DATE;
                dbCommand.Parameters.Add(new SQLiteParameter("@projectMetaDataID", PROJECT_METADATA_ID_START_DATE));
                dbCommand.Parameters.Add(new SQLiteParameter("@dateTimeValue", DateTime.UtcNow));
                dbCommand.ExecuteNonQuery();
            }

            // Record the first version of the project.
            CreateProjectVersion();
        }

        public void Open()
        {
            SQLiteConnection dbConnection = GetConnection();
            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_READ_DATABASE_SCHEMA_VERSION_ID;
                dbCommand.Parameters.Add(new SQLiteParameter("@projectMetaDataID", PROJECT_METADATA_ID_SCHEMA_VERSION));
                using (SQLiteDataReader reader = dbCommand.ExecuteReader(CommandBehavior.SingleRow | CommandBehavior.SingleResult))
                {
                    if (!reader.Read())
                    {
                        throw new Exception<SQLiteDataAccessExceptionArgs>
                        (
                            new SQLiteDataAccessExceptionArgs(this.DatabaseFilePath, SQLiteDataAccessExceptionArgs.ErrorCode.DatabaseSchemaVersionNotFound)
                        );
                    }

                    Int32 databaseSchemaVersion = reader.GetInt32(reader.GetOrdinal("IntegralValue"));
                    if (databaseSchemaVersion != 1)
                    {
                        throw new Exception<SQLiteDataAccessExceptionArgs>
                        (
                            new SQLiteDataAccessExceptionArgs(this.DatabaseFilePath, SQLiteDataAccessExceptionArgs.ErrorCode.UnsupportedDatabaseSchemaVersion),
                            String.Format(Resources.ErrorUnsupportedSchemaVersion, databaseSchemaVersion)
                        );
                    }
                    this.mSchemaVersion = databaseSchemaVersion;
                }
            }

            // Get the version number of the database
            FetchCurrentProjectVersion();

            // Get the number of minutes per working day
            FetchWorkingDayLengthMinutes();

            // Get the record ID of the current work day.
            GetCurrentWorkDay();
        }

        public void CreateProjectVersion()
        {
            SQLiteConnection dbConnection = GetConnection();
            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_CREATE_PROJECT_VERSION;
                dbCommand.ExecuteNonQuery();
            }

            FetchCurrentProjectVersion();
        }

        private void GetCurrentWorkDay()
        {
            SQLiteConnection dbConnection = GetConnection();

            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_GET_CURRENT_WORK_DAY;
                using (SQLiteDataReader reader = dbCommand.ExecuteReader(CommandBehavior.SingleRow | CommandBehavior.SingleResult))
                {
                    if (reader.Read())
                    {
                        this.mCurrentWorkDayID = reader.GetInt32(reader.GetOrdinal("DaysWorkedID"));
                        this.mCurrentWorkDayDate = reader.GetDateTime(reader.GetOrdinal("CalendarDate"));
                    }
                    else
                    {
                        this.mCurrentWorkDayID = 0;
                        this.mCurrentWorkDayDate = DateTime.Now.Date;
                    }
                }
            }
        }

        private void FetchWorkingDayLengthMinutes()
        {
            SQLiteConnection dbConnection = GetConnection();

            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_READ_WORK_DAY_LENGTH;
                dbCommand.Parameters.Add(new SQLiteParameter("@projectMetaDataID", PROJECT_METADATA_ID_MINUTES_PER_WORK_DAY));
                using (SQLiteDataReader reader = dbCommand.ExecuteReader(CommandBehavior.SingleRow | CommandBehavior.SingleResult))
                {
                    if (reader.Read())
                    {
                        this.mWorkDayLengthMinutes = reader.GetInt32(reader.GetOrdinal("IntegralValue"));
                    }
                    else
                    {
                        this.mWorkDayLengthMinutes = 450;
                    }
                }
            }
        }

        protected void FetchCurrentProjectVersion()
        {
            SQLiteConnection dbConnection = GetConnection();

            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_GET_CURRENT_PROJECT_VERSION;
                using (SQLiteDataReader reader = dbCommand.ExecuteReader(CommandBehavior.SingleRow | CommandBehavior.SingleResult))
                {
                    if (!reader.Read())
                    {
                        throw new Exception<SQLiteDataAccessExceptionArgs>
                        (
                            new SQLiteDataAccessExceptionArgs(this.DatabaseFilePath, SQLiteDataAccessExceptionArgs.ErrorCode.ProjectVersionNotFound)
                        );
                    }

                    this.mCurrentProjectVersionID = reader.GetInt32(reader.GetOrdinal("ProjectVersionID"));
                }
            }
        }

        /// <summary>
        /// Returns a collection of project versions.
        /// </summary>
        /// <returns>
        /// Returns a collection of <see cref="ProjectVersionDTO"/> project version records.
        /// </returns>
        public IEnumerable<ProjectVersionDTO> GetProjectVersions()
        {
            List<ProjectVersionDTO> result = new List<ProjectVersionDTO>();

            SQLiteConnection dbConnection = GetConnection();

            SortedList<Int32, ProjectVersionDTO> resultCollection = new SortedList<int, ProjectVersionDTO>
            (
                new IntDescendingSortComparer()
            );

            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_GET_ALL_VERSIONS;
                using (SQLiteDataReader reader = dbCommand.ExecuteReader())
                {
                    Int32 colIndexProjectVersionID = reader.GetOrdinal("ProjectVersionID");
                    Int32 colIndexVersionDate = reader.GetOrdinal("VersionDate");

                    while (reader.Read())
                    {
                        Int32 projectVersionID = reader.GetInt32(colIndexProjectVersionID);
                        DateTime versionDate = reader.GetDateTime(colIndexVersionDate);

                        resultCollection.Add(projectVersionID, new ProjectVersionDTO(projectVersionID, versionDate));
                    }
                }
            }
            foreach (var collectionRec in resultCollection)
            {
                result.Add(collectionRec.Value);
            }

            return result;
        }

        public void SetLastUpdateTime()
        {
            SQLiteConnection dbConnection = GetConnection();
            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_UPDATE_LAST_UPDATE_TIME;
                dbCommand.Parameters.Add(new SQLiteParameter("@projectMetaDataID", PROJECT_METADATA_ID_LAST_UPDATE_DATE));
                dbCommand.ExecuteNonQuery();
                this.mDatabaseLastUpdatedDate = DateTime.UtcNow;
            }
        }

        public void FetchLastUpdateTime()
        {
            SQLiteConnection dbConnection = GetConnection();
            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_FETCH_LAST_UPDATE_TIME;
                dbCommand.Parameters.Add(new SQLiteParameter("@projectMetaDataID", PROJECT_METADATA_ID_LAST_UPDATE_DATE));
                dbCommand.ExecuteNonQuery();
                using (SQLiteDataReader reader = dbCommand.ExecuteReader(CommandBehavior.SingleRow | CommandBehavior.SingleResult))
                {
                    if (!reader.Read())
                    {
                        throw new Exception<SQLiteDataAccessExceptionArgs>
                        (
                            new SQLiteDataAccessExceptionArgs(this.DatabaseFilePath, SQLiteDataAccessExceptionArgs.ErrorCode.DatabaseLastUpdateDateTimeNotFound)
                        );
                    }
                    this.mDatabaseLastUpdatedDate = reader.GetDateTime(reader.GetOrdinal("DateTimeValue"));
                }
            }
        }

        public void SetMinutesPerWorkDay(Int32 workDayLengthMinutes)
        {
            SQLiteConnection dbConnection = GetConnection();
            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_STORE_MINUTES_PER_WORK_DAY;
                dbCommand.Parameters.Add(new SQLiteParameter("@projectMetaDataID", PROJECT_METADATA_ID_MINUTES_PER_WORK_DAY));
                dbCommand.Parameters.Add(new SQLiteParameter("@integralValue", workDayLengthMinutes));
                dbCommand.ExecuteNonQuery();
            }
            this.mWorkDayLengthMinutes = workDayLengthMinutes;
        }

        /// <summary>
        ///     Create an archive copy of a current task item.
        /// </summary>
        /// <param name="taskItemID">
        ///     ID of the item to archive.
        /// </param>
        public void ArchiveTaskItem(Int32 taskItemID)
        {
            SQLiteConnection dbConnection = GetConnection();
            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_ARCHIVE_TASK_ITEM;
                dbCommand.Parameters.Add(new SQLiteParameter("@taskItemID", taskItemID));
                dbCommand.Parameters.Add(new SQLiteParameter("@projectVersionID", mCurrentProjectVersionID));
                dbCommand.ExecuteNonQuery();
            }
        }

        public void DeleteTaskItem(Int32 taskItemID, Boolean isDeleted)
        {
            ArchiveTaskItem(taskItemID);

            SQLiteConnection dbConnection = GetConnection();
            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_DELETE_TASK_ITEM;
                dbCommand.Parameters.Add(new SQLiteParameter("@taskItemID", taskItemID));
                dbCommand.Parameters.Add(new SQLiteParameter("@isDeleted", isDeleted ? 1 : 0));
                dbCommand.ExecuteNonQuery();
            }
        }

        public void InsertOrUpdateTaskItem
        (
            Int32 taskItemID,
            Int32 parentTaskItemID,
            String itemDescription,
            Int32 estimatedTimeMinutes,
            Int32 minimumTimeMinutes,
            Int32 maximumTimeMinutes,
            Int32 percentageComplete,
            Int32 timeSpentMinutes
        )
        {
            ArchiveTaskItem(taskItemID);

            SQLiteConnection dbConnection = GetConnection();
            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_STORE_UPDATE_TASK_ITEM;
                dbCommand.Parameters.Add(new SQLiteParameter("@taskItemID", taskItemID));
                if (parentTaskItemID == 0)
                {
                    dbCommand.Parameters.Add(new SQLiteParameter("@parentTaskItemID", DBNull.Value));
                }
                else
                {
                    dbCommand.Parameters.Add(new SQLiteParameter("@parentTaskItemID", parentTaskItemID));
                }
                dbCommand.Parameters.Add(new SQLiteParameter("@itemDescription", itemDescription));
                dbCommand.Parameters.Add(new SQLiteParameter("@estimatedTimeMinutes", estimatedTimeMinutes));
                dbCommand.Parameters.Add(new SQLiteParameter("@minimumTimeMinutes", minimumTimeMinutes));
                dbCommand.Parameters.Add(new SQLiteParameter("@maximumTimeMinutes", maximumTimeMinutes));
                dbCommand.Parameters.Add(new SQLiteParameter("@percentageComplete", percentageComplete));
                dbCommand.Parameters.Add(new SQLiteParameter("@timeSpentMinutes", timeSpentMinutes));
                dbCommand.ExecuteNonQuery();
            }
        }

        public IEnumerable<ProjectTreeItemBase> GetTaskItems(Boolean onlyActiveItems)
        {
            List<ProjectTreeItemBase> result = new List<ProjectTreeItemBase>();

            SQLiteConnection dbConnection = GetConnection();
            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_FETCH_ACTIVE_TASK_ITEMS;
                Dictionary<Int32, ProjectTreeItemBase> taskItems = new Dictionary<Int32, ProjectTreeItemBase>();

                using (SQLiteDataReader reader = dbCommand.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    Int32 colIndexTaskItemID = reader.GetOrdinal("TaskItemID");
                    Int32 colIndexParentTaskItemID = reader.GetOrdinal("ParentTaskItemID");
                    Int32 colIndexItemDescription = reader.GetOrdinal("ItemDescription");
                    Int32 colIndexEstimatedTimeMinutes = reader.GetOrdinal("EstimatedTimeMinutes");
                    Int32 colIndexMinimumTimeMinutes = reader.GetOrdinal("MinimumTimeMinutes");
                    Int32 colIndexMaximumTimeMinutes = reader.GetOrdinal("MaximumTimeMinutes");
                    Int32 colIndexPercentageComplete = reader.GetOrdinal("PercentageComplete");
                    Int32 colIndexTimeSpentMinutes = reader.GetOrdinal("TimeSpentMinutes");

                    while (reader.Read())
                    {
                        taskItems.Add
                        (
                            reader.GetInt32(colIndexTaskItemID),
                            new ProjectTreeBranchItem()
                            {
                                ProjectItemID = reader.GetInt32(colIndexTaskItemID),
                                ItemDescription = reader.GetString(colIndexItemDescription),
                                ParentProjectItemID = reader.IsDBNull(colIndexParentTaskItemID) ? 0 : reader.GetInt32(colIndexParentTaskItemID),
                                EstimatedTimeMinutes = reader.GetInt32(colIndexEstimatedTimeMinutes),
                                MinimumTimeMinutes = reader.GetInt32(colIndexMinimumTimeMinutes),
                                MaximumTimeMinutes = reader.GetInt32(colIndexMaximumTimeMinutes),
                                TimeSpentMinutes = reader.GetInt32(colIndexTimeSpentMinutes),
                                PercentageComplete = reader.GetInt32(colIndexPercentageComplete)
                            }
                        );
                    }
                }
                List<Int32> sortedKeys = taskItems.Keys.ToList();
                sortedKeys.Sort();
                foreach (Int32 key in sortedKeys)
                {
                    result.Add(taskItems[key]);
                }
            }

            return result;
        }

        public void LogWorkDay(DateTime workDayDate)
        {
            SQLiteConnection dbConnection = GetConnection();
            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_LOG_WORK_DAY;
                dbCommand.Parameters.Add(new SQLiteParameter("@calendarDate", workDayDate.Date));
                dbCommand.ExecuteNonQuery();
            }

            GetCurrentWorkDay();
        }

        public DateTime WorkDayDate => this.mCurrentWorkDayDate;

        /// <summary>
        ///     Get the ID of the current work day.
        /// </summary>
        public Int32 WorkDayID => this.mCurrentWorkDayID;

        public IEnumerable<DaysWorkedDTO> GetWorkDays()
        {
            List<DaysWorkedDTO> result = new List<DaysWorkedDTO>();

            SQLiteConnection dbConnection = GetConnection();
            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_GET_WORK_DAYS;
                using (SQLiteDataReader reader = dbCommand.ExecuteReader())
                {
                    Int32 colIndexDaysWorkedID = reader.GetOrdinal("DaysWorkedID");
                    Int32 colIndexCalendarDate = reader.GetOrdinal("CalendarDate");
                    Int32 colIndexProjectPercentageComplete = reader.GetOrdinal("ProjectPercentageComplete");

                    Dictionary<Int32, DaysWorkedDTO> dates = new Dictionary<Int32, DaysWorkedDTO>();

                    while (reader.Read())
                    {
                        dates.Add
                        (
                            reader.GetInt32(colIndexDaysWorkedID), 
                            new DaysWorkedDTO
                            (
                                reader.GetInt32(colIndexDaysWorkedID), 
                                reader.GetDateTime(colIndexCalendarDate),
                                reader.GetInt32(colIndexProjectPercentageComplete)
                            )
                        );
                    }

                    List<Int32>sortedKeys = dates.Keys.ToList();
                    sortedKeys.Sort();
                    foreach (Int32 key in sortedKeys)
                    {
                        result.Add(dates[key]);
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     Update the percentage of the project that is complete on a specific work day.
        /// </summary>
        /// <projectPercentageComplete>
        ///     Percentage of the project that is complete.
        /// </projectPercentageComplete>
        public void SetWorkDayCompletionPercentage(Int32 projectPercentageComplete)
        {
            if (this.mCurrentWorkDayID < 1)
            {
                throw new Exception<SQLiteDataAccessExceptionArgs>(new SQLiteDataAccessExceptionArgs(SQLiteDataAccessExceptionArgs.ErrorCode.NoCurrentWorkDaySet));
            }

            SQLiteConnection dbConnection = GetConnection();
            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_UPDATE_WORK_DAY_TIME_SPENT;
                dbCommand.Parameters.Add(new SQLiteParameter("@daysWorkedID", this.mCurrentWorkDayID));
                dbCommand.Parameters.Add(new SQLiteParameter("@projectPercentageComplete", Math.Max(0, projectPercentageComplete)));
                dbCommand.ExecuteNonQuery();
            }
            
        }

        public Int32 GetHighestTaskItemID()
        {
            SQLiteConnection dbConnection = GetConnection();
            using (SQLiteCommand dbCommand = dbConnection.CreateCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = SQL_GET_MAX_TASK_ITEM_ID;
                using (SQLiteDataReader reader = dbCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32(reader.GetOrdinal("TaskItemID"));
                    }

                    return 0;
                }
            }
        }

        #endregion IDataAccess implementation

        #region IDisposable implementation
        public void Dispose()
        {
            if (mSQLiteConnection != null)
            {
                mSQLiteConnection.Dispose();
                mSQLiteConnection = null;
            }
        }
        #endregion IDisposable implementation

        #region Public properties
        /// <summary>
        ///     Path to the file that contains the data.
        /// </summary>
        public String DatabaseFilePath
        {
            get { return this.mDatabaseFilePath; }
            set { this.mDatabaseFilePath = value; }
        }

        /// <summary>
        /// Returns the schema version of the data store.
        /// </summary>
        public Int32 SchemaVersion => this.mSchemaVersion;

        /// <summary>
        /// Get the date and time when the store was last updated.
        /// </summary>
        public DateTime LastUpdateTime => this.mDatabaseLastUpdatedDate;

        /// <summary>
        /// Gets the version number of the project.
        /// </summary>
        public Int32 ProjectVersion => this.mCurrentProjectVersionID;

        /// <summary>
        /// Gets the start date of the project.
        /// </summary>
        public DateTime ProjectStartDate => this.mProjectStartDate;

        /// <summary>
        /// Gets the number of minutes per work day.
        /// </summary>
        public Int32 MinutesPerWorkDay => this.mWorkDayLengthMinutes;
        #endregion Public properties

        #region Private helper methods
        private SQLiteConnection GetConnection()
        {
            if (String.IsNullOrEmpty(DatabaseFilePath))
            {
                throw new Exception<SQLiteDataAccessExceptionArgs>(new SQLiteDataAccessExceptionArgs(SQLiteDataAccessExceptionArgs.ErrorCode.EmptyFilePath));
            }

            if (mSQLiteConnection != null)
            {
                if (mSQLiteConnection.State != ConnectionState.Open)
                {
                    mSQLiteConnection.Dispose();
                    mSQLiteConnection.Close();
                }
            }

            if (mSQLiteConnection == null)
            {
                SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder(Settings.Default.SQLiteConnectionStringTemplate);
                connectionString.DataSource = this.mDatabaseFilePath;
                mSQLiteConnection = new SQLiteConnection(connectionString.ToString());
                mSQLiteConnection.Open();
            }

            return this.mSQLiteConnection;
        }
        #endregion Private helper methods 

    } // class SQLiteDataAccess
} // namespace ProjectEstimationTool.Model
