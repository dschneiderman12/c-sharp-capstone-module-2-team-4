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


        public int GetAccountNumber(string username)
        {
            int accountNumber = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"SELECT account_id From account
        JOIN tenmo_user ON account.user_id=tenmo_user.user_id
        WHERE username = @username", conn);
                cmd.Parameters.AddWithValue("@username", username);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    accountNumber = Convert.ToInt32(reader["account_id"]);
                }
            }
            return accountNumber;

        }

        public Tuple<decimal, string> GetBalance(string username,int userId)
        {
            decimal returnBalance = 0;
            string columnLength = string.Empty;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"SELECT balance, COL_LENGTH('account','balance') AS Result FROM account
                                                JOIN tenmo_user ON tenmo_user.user_id = account.user_id
                                                WHERE username=@username AND account.user_id=@userId", conn);
                cmd.Parameters.AddWithValue("@userId", userId);
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
