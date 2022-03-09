using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.DAO
{
    public class TransferSqlDao : ITransferDao
    {
        private readonly string connectionString;

        public TransferSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public bool CheckBalance(decimal moneyToTransfer, int accountFrom)
        {
            decimal returnBalance = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(@"SELECT balance FROM account
                                                JOIN tenmo_user ON tenmo_user.user_id = account.user_id
                                                WHERE account_id = @account_id", conn);
                    cmd.Parameters.AddWithValue("@account_id", accountFrom);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        returnBalance = Convert.ToDecimal(reader["balance"]);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            if (returnBalance >= moneyToTransfer)
            {
                return true;
            }
            return false;
        }

        public bool SendTransfer(decimal moneyToTransfer, int accountTo, int accountFrom)
        {
            if (CheckBalance(moneyToTransfer, accountFrom))
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand(@"UPDATE account
                                                        SET balance = balance - @amount
                                                        WHERE account_id = @accountFrom;
                                                        
                                                        UPDATE account
                                                        SET balance = balance + @amount
                                                        WHERE account_id = @accountTo;", conn);
                        cmd.Parameters.AddWithValue("@accountFrom", accountFrom);
                        cmd.Parameters.AddWithValue("@accountTo", accountTo);
                        cmd.Parameters.AddWithValue("@amount", moneyToTransfer);

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (SqlException)
                {
                    throw;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
