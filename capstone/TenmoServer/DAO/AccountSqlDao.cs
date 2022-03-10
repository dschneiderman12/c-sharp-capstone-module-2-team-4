using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class AccountSqlDao : IAccountDao
    {
        private readonly string connectionString;

        public AccountSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        
        public Tuple<decimal,string> GetBalance(int accountId, string username)
        {
            decimal returnBalance = 0;
            string columnLength = string.Empty;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(@"SELECT balance, COL_LENGTH('account','balance') AS Result FROM account
                                                JOIN tenmo_user ON tenmo_user.user_id = account.user_id
                                                WHERE account_id = @account_id AND username=@username", conn);
                    cmd.Parameters.AddWithValue("@account_id", accountId);
                    cmd.Parameters.AddWithValue("@username", username);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                    returnBalance = Convert.ToDecimal(reader["balance"]);
                    columnLength = Convert.ToString(reader["Result"]);
                    }
                }
         
            return Tuple.Create(returnBalance, columnLength);
        }
    }
}
