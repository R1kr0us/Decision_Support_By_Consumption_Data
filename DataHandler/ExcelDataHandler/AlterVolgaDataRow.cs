using System;
using System.ComponentModel;

namespace ExcelDataHandler
{
    public class AlterVolgaDataRow
    {
        public DateTime ReadingDate { get; set; }
        public double TotalConsumptionReading { get; set; }
        public double AlterVolgaReading { get; set; }
        public double FirstEntryReading { get; set; }
        public double SecondEntryReading { get; set; }
        public double ShockFreezingReading { get; set; }
        public double SpiralShockReading { get; set; }
        public double TunnelShockReading { get; set; }
    }
}