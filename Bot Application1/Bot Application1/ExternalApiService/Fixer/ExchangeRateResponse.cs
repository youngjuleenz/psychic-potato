﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot_Application1.ExternalApiService.Fixer
{
    public class ExchangeRateResponse
    {
        public string Base { get; set; }
        public string Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }

}