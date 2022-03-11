using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Account
    {
        internal static object Identity;
        public int AccountId { get; set; }
        public decimal Balance { get; set; }
        public int UserId { get; set; }

        public Account(int accountId, decimal balance, int userId)
        {
            AccountId = accountId;
            Balance = balance;
            UserId = userId;
        }

    }
}
