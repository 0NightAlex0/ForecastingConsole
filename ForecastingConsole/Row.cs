using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForecastingConsole
{
    class Row
    {
        public double Demand { get; set; }
        public double Smoothed { get; set; }
        public double Trend { get; set; }
        public double Forecast { get; set; }
        public double SquaredErrors { get; set; }
        public double StandardError { get; set; }
    }
}
