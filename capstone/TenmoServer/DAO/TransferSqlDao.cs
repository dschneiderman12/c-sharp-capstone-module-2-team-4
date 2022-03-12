using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDao : ITransferDao
    {
        private readonly string connectionString;

        public TransferSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public Transfer GetTransfer(int transferId)
        {
            Transfer transfer = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {

                conn.Open();
                SqlCommand cmd = new SqlCommand(@"SELECT username, transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount
                                                FROM transfer
                                                JOIN account ON account.account_id = transfer.account_from
                                                JOIN tenmo_user ON tenmo_user.user_id = account.user_id
                                                WHERE transfer_id = @transfer_id", conn);
                cmd.Parameters.AddWithValue("@transfer_id", transferId);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    transfer = createTransferFromReader(reader);
                }
            }
            return transfer;
        }

        public Transfer CreateRequest(Transfer newTransfer)
        {
            int newTransferId;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount)
                                                OUTPUT INSERTED.transfer_id
                                                VALUES (1, 1, @account_from, @account_to, @amount);", conn);
                cmd.Parameters.AddWithValue("@account_from", newTransfer.AccountFromId);
                cmd.Parameters.AddWithValue("@account_to", newTransfer.AccountToId);
                cmd.Parameters.AddWithValue("@amount", newTransfer.TransferAmount);

                newTransferId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return GetTransfer(newTransferId);
        }

        public Transfer CreateSend(Transfer newTransfer)
        {
            int newTransferId;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount)
                                                OUTPUT INSERTED.transfer_id
                                                VALUES (2, 2, @account_from, @account_to, @amount);", conn);
                cmd.Parameters.AddWithValue("@account_from", newTransfer.AccountFromId);
                cmd.Parameters.AddWithValue("@account_to", newTransfer.AccountToId);
                cmd.Parameters.AddWithValue("@amount", newTransfer.TransferAmount);

                newTransferId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return GetTransfer(newTransferId);
        }

        public void ExecuteTransfer(Transfer transfer)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"UPDATE account
                                                SET balance = balance - @amount
                                                WHERE account_id = @account_from;
                                                
                                                UPDATE account
                                                SET balance = balance + @amount
                                                WHERE account_id = @account_to;

                                                UPDATE transfer
                                                SET transfer_status_id = 2
                                                WHERE transfer_id = @transfer_id", conn);

                cmd.Parameters.AddWithValue("@account_from", transfer.AccountFromId);
                cmd.Parameters.AddWithValue("@account_to", transfer.AccountToId);
                cmd.Parameters.AddWithValue("@amount", transfer.TransferAmount);
                cmd.Parameters.AddWithValue("@transfer_id", transfer.TransferId);

                cmd.ExecuteNonQuery();
            }
        }

        public void DenyTransfer(Transfer transfer)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"UPDATE transfer
                                                SET transfer_status_id = 3
                                                WHERE transfer_id = @transfer_id", conn);

                cmd.Parameters.AddWithValue("@transfer_id", transfer.TransferId);

                cmd.ExecuteNonQuery();
            }
        }

        public List<User> ListUsers(string username)
        {
            List<User> users = new List<User>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"SELECT user_id, username FROM tenmo_user
                                                WHERE username != @username;", conn);
                cmd.Parameters.AddWithValue("@username", username);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    User user = new User();
                    user.UserId = Convert.ToInt32(reader["user_id"]);
                    user.Username = Convert.ToString(reader["username"]);
                    users.Add(user);
                }
            }
            return users;
        }

        public Dictionary<string, Transfer> ListCompletedTransfers(int accountId)
        {
            Dictionary<string, Transfer> userTransfers = new Dictionary<string, Transfer>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"SELECT username, transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount
                                                FROM transfer
                                                JOIN account ON account.account_id = transfer.account_from
                                                JOIN tenmo_user ON tenmo_user.user_id = account.user_id
                                                WHERE transfer_status_id = 2 AND account_to = @account_id                                                
                                                UNION
                                                SELECT username, transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount
                                                FROM transfer
                                                JOIN account ON account.account_id = transfer.account_to
                                                JOIN tenmo_user ON tenmo_user.user_id = account.user_id
                                                WHERE transfer_status_id = 2 AND account_from = @account_id
                                                ORDER BY transfer_id ASC;", conn);
                cmd.Parameters.AddWithValue("@account_id", accountId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string idAsString = Convert.ToString(reader["transfer_id"]);
                    Transfer transfer = createTransferFromReader(reader);
                    if (transfer.AccountToId == accountId)
                    {
                        transfer.UserFrom = Convert.ToString(reader["username"]);
                    }
                    else if (transfer.AccountFromId == accountId)
                    {
                        transfer.UserTo = Convert.ToString(reader["username"]);
                    }
                    userTransfers[idAsString] = transfer;
                }
            }
            return userTransfers;
        }

        private Transfer createTransferFromReader(SqlDataReader reader)
        {
            Transfer transfer = new Transfer();
            transfer.TransferId = Convert.ToInt32(reader["transfer_id"]);
            transfer.TransferTypeId = Convert.ToInt32(reader["transfer_type_id"]);
            transfer.TransferStatusId = Convert.ToInt32(reader["transfer_status_id"]);
            transfer.AccountFromId = Convert.ToInt32(reader["account_from"]);
            transfer.AccountToId = Convert.ToInt32(reader["account_to"]);
            transfer.TransferAmount = Convert.ToDecimal(reader["amount"]);

            return transfer;
        }

    }
}