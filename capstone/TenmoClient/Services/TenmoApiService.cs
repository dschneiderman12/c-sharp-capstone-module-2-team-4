using RestSharp;
using System.Collections.Generic;
using TenmoClient.Models;


namespace TenmoClient.Services
{
    public class TenmoApiService : AuthenticatedApiService
    {
        public readonly string ApiUrl;

        public TenmoApiService(string apiUrl) : base(apiUrl) { }

        // Add methods to call api here...

        public decimal GetBalance(string accountName)
        {

            RestRequest request = new RestRequest($"account/balance/{accountName}");
            IRestResponse <Account> response = client.Get<Account>(request);

            CheckForError(response);
            return response.Data.Balance;
        }
    }
}
