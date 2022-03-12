using System;
using System.Collections.Generic;
using TenmoClient.Models;

namespace TenmoClient.Services
{
    public class TenmoConsoleService : ConsoleService
    {
        /************************************************************
            Print methods
        ************************************************************/
        public void PrintLoginMenu()
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("Welcome to TEnmo!");
            Console.WriteLine("1: Login");
            Console.WriteLine("2: Register");
            Console.WriteLine("0: Exit");
            Console.WriteLine("---------");
        }

        public void PrintMainMenu(string username)
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine($"Hello, {username}!");
            Console.WriteLine("1: View your current balance");
            Console.WriteLine("2: View your past transfers");
            Console.WriteLine("3: View your pending requests");
            Console.WriteLine("4: Send TE bucks");
            Console.WriteLine("5: Request TE bucks");
            Console.WriteLine("6: Log out");
            Console.WriteLine("0: Exit");
            Console.WriteLine("---------");
        }
        public LoginUser PromptForLogin()
        {
            string username = PromptForString("User name");
            if (String.IsNullOrWhiteSpace(username))
            {
                return null;
            }
            string password = PromptForHiddenString("Password");

            LoginUser loginUser = new LoginUser
            {
                Username = username,
                Password = password
            };
            return loginUser;
        }

        // Add application-specific UI methods here...

        public void PrintBalance(string username, decimal balance)
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine($"Hello, {username}!");
            Console.WriteLine($"Your current account balance is: {balance.ToString("C")}");

            Console.WriteLine("---------");
        }

        public void PrintTransfers(Dictionary<string, Transfer> transfers)
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine($"Transfer ID             From/to             Amount");

            foreach (KeyValuePair<string, Transfer> transfer in transfers)
            {
                if (transfer.Value.UserFrom == null)
                {
                    Console.WriteLine($"{transfer.Key}                To: {transfer.Value.UserTo}            {transfer.Value.TransferAmount.ToString("C")}");
                }
                else if (transfer.Value.UserTo == null)
                {
                    Console.WriteLine($"{transfer.Key}              From: {transfer.Value.UserFrom}            {transfer.Value.TransferAmount.ToString("C")}");
                }
            }

            Console.WriteLine("---------------------------------------------------");
        }



        public void PrintTransfer(Dictionary<string, Transfer> transferList)
        {
            string idSelected = Console.ReadLine();
            Transfer info = transferList[idSelected];

            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("Transfer Details");
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine($"Id: {info.TransferId}");
            Console.WriteLine($"From: {info.UserFrom}");
            Console.WriteLine($"To: {info.UserTo}");
            Console.WriteLine("Status: Approved");
            Console.WriteLine($"Amount: {info.TransferAmount.ToString("C")}");
        }

        public void PrintUsers(List<User> users)
        {
            Console.WriteLine("Please choose an option: 4");
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("------------------Users----------------------");
            Console.WriteLine("|      Id   |   Username              ");
            Console.WriteLine("--------------------------------------------");
            foreach (User user in users)
            {
                Console.WriteLine($"      {user.UserId}  |  {user.Username}");
            }
        }

        public void PrintSendingOptions()
        {
            Console.WriteLine("Id of the user you are requesting from[0]: ");
        }

        public void PrintTransfer(Dictionary<string, Transfer> transferList)
        {
            string idSelected = Console.ReadLine();
            Transfer info = transferList[idSelected];

            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("Transfer Details");
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine($"Id: {info.TransferId}");
            Console.WriteLine($"From: {info.UserFrom}");
            Console.WriteLine($"To: {info.UserTo}");
            Console.WriteLine("Status: Approved");
            Console.WriteLine($"Amount: {info.TransferAmount.ToString("C")}");
        }


    }
}
