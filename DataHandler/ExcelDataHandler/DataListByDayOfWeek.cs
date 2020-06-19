using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;

namespace ExcelDataHandler
{
    public class DataListByDayOfWeek
    {
        public List<AlterVolgaDataRow> RawDataList { get; set; }
        public DayOfWeek WeekDay { get; set; }

        public DataListByDayOfWeek(DayOfWeek weekDay, List<AlterVolgaDataRow> rawDataList)
        {
            WeekDay = weekDay;
            RawDataList = rawDataList;
        }

        public DataListByDayOfWeek(DayOfWeek weekDay)
        {
            WeekDay = weekDay;
            RawDataList = new List<AlterVolgaDataRow>();
        }
        
    }
}