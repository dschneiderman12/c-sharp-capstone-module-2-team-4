using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.DAO
{
    public interface ITransferDao
    {
        public bool SendTransfer(decimal moneyToTransfer, int accountTo, int accountFrom);
        //public void ViewTransfers();
        //public void GetTransferById(int id);

    }
}
