using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot_Application1.ExternalApiService.SpellCheck
{
    public class SpellCheckApiResponse
    {
        public string _Type { get; set; }
        public List<BingFlaggedToken> FlaggedTokens { get; set; }
    }

    public class BingFlaggedToken
    {
        public int Offset { get; set; }
        public string Token { get; set; }
        public string Type { get; set; }
        public List<BingSuggestion> Suggestions { get; set; }
    }

    public class BingSuggestion
    {
        public string Suggestion { get; set; }
        public decimal Score { get; set; }
    }
}