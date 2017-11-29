using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace Bot_Application1
{
    public class AzureManager
    {
        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<YoungJusBankBotModel> userinformationTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("http://youngjusbankbotmodel.azurewebsites.net");
            this.userinformationTable = this.client.GetTable<YoungJusBankBotModel>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }

        public async Task<List<YoungJusBankBotModel>> GetUserInformation()
        {
            return await this.userinformationTable.ToListAsync();
        }

        public async Task AddUserInformation(YoungJusBankBotModel user)
        {
            await this.userinformationTable.InsertAsync(user);
        }

        public async Task DeleteUserInformation(YoungJusBankBotModel user)
        {
            await this.userinformationTable.DeleteAsync(user);
        }

    }
}