using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]

    public class AccountController : ControllerBase
    {
        private readonly IAccountDao accountDao;
 
        public AccountController(IAccountDao _accountDao)
        {
            accountDao = _accountDao;
        }

        [HttpGet("balance/{accountId}")]

        public ActionResult GetBalance(int accountId)
        {
            string username = User.FindFirst("name")?.Value;
            decimal Balance = accountDao.GetBalance(accountId, username).Item1;
            string ColumnLength = accountDao.GetBalance(accountId, username).Item2;
            if (ColumnLength != "")
            {
                return Ok(Balance);
            }
            else
            {
                return BadRequest();
            }
         

        }

        [HttpPost("balance")]

        public void SendTransfer()
        {

        }

        public void ViewTransfers()
        {

        }

        public void GetTransferById(int id)
        {

        }



    }
}
