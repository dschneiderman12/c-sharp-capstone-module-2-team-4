﻿using Microsoft.AspNetCore.Mvc;
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
                SqlCommand cmd = new SqlCommand(@"SELECT transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount
                                                FROM transfer
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

        public Transfer Create(Transfer newTransfer)
        {
            int newTransferId;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount)
                                                VALUES (1, 1, @account_from, @account_to, @amount);", conn);
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
                                                SET transfer_type_id = 2, transfer_status_id = 2
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

        public List<User> ListUsers()
        {
            List<User> users = new List<User>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"SELECT user_id, username FROM tenmo_user;", conn);

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

        public List<Transfer> ListCompletedTransfers()
        {
            List<Transfer> completedTransfers = new List<Transfer>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                //fix
                SqlCommand cmd = new SqlCommand(@"SELECT transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount
                                                FROM transfer
                                                WHERE transfer_status_id = 2", conn);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Transfer transfer = createTransferFromReader(reader);
                    completedTransfers.Add(transfer);
                }
            }
            return completedTransfers;
        }

        
        //public Transfer CreateTransfer(decimal moneyToTransfer, int accountFrom, int accountTo)
        //{
        //    Transfer transfer = new Transfer();
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();

        //            SqlCommand cmd = new SqlCommand(@"INSERT INTO transfer (transfer_type_id, transfer_status_id, account_from, account_to, amount)
        //                                            VALUES (1, 1, @account_from, @account_to, @amount);", conn);
        //            cmd.Parameters.AddWithValue("@account_from", accountFrom);
        //            cmd.Parameters.AddWithValue("@account_to", accountTo);
        //            cmd.Parameters.AddWithValue("@amount", moneyToTransfer);

        //            cmd.ExecuteScalar();
        //        }
        //    }
        //    catch (SqlException)
        //    {
        //        throw;
        //    }
        //}

        //public bool CheckBalance(decimal moneyToTransfer, int accountFrom)
        //public bool CheckBalance(Transfer transfer)
        //{
        //    decimal currentBalance = 0;
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();

        //            SqlCommand cmd = new SqlCommand(@"SELECT balance FROM account
        //                                            JOIN tenmo_user ON tenmo_user.user_id = account.user_id
        //                                            WHERE account_id = @account_from;", conn);
        //            cmd.Parameters.AddWithValue("@account_from", transfer.AccountFromId);
        //            cmd.Parameters.AddWithValue("@amount", transfer.TransferAmount);

        //            SqlDataReader reader = cmd.ExecuteReader();

        //            if (reader.Read())
        //            {
        //                currentBalance = Convert.ToDecimal(reader["balance"]);
        //            }
        //        }
        //    }
        //    catch (SqlException)
        //    {
        //        throw;
        //    }

        //    if (currentBalance >= transfer.TransferAmount)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        //public bool SendTransfer(decimal moneyToTransfer, int accountFrom, int accountTo, int transferId)
        //{
        //    if (CheckBalance(transfer))
        //    {
        //        try
        //        {
        //            using (SqlConnection conn = new SqlConnection(connectionString))
        //            {
        //                conn.Open();

        //                SqlCommand cmd = new SqlCommand(@"UPDATE account
        //                                                SET balance = balance - @amount
        //                                                WHERE account_id = @accountFrom;

        //                                                UPDATE account
        //                                                SET balance = balance + @amount
        //                                                WHERE account_id = @accountTo;

        //                                                UPDATE transfer", conn);
        //                cmd.Parameters.AddWithValue("@accountFrom", accountFrom);
        //                cmd.Parameters.AddWithValue("@accountTo", accountTo);
        //                cmd.Parameters.AddWithValue("@amount", moneyToTransfer);

        //                cmd.ExecuteNonQuery();
        //            }
        //        }
        //        catch (SqlException)
        //        {
        //            throw;
        //        }
        //        return true;
        //    }
        //    else
        //    {


        //        return false;
        //    }
        //}

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
