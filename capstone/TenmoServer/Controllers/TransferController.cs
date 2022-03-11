﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TenmoServer.DAO;
using TenmoServer.Models;
using System.Runtime.Serialization.Json;

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

            if (accountId == transfer.AccountFromId)
            {
                return StatusCode(401);
            }
            else if (accountId == transfer.AccountToId)
            {
                Transfer added = transferDao.CreateRequest(transfer);
                return Created($"/transfer/{added.TransferId}", added);
            }
            return StatusCode(400);
        }

        [HttpPost("send")] //creates a transfer send, checks balance, and executes or denies all in one method
        public ActionResult<Transfer> NewTransferSend(Transfer transfer)
        {
            Transfer sendMoney = transferDao.CreateSend(transfer);

            string username = User.FindFirst("name")?.Value;
            string userIdString = User.FindFirst("sub")?.Value;
            int userId = int.Parse(userIdString);
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
        public ActionResult<Transfer> UpdateRequestedTransfer(int transferId)
        {
            string username = User.FindFirst("name")?.Value;
            int accountId = accountDao.GetAccountNumber(username);
            string userIdString = User.FindFirst("sub")?.Value;
            int userId = int.Parse(userIdString);
            
            Transfer transferToUpdate = transferDao.GetTransfer(transferId);
            decimal balanceFromAccount = accountDao.GetBalance(username, userId).Item1;

            if (accountId == transferToUpdate.AccountFromId)
            {
                if (balanceFromAccount >= transferToUpdate.TransferAmount)
                {
                    transferDao.ExecuteTransfer(transferToUpdate);
                    return Ok();
                }
                return StatusCode(400);
            }
            else
            {
                transferDao.DenyTransfer(transferToUpdate);
                return StatusCode(401);
            }
        }

        [HttpGet("completed")]
        public Dictionary<string, Transfer> ViewTransfers()
        {
            string username = User.FindFirst("name")?.Value;
            int accountId = accountDao.GetAccountNumber(username);
            return transferDao.ListCompletedTransfers(accountId);
        }

        [HttpGet("{transferId}")]
        public ActionResult<Transfer> GetTransferById(int transferId)
        {
            string username = User.FindFirst("name")?.Value;
            int accountId = accountDao.GetAccountNumber(username);
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
