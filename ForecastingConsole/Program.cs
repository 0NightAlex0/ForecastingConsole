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
            Dictionary<double, SesRow> dataSet = ParseDataSet(@"D:\github\ForecastingConsole\ForecastingConsole\SwordForecastingCSV.csv", ",");
            Ses ses = new Ses(dataSet);
            var x = ses.GetBestAlpha();
            Console.WriteLine(x);
        }

        public static Dictionary<double, SesRow> ParseDataSet(string path, string seperator)
        {
            string[] lines = File.ReadAllLines(path);
            Dictionary<double, SesRow> dataSet = new Dictionary<double, SesRow>();
            // row
            foreach(string line in lines.Skip(1).Take(36))
            {
                string[] lineSplit = line.Split(',');
                double time = Convert.ToDouble(lineSplit[0]);
                dataSet.Add(time, new SesRow());
                dataSet[time].Demand = Convert.ToDouble(lineSplit[1]);

            }
            return dataSet;
        }
    }
}
