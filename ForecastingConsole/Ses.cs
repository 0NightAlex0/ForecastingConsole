using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForecastingConsole
{
    class Ses
    {
        Dictionary<double, SesRow> _data;
        public Ses(Dictionary<double, SesRow> data)
        {
            _data = data;
            Initialize();
        }

        private void Initialize()
        {
            double sum = 0;
            int count = 12;
            foreach(KeyValuePair<double, SesRow> pair in _data.Take(count))
            {
                sum += pair.Value.Demand;
            }
            _data[1].Forecast = sum / count;
        }

        private void CalculateForecast(double alpha)
        {
            SesRow prevRow;
            foreach(KeyValuePair<double, SesRow> pair in _data.Skip(1))
            {
                prevRow = _data[pair.Key - 1];
                pair.Value.Forecast = alpha * prevRow.Demand + (1 - alpha) * prevRow.Forecast;
            }
        }

        private void CalculateSquaredErrors()
        {
            SesRow row;
            foreach (KeyValuePair<double, SesRow> pair in _data)
            {
                row = pair.Value;
                row.SquaredErrors = Math.Pow(row.Forecast - row.Demand, 2);
            }
        }

        private double CalculateStandardError()
        {
            double sum = 0;
            foreach (KeyValuePair<double, SesRow> pair in _data)
            {
                sum += pair.Value.SquaredErrors;
            }

            return Math.Sqrt(sum / (_data.Count - 1));
        }

        public double GetBestAlpha()
        {
            Tuple<double, double> best = new Tuple<double, double>(0, Double.MaxValue);
            double alpha = 0.1;
            double lastAlpha = 0.9;
            while(alpha <= lastAlpha)
            {
                CalculateForecast(alpha);
                CalculateSquaredErrors();
                double error = CalculateStandardError();
                // if current error is less than the lowest error
                if(error < best.Item2)
                {
                    best = new Tuple<double, double>(alpha, error);
                }
                alpha += 0.1;
            }

            return best.Item1;
        }
    }
}
