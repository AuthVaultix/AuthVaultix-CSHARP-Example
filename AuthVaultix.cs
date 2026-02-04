using System;
using System.Text;
using System.IO;
using System.Net;
using System.Management;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AuthVaultix
{
    public class AuthVaultixClient
    {
        private const string URL = "https://api.authvaultix.com/api/1.0/"; //apiUrl.TrimEnd('/') + "/"; 

        private string AppName;
        private string OwnerId;
        private string Secret;
        private string ApiUrl;
        private string Version;

        public AuthVaultixClient(string appName, string ownerId, string secret, string version)
        {
            AppName = appName;
            OwnerId = ownerId;
            Secret = secret;
            Version = version;
            ApiUrl = URL;
        }

        public void Init()
        {
            if (Initialized) return;

            string sentKey = GenerateIV();
            EncKey = sentKey + "-" + Secret;

            var data = new NameValueCollection
            {
                ["type"] = "init",
                ["name"] = AppName,
                ["ownerid"] = OwnerId,
                ["ver"] = Version,
                ["enckey"] = sentKey
            };

            string response = Request(ApiUrl, data, out string signature);

            if (response == "Authvaultix_Invalid")
                ErrorHandler.Error("Application not found");

            var json = JsonConvert.DeserializeObject<InitResponse>(response);

            if (!json.success)
            {
                if (json.message == "invalidver")
                {
                    ErrorHandler.Error("Update required! Please download latest version.");
                }

                ErrorHandler.Error(json.message);
            }

            SessionId = json.sessionid;
            Initialized = true;
            UseFullKey = true;

            Console.WriteLine("Session Initialized: " + SessionId);
        }
        // ======================
        // LOGIN
        // ======================
        public UserInfo Login(string username, string password)
        {
            InitGuard.EnsureInitialized(Initialized);

            string hwid = SID.Get();

            var data = new NameValueCollection
            {
                ["type"] = "login",
                ["username"] = username,
                ["pass"] = password,
                ["hwid"] = hwid,
                ["sessionid"] = SessionId,
                ["name"] = AppName,
                ["ownerid"] = OwnerId
            };

            string response = Request(ApiUrl, data, out string signature);

            var json = JsonConvert.DeserializeObject<LoginResponse>(response);

            if (!json.success)
                throw new Exception(json.message);

            CurrentUser = json.info;
            return CurrentUser;
        }
        // ======================
        // REGISTER
        // ======================
        public UserInfo Register(string username, string password, string licenseKey, string email = "")
        {
            InitGuard.EnsureInitialized(Initialized);

            string hwid = SID.Get();

            var data = new NameValueCollection
            {
                ["type"] = "register",
                ["username"] = username,
                ["pass"] = password,
                ["key"] = licenseKey,
                ["email"] = email,
                ["hwid"] = hwid,
                ["sessionid"] = SessionId,
                ["name"] = AppName,
                ["ownerid"] = OwnerId
            };

            string response = Request(ApiUrl, data, out string signature);

            var json = JsonConvert.DeserializeObject<RegisterResponse>(response);

            if (!json.success)
                throw new Exception(json.message);

            CurrentUser = json.info;
            return CurrentUser;
        }

        // ======================
        // LICENSE LOGIN (AUTO REGISTER)
        // ======================
        public UserInfo LicenseLogin(string licenseKey)
        {
            InitGuard.EnsureInitialized(Initialized);

            string hwid = SID.Get();

            var data = new NameValueCollection
            {
                ["type"] = "license",
                ["key"] = licenseKey,
                ["hwid"] = hwid,
                ["sessionid"] = SessionId,
                ["name"] = AppName,
                ["ownerid"] = OwnerId
            };

            string response = Request(ApiUrl, data, out string signature);

            var json = JsonConvert.DeserializeObject<LoginResponse>(response);

            if (!json.success)
                throw new Exception(json.message);

            CurrentUser = json.info;
            return CurrentUser;
        }

        // ======================
        // LOG
        // ======================
        public void Log(string message)
        {
            InitGuard.EnsureInitialized(Initialized);

            var data = new NameValueCollection
            {
                ["type"] = "log",
                ["message"] = message,
                ["pcuser"] = Environment.UserName,
                ["sessionid"] = SessionId,
                ["name"] = AppName,
                ["ownerid"] = OwnerId
            };

            string response = Request(ApiUrl, data, out string signature);

            var json = JsonConvert.DeserializeObject<BasicResponse>(response);

            if (!json.success)
                throw new Exception(json.message);

            LastMessage = json.message;
        }

        // ======================
        // DOWNLOAD FILE
        // ======================
        public byte[] Download(string fileId)
        {
            InitGuard.EnsureInitialized(Initialized);

            var data = new NameValueCollection
            {
                ["type"] = "file",
                ["fileid"] = fileId,
                ["sessionid"] = SessionId,
                ["name"] = AppName,
                ["ownerid"] = OwnerId
            };

            string response = Request(ApiUrl, data, out string signature);

            var json = JsonConvert.DeserializeObject<DownloadResponse>(response);

            LastMessage = json.message;
            LastMessage1 = json.message;

            if (!json.success)
            {
                throw new Exception(json.message);
            }

            return Convert.FromBase64String(json.contents);
        }
        // ======================
        // CHECK SESSION
        // ======================
        public void Check()
        {
            InitGuard.EnsureInitialized(Initialized);

            var data = new NameValueCollection
            {
                ["type"] = "check",
                ["sessionid"] = SessionId,
                ["name"] = AppName,
                ["ownerid"] = OwnerId
            };

            string response = Request(ApiUrl, data, out string signature);

            var json = JsonConvert.DeserializeObject<CheckResponse>(response);

            LastMessage = json.message;
            LastMessage1 = json.message;
        }

        // ======================
        // GET USER VARIABLE
        // ======================
        public string GetVar(string varName)
        {
            InitGuard.EnsureInitialized(Initialized);

            var data = new NameValueCollection
            {
                ["type"] = "getvar",
                ["var"] = varName,
                ["sessionid"] = SessionId,
                ["name"] = AppName,
                ["ownerid"] = OwnerId
            };

            string response = Request(ApiUrl, data, out string signature);

            var json = JsonConvert.DeserializeObject<GetVarResponse>(response);

            if (!json.success)
                throw new Exception(json.message);

            return json.response;
        }
        // ======================
        // SET USER VARIABLE
        // ======================
        public void SetVar(string varName, string value)
        {
            InitGuard.EnsureInitialized(Initialized);

            var data = new NameValueCollection
            {
                ["type"] = "setvar",
                ["var"] = varName,
                ["data"] = value,
                ["sessionid"] = SessionId,
                ["name"] = AppName,
                ["ownerid"] = OwnerId
            };

            string response = Request(ApiUrl, data, out string signature);

            var json = JsonConvert.DeserializeObject<BasicResponse>(response);

            LastMessage = json.message;
            LastMessage1 = json.message;

            if (!json.success)
            {
                throw new Exception(json.message);
            }
        }
        // ======================
        // FETCH ONLINE USERS
        // ======================
        public List<OnlineUser> FetchOnline()
        {
            InitGuard.EnsureInitialized(Initialized);

            var data = new NameValueCollection
            {
                ["type"] = "fetchonline",
                ["sessionid"] = SessionId,
                ["name"] = AppName,
                ["ownerid"] = OwnerId
            };

            string response = Request(ApiUrl, data, out string signature);

            var json = JsonConvert.DeserializeObject<FetchOnlineResponse>(response);

            if (!json.success)
                throw new Exception(json.message);

            return json.users;
        }
        // ======================
        // BAN USER (SELF BAN / CURRENT SESSION)
        // ======================
        public void Ban(string reason)
        {
            InitGuard.EnsureInitialized(Initialized);

            var data = new NameValueCollection
            {
                ["type"] = "ban",
                ["sessionid"] = SessionId,
                ["name"] = AppName,
                ["ownerid"] = OwnerId,
                ["reason"] = reason
            };

            string response = Request(ApiUrl, data, out string signature);

            var json = JsonConvert.DeserializeObject<BanResponse>(response);

            LastMessage = json.message;
            LastMessage1 = json.message;

            if (!json.success)
            {
                throw new Exception(json.message);
            }

            MessageBox.Show(LastMessage);
            MessageBox.Show("Please reopen this program");
            Environment.Exit(0);
        }
        // ======================
        // LOGOUT
        // ======================
        public void Logout()
        {
            InitGuard.EnsureInitialized(Initialized);

            var data = new NameValueCollection
            {
                ["type"] = "logout",
                ["sessionid"] = SessionId,
                ["name"] = AppName,
                ["ownerid"] = OwnerId
            };

            string response = Request(ApiUrl, data, out string signature);

            var json = JsonConvert.DeserializeObject<LogoutResponse>(response);

            if (!json.success)
                throw new Exception(json.message);

            SessionId = null;
            Initialized = false;

            Console.WriteLine("Logged out successfully");
        }
        // ======================
        // CHANGE USERNAME
        // ======================
        public void ChangeUsername(string newUsername)
        {
            InitGuard.EnsureInitialized(Initialized);

            if (string.IsNullOrWhiteSpace(newUsername))
                throw new Exception("New username cannot be empty");

            var data = new NameValueCollection
            {
                ["type"] = "changeusername",
                ["sessionid"] = SessionId,
                ["newUsername"] = newUsername,
                ["name"] = AppName,
                ["ownerid"] = OwnerId
            };

            string response = Request(ApiUrl, data, out string signature);

            var json = JsonConvert.DeserializeObject<ChangeUsernameResponse>(response);

            if (!json.success)
                throw new Exception(json.message);

            SessionId = null;
            Initialized = false;

            Console.WriteLine("Username changed successfully, user logged out.");
        }
        // ======================
        // CHECK BLACKLIST
        // ======================
        public void CheckBlacklist()
        {
            InitGuard.EnsureInitialized(Initialized);

            string hwid = SID.Get();

            var data = new NameValueCollection
            {
                ["type"] = "checkblacklist",
                ["sessionid"] = SessionId,
                ["name"] = AppName,
                ["ownerid"] = OwnerId,
                ["hwid"] = hwid
            };

            string response = Request(ApiUrl, data, out string signature);

            var json = JsonConvert.DeserializeObject<BlacklistResponse>(response);

            LastMessage = json.message;
            LastMessage1 = json.message;

            Console.WriteLine("Client is NOT blacklisted");
        }
        // ======================
        // FORGOT PASSWORD
        // ======================
        public void ForgotPassword(string username, string email)
        {
            InitGuard.EnsureInitialized(Initialized);

            var data = new NameValueCollection
            {
                ["type"] = "forgot",
                ["username"] = username,
                ["email"] = email,
                ["sessionid"] = SessionId,
                ["name"] = AppName,
                ["ownerid"] = OwnerId
            };

            string response = Request(ApiUrl, data, out string signature);

            var json = JsonConvert.DeserializeObject<ForgotPasswordResponse>(response);

            if (!json.success)
                throw new Exception(json.message);

            Console.WriteLine("Reset email sent successfully");
        }
        // ======================
        // UPGRADE
        // ======================
        public void Upgrade(string username, string licenseKey)
        {
            InitGuard.EnsureInitialized(Initialized);

            var data = new NameValueCollection
            {
                ["type"] = "upgrade",
                ["username"] = username,
                ["key"] = licenseKey,
                ["sessionid"] = SessionId,
                ["name"] = AppName,
                ["ownerid"] = OwnerId
            };

            string response = Request(ApiUrl, data, out string signature);

            var json = JsonConvert.DeserializeObject<UpgradeResponse>(response);

            if (!json.success)
                throw new Exception(json.message);

            Console.WriteLine("Upgrade successful: " + json.users[0].name);
        }
        // ======================
        // GET GLOBAL VAR
        // ======================
        public string GetGlobalVar(string varKey)
        {
            InitGuard.EnsureInitialized(Initialized);

            var data = new NameValueCollection
            {
                ["type"] = "var",
                ["sessionid"] = SessionId,
                ["varid"] = varKey,
                ["name"] = AppName,
                ["ownerid"] = OwnerId
            };

            string response = Request(ApiUrl, data, out string signature);

            var json = JsonConvert.DeserializeObject<GlobalVarResponse>(response);

            if (!json.success)
                throw new Exception(json.message);

            return json.message;
        }
        // ======================
        // CHAT SEND
        // ======================
        public async Task<bool> ChatSend(string message, string channel)
        {
            InitGuard.EnsureInitialized(Initialized);

            var data = new NameValueCollection
            {
                ["type"] = "chatsend",
                ["message"] = message,
                ["channel"] = channel,
                ["sessionid"] = SessionId,
                ["name"] = AppName,
                ["ownerid"] = OwnerId
            };

            string response = Request(ApiUrl, data, out string signature);

            var json = JsonConvert.DeserializeObject<ChatResponse>(response);

            if (!json.success)
            {
                LastResponseMessage = json.message;
                return false;
            }

            LastResponseMessage = json.message;
            return true;
        }


        public async Task<List<ChatMessage>> ChatFetch(string channel)
        {
            InitGuard.EnsureInitialized(Initialized);

            var data = new NameValueCollection
            {
                ["type"] = "chatfetch",
                ["channel"] = channel,
                ["sessionid"] = SessionId,
                ["ownerid"] = OwnerId
            };

            string response = Request(ApiUrl, data, out string signature);

            var json = JsonConvert.DeserializeObject<ChatFetchResponse>(response);

            if (!json.success)
                throw new Exception(json.message);

            return json.messages;
        }

        private string Request(string url, NameValueCollection data, out string signature)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Proxy = null;

                    // SSL check
                    ServicePointManager.ServerCertificateValidationCallback += AssertSSL;

                    byte[] raw = client.UploadValues(url, data);

                    string response = Encoding.UTF8.GetString(raw);
                    signature = client.ResponseHeaders["signature"];

                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                    string type = data["type"];
                    if (!VerifySignature(response, signature, type))
                    {
                        ErrorHandler.Error("Signature verification failed. Request tampered");
                        // ErrorHandler already does Environment.Exit(0)
                        return null;
                    }

                    return response;
                }
            }
            catch (WebException webex)
            {
                if (webex.Response is HttpWebResponse resp)
                {
                    switch (resp.StatusCode)
                    {
                        case (HttpStatusCode)429:
                            ErrorHandler.Error("You're connecting too fast, slow down.");
                            break;
                        default:
                            ErrorHandler.Error("Connection failure. Please try again later.");
                            break;
                    }
                }
                else
                {
                    ErrorHandler.Error("Network error. Check internet or firewall.");
                }

                signature = null;
                return null;
            }
        }

        private static bool AssertSSL
        (
            object sender,
            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
            System.Security.Cryptography.X509Certificates.X509Chain chain,
            System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            if ((!certificate.Issuer.Contains("Cloudflare") && !certificate.Issuer.Contains("Google") && !certificate.Issuer.Contains("Let's Encrypt")) || sslPolicyErrors != System.Net.Security.SslPolicyErrors.None)
            {
                ErrorHandler.Error("SSL assertion failed. Possible MITM or proxy.");
                return false;
            }
            return true;
        }
        private bool VerifySignature(string body, string serverSignature, string type)
        {
            if (type == "log" || type == "file")
                return true;

            if (string.IsNullOrEmpty(serverSignature))
                return false;

            string signKey = (type == "init")
                ? EncKey.Substring(17, 64)
                : EncKey;

            string localSig = HashHMAC(signKey, body);
            return FixedTimeEquals.CheckStringsFixedTime(localSig, serverSignature);
        }
        public static class ErrorHandler
        {
            [DllImport("kernel32.dll")]
            private static extern bool AllocConsole();

            private static readonly string LogFile = "error_logs.txt";

            public static void Error(string msg)
            {
                AllocConsole();
                var stdOut = Console.OpenStandardOutput();
                var writer = new StreamWriter(stdOut) { AutoFlush = true };
                Console.SetOut(writer);
                Console.SetError(writer);

                try
                {
                    string log = "==============================\n" + "TIME: " + DateTime.Now + "\n" + "ERROR: " + msg + "\n" + "==============================\n\n";
                    File.AppendAllText(LogFile, log);
                }
                catch { }

                Console.Title = "AuthVaultix - Error";
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("=======================================");
                Console.WriteLine("AUTHVAULTIX ERROR");
                Console.WriteLine("=======================================");
                Console.WriteLine(msg);
                Console.WriteLine("=======================================");
                Console.ResetColor();
                Console.WriteLine();
                for (int i = 4; i >= 1; i--)
                {
                    Console.Write($"\rExiting in {i} seconds...");
                    Thread.Sleep(1000);
                }

                Environment.Exit(0);
            }
        }
        public static class FixedTimeEquals
        {
            public static bool CheckStringsFixedTime(string str1, string str2)
            {
                if (str1.Length != str2.Length)
                    return false;

                int result = 0;
                for (int i = 0; i < str1.Length; i++)
                    result |= str1[i] ^ str2[i];

                return result == 0;
            }
        }

        private static string HashHMAC(string key, string data)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
        // ======================
        // UTILS
        // ======================
        private static string GenerateIV()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 16);
        }

        public static class InitGuard
        {
            public static void EnsureInitialized(bool initialized)
            {
                if (!initialized)
                {
                    ErrorHandler.Error("SDK not initialized.\nCall Client.Init() before using any API.");
                }
            }
        }

        public string LastMessage1 { get; private set; }
        public UserInfo CurrentUser { get; private set; }
        public string LastMessage { get; private set; }
        public string LastResponseMessage { get; private set; }
        public UserInfo UserData { get; private set; }
        public bool UseFullKey { get; private set; }
        public string SessionId { get; private set; }
        public bool Initialized { get; private set; }

        private string EncKey;

    }
    public class BasicResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
    }

    public class DownloadResponse   //download deta 
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string contents { get; set; }
    }

    public class CheckResponse //session cheack 
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string role { get; set; }
    }

    public class GetVarResponse //get user variable & set user variable
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string response { get; set; }
    }

    public class FetchOnlineResponse //feach online user risponce 
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<OnlineUser> users { get; set; }
    }

    public class OnlineUser //feach online user 
    {
        public string credential { get; set; }
    }

    public class BanResponse //ban login user
    {
        public bool success { get; set; }
        public string message { get; set; }
    }

    public class LogoutResponse //logout & kill session & exit application
    {
        public bool success { get; set; }
        public string message { get; set; }
    }

    public class ChangeUsernameResponse // change uername
    {
        public bool success { get; set; }
        public string message { get; set; }
    }

    public class BlacklistResponse // mlacklst risponce
    {
        public bool success { get; set; }
        public string message { get; set; }
    }

    public class ForgotPasswordResponse // forgot pass risponce
    {
        public bool success { get; set; }
        public string message { get; set; }
    }

    public class UpgradeResponse // UpgradeUser risponce
    {
        public bool success { get; set; }
        public string message { get; set; }
        public UpgradeUser[] users { get; set; }
    }

    public class UpgradeUser // UpgradeUser risponce
    {
        public string name { get; set; } // subscription name
    }

    public class GlobalVarResponse //GlobalVar
    {
        public bool success { get; set; }
        public string message { get; set; }
    }

    public class ChatResponse //ChatResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
    }
    public class RegisterResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public UserInfo info { get; set; }
    }

    public class LoginResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public UserInfo info { get; set; }
    }

    public class UserInfo
    {
        public string username { get; set; }
        public string ip { get; set; }
        public string hwid { get; set; }
        public string createdate { get; set; }
        public string lastlogin { get; set; }
        public Subscription[] subscriptions { get; set; }
    }

    public class Subscription
    {
        public string subscription { get; set; }
        public string key { get; set; }
        public string expiry { get; set; }
        public long timeleft { get; set; }

    }

    public class ChatMessage
    {
        public string author { get; set; }
        public string role { get; set; }
        public string message { get; set; }
        public long timestamp { get; set; }
    }

    public class ChatFetchResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<ChatMessage> messages { get; set; }
    }

    public class InitResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string sessionid { get; set; }
        public string ownerid { get; set; }
        public AppInfo appinfo { get; set; }
    }

    public class AppInfo
    {
        public string version { get; set; }
        public string customerPanelLink { get; set; }
    }
}
public static class SID
{
    public static string Get()
    {
        string machine = Environment.MachineName;
        string user = Environment.UserName;
        string domain = Environment.UserDomainName;
        string os = Environment.OSVersion.VersionString;
        string arch = Environment.Is64BitOperatingSystem ? "x64" : "x86";
        string clr = Environment.Version.ToString();
        string culture = CultureInfo.CurrentCulture.Name;
        string sid = WindowsIdentity.GetCurrent().User.Value;
        string raw = string.Join("|", machine, user, domain, os, arch, clr, culture, sid);
        return Sha256Pretty(raw);
    }

    private static string Sha256Pretty(string input)
    {
        using (var sha = SHA256.Create())
        {
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sb = new StringBuilder();

            foreach (byte b in bytes)
                sb.Append(b.ToString("X2"));

            return SplitEvery(sb.ToString(), 4, "-");
        }
    }

    private static string SplitEvery(string text, int size, string separator)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < text.Length; i += size)
        {
            if (i > 0)
                sb.Append(separator);

            sb.Append(text.Substring(i, Math.Min(size, text.Length - i)));
        }
        return sb.ToString();
    }
}