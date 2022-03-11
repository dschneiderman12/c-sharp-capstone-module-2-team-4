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

    public class AccountController : ControllerBase
    {
        private readonly IAccountDao accountDao;

        public AccountController(IAccountDao _accountDao)
        {
            accountDao = _accountDao;
        }


  
        [HttpGet("balance/{accountName}")]

        public ActionResult GetBalance(string accountName)
        {
            string username = accountName;
            string userIdString =User.FindFirst("sub")?.Value;
            int userId = int.Parse(userIdString);
            decimal Balance = accountDao.GetBalance(username, userId).Item1;
            string ColumnLength = accountDao.GetBalance( username,userId).Item2;
            if (ColumnLength != "")
            {
                return Ok(Balance);
            }
            else
            {
                return BadRequest();
            }



        }

    }
}
