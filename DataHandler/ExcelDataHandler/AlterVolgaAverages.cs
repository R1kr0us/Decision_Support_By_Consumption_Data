using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;

namespace ExcelDataHandler
{
    public class AlterVolgaAverages
    {
        public int Hour { get; set; }
        public int Minute { get; set; }
        public double TotalConsumptionReading { get; set; }
        public double AlterVolgaReading { get; set; }
        public double FirstEntryReading { get; set; }
        public double SecondEntryReading { get; set; }
        public double ShockFreezingReading { get; set; }
        public double SpiralShockReading { get; set; }
        public double TunnelShockReading { get; set; }

        public AlterVolgaAverages(int hour, int minute)
        {
            Hour = hour;
            Minute = minute;
        }

        public void GetAverages(IEnumerable<AlterVolgaDataRow> dataRow)
        {
            TotalConsumptionReading = dataRow.Select(row => row.TotalConsumptionReading).Where(data=>data >= 0).Sum() /
                                      dataRow.Select(row=>row.TotalConsumptionReading).Count(data => data >= 0);
            AlterVolgaReading = dataRow.Select(row => row.AlterVolgaReading).Where(data=>data >= 0).Sum() /
                                dataRow.Select(row => row.AlterVolgaReading).Count(data => data >= 0);;
            FirstEntryReading = dataRow.Select(row => row.FirstEntryReading).Where(data=>data >= 0).Sum() /
                                dataRow.Select(row => row.FirstEntryReading).Count(data => data >= 0);;
            SecondEntryReading = dataRow.Select(row => row.SecondEntryReading).Where(data=>data >= 0).Sum() /
                                 dataRow.Select(row => row.SecondEntryReading).Count(data => data >= 0);;
            ShockFreezingReading = dataRow.Select(row => row.ShockFreezingReading).Where(data=>data >= 0).Sum() /
                                   dataRow.Select(row => row.ShockFreezingReading).Count(data => data >= 0);;
            SpiralShockReading = dataRow.Select(row => row.SpiralShockReading).Where(data=>data >= 0).Sum() /
                                 dataRow.Select(row => row.SpiralShockReading).Count(data => data >= 0);;
            TunnelShockReading = dataRow.Select(row => row.TunnelShockReading).Where(data=>data >= 0).Sum() /
                                 dataRow.Select(row => row.TunnelShockReading).Count(data => data >= 0);;
        }
        
    }
}