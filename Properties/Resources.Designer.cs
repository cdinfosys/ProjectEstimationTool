﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProjectEstimationTool.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ProjectEstimationTool.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unsaved Changes.
        /// </summary>
        internal static string CaptionUnsavedChanges {
            get {
                return ResourceManager.GetString("CaptionUnsavedChanges", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Task.
        /// </summary>
        internal static string DefaultNewTaksDescription {
            get {
                return ResourceManager.GetString("DefaultNewTaksDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to TO DO: Project Description.
        /// </summary>
        internal static string DefaultRootTaksDescription {
            get {
                return ResourceManager.GetString("DefaultRootTaksDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Select a date that is after {0}.
        /// </summary>
        internal static string ErrorDateBeforeLastWorkDay {
            get {
                return ResourceManager.GetString("ErrorDateBeforeLastWorkDay", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enter a value that is below {0:G2} hours.
        /// </summary>
        internal static string ErrorGreaterThanMaxHours {
            get {
                return ResourceManager.GetString("ErrorGreaterThanMaxHours", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enter a value that is below {0:G0} minutes.
        /// </summary>
        internal static string ErrorGreaterThanMaxMinutes {
            get {
                return ResourceManager.GetString("ErrorGreaterThanMaxMinutes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enter a value that is over  {0:G2} hours..
        /// </summary>
        internal static string ErrorLessThanMinimumHours {
            get {
                return ResourceManager.GetString("ErrorLessThanMinimumHours", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enter a value that is over {0:G2} minutes.
        /// </summary>
        internal static string ErrorLessThanMinimumMinutes {
            get {
                return ResourceManager.GetString("ErrorLessThanMinimumMinutes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Value must be numeric.
        /// </summary>
        internal static string ErrorNotNumeric {
            get {
                return ResourceManager.GetString("ErrorNotNumeric", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Schema version {0} is not supported by this version of the program.
        /// </summary>
        internal static string ErrorUnsupportedSchemaVersion {
            get {
                return ResourceManager.GetString("ErrorUnsupportedSchemaVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Project Plan.
        /// </summary>
        internal static string ExcelSheetName {
            get {
                return ResourceManager.GetString("ExcelSheetName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Leaf item cannot have a child.
        /// </summary>
        internal static string LeafTreeItemExceptionMessage {
            get {
                return ResourceManager.GetString("LeafTreeItemExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hours.
        /// </summary>
        internal static string MeasurementHoursLabel {
            get {
                return ResourceManager.GetString("MeasurementHoursLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Minutes.
        /// </summary>
        internal static string MeasurementMinutesLabel {
            get {
                return ResourceManager.GetString("MeasurementMinutesLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No error.
        /// </summary>
        internal static string NoError {
            get {
                return ResourceManager.GetString("NoError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .xlsx.
        /// </summary>
        internal static string ProjectExportDefaultExtension {
            get {
                return ResourceManager.GetString("ProjectExportDefaultExtension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Excel Spreadsheets|*.xlsx|All Files|*.*.
        /// </summary>
        internal static string ProjectExportFileNameFilter {
            get {
                return ResourceManager.GetString("ProjectExportFileNameFilter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .petproj.
        /// </summary>
        internal static string ProjectFileDefaultExtension {
            get {
                return ResourceManager.GetString("ProjectFileDefaultExtension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Estimation Tool Projects|*.petproj|All Files|*.*.
        /// </summary>
        internal static string ProjectFileOpenSaveFilter {
            get {
                return ResourceManager.GetString("ProjectFileOpenSaveFilter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Changes to the project have not been saved. Would you like to save these changes?.
        /// </summary>
        internal static string QuestionSaveChanges {
            get {
                return ResourceManager.GetString("QuestionSaveChanges", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This property can only be set on a leaf item.
        /// </summary>
        internal static string SetBranchTreeItemPropertyExceptionMessage {
            get {
                return ResourceManager.GetString("SetBranchTreeItemPropertyExceptionMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Database last update date and time record is missing.
        /// </summary>
        internal static string SQLiteDataAccess_DatabaseLastUpdateDateTimeNotFound {
            get {
                return ResourceManager.GetString("SQLiteDataAccess_DatabaseLastUpdateDateTimeNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Database schema version number not found.
        /// </summary>
        internal static string SQLiteDataAccess_DatabaseSchemaVersionNotFound {
            get {
                return ResourceManager.GetString("SQLiteDataAccess_DatabaseSchemaVersionNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Path to the database file must be specified.
        /// </summary>
        internal static string SQLiteDataAccess_EmptyFilePath {
            get {
                return ResourceManager.GetString("SQLiteDataAccess_EmptyFilePath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid file path.
        /// </summary>
        internal static string SQLiteDataAccess_InvalidFilePath {
            get {
                return ResourceManager.GetString("SQLiteDataAccess_InvalidFilePath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Set up a work day for logging time against.
        /// </summary>
        internal static string SQLiteDataAccess_NoCurrentWorkDaySet {
            get {
                return ResourceManager.GetString("SQLiteDataAccess_NoCurrentWorkDaySet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Project version number not found.
        /// </summary>
        internal static string SQLiteDataAccess_ProjectVersionNotFound {
            get {
                return ResourceManager.GetString("SQLiteDataAccess_ProjectVersionNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unsupported schema version.
        /// </summary>
        internal static string SQLiteDataAccess_UnsupportedDatabaseSchemaVersion {
            get {
                return ResourceManager.GetString("SQLiteDataAccess_UnsupportedDatabaseSchemaVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enter a value for this field.
        /// </summary>
        internal static string TaskItemValidation_EmptyField {
            get {
                return ResourceManager.GetString("TaskItemValidation_EmptyField", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Undefined error.
        /// </summary>
        internal static string UndefinedError {
            get {
                return ResourceManager.GetString("UndefinedError", resourceCulture);
            }
        }
    }
}
