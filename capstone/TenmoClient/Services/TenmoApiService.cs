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
            IRestResponse<decimal> response = client.Get<decimal>(request);

            CheckForError(response);

            return response.Data;
        }

        public Dictionary<Transfer, string> ViewTransfers()
        {
            RestRequest request = new RestRequest($"transfer/completed");
            IRestResponse<Dictionary<Transfer, string>> response = client.Get<Dictionary<Transfer, string>>(request);

            CheckForError(response);

            return response.Data;

        }
    }
}
