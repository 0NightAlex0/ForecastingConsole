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

        public void CalculateRows(double alpha, double beta)
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
        // fj alpha 0.8 and beta 0.8 best
        public List<Tuple<double, double>> GetAllAlphaBetaCombinations()
        {
            List<Tuple<double, double>> combinations = new List<Tuple<double, double>>();
            double begin = 0.1;
            double end = 0.9;
            for (double alpha = begin; alpha <= end; alpha += 0.1)
            {
                for (double beta = begin; beta <= end; beta += 0.1)
                {
                    //Console.WriteLine($"alpha: {i}  beta:{j}");
                    combinations.Add(new Tuple<double, double>(alpha, beta));
                }
            }
            return combinations;
        }

        public Tuple<double, double> GetBestAlphaBetaCombination()
        {
            // error (alpha and beta)
            Tuple<double, Tuple<double, double>> best = new Tuple<double, Tuple<double, double>>(Double.MaxValue, new Tuple<double, double>(0, 0));
            List<Tuple<double, double>> combinations = GetAllAlphaBetaCombinations();
            foreach (Tuple<double, double> combination in combinations)
            {
                double alpha = combination.Item1;
                double beta = combination.Item2;

                CalculateRows(alpha, beta);
                double error = CalculateStandardError();
                // if current error is less than the lowest error
                if (error < best.Item1)
                {
                    best = new Tuple<double, Tuple<double, double>>(error, new Tuple<double, double>(alpha, beta));

                }
            }
            return best.Item2;
        }
    }
}
