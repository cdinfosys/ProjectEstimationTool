using System;

namespace ProjectEstimationTool.Classes
{
    /// <summary>
    ///     DTO for records retrieved from the DaysWorked table.
    /// </summary>
    public class DaysWorkedDTO
    {
        private readonly Int32 mDaysWorkedID;
        private readonly DateTime mCalendarDate;
        private readonly Int32 mTimeSpentMinutes;

        public DaysWorkedDTO
        (
            Int32 daysWorkedID,
            DateTime calendarDate,
            Int32 timeSpentMinutes
        )
        {
            this.mDaysWorkedID = daysWorkedID;
            this.mCalendarDate = calendarDate.Date;
            this.mTimeSpentMinutes = timeSpentMinutes;
        }

        public Int32 DaysWorkedID => mDaysWorkedID;
        public DateTime CalendarDate => mCalendarDate;
        public Int32 TimeSpentMinutes => mTimeSpentMinutes;

    } // class DaysWorkedDTO
} // namespace ProjectEstimationTool.Classes
