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
        private readonly IAccountDao accountDao;

        public TransferController(ITransferDao _transferDao, IAccountDao _accountDao)
        {
            transferDao = _transferDao;
            accountDao = _accountDao;
        }

        [HttpGet("users")]
        public List<User> ListUsersForTransfers()
        {
            string username = User.FindFirst("name")?.Value;
            return transferDao.ListUsers(username);
        }

        [HttpPost]
        public ActionResult<Transfer> NewTransferRequest(Transfer transfer)
        {
            Transfer added = transferDao.CreateRequest(transfer);
            return Created($"/transfer/{added.TransferId}", added);
        }

        [HttpPost]
        public ActionResult<Transfer> NewTransferSend(Transfer transfer)
        {
            Transfer added = transferDao.CreateSend(transfer);
            return Created($"/transfer/{added.TransferId}", added);
        }

        //[HttpPut("{transferId}")]
        //public ActionResult<Transfer> UpdateTransfer(int transferId)
        //{
        //    string username = User.FindFirst("name")?.Value;
        //    Transfer transferToUpdate = transferDao.GetTransfer(transferId);
        //    decimal balanceFromAccount = accountDao.GetBalance(transferToUpdate.AccountFromId, username).Item1;

        //    if ((balanceFromAccount >= transferToUpdate.TransferAmount) && (transferToUpdate.TransferAmount > 0))
        //    {
        //        transferDao.ExecuteTransfer(transferToUpdate);
        //        return Ok();
        //    }
        //    else
        //    {
        //        transferDao.DenyTransfer(transferToUpdate);
        //        return StatusCode(400);
        //    }
        //}

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
