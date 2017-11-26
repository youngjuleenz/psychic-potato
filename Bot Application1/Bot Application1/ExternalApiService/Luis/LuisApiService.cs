using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Bot_Application1.ExternalApiService.Luis
{
    public class LuisApiService
    {
        private const string baseUrl = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/8ae79516-51d4-4dac-a9d9-45b447b97ede";

        public static LuisResponse GetLuisResponse(string command)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            string urlParam = "?subscription-key=e69f7941f5854d3ba944a77088e2c559&verbose=true&timezoneOffset=0&q=" + command;

            HttpResponseMessage response = client.GetAsync(urlParam).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseObject = response.Content.ReadAsAsync<LuisResponse>().Result;
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