using System;

namespace ProjectEstimationTool.Classes
{
    /// <summary>
    ///     DTO for records retrieved from the DaysWorked table.
    /// </summary>
    class DaysWorkedDTO
    {
        private readonly Int32 mDaysWorkedID;
        private readonly DateTime mCalendarDate;
        private Int32 mProjectPercentageComplete;

        public DaysWorkedDTO
        (
            Int32 daysWorkedID,
            DateTime calendarDate,
            Int32 projectPercentageComplete
        )
        {
            this.mDaysWorkedID = daysWorkedID;
            this.mCalendarDate = calendarDate.Date;
            this.mProjectPercentageComplete = projectPercentageComplete;
        }

        public Int32 DaysWorkedID => mDaysWorkedID;
        public DateTime CalendarDate => mCalendarDate;

        public Int32 ProjectPercentageComplete
        {
            get
            {
                return this.mProjectPercentageComplete;
            }
            set 
            {
                this.mProjectPercentageComplete = value;
            }
        }

    } // class DaysWorkedDTO
} // namespace ProjectEstimationTool.Classes
