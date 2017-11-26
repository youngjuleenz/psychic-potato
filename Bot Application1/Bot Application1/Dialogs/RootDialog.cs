using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Bot_Application1.ExternalApiService.Luis;
using Bot_Application1.ExternalApiService.Fixer;

namespace Bot_Application1.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        protected string id { get; set; }
        protected string command { get; set; }
        protected string currency { get; set; }
        protected decimal exchangeRate { get; set; }
       
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
            //var currencies = entities.Select(s => s.Entity.ToUpper()).ToList();
            var currency = entities.OrderBy(e => e.Score).LastOrDefault()?.Entity.ToUpper();
            var exchangeRateResponse = ExchangeRateApiService.GetExchangeRateResponse(currency);
            var exchangeRate = exchangeRateResponse.Rates[currency];
                

            if (response.TopScoringIntent.Intent == "None")
            {
                await context.PostAsync($"Invalid request, please enter valid requests");
            }
            else
            {
                await context.PostAsync($"1 NZD is equal to {exchangeRate} {currency}");
            }
        }
    }
}