using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDao
    {
        public Transfer GetTransfer(int transferId);
        public Transfer Create(Transfer newTransfer);
        public void ExecuteTransfer(Transfer transfer);
        public void DenyTransfer(Transfer transfer);
        public List<User> ListUsers(string username);
        public List<Transfer> ListCompletedTransfers();
    }
}
