using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AuthVaultix;

namespace AuthVaultixNativeAotExample
{
    internal class Program
    {
        private static readonly AuthVaultixClient Client = new AuthVaultixClient(

   		 appName: "",   // Replace with your App Name
   		 ownerId: "",   // Replace with your Owner ID
    		 secret: "",    // Replace with your Secret Key
   		 version: "1.0" // Current Version
        );

        private static void SafeClear()
        {
            try
            {
                Console.Clear();
            }
            catch (System.IO.IOException)
            {
                // Ignore if console handle is redirected or invalid
            }
        }

        private static ConsoleKeyInfo SafeReadKey()
        {
            try
            {
                return Console.ReadKey(true);
            }
            catch (InvalidOperationException)
            {
                try
                {
                    int ch = Console.Read();
                    return new ConsoleKeyInfo((char)ch, ConsoleKey.Enter, false, false, false);
                }
                catch
                {
                    return new ConsoleKeyInfo('\0', ConsoleKey.Enter, false, false, false);
                }
            }
        }

        private static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            SafeClear();
            PrintHeader();

            // AntiTamper Init se pehle start
            AntiTamper.OnTamperDetected += (reason) =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[!] TAMPER DETECTED: {reason}");

                try
                {
                    if (!string.IsNullOrEmpty(Client.SessionId))
                        Client.Tamper(reason);
                }
                catch { }

                Console.WriteLine("[!] Application will close.");
                Console.ResetColor();
                Thread.Sleep(3000);
                Environment.Exit(0);
            };

