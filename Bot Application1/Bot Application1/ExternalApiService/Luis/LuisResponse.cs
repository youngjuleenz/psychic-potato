using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot_Application1.ExternalApiService.Luis
{
    public class LuisResponse
    {
        public string Query { get; set; }
        public LuisIntent TopScoringIntent { get; set; }
        public List<LuisIntent> Intents { get; set; }
        public List<LuisEntity> Entities { get; set; }
    }

    public class LuisIntent
    {
        public string Intent { get; set; }
        public double Score { get; set; }
    }
    
    public class LuisEntity
    {
        public string Entity { get; set; }
        public string Type { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public double Score { get; set; }
    }
}