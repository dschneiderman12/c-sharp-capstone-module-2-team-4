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
        public List<User> ListUsersForTransfers()
        {
            string username = User.FindFirst("name")?.Value;
            return transferDao.ListUsers(username);
        }

        [HttpPost]
        public ActionResult<Transfer> NewTransfer(Transfer transfer)
        {
            Transfer added = transferDao.Create(transfer);
            return Created($"/transfer/{added.TransferId}", added);
        }

        //[HttpGet("{username}")]
        //public List<Transfer> ViewTransfers()
        //{
        //    return transferDao.ListCompletedTransfers();
        //}

        //[HttpGet("{transferId}")]
        //public ActionResult<Transfer> GetTransferById(int transferId)
        //{
        //    Transfer transfer = transferDao.GetTransfer(transferId);

        //    if (transfer != null)
        //    {
        //        return transfer;
        //    }
        //    else
        //    {
        //        return NotFound();
        //    }
        //}


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
    }
}
