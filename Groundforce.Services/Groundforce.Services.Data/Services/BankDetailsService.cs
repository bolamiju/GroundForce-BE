using Groundforce.Services.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Groundforce.Services.Data.Services
{
    public class BankDetailsService : IBankDetailsService
    {
        private readonly BankDetailsSettings _bankDetailsSettings;
        private string url = "https://api.wallets.africa/transfer/bank/account/enquire";
        public BankDetailsService(IOptions<BankDetailsSettings> bankDetailsSettings)
        {
            _bankDetailsSettings = bankDetailsSettings.Value;
        }

        public async Task<string> GetAccountName(string accountNumber, string bankCode)
        {
            var bankDetails = new 
            {
                AccountNumber = accountNumber,
                BankCode = bankCode,
                SecretKey = _bankDetailsSettings.SecretKey
            };

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bankDetailsSettings.PublicKey);
            StringContent queryString = new StringContent(JsonSerializer.Serialize(bankDetails), Encoding.UTF8, "application/json");
            var getResponse = await client.PostAsync(new Uri(url), queryString);
            var content = await getResponse.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<BankDetailsResponse>(content);
            result.AccountName = result.AccountName.Replace("  ", " ");
            return result.AccountName;
        }
    }
}
