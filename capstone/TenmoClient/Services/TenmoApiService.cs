using Newtonsoft.Json;
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
            RestRequest request = new RestRequest("transfer/completed");
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

        public Transfer SendTeBucks(Transfer transfer)
        {
            RestRequest request = new RestRequest("transfer/send");
            request.AddJsonBody(transfer);
            IRestResponse<Transfer> response = client.Post<Transfer>(request);

            CheckForError(response);
            return response.Data;
        }

        public List<User> ListUsersForTransfers()
        {
            RestRequest request = new RestRequest("transfer/users");
            IRestResponse<List<User>> response = client.Get<List<User>>(request);

            CheckForError(response);
            return response.Data;
        }

        public Transfer RequestTeBucks(Transfer transfer)
        {
            RestRequest request = new RestRequest("transfer/request");
            request.AddJsonBody(transfer);
            IRestResponse<Transfer> response = client.Post<Transfer>(request);

            CheckForError(response);
            return response.Data;
        }

        public Dictionary<string, Transfer> ViewPendingTransfers()
        {
            RestRequest request = new RestRequest("transfer/pending");
            IRestResponse<Dictionary<string, Transfer>> response = client.Get<Dictionary<string, Transfer>>(request);

            CheckForError(response);
            return response.Data;
        }

        public bool RespondToTransferRequest(int transferId)
        {
            RestRequest request = new RestRequest($"transfer/{transferId}");            
            IRestResponse response = client.Put(request);

            CheckForError(response);
            return true;
        }

        public bool DenyTransferByUser(int transferId)
        {
            RestRequest request = new RestRequest($"transfer/deny/{transferId}");            
            IRestResponse response = client.Put(request);

            CheckForError(response);
            return true;
        }

    }
}
