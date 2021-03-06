using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Models
{
    public class Account
    {
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
