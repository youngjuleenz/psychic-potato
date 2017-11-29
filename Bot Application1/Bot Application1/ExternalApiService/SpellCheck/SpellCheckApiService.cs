using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace Bot_Application1.ExternalApiService.SpellCheck
{
    public class SpellCheckApiService
    {
        // NOTE: Replace this example key with a valid subscription key.
        static string key = "053d6d26eac94e8786b26507ba0c8c99";

        private const string baseUrl = "https://api.cognitive.microsoft.com/bing/v7.0/spellcheck/";

        public static SpellCheckApiResponse GetSpellCheckApiResponse(string command)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

            string urlParam = $"?text={command}&mode=spell";

            HttpResponseMessage response = client.GetAsync(urlParam).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseObject = response.Content.ReadAsAsync<SpellCheckApiResponse>().Result;
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