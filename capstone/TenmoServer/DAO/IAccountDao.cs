using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountDao
    {
        int GetAccountNumber(string username);
      Tuple<decimal,string> GetBalance(string username,int userId);

    }
}
