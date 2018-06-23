using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForecastingConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<double, Row> dataSet = ParseDataSet(@"D:\github\ForecastingConsole\ForecastingConsole\SwordForecastingCSV.csv", ",");
            //Ses ses = new Ses(dataSet);
            //double sesAlpha = ses.GetBestAlpha();
            //ses.CalculateFutureForeCast(sesAlpha, 37, 48);

            Des des = new Des(dataSet);
            //double  = ses.GetBestAlpha();
            //des.CalculateFutureForeCast(sesAlpha, 37, 48);
        }

        public static Dictionary<double, Row> ParseDataSet(string path, string seperator)
        {
            string[] lines = File.ReadAllLines(path);
            Dictionary<double, Row> dataSet = new Dictionary<double, Row>();
            // row
            foreach(string line in lines.Skip(1).Take(36))
            {
                string[] lineSplit = line.Split(',');
                double time = Convert.ToDouble(lineSplit[0]);
                dataSet.Add(time, new Row());
                dataSet[time].Demand = Convert.ToDouble(lineSplit[1]);

            }
            return dataSet;
        }
    }
}
