using System;
using System.Collections.Generic;
using TenmoClient.Models;
using TenmoClient.Services;

namespace TenmoClient
{
    public class TenmoApp
    {
        private readonly TenmoConsoleService console = new TenmoConsoleService();
        private readonly TenmoApiService tenmoApiService;

        public TenmoApp(string apiUrl)
        {
            tenmoApiService = new TenmoApiService(apiUrl);
        }

        public void Run()
        {
            bool keepGoing = true;
            while (keepGoing)
            {
                // The menu changes depending on whether the user is logged in or not
                if (tenmoApiService.IsLoggedIn)
                {
                    keepGoing = RunAuthenticated();
                }
                else // User is not yet logged in
                {
                    keepGoing = RunUnauthenticated();
                }
            }
        }

        private bool RunUnauthenticated()
        {
            console.PrintLoginMenu();
            int menuSelection = console.PromptForInteger("Please choose an option", 0, 2, 1);
            while (true)
            {
                if (menuSelection == 0)
                {
                    return false;   // Exit the main menu loop
                }

                if (menuSelection == 1)
                {
                    // Log in
                    Login();
                    return true;    // Keep the main menu loop going
                }

                if (menuSelection == 2)
                {
                    // Register a new user
                    Register();
                    return true;    // Keep the main menu loop going
                }
                console.PrintError("Invalid selection. Please choose an option.");
                console.Pause();
            }
        }

        private bool RunAuthenticated()
        {
            console.PrintMainMenu(tenmoApiService.Username);
            int menuSelection = console.PromptForInteger("Please choose an option", 0, 6);
            if (menuSelection == 0)
            {
                // Exit the loop
                return false;
            }

            if (menuSelection == 1)
            {
                ShowBalance();
                //return true;
                // View your current balance

            }

            if (menuSelection == 2)
            {
                ShowPastTransfers();

                Console.WriteLine("Please enter transfer ID to view details (0 to cancel): ");
                ShowTransferById();
                // View your past transfers
            }

            if (menuSelection == 3)
            {
                //ShowPendingRequests();
                // View your pending requests
            }

            if (menuSelection == 4)
            {
                ShowUsersToSendBucks();

                SendTEBucks();
                // Send TE bucks
            }

            if (menuSelection == 5)
            {
                //RequestTeBucks();
                // Request TE bucks
            }

            if (menuSelection == 6)
            {
                // Log out
                tenmoApiService.Logout();
                console.PrintSuccess("You are now logged out");
            }

            return true;    // Keep the main menu loop going
        }

        private void Login()
        {
            LoginUser loginUser = console.PromptForLogin();
            if (loginUser == null)
            {
                return;
            }

            try
            {
                ApiUser user = tenmoApiService.Login(loginUser);
                if (user == null)
                {
                    console.PrintError("Login failed.");
                }
                else
                {
                    console.PrintSuccess("You are now logged in");
                }
            }
            catch (Exception)
            {
                console.PrintError("Login failed.");
            }
            console.Pause();
        }

        private void Register()
        {
            LoginUser registerUser = console.PromptForLogin();
            if (registerUser == null)
            {
                return;
            }
            try
            {
                bool isRegistered = tenmoApiService.Register(registerUser);
                if (isRegistered)
                {
                    console.PrintSuccess("Registration was successful. Please log in.");
                }
                else
                {
                    console.PrintError("Registration was unsuccessful.");
                }
            }
            catch (Exception)
            {
                console.PrintError("Registration was unsuccessful.");
            }
            console.Pause();
        }

        private void ShowBalance()
        {

            try
            {
                string accountName = tenmoApiService.Username;
                decimal balance = tenmoApiService.GetBalance(accountName);
                console.PrintBalance(tenmoApiService.Username, balance);

            }
            catch (Exception ex)
            {
                console.PrintError(ex.Message);
            }

            console.Pause();

        }


        private void ShowPastTransfers()
        {
            try
            {
                Dictionary<string, Transfer> transfers = tenmoApiService.ViewTransfers();
                console.PrintTransfers(transfers);

            }
            catch (Exception ex)
            {
                console.PrintError(ex.Message);
            }

            console.Pause();
        }

        private void ShowTransferById()
        {
            try
            {
                Dictionary<string, Transfer> transfers = tenmoApiService.ViewTransfers();
                console.PrintTransfer(transfers);
            }
            catch (Exception ex)
            {
                console.PrintError(ex.Message);
            }

            console.Pause();
        }

        private void ShowUsersToSendBucks()
        {
            try
            {
                List<User> users = tenmoApiService.ListUsersForTransfers();
                console.PrintUsers(users);
            }
            catch (Exception ex)
            {
                console.PrintError(ex.Message);
            }

            console.Pause();

        }

        private void SendTEBucks()
        {
            Transfer transfer = new Transfer();

            transfer.AccountToId = console.PromptForInteger("Id of the user you are requesting from[0]");
            transfer.TransferAmount = console.PromptForInteger("Enter amount to request");

            try
            {

                Transfer newtransfer = tenmoApiService.SendTeBucks(transfer);
            }
            catch (Exception ex)
            {
                console.PrintError(ex.Message);
            }

            console.Pause();
        }
    }
}