            Thread tamperThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        AntiTamper.Check();
                    }
                    catch { }

                    Thread.Sleep(5000);
                }
            })
            {
                IsBackground = true
            };
            tamperThread.Start();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[*] Connecting to AuthVaultix Servers...");
            Console.ResetColor();

            if (!Client.Init())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[!] Initialization Failed: {Client.RisponceCollection}");
                Console.ResetColor();
                Console.WriteLine("\nPress any key to exit...");
                SafeReadKey();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[+] Connection Established successfully!\n");
            Console.ResetColor();
            Thread.Sleep(800);

            MainMenuLoop();
        }

        private static void PrintHeader()
        {
            Console.WriteLine(@"
      █████╗ ██╗   ██╗████████╗██╗  ██╗██╗   ██╗ █████╗ ██╗   ██╗██╗  ████████╗██╗██╗  ██╗
     ██╔══██╗██║   ██║╚══██╔══╝██║  ██║██║   ██║██╔══██╗██║   ██║██║  ╚══██╔══╝██║╚██╗██╔╝
     ███████║██║   ██║   ██║   ███████║██║   ██║███████║██║   ██║██║     ██║   ██║ ╚███╔╝ 
     ██╔══██║██║   ██║   ██║   ██╔══██║╚██╗ ██╔╝██╔══██║██║   ██║██║     ██║   ██║ ██╔██╗ 
     ██║  ██║╚██████╔╝   ██║   ██║  ██║ ╚████╔╝ ██║  ██║╚██████╔╝███████╗██║   ██║██╔╝ ██╗
     ╚═╝  ╚═╝ ╚═════╝    ╚═╝   ╚═╝  ╚═╝  ╚═══╝  ╚═╝  ╚═╝ ╚═════╝ ╚══════╝╚═╝   ╚═╝╚═╝  ╚═╝
                      - Native AOT C# Console Example -
            ");
            Console.ResetColor();
            Console.WriteLine(new string('=', 94));
        }

        private static void MainMenuLoop()
        {
            while (true)
            {
                SafeClear();
                PrintHeader();
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(" >>> MAIN MENU <<< ");
                Console.ResetColor();
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register");
                Console.WriteLine("3. License Key Login");
                Console.WriteLine("4. Forgot Password");
                Console.WriteLine("5. Exit");
                Console.WriteLine(new string('-', 35));
                Console.Write("Choose an option: ");

                string? choice = Console.ReadLine();
                if (choice == null) return;
                switch (choice)
                {
                    case "1":
                        HandleLogin();
                        break;
                    case "2":
                        HandleRegister();
                        break;
                    case "3":
                        HandleLicenseLogin();
                        break;
                    case "4":
                        HandleForgotPassword();
                        break;
                    case "5":
                        Console.WriteLine("\nGoodbye!");
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[!] Invalid option. Press any key to retry.");
                        Console.ResetColor();
                        SafeReadKey();
                        break;
                }
            }
        }

        private static void HandleLogin()
        {
            SafeClear();
            PrintHeader();
            Console.WriteLine(" >>> LOGIN <<< ");
            Console.Write("Enter Username: ");
            string? username = Console.ReadLine();
            Console.Write("Enter Password: ");
            string? password = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[!] Credentials cannot be empty.");
                Console.ResetColor();
                SafeReadKey();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[*] Authenticating...");
            Console.ResetColor();

            if (Client.Login(username, password))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[+] Login Successful!");
                Console.ResetColor();
                Thread.Sleep(1000);
                UserDashboardLoop();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[!] Login Failed: {Client.RisponceCollection}");
                Console.ResetColor();
                Console.WriteLine("\nPress any key to return to main menu.");
                SafeReadKey();
            }
        }

        private static void HandleRegister()
        {
            SafeClear();
            PrintHeader();
            Console.WriteLine(" >>> REGISTER <<< ");
            Console.Write("Enter Username: ");
            string? username = Console.ReadLine();
            Console.Write("Enter Password: ");
            string? password = Console.ReadLine();
            Console.Write("Enter License Key: ");
            string? license = Console.ReadLine();
            Console.Write("Enter Email (Optional, press Enter to skip): ");
            string? email = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(license))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[!] Username, Password, and License Key are required.");
                Console.ResetColor();
                SafeReadKey();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[*] Registering Account...");
            Console.ResetColor();

            if (Client.Register(username, password, license, email ?? ""))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[+] Registration Successful! Welcome to AuthVaultix.");
                Console.ResetColor();
                Thread.Sleep(1500);
                UserDashboardLoop();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[!] Registration Failed: {Client.RisponceCollection}");
                Console.ResetColor();
                Console.WriteLine("\nPress any key to return.");
                SafeReadKey();
            }
        }

        private static void HandleLicenseLogin()
        {
            SafeClear();
            PrintHeader();
            Console.WriteLine(" >>> LICENSE LOGIN <<< ");
            Console.Write("Enter License Key: ");
            string? license = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(license))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[!] License Key cannot be empty.");
                Console.ResetColor();
                SafeReadKey();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[*] Verifying License...");
            Console.ResetColor();

            if (Client.LicenseLogin(license))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[+] License Login Successful!");
                Console.ResetColor();
                Thread.Sleep(1000);
                UserDashboardLoop();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[!] Verification Failed: {Client.RisponceCollection}");
                Console.ResetColor();
                Console.WriteLine("\nPress any key to return.");
                SafeReadKey();
            }
        }

        private static void HandleForgotPassword()
        {
            SafeClear();
            PrintHeader();
            Console.WriteLine(" >>> RESET PASSWORD <<< ");
            Console.Write("Enter Username: ");
            string? username = Console.ReadLine();
            Console.Write("Enter Registered Email: ");
            string? email = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[!] Fields cannot be empty.");
                Console.ResetColor();
                SafeReadKey();
                return;
            }

            if (Client.ForgotPassword(username, email))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[+] Password reset request submitted successfully! Check your email.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[!] Request Failed: {Client.RisponceCollection}");
            }
            Console.ResetColor();
            Console.WriteLine("\nPress any key to return.");
            SafeReadKey();
        }

        private static void UserDashboardLoop()
        {
            while (true)
            {
                        SafeClear();
                        PrintHeader();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"                    >>> Welcome, {Client.CurrentUser?.username ?? "User"} <<< ");
                        Console.ResetColor();
                        Console.WriteLine($"User IP:          {Client.CurrentUser?.ip}");
                        Console.WriteLine($"HWID]:            {Client.CurrentUser?.hwid}");
                        Console.WriteLine($"Creation Date:    {Client.CurrentUser?.CreationDateFormatted}");
                        Console.WriteLine($"Last Login:       {Client.CurrentUser?.LastLoginFormatted}");
                
                if (Client.CurrentUser?.subscriptions != null && Client.CurrentUser.subscriptions.Length > 0)
                {
                    for (int i = 0; i < Client.CurrentUser.subscriptions.Length; i++)
                    {
                        var sub = Client.CurrentUser.subscriptions[i];
                        Console.WriteLine($"Subscriptions     {i + 1}:      {sub.subscription}");
                        Console.WriteLine($"License Key:      {sub.key}");
                        Console.WriteLine($"Expiry Date:      {sub.ExpiryFormatted}");
                        Console.WriteLine($"Time Left:        {sub.TimeLeft}");
                    }
                }
                Console.WriteLine(new string('=', 94));

                Console.WriteLine("1. Verify Session");
                Console.WriteLine("2. Fetch Online Users");
                Console.WriteLine("3. Access Chatroom");
                Console.WriteLine("4. Change Username");
                Console.WriteLine("5. Get Global Variable");
                Console.WriteLine("6. Send Log Message");
                Console.WriteLine("7. Verify Blacklist status");
                Console.WriteLine("8. Logout & Return to Main Menu");
                Console.WriteLine(new string('-', 35));
                Console.Write("Choose an option: ");

                string? choice = Console.ReadLine();
                if (choice == null) return;
                switch (choice)
                {
                    case "1":
                        VerifySession();
                        break;
                    case "2":
                        ShowOnlineUsers();
                        break;
                    case "3":
                        ChatroomLoop().GetAwaiter().GetResult();
                        break;
                    case "4":
                        ChangeUsername();
                        break;
                    case "5":
                        FetchGlobalVar();
                        break;
                    case "6":
                        SendServerLog();
                        break;
                    case "7":
                        CheckBlacklist();
                        break;
                    case "8":
                        Console.WriteLine("\nLogging out...");
                        try { Client.Logout(); } catch { }
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[!] Invalid option. Press any key to retry.");
                        Console.ResetColor();
                        SafeReadKey();
                        break;
                }
            }
        }

        private static void VerifySession()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[*] Contacting Server to validate session...");
            Console.ResetColor();

            if (Client.Check())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[+] Session Valid! Server message: {Client.LastMessage}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[!] Session Invalid: {Client.RisponceCollection}");
            }
            Console.ResetColor();
            Console.WriteLine("\nPress any key to return.");
            SafeReadKey();
        }

        private static void ShowOnlineUsers()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[*] Fetching online clients...");
            Console.ResetColor();

            if (Client.FetchOnline(out var users, out string? msg))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[+] Fetch Successful! Total Users Online: {users?.Count ?? 0}");
                Console.ResetColor();
                if (users != null)
                {
                    foreach (var user in users)
                    {
                        Console.WriteLine($" - {user.credential}");
                    }
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[!] Fetch Failed: {msg}");
            }
            Console.ResetColor();
            Console.WriteLine("\nPress any key to return.");
            SafeReadKey();
        }

        private static async Task ChatroomLoop()
        {
            string channel = "test";
            while (true)
            {
                SafeClear();
                PrintHeader();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($" >>> CHATROOM - Channel: {channel} <<< ");
                Console.ResetColor();
                Console.WriteLine("Type '/exit' to return. Type '/refresh' to reload messages manually.");
                Console.WriteLine(new string('-', 55));

                var messages = await Client.ChatFetch(channel);
                if (messages != null && messages.Count > 0)
                {
                    foreach (var msg in messages)
                    {
                        string timeStr = DateTimeOffset.FromUnixTimeSeconds(msg.timestamp).ToLocalTime().ToString("hh:mm tt");
                        Console.ForegroundColor = msg.role == "Developer" ? ConsoleColor.Cyan : ConsoleColor.Gray;
                        Console.Write($"[{timeStr}] ");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"{msg.author}: ");
                        Console.ResetColor();
                        Console.WriteLine(msg.message);
                    }
                }
                else
                {
                    Console.WriteLine("No messages found in this channel.");
                }

                Console.WriteLine(new string('-', 55));
                Console.Write("Message > ");
                string? input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input)) continue;
                if (input.Equals("/exit", StringComparison.OrdinalIgnoreCase)) return;
                if (input.Equals("/refresh", StringComparison.OrdinalIgnoreCase)) continue;

                if (Client.ChatSend(input, channel, out string? serverMsg))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[+] Sent!");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[!] Error: {serverMsg}");
                    Console.ResetColor();
                    Thread.Sleep(1500);
                }
            }
        }

        private static void ChangeUsername()
        {
            Console.Write("Enter New Username: ");
            string? newUsername = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newUsername)) return;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[*] Updating username on server...");
            Console.ResetColor();

            try
            {
                Client.ChangeUsername(newUsername);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[+] Username changed successfully! Session closed. Please log in again.");
                Console.ResetColor();
                SafeReadKey();
                throw new Exception("Session Terminated"); // Will return to main menu
            }
            catch (Exception ex) when (ex.Message == "Session Terminated")
            {
                // Force return to main menu
                throw;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[!] Error: {ex.Message}");
                Console.ResetColor();
                SafeReadKey();
            }
        }

        private static void FetchGlobalVar()
        {
            Console.Write("Enter Global Variable ID: ");
            string? varId = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(varId)) return;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[*] Fetching variable...");
            Console.ResetColor();

            string? val = Client.GetGlobalVar(varId);
            if (val != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[+] Value: {val}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[!] Error: {Client.RisponceCollection}");
            }
            Console.ResetColor();
            Console.WriteLine("\nPress any key to return.");
            SafeReadKey();
        }

        private static void SendServerLog()
        {
            Console.Write("Enter Message to Log: ");
            string? message = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(message)) return;

            if (Client.Log(message, out string? serverMsg))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[+] Log Sent! Server replied: {serverMsg}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[!] Failed: {serverMsg}");
            }
            Console.ResetColor();
            Console.WriteLine("\nPress any key to return.");
            SafeReadKey();
        }

        private static void CheckBlacklist()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[*] Checking blacklist status...");
            Console.ResetColor();

            if (Client.CheckBlacklist(out string? msg))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[+] Status: {msg}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[!] Status: {msg}");
            }
            Console.ResetColor();
            Console.WriteLine("\nPress any key to return.");
            SafeReadKey();
        }
    }
}
