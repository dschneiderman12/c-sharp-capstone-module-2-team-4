using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {

        public int TransferId { get; set; }
        public int TransferStatusId { get; set; }
        public int TransferTypeId { get; set; }
        public int AccountFromId { get; set; }
        public int AccountToId { get; set; }
        public decimal TransferAmount { get; set; }

        public Transfer()
        {

        }
        public Transfer(int transferId, int transferStatusId, int transferTypeId, int accountFromId,int accountToId, decimal transferAmount)
        {

            TransferId = transferId;
            TransferStatusId = transferStatusId;
            TransferTypeId = transferTypeId;
            AccountFromId = accountFromId;
            AccountToId = accountToId;
            TransferAmount = transferAmount;
        }

    }
}
