using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForecastingConsole
{
    class Des
    {
        private Dictionary<double, Row> _data;
        private readonly int _defaultDataLength;
        public Des(Dictionary<double, Row> data)
        {
            _data = data;
            _defaultDataLength = data.Count;
            Initialize();
            CalculateRows(0.5, 0.5);
            
        }

        private void Initialize()
        {
            Row secondRow = _data[2];
            secondRow.Smoothed = secondRow.Demand;
            secondRow.Trend = secondRow.Demand - _data[1].Demand;
        }

        private void CalculateRows(double alpha, double beta)
        {
            foreach (KeyValuePair<double, Row> pair in _data.Skip(2))
            {
                Row prevRow = _data[pair.Key - 1];
                Row currentRow = pair.Value;
                CalculateSmoothed(prevRow, currentRow, alpha);
                CalculateTrend(prevRow, currentRow, beta);
                CalculateForecast(prevRow, currentRow);
                CalculateSquaredErrors(currentRow);
            }
        }

        private void CalculateSmoothed(Row prevRow, Row currentRow, double alpha)
        {
            currentRow.Smoothed = alpha * currentRow.Demand + (1 - alpha) * (prevRow.Smoothed + prevRow.Trend);
        }

        private void CalculateTrend(Row prevRow, Row currentRow, double beta)
        {
            currentRow.Trend = beta * (currentRow.Smoothed - prevRow.Smoothed) + (1 - beta) * prevRow.Trend;
        }

        private void CalculateForecast(Row prevRow, Row currentRow)
        {
            currentRow.Forecast = prevRow.Smoothed + prevRow.Trend;
        }

        private void CalculateSquaredErrors(Row currentRow)
        {
            currentRow.SquaredErrors = Math.Pow(currentRow.Demand - currentRow.Forecast, 2);
        }

        private double CalculateStandardError()
        {
            double sum = 0;
            foreach (KeyValuePair<double, Row> pair in _data.Skip(2))
            {
                sum += pair.Value.SquaredErrors;
            }

            return Math.Sqrt(sum / (_data.Skip(2).Count() - 2));
        }

        public void CalculateFutureForeCast(double alpha, double beta, int begin, int end)
        {
            CalculateRows(alpha, beta);
            Row lastRow = _data[_defaultDataLength];
            for (int i = begin; i <= end; i++)
            {
                double forecast = lastRow.Smoothed + (i - _defaultDataLength) * lastRow.Trend;
                _data.Add(i, new Row { Forecast = forecast });
            }
        }
    }
}
