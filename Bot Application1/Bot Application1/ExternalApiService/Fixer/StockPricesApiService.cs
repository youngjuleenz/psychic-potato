using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace Bot_Application1.ExternalApiService.Fixer
{
    public class StockPricesApiService
    {
        private const string baseUrl = "https://www.alphavantage.co/query";

        public static string GetStockPricesResponse(string symbol)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            string urlParam = "?function=TIME_SERIES_INTRADAY&symbol=" + symbol + "&interval=1min&apikey=SD0ER6FVK288ISJM";

            HttpResponseMessage response = client.GetAsync(urlParam).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseObject = response.Content.ReadAsAsync<object>().Result;
                return responseObject.ToString();
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }
    }
}