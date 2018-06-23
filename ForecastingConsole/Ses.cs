using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForecastingConsole
{
    class Ses
    {
        private Dictionary<double, Row> _data;
        private readonly int _defaultDataLength;
        public Ses(Dictionary<double, Row> data)
        {
            _data = data;
            _defaultDataLength = data.Count;
            Initialize();
        }

        private void Initialize()
        {
            double sum = 0;
            int count = 12;
            Row row = _data[1];
            foreach(KeyValuePair<double, Row> pair in _data.Take(count))
            {
                sum += pair.Value.Demand;
            }
            row.Smoothed = sum / count;
            row.SquaredErrors = Math.Pow(row.Smoothed - row.Demand, 2);
        }
        private void CalculateRows(double alpha)
        {
            foreach (KeyValuePair<double, Row> pair in _data.Skip(1))
            {
                Row prevRow = _data[pair.Key - 1];
                Row currentRow = pair.Value;
                CalculateSmoothed(prevRow, currentRow, alpha);
                CalculateSquaredErrors(currentRow);
            }
        }

        private void CalculateSmoothed(Row prevRow, Row currentRow, double alpha)
        {
            currentRow.Smoothed = alpha * prevRow.Demand + (1 - alpha) * prevRow.Smoothed;
        }

        private void CalculateSquaredErrors(Row currentRow)
        {
            currentRow.SquaredErrors = Math.Pow(currentRow.Smoothed - currentRow.Demand, 2);
        }

        private double CalculateStandardError()
        {
            double sum = 0;
            foreach (KeyValuePair<double, Row> pair in _data)
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
            while (alpha <= lastAlpha)
            {
                CalculateRows(alpha);
                double error = CalculateStandardError();
                // if current error is less than the lowest error
                if (error < best.Item2)
                {
                    best = new Tuple<double, double>(alpha, error);
                }
                alpha += 0.1;
            }

            return best.Item1;
        }

        public void CalculateFutureForeCast(double alpha, int begin, int end)
        {
            CalculateRows(alpha);
            Row lastRow = _data[_defaultDataLength];
            double forecast = alpha * lastRow.Demand + (1 - alpha) * lastRow.Smoothed;
            for (int i = begin; i <= end; i++)
            {
                _data.Add(i, new Row { Smoothed = forecast });
            }
        }
    }
}
