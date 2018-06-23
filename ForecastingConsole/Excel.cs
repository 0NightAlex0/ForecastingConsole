using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
namespace ForecastingConsole
{
    class Excel
    {
        string path;
        _Application excel = new Application();
        Workbook workbook;
        Worksheet worksheet;
        Range range;
        private int _timeColumn = 1;
        private int _demandColumn = 2;

        public Excel(string path, int sheet)
        {
            this.path = path;
            workbook = excel.Workbooks.Open(path);
            worksheet = workbook.Worksheets[sheet];
            range = worksheet.UsedRange;
        }
        // needs to fix this
        public Dictionary<double, SesRow> GetSes(int rowCount, int columnCount)
        {
            Dictionary<double, SesRow> row = new Dictionary<double, SesRow>();
            // data starts at second row
            for (int i = 1; i <= rowCount; i++)
            {
                for (int j = 1; j <= columnCount; j++)
                {
                    //write the value to the console
                    dynamic cell = range.Cells[i+1, j];
                    if (cell != null && cell.Value2 != null)
                    {
                        // time
                        if (j == _timeColumn)
                        {
                            row.Add(cell.Value2, new SesRow());
                        }
                        // demand
                        else if (j == _demandColumn)
                        {
                            // dirty way to connect the time and its demand
                            row[i].Demand = cell.Value2;
                        }
                    }
                }
            }
            return row;
        }

        public double GetCell(int row, int column)
        {
            return range.Cells[row, column].Value2;
        }

        public void End()
        {
            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();


            // all COM objects must be referenced and released individually

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(range);
            Marshal.ReleaseComObject(worksheet);

            //close and release
            workbook.Close();
            Marshal.ReleaseComObject(workbook);

            //quit and release
            excel.Quit();
            Marshal.ReleaseComObject(excel);
        }
    }
}
