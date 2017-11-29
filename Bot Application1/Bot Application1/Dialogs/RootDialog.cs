﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Bot_Application1.ExternalApiService.Luis;
using Bot_Application1.ExternalApiService.Fixer;
using Newtonsoft.Json.Linq;


namespace Bot_Application1.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        protected string id { get; set; }
        protected string pw { get; set; }
        protected string command { get; set; }
        protected string currency { get; set; }
        protected decimal exchangeRate { get; set; }
        protected string symbol { get; set; }
        private UserInformation currentUser { get; set; }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("Welcome to Contoso Bank, what is your ID?");
            context.Wait(IdReceived);

        }

        public async Task IdReceived(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            this.id = (await argument).Text;

            var users = await AzureManager.AzureManagerInstance.GetUserInformation();
            if (!users.Select(u => u.username).Contains(id))
            {
                await context.PostAsync("Invalid username. Please try again");
            }
            else
            {
                currentUser = users.FirstOrDefault(u => u.username == id);
                await context.PostAsync("Please enter your password");
                context.Wait(PasswordReceived);
            }
        }



        public async Task PasswordReceived(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            this.pw = (await argument).Text;
            if (currentUser.password != pw)
            {
                currentUser = null;
                await context.PostAsync($"Wrong password! Please try log in again by entering your Id.");
                context.Wait(IdReceived);
            }
            else
            {
                await context.PostAsync($"Hello {currentUser.username}, How can I help you? 1. Exchange rates 2. Stock prices");
                context.Wait(ExecuteCommand);
            }
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
                }
                else
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
                }
                else
                {
                    var jObj = JObject.Parse(stockPriceResponse);
                    var metadata = jObj["Meta Data"].ToObject<Dictionary<string, string>>();
                    var timeseries = jObj["Time Series (1min)"].ToObject<Dictionary<string, Dictionary<string, string>>>();
                    var latestUpdateKey = timeseries.Keys.First();
                    var latestUpdate = timeseries[latestUpdateKey];
                    var closingValue = latestUpdate["4. close"];

                    await context.PostAsync($"Stock price of {symbol} is {closingValue} USD");
                }
            }


        }
    }
}