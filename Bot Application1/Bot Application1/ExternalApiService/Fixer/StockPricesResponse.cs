using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot_Application1.ExternalApiService.Fixer
{
    public class StockPricesResponse
    {
        public Dictionary<string, string> Metadata { get; set; }
        //public Dictionary<string, decimal> Timeseries { get; set; }
        public Dictionary<string, Dictionary<string, double>> Timeseries { get; set; }
        public StockPrice TimeSeries { get; set; }
    }

    public class StockPrice
    {
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public long Volume { get; set; }

    }
}