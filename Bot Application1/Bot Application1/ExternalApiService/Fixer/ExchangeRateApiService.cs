using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Bot_Application1.ExternalApiService.Fixer
{
    public class ExchangeRateApiService
    {
        private const string baseUrl = "https://api.fixer.io/latest";

        public static ExchangeRateResponse GetExchangeRateResponse(string currency)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            string urlParam = "?base=NZD&symbols=" + currency;

            HttpResponseMessage response = client.GetAsync(urlParam).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseObject = response.Content.ReadAsAsync<ExchangeRateResponse>().Result;
                return responseObject;
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return null;
        }
    }
}