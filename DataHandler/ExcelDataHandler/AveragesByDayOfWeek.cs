using System;
using System.Collections.Generic;

namespace ExcelDataHandler
{
    public class AveragesByDayOfWeek
    {
        public List<AlterVolgaAverages> RowAveragesDataList { get; set; }
        public DayOfWeek WeekDay { get; set; }
        public AveragesByDayOfWeek(DayOfWeek weekDay)
        {
            WeekDay = weekDay;
            RowAveragesDataList = new List<AlterVolgaAverages>();
        }
    }
}