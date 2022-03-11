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

        [HttpPost("request")]
        public ActionResult<Transfer> NewTransferRequest(Transfer transfer)
        {
            Transfer added = transferDao.CreateRequest(transfer);
            return Created($"/transfer/{added.TransferId}", added);
        }

        [HttpPost("send")]
        public ActionResult<Transfer> NewTransferSend(Transfer transfer)
        {
            Transfer added = transferDao.CreateSend(transfer);
            return Created($"/transfer/{added.TransferId}", added);
        }

        [HttpPut("{transferId}")]
        public ActionResult<Transfer> UpdateTransfer(int transferId)
        {
            string username = User.FindFirst("name")?.Value;
            string userIdString = User.FindFirst("sub")?.Value;
            int userId = int.Parse(userIdString);
            Transfer transferToUpdate = transferDao.GetTransfer(transferId);
            decimal balanceFromAccount = accountDao.GetBalance(username,userId).Item1;

            if ((balanceFromAccount >= transferToUpdate.TransferAmount) && (transferToUpdate.TransferAmount > 0))
            {
                transferDao.ExecuteTransfer(transferToUpdate);
                return Ok();
            }
            else
            {
                transferDao.DenyTransfer(transferToUpdate);
                return StatusCode(400);
            }
        }

        [HttpGet("completed")]
        public Dictionary<Transfer, string> ViewTransfers()
        {
            string username = User.FindFirst("name")?.Value;
            int accountId = accountDao.GetAccountNumber(username);
            
            return transferDao.ListCompletedTransfers(accountId);
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
