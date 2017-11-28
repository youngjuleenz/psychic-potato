using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Bot_Application1.ExternalApiService.Luis;
using Bot_Application1.ExternalApiService.Fixer;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Bot_Application1.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        protected string id { get; set; }
        protected string command { get; set; }
        protected string currency { get; set; }
        protected decimal exchangeRate { get; set; }
        protected string symbol { get; set; }
       
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("Welcome to Contoso Bank, what is your ID?");
            context.Wait(IdReceived);
            //var activity = await result as Activity;

            // calculate something for us to return
            //int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            //await context.PostAsync($"You sent {activity.Text} which was {length} characters");

            //context.Wait(MessageReceivedAsync);
        }

        public async Task IdReceived(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            this.id = (await argument).Text;
            await context.PostAsync($"Hello {id}, How can I help you? 1. Exchange rates 2. Stock prices");
            context.Wait(ExecuteCommand);
        }

        public async Task ExecuteCommand(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            this.command = (await argument).Text;
            var response = LuisApiService.GetLuisResponse(command);
            var entities = response.Entities;
            //var symbol = response.
            //var currencies = entities.Select(s => s.Entity.ToUpper()).ToList();

            if (response.TopScoringIntent.Intent == "None")
            {
                await context.PostAsync("Invalid request, please enter a valid request");
            }
            else if (response.TopScoringIntent.Intent == "GetExchangeRate")
            {
                var currency = entities.OrderBy(e => e.Score).LastOrDefault()?.Entity.ToUpper();
                var exchangeRateResponse = ExchangeRateApiService.GetExchangeRateResponse(currency);
                if (exchangeRateResponse.Rates.Count == 0)
                {
                    await context.PostAsync($"That is not a valid type of currency!! Please try again.");
                } else
                {
                    var exchangeRate = exchangeRateResponse.Rates[currency];

                    await context.PostAsync($"1 NZD is equal to {exchangeRate} {currency}");
                } 
            }
            else if (response.TopScoringIntent.Intent == "GetStockPrice")
            {
                var symbol = entities.OrderBy(e => e.Score).LastOrDefault()?.Entity.ToUpper();
                var stockPriceResponse = StockPricesApiService.GetStockPricesResponse(symbol);
                if (stockPriceResponse.Contains("Invalid API call"))
                {
                    await context.PostAsync("Invalid stock price symbol. Please try again.");
                } else
                {
                    var jObj = JObject.Parse(stockPriceResponse);
                    var metadata = jObj["Meta Data"].ToObject<Dictionary<string, string>>();
                    var timeseries = jObj["Time Series (1min)"].ToObject<Dictionary<string, Dictionary<string, string>>>();
                    var latestUpdateKey = timeseries.Keys.First();
                    var latestUpdate = timeseries[latestUpdateKey];
                    var closingValue = latestUpdate["4. close"];

                    await context.PostAsync($"Stock price of {symbol} is {closingValue} USD");
                }



                //var stockPrice = stockPriceResponse.Timeseries[Close];
                
          

            }

            
        }
    }
}