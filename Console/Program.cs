using AuthVaultix;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AuthVaultix
{
    class Program
    {

        /*
        * 
        * WATCH THIS VIDEO TO SETUP APPLICATION: 
        * 
        * READ HERE TO LEARN ABOUT AuthVaultix FUNCTIONS 
        *
        */

        public static api AuthVaultixApp = new api(
                name: "",
                ownerid: "",
                secret: "",
                version: ""                  
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern ushort GlobalFindAtom(string lpString);

        static void Main(string[] args)
        {
            securityChecks();
            Console.Title = "Loader";
            Console.WriteLine("\n\n Connecting..");
            AuthVaultixApp.init();

            Console.Write(
                "\n [1] Login" +
                "\n [2] Register" +
                "\n [3] Upgrade" +
                "\n [4] License key only" +
                "\n [5] Forgot password" +
                "\n\n Choose option: "
            );

            string username, password, key, email, code;

            string input = Console.ReadLine();

            if (!int.TryParse(input, out int option) || option < 1 || option > 5)
            {
                Console.WriteLine("\n ❌ Invalid option. Please enter a number between 1 and 5.");
                Thread.Sleep(2000);
                Environment.Exit(0);
            }

            switch (option)
            {
                case 1:
                    Console.Write("\n Enter username: ");
                    username = Console.ReadLine();
                    Console.Write("\n Enter password: ");
                    password = Console.ReadLine();
                    AuthVaultixApp.login(username, password);
                    break;
                case 2:
                    Console.Write("\n Enter username: ");
                    username = Console.ReadLine();
                    Console.Write("\n Enter password: ");
                    password = Console.ReadLine();
                    Console.Write("\n Enter license: ");
                    key = Console.ReadLine();
                    Console.Write("\n Enter email (just press enter if none): ");
                    email = Console.ReadLine();
                    AuthVaultixApp.register(username, password, key, email);
                    break;
                case 3:
                    Console.Write("\n Enter username: ");
                    username = Console.ReadLine();
                    Console.Write("\n Enter license: ");
                    key = Console.ReadLine();
                    AuthVaultixApp.upgrade(username, key);
                    Console.WriteLine("\n Status: " + AuthVaultixApp.response.message);
                    Thread.Sleep(2500);
                    TerminateProcess(GetCurrentProcess(), 1);
                    break;
                case 4:
                    Console.Write("\n Enter license: ");
                    key = Console.ReadLine();
                    AuthVaultixApp.license(key);
                    break;
                case 5:
                    Console.Write("\n Enter username: ");
                    username = Console.ReadLine();
                    Console.Write("\n Enter email: ");
                    email = Console.ReadLine();
                    AuthVaultixApp.forgot(username, email);
                    Console.WriteLine("\n Status: " + AuthVaultixApp.response.message);
                    Thread.Sleep(2500);
                    TerminateProcess(GetCurrentProcess(), 1);
                    break;
                default:
                    Console.WriteLine("\n Invalid Selection");
                    Thread.Sleep(2500);
                    TerminateProcess(GetCurrentProcess(), 1);
                    break;
            }

            if (!AuthVaultixApp.response.success)
            {
                Console.WriteLine("\n Status: " + AuthVaultixApp.response.message);
                Thread.Sleep(2500);
                TerminateProcess(GetCurrentProcess(), 1);
            }

            Console.WriteLine("\n\n Logged In!"); 
            Console.WriteLine("\n User data:");
            Console.WriteLine(" Username: " + AuthVaultixApp.user_data.username);
            Console.WriteLine(" License: " + AuthVaultixApp.user_data.subscriptions[0].key); 
            Console.WriteLine(" IP address: " + AuthVaultixApp.user_data.ip);
            Console.WriteLine(" Hardware-Id: " + AuthVaultixApp.user_data.hwid);
            Console.WriteLine(" Created at: " + UnixTimeToDateTime(long.Parse(AuthVaultixApp.user_data.createdate)));
            if (!string.IsNullOrEmpty(AuthVaultixApp.user_data.lastlogin)) 
                Console.WriteLine(" Last login at: " + UnixTimeToDateTime(long.Parse(AuthVaultixApp.user_data.lastlogin)));
            Console.WriteLine(" Your subscription(s):");
            for (var i = 0; i < AuthVaultixApp.user_data.subscriptions.Count; i++)
            {
                Console.WriteLine(" Subscription name: " + AuthVaultixApp.user_data.subscriptions[i].subscription + " - Expires at: " + UnixTimeToDateTime(long.Parse(AuthVaultixApp.user_data.subscriptions[i].expiry)) + " - Time left in seconds: " + AuthVaultixApp.user_data.subscriptions[i].timeleft);
            }

            Console.WriteLine("\n Press any key to close...");
            Console.ReadKey(true); // waits for any key press
            Environment.Exit(0);

        }

        public static bool SubExist(string name)
        {
            if(AuthVaultixApp.user_data.subscriptions.Exists(x => x.subscription == name))
                return true;
            return false;
        }
		
        public static DateTime UnixTimeToDateTime(long unixtime)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
            try
            {
                dtDateTime = dtDateTime.AddSeconds(unixtime).ToLocalTime();
            }
            catch
            {
                dtDateTime = DateTime.MaxValue;
            }
            return dtDateTime;
        }

        static void checkAtom()
        {
            Thread atomCheckThread = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(60000); // give people 1 minute to login
                    ushort foundAtom = GlobalFindAtom(AuthVaultixApp.ownerid);
                    if (foundAtom == 0)
                    {
                        TerminateProcess(GetCurrentProcess(), 1);
                    }
                }
            });

            atomCheckThread.IsBackground = true; 
            atomCheckThread.Start();
        }

        static void securityChecks()
        {

            var frames = new StackTrace().GetFrames();
            foreach (var frame in frames)
            {
                MethodBase method = frame.GetMethod();
                if (method != null && method.DeclaringType?.Assembly != Assembly.GetExecutingAssembly())
                {
                    TerminateProcess(GetCurrentProcess(), 1);
                }
            }

            var harmonyAssembly = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "0Harmony");

            if (harmonyAssembly != null)
            {
                TerminateProcess(GetCurrentProcess(), 1);
            }

            checkAtom();
        }
        
    }
}
