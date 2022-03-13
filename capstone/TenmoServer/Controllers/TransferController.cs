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

        [HttpPost("request")] //checks to make sure the account from is not current user requesting, and account to is the current user requesting
        public ActionResult<Transfer> NewTransferRequest(Transfer transfer)
        {
            string username = User.FindFirst("name")?.Value;
            int accountId = accountDao.GetAccountNumber(username);
            string userIdString = User.FindFirst("sub")?.Value;
            int userId = int.Parse(userIdString);

            transfer.AccountToId = userId;

            if (accountId == transfer.AccountFromId)
            {
                return StatusCode(401);
            }
            else //if (accountId == transfer.AccountToId)
            {
                Transfer added = transferDao.CreateRequest(transfer);
                return Created($"/transfer/{added.TransferId}", added);
            }
            //return StatusCode(400);
        }

        [HttpPost("send")] //creates a transfer send, checks balance, and executes or denies all in one method
        public ActionResult<Transfer> NewTransferSend(Transfer transfer)
        {
            string username = User.FindFirst("name")?.Value;
            string userIdString = User.FindFirst("sub")?.Value;
            int userId = int.Parse(userIdString);

            transfer.AccountFromId = userId;
            
            Transfer sendMoney = transferDao.CreateSend(transfer);

            decimal balanceFromAccount = accountDao.GetBalance(username, userId).Item1;

            if (balanceFromAccount >= sendMoney.TransferAmount)
            {
                transferDao.ExecuteTransfer(sendMoney);
                return Ok();
            }
            else
            {
                transferDao.DenyTransfer(sendMoney);
                return StatusCode(400);
            }
        }

        [HttpPut("{transferId}")] //checks to see the right user is approving the request and there is a great enough balance
        public ActionResult UpdateRequestedTransfer(int transferId)
        {
            string username = User.FindFirst("name")?.Value;
            int accountId = accountDao.GetAccountNumber(username);
            string userIdString = User.FindFirst("sub")?.Value;
            int userId = int.Parse(userIdString);
            
            Transfer transferToUpdate = transferDao.GetTransfer(transferId);
            decimal balanceFromAccount = accountDao.GetBalance(username, userId).Item1;
            if (transferToUpdate != null)
            {
                if (accountId == transferToUpdate.AccountFromId)
                {
                    if (balanceFromAccount >= transferToUpdate.TransferAmount)
                    {
                        transferDao.ExecuteTransfer(transferToUpdate);
                        return Ok();
                    }
                    transferDao.DenyTransfer(transferToUpdate);
                    //throw new Exception(ex.ErrorCode, ex.message);
                    return BadRequest(new { message = "Balance too low to complete transfer. Transfer denied." });
                }
                else
                {
                    transferDao.DenyTransfer(transferToUpdate);
                    return StatusCode(401);
                }
            }
            else
            {
                return BadRequest(new { message = "Incorrect transfer ID." });
            }
        }

        [HttpPut("deny/{transferId}")]
        public ActionResult UserDeniedTransfer(int transferId)
        {
            Transfer transferToUpdate = transferDao.GetTransfer(transferId);
            transferDao.DenyTransfer(transferToUpdate);
            return Ok();
        }

        [HttpGet("completed")]
        public Dictionary<string, Transfer> ViewTransfers()
        {
            string username = User.FindFirst("name")?.Value;
            int accountId = accountDao.GetAccountNumber(username);
            return transferDao.ListCompletedTransfers(accountId);
        }

        [HttpGet("pending")]
        public Dictionary<string, Transfer> ViewPendingTransfers()
        {
            string username = User.FindFirst("name")?.Value;
            int accountId = accountDao.GetAccountNumber(username);
            return transferDao.ListPendingTransfers(accountId);
        }

        [HttpGet("{transferId}")]
        public Dictionary<string, Transfer> GetTransferById()
        {
            string username = User.FindFirst("name")?.Value;
            int accountId = accountDao.GetAccountNumber(username);
            //return transferDao.ListCompletedTransfers(accountId);
            Dictionary<string, Transfer> transferList = transferDao.ListCompletedTransfers(accountId);

            foreach (KeyValuePair<string, Transfer> transfer in transferList)
            {
                if (transfer.Value.UserFrom == null)
                {
                    transfer.Value.UserFrom = User.Identity.Name;
                }
                if (transfer.Value.UserTo == null)
                {
                    transfer.Value.UserTo = User.Identity.Name;
                }
            }
            return transferList;
        }
    }
}
