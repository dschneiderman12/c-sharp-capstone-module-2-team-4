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

        public Dictionary<string, Transfer> ViewTransfers()
        {
            RestRequest request = new RestRequest($"transfer/completed");
            IRestResponse<Dictionary<string, Transfer>> response = client.Get<Dictionary<string, Transfer>>(request);

            CheckForError(response);
            return response.Data;
        }

        public Dictionary<string, Transfer> GetTransferById(string transferId)
        {
            RestRequest request = new RestRequest($"transfer/{transferId}");
            IRestResponse<Dictionary<string, Transfer>> response = client.Get<Dictionary<string, Transfer>>(request);

            CheckForError(response);
            return response.Data;
        }
    }
}
