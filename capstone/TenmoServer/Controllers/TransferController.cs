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

        [HttpGet("users")]
        public List<User> ListUsers()
        {
            return transferDao.ListUsers();
        }

    
        //[HttpPut]
        //public ActionResult SendTransfer(decimal moneyToTransfer, int accountTo, int accountFrom)
        //{
        //    bool transfer = transferDao.SendTransfer(moneyToTransfer, accountTo, accountFrom);
        //    if(!transfer)
        //    {
        //        return StatusCode(400);
        //    }
        //    return StatusCode(202);
        //}

        [HttpGet("{userId}/transfers}")]
        public List<Transfer> ViewTransfers(int userId)
        {
            return transferDao.ListCompletedTransfers();
        }

        [HttpGet("{transferId}")]
        public ActionResult<Transfer> GetTransferById(int transferId)
        {
            Transfer transfer = transferDao.GetTransfer(transferId);

            if (transfer != null)
            {
                return transfer;
            }
            else
            {
                return NotFound();
            }
        }
    }
}
