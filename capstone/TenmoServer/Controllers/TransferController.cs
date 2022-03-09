using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TransferController : ControllerBase
    {
        private readonly ITransferDao transferDao;

        public TransferController(ITransferDao _transferDao)
        {
            transferDao = _transferDao;
        }
    
        [HttpPut]
        public ActionResult SendTransfer(decimal moneyToTransfer, int accountTo, int accountFrom)
        {
            bool transfer = transferDao.SendTransfer(moneyToTransfer, accountTo, accountFrom);
            if(!transfer)
            {
                return StatusCode(400);
            }
            return StatusCode(202);
        }

        public void ViewTransfers()
        {

        }

        public void GetTransferById(int id)
        {

        }
    }
}
