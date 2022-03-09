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

        [HttpGet("balance/{accountId}")]
        
        public decimal GetBalance(int accountId)
        {
            User testUser = new User();
            testUser.Username = User.Identity.Name;

            decimal Balance = accountDao.GetBalance(accountId);
            return Balance;

        }


    }
}
