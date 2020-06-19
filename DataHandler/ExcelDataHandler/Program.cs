using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace ExcelDataHandler
{
    class Program
    {
        private static ExcelPackage _excelPackage = new ExcelPackage(new FileInfo(@"D:\Универ\Диплом.xlsx"));
        private static ExcelWorksheet _workSheet = _excelPackage.Workbook.Worksheets[1];
        
        
        static void Main(string[] args)
        {

            var dataList = GetAlterVolgaData();
            var totalConsList = ListOfTotalConsData();
            var rawDataGroupedByWeekDay = dataList.GroupBy(row => row.ReadingDate.DayOfWeek)
                .ToDictionary(q=>q.Key,q=>q.ToList())
                .Select(row => new DataListByDayOfWeek(row.Key,row.Value)).ToList();
            
            var totalDataGroupedByWeekDay = totalConsList.GroupBy(row => row.ReadingDate.DayOfWeek)
                .ToDictionary(q=>q.Key,q=>q.ToList())
                .Select(row => new DataListByDayOfWeek(row.Key,row.Value)).ToList();
            
            
            
            var listOfAverageDataByDayOfWeek = GetAveragesGroupedByHoursAndMinutes(new List<DataListByDayOfWeek>(rawDataGroupedByWeekDay));
            
            var differencesByDayOfWeek =
                GetDifferencesAveragesCurrent(rawDataGroupedByWeekDay, listOfAverageDataByDayOfWeek);
            
            var averageDifferencesByDayOfWeek = GetAveragesGroupedByHoursAndMinutes(differencesByDayOfWeek);

            var totalConsDifferencesByDayOfWeek =
                GetDifferencesAveragesCurrent(totalDataGroupedByWeekDay, listOfAverageDataByDayOfWeek);

            var differencesTotalAverageByDayOfWeek =
                GetDifferencesAveragesCurrent(totalConsDifferencesByDayOfWeek, averageDifferencesByDayOfWeek);

            foreach (var row in differencesTotalAverageByDayOfWeek)
            {
                //here
                row.RawDataList.ForEach(diff => diff.SecondEntryReading = diff.SecondEntryReading < 0 
                    ? diff.SecondEntryReading = 0 
                    : diff.SecondEntryReading);
            }
            
            var path = @"D:\Универ\Данные альтернативы\Вводный_счетчик№2_Результат.xlsx";
            
            using (var package = new ExcelPackage(new FileInfo(path)))
            {
                var ws = package.Workbook.Worksheets[1];
                var i = 1;
                var a = "A";
                var b = "B";
                foreach (var row in differencesTotalAverageByDayOfWeek.SelectMany(diff=>diff.RawDataList)
                    .OrderBy(t=>t.ReadingDate))
                {
                    var strDate = a + i.ToString();
                    var strValue = b + i.ToString();
                    ws.Cells[strDate].Value = row.ReadingDate;
                    ws.Cells[strValue].Value = row.SecondEntryReading; // here
                    i++;
                }
                package.Save();
            }
            
            _excelPackage.Dispose();
        }

        

        static List<AlterVolgaDataRow> GetAlterVolgaData()
        {
            var listAlterVolgaData = new List<AlterVolgaDataRow>();
            for (int row = 2; row <= _workSheet.Dimension.End.Row; row++)
            {
                listAlterVolgaData.Add(new AlterVolgaDataRow()
                {
                    ReadingDate = DateTime.Parse(_workSheet.Cells[row,1].Value.ToString()),
                    TotalConsumptionReading = Convert.ToDouble(_workSheet.Cells[row,2].Value.ToString()),
                    AlterVolgaReading = Convert.ToDouble(_workSheet.Cells[row,3].Value.ToString()),
                    FirstEntryReading = Convert.ToDouble(_workSheet.Cells[row,4].Value.ToString()),
                    SecondEntryReading= Convert.ToDouble(_workSheet.Cells[row,5].Value.ToString()),
                    ShockFreezingReading= Convert.ToDouble(_workSheet.Cells[row,6].Value.ToString()),
                    SpiralShockReading= Convert.ToDouble(_workSheet.Cells[row,7].Value.ToString()),
                    TunnelShockReading= Convert.ToDouble(_workSheet.Cells[row,8].Value.ToString()),
                });
            }

            return listAlterVolgaData;
        }
        
        static List<DataListByDayOfWeek> GetDifferencesAveragesCurrent(List<DataListByDayOfWeek> listOfDataByDayWeek,
            List<AveragesByDayOfWeek> listOfAveragesByDayWeek)
        {
            var listOfDifferencesAveragesCurrentByDayOfWeek = new List<DataListByDayOfWeek>(); 
            foreach (var dayOfWeek in listOfDataByDayWeek.Select(row=>row.WeekDay))
            {
                var listOfDifferencesAveragesCurrentOfSpecificDayOfWeek = new DataListByDayOfWeek(dayOfWeek); // creating object for keep information about specific day of week
                var appropriateListOfAverages = listOfAveragesByDayWeek
                    .Single(avg => avg.WeekDay == dayOfWeek).RowAveragesDataList;
                foreach (var row in listOfDataByDayWeek.Single(avg=>avg.WeekDay == dayOfWeek).RawDataList)
                {
                    var appropriateAverage = appropriateListOfAverages
                                                 .SingleOrDefault(avg =>
                                                     avg.Hour == row.ReadingDate.Hour &&
                                                     avg.Minute == row.ReadingDate.Minute)
                                             ?? new AlterVolgaAverages(row.ReadingDate.Hour, row.ReadingDate.Minute)
                                             {
                                                 TotalConsumptionReading = row.TotalConsumptionReading,
                                                 AlterVolgaReading = row.AlterVolgaReading,
                                                 FirstEntryReading = row.FirstEntryReading,
                                                 SecondEntryReading = row.SecondEntryReading,
                                                 ShockFreezingReading = row.ShockFreezingReading,
                                                 SpiralShockReading = row.SpiralShockReading,
                                                 TunnelShockReading = row.TunnelShockReading
                                             }; // The program shouldn't show us that something goes wrong so differences will be zeroes

                    listOfDifferencesAveragesCurrentOfSpecificDayOfWeek.RawDataList.Add(new AlterVolgaDataRow()
                    {
                        ReadingDate = row.ReadingDate,
                        TotalConsumptionReading = row.TotalConsumptionReading - appropriateAverage.TotalConsumptionReading,
                        AlterVolgaReading = row.AlterVolgaReading - appropriateAverage.AlterVolgaReading,
                        FirstEntryReading = row.FirstEntryReading - appropriateAverage.FirstEntryReading,
                        SecondEntryReading = row.SecondEntryReading - appropriateAverage.SecondEntryReading,
                        ShockFreezingReading = row.ShockFreezingReading - appropriateAverage.ShockFreezingReading,
                        SpiralShockReading = row.SpiralShockReading - appropriateAverage.SpiralShockReading,
                        TunnelShockReading = row.TunnelShockReading - appropriateAverage.TunnelShockReading
                    });
                }
                listOfDifferencesAveragesCurrentByDayOfWeek.Add(listOfDifferencesAveragesCurrentOfSpecificDayOfWeek);
            }

            return listOfDifferencesAveragesCurrentByDayOfWeek;
        }
        
        private static List<AveragesByDayOfWeek> GetAveragesGroupedByHoursAndMinutes(List<DataListByDayOfWeek> listOfDataByDayWeek)
        {
            var listOfAverageHoursDateByDayOfWeek = new List<AveragesByDayOfWeek>();
            foreach (var dayOfWeek in listOfDataByDayWeek.Select(row => row.WeekDay))
            {
                var listOfAverageHoursDate = new AveragesByDayOfWeek(dayOfWeek);
                
                var dataListForSpecificWeekDay = listOfDataByDayWeek
                    .Single(row => row.WeekDay == dayOfWeek)
                    .RawDataList;
                
                for (int hour = 0; hour <= 23; hour++)
                {
                    var zeroMinutesData = dataListForSpecificWeekDay.Where(row =>
                        row.ReadingDate.Hour == hour && row.ReadingDate.Minute == 0);
                    var averageForZero = new AlterVolgaAverages(hour, 0);
                    averageForZero.GetAverages(zeroMinutesData);
                    listOfAverageHoursDate.RowAveragesDataList.Add(averageForZero);

                    var fifteenMinutesData = dataListForSpecificWeekDay.Where(row =>
                        row.ReadingDate.Hour == hour && row.ReadingDate.Minute == 15);
                    var averageForFifteen = new AlterVolgaAverages(hour, 15);
                    averageForFifteen.GetAverages(fifteenMinutesData);
                    listOfAverageHoursDate.RowAveragesDataList.Add(averageForFifteen);

                    var thirtyMinutesData = dataListForSpecificWeekDay.Where(row =>
                        row.ReadingDate.Hour == hour && row.ReadingDate.Minute == 30);
                    var averageForThirty = new AlterVolgaAverages(hour, 30);
                    averageForThirty.GetAverages(thirtyMinutesData);
                    listOfAverageHoursDate.RowAveragesDataList.Add(averageForThirty);

                    var fourtyFiveMinutesData = dataListForSpecificWeekDay.Where(row =>
                        row.ReadingDate.Hour == hour && row.ReadingDate.Minute == 45);
                    var averageForFourtyFive = new AlterVolgaAverages(hour, 45);
                    averageForFourtyFive.GetAverages(fourtyFiveMinutesData);
                    listOfAverageHoursDate.RowAveragesDataList.Add(averageForFourtyFive);
                }
                listOfAverageHoursDateByDayOfWeek.Add(listOfAverageHoursDate);
            }

            return listOfAverageHoursDateByDayOfWeek;
        }
        
        private static List<AlterVolgaDataRow> _currentValues = new List<AlterVolgaDataRow>()
        {
            new AlterVolgaDataRow(){
                ReadingDate = new DateTime(2020,5,1,0,0, 0),
                TotalConsumptionReading = 25.97,
                AlterVolgaReading = 20.75,	
                FirstEntryReading = 3.43,
                SecondEntryReading = 1.80,
                ShockFreezingReading = 0.11,
                SpiralShockReading = 0.03,
                TunnelShockReading = 0.00
            },
            new AlterVolgaDataRow()
            {
                ReadingDate = new DateTime(2020,5,1,0,15, 0),
                TotalConsumptionReading = 26.68,
                AlterVolgaReading = 22.28,	
                FirstEntryReading = 2.55,
                SecondEntryReading = 1.84,
                ShockFreezingReading = 0.11,
                SpiralShockReading = 0.03,
                TunnelShockReading = 0.00
            },
            new AlterVolgaDataRow()
            {
                ReadingDate = new DateTime(2020,5,1,0,30, 0),
                TotalConsumptionReading = 29.04,
                AlterVolgaReading = 24.27,	
                FirstEntryReading = 2.84,
                SecondEntryReading = 1.93,
                ShockFreezingReading = 0.11,
                SpiralShockReading = 0.03,
                TunnelShockReading = 0.00

            },
            new AlterVolgaDataRow()
            {
                ReadingDate = new DateTime(2020,5,1,0,45, 0),
                TotalConsumptionReading = 27.96,
                AlterVolgaReading = 22.68,	
                FirstEntryReading = 3.36,
                SecondEntryReading = 1.92,
                ShockFreezingReading = 0.11,
                SpiralShockReading = 0.03,
                TunnelShockReading = 0.00

            }
        };

        private static List<AlterVolgaDataRow> ListOfTotalConsData()
        {
            var excelPackageOfTotalCons = new ExcelPackage(new FileInfo(@"D:\Универ\Данные альтернативы\Вводный_счетчик№2.xlsx"));
            var workSheetOfTotalConsExcel = excelPackageOfTotalCons.Workbook.Worksheets[1];
            var listOfTotalConsData = new List<AlterVolgaDataRow>();
            for (int row = 2; row <= workSheetOfTotalConsExcel.Dimension.End.Row; row++)
            {
                listOfTotalConsData.Add(new AlterVolgaDataRow()
                {
                    ReadingDate = DateTime.Parse(workSheetOfTotalConsExcel.Cells[row,1].Value.ToString()),
                    SecondEntryReading = Convert.ToDouble(workSheetOfTotalConsExcel.Cells[row,2].Value.ToString()), //here
                });
            }
            return listOfTotalConsData;
        }
    }
}