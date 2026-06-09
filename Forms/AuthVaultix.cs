using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AuthVaultix
{
    public class AuthVaultixClient
    {
        private readonly AuthVaultixCore _core;

        public AuthVaultixClient(string appName, string ownerId, string secret, string version)
        {
            _core = new AuthVaultixCore(appName, ownerId, secret, version);
        }

        public string RisponceCollection => _core.RisponceCollection;
        public string LastMessage1 => _core.LastMessage1;
        public string LastMessage => _core.LastMessage;
        public string LastResponseMessage => _core.LastResponseMessage;
        public UserInfo CurrentUser => _core.CurrentUser;
        public UserInfo UserData => _core.UserData;
        public bool UseFullKey => _core.UseFullKey;
        public string SessionId => _core.SessionId;
        public bool Initialized => _core.Initialized;

        public bool Init() => _core.InitializeContext();
        public bool Login(string username, string password) => _core.AuthenticateUser(username, password);
        public bool Check() => _core.ValidateSession();
        public bool Register(string username, string password, string licenseKey, string email = "") => _core.RegisterAccount(username, password, licenseKey, email);
        public bool LicenseLogin(string licenseKey) => _core.LicenseAccess(licenseKey);
        public bool Log(string message, out string serverMessage) => _core.SendLog(message, out serverMessage);
        public bool Download(string fileId, out byte[] fileBytes, out string serverMessage) => _core.RetrieveFile(fileId, out fileBytes, out serverMessage);
        public bool FetchOnline(out List<OnlineUser> users, out string serverMessage) => _core.GetOnlineClients(out users, out serverMessage);
        public bool Ban(string reason, out string serverMessage) => _core.EnforceBan(reason, out serverMessage);
        public void Logout() => _core.TerminateSession();
        public void ChangeUsername(string newUsername) => _core.UpdateUsername(newUsername);
        public bool CheckBlacklist(out string serverMessage) => _core.VerifyBlacklist(out serverMessage);
        public bool ForgotPassword(string username, string email) => _core.TriggerPasswordReset(username, email);
        public bool Upgrade(string username, string licenseKey) => _core.ApplyUpgrade(username, licenseKey);
        public string GetGlobalVar(string varKey) => _core.FetchGlobalVariable(varKey);
        public string GetVar(string varName) => _core.FetchUserVariable(varName);
        public bool SetVar(string varName, string value) => _core.UpdateUserVariable(varName, value);
        public bool ChatSend(string message, string channel, out string serverMessage) => _core.TransmitChatMessage(message, channel, out serverMessage);
        public Task<List<ChatMessage>> ChatFetch(string channel) => _core.RetrieveChatHistory(channel);
        public bool Tamper(string reason) => _core.ReportTampering(reason);
        public bool CheckFeaturePermission(string feature) => _core.CheckFeaturePermission(feature);
    }

    public class OnlineUser
    {
        public string credential { get; set; }
    }

    public class UserInfo
    {
        public string username { get; set; }
        public string ip { get; set; }
        public string hwid { get; set; }
        public string createdate { get; set; }
        public string lastlogin { get; set; }
        public Subscription[] subscriptions { get; set; }

        private DateTime? ParseUnix(string value)
        {
            if (long.TryParse(value, out long ts))
                return DateTimeOffset.FromUnixTimeSeconds(ts).LocalDateTime;
            return null;
        }

        public DateTime? CreationDate => ParseUnix(createdate);
        public DateTime? LastLoginDate => ParseUnix(lastlogin);
        public string CreationDateFormatted => CreationDate?.ToString("dd/MM/yyyy hh:mm tt") ?? "Invalid date";
        public string LastLoginFormatted => LastLoginDate?.ToString("dd/MM/yyyy hh:mm tt") ?? "Invalid date";
    }

    public class Subscription
    {
        public string subscription { get; set; }
        public string key { get; set; }
        public string expiry { get; set; }
        public long timeleft { get; set; }

        private long? ExpiryTimestamp => long.TryParse(expiry, out long ts) ? ts : (long?)null;
        public DateTime? ExpiryDate => ExpiryTimestamp.HasValue ? DateTimeOffset.FromUnixTimeSeconds(ExpiryTimestamp.Value).LocalDateTime : (DateTime?)null;
        public string ExpiryFormatted => ExpiryDate?.ToString("dd/MM/yyyy hh:mm tt") ?? "Invalid date";
        public string TimeLeft
        {
            get
            {
                if (ExpiryDate == null) return "N/A";
                var diff = ExpiryDate.Value - DateTime.Now;
                if (diff.TotalSeconds <= 0) return "Expired";
                return $"{diff.Days}d {diff.Hours}h {diff.Minutes}m {diff.Seconds}s";
            }
        }
    }

    public class ChatMessage
    {
        public string author { get; set; }
        public string role { get; set; }
        public string message { get; set; }
        public long timestamp { get; set; }
    }

    public class AppInfo
    {
        public string version { get; set; }
        public string customerPanelLink { get; set; }
    }

    public class UpgradeUser
    {
        public string name { get; set; }
    }

        internal class DtoBasic
    {
        [JsonProperty("success")] public bool Success { get; set; }
        [JsonProperty("message")] public string Msg { get; set; }
    }

    internal class DtoInit : DtoBasic
    {
        [JsonProperty("sessionid")] public string SessId { get; set; }
        [JsonProperty("appinfo")] public AppInfo AppInfo { get; set; }
    }

    internal class DtoAuth : DtoBasic
    {
        [JsonProperty("info")] public UserInfo Profile { get; set; }
        [JsonProperty("sessionid")] public string SessId { get; set; }
        [JsonProperty("permissions")] public List<string> Permissions { get; set; }
    }

    internal class DtoData : DtoBasic
    {
        [JsonProperty("contents")] public string B64Data { get; set; }
    }

    internal class DtoVar : DtoBasic
    {
        [JsonProperty("response")] public string VarData { get; set; }
    }

    internal class DtoOnline : DtoBasic
    {
        [JsonProperty("users")] public List<OnlineUser> UserList { get; set; }
    }

    internal class DtoChat : DtoBasic
    {
        [JsonProperty("code")] public int ErrCode { get; set; }
        [JsonProperty("remaining_seconds")] public int RemainingSec { get; set; }
        [JsonProperty("muted_until")] public string MutedTime { get; set; }
        [JsonProperty("remaining_human")] public string MutedHuman { get; set; }
    }

    internal class DtoChatHistory : DtoBasic
    {
        [JsonProperty("messages")] public List<ChatMessage> Log { get; set; }
    }

    internal class DtoUpgrade : DtoBasic
    {
        [JsonProperty("users")] public List<UpgradeUser> Upgraded { get; set; }
    }

    internal class AuthVaultixCore
    {
        private readonly string _appName;
        private readonly string _ownerId;
        private readonly string _secret;
        private readonly string _version;
        private readonly string _apiUrl = "https://authvaultix.com/api/1.0/";

        public string RisponceCollection { get; internal set; } = "";
        public string LastMessage1 { get; internal set; }
        public string LastMessage { get; internal set; }
        public string LastResponseMessage { get; internal set; }
        public UserInfo CurrentUser { get; internal set; }
        public UserInfo UserData { get; internal set; }
        public bool UseFullKey { get; internal set; }
        public string SessionId { get; internal set; }
        public bool Initialized { get; internal set; }
        public List<string> UserPermissions { get; internal set; } = new List<string>();

        private string _encryptionKey;

        public AuthVaultixCore(string appName, string ownerId, string secret, string version)
        {
            if (string.IsNullOrWhiteSpace(appName) || string.IsNullOrWhiteSpace(ownerId) || string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(version))
            {
                Process.Start("https://youtu.be/rJ1x1fYiZoU?si=GffkGIAGupPHWa0x");
                Process.Start("https://authvaultix.com/win/app/");
                Thread.Sleep(2000);
                Diagnostics.Crash("Application not setup correctly.\nPlease watch the YouTube video for setup.");
            }
            _appName = appName;
            _ownerId = ownerId;
            _secret = secret;
            _version = version;
        }

        private void EnsureReady()
        {
            if (!Initialized) Diagnostics.Crash("SDK not initialized.\nCall Client.Init() before using any API.");
        }

        public bool InitializeContext()
        {
            RisponceCollection = "Initialization failed1";
            if (Initialized) return true;

            string iv = Guid.NewGuid().ToString("N").Substring(0, 16);
            _encryptionKey = iv + "-" + _secret;
            string hash = VaultixCrypto.FileHash(Process.GetCurrentProcess().MainModule.FileName);
            var payload = new PayloadBuilder("init")
                .WithValue("ver", _version)
                .WithValue("enckey", iv)
                .WithValue("hash", hash)
                .WithValue("name", _appName)
                .WithValue("ownerid", _ownerId)
                .Compile();

            string resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "init", out _);
            if (resp == "Authvaultix_Invalid") Diagnostics.Crash("App not found");

            var dto = JsonConvert.DeserializeObject<DtoInit>(resp);
            if (dto == null) Diagnostics.Crash("Invalid JSON");
            if (!dto.Success) Diagnostics.Crash(dto.Msg);

            SessionId = dto.SessId;
            Initialized = true;
            Console.WriteLine("Session Initialized: " + SessionId);
            return true;
        }

        public bool AuthenticateUser(string username, string password)
        {
            RisponceCollection = null;
            EnsureReady();

            var payload = new PayloadBuilder("login")
                .WithContext(_appName, _ownerId, SessionId)
                .WithValue("username", username)
                .WithValue("pass", password)
                .WithValue("hwid", HardwareIdentifier.Fetch())
                .Compile();

            string resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "login", out _);
            var dto = JsonConvert.DeserializeObject<DtoAuth>(resp);

            if (dto == null || !dto.Success)
            {
                RisponceCollection = dto?.Msg ?? "Login failed";
                return false;
            }

            CurrentUser = dto.Profile;
            UserPermissions = dto.Permissions ?? new List<string>();
            if (!string.IsNullOrWhiteSpace(dto.SessId)) SessionId = dto.SessId;
            return true;
        }

        public bool ValidateSession()
        {
            RisponceCollection = null;
            EnsureReady();
            if (string.IsNullOrWhiteSpace(SessionId)) Diagnostics.Crash("Session missing");

            var payload = new PayloadBuilder("check")
                .WithContext(_appName, _ownerId, SessionId)
                .Compile();

            string resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "check", out _);
            if (resp == null) Diagnostics.Crash("Connection failed");
            if (string.IsNullOrWhiteSpace(resp) || resp[0] != '{') Diagnostics.Crash("Invalid response format");

            var dto = JsonConvert.DeserializeObject<DtoBasic>(resp);
            if (dto == null) Diagnostics.Crash("Invalid JSON");
            if (!dto.Success) Diagnostics.Crash(dto.Msg ?? "Session check failed");

            RisponceCollection = dto.Msg;
            LastMessage = RisponceCollection;
            LastMessage1 = RisponceCollection;
            return true;
        }

        public bool RegisterAccount(string username, string password, string licenseKey, string email)
        {
            RisponceCollection = null;
            EnsureReady();

            var payload = new PayloadBuilder("register")
                .WithContext(_appName, _ownerId, SessionId)
                .WithValue("username", username)
                .WithValue("pass", password)
                .WithValue("key", licenseKey)
                .WithValue("email", email)
                .WithValue("hwid", HardwareIdentifier.Fetch())
                .Compile();

            string resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "register", out _);
            var dto = JsonConvert.DeserializeObject<DtoAuth>(resp);

            if (dto == null || !dto.Success)
            {
                RisponceCollection = dto?.Msg;
                return false;
            }

            CurrentUser = dto.Profile;
            UserPermissions = dto.Permissions ?? new List<string>();
            if (!string.IsNullOrWhiteSpace(dto.SessId)) SessionId = dto.SessId;
            return true;
        }

        public bool LicenseAccess(string licenseKey)
        {
            EnsureReady();
            var payload = new PayloadBuilder("license")
                .WithContext(_appName, _ownerId, SessionId)
                .WithValue("key", licenseKey)
                .WithValue("hwid", HardwareIdentifier.Fetch())
                .Compile();

            string resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "license", out _);
            var dto = JsonConvert.DeserializeObject<DtoAuth>(resp);

            if (dto == null || !dto.Success)
            {
                RisponceCollection = dto?.Msg;
                return false;
            }

            CurrentUser = dto.Profile;
            UserPermissions = dto.Permissions ?? new List<string>();
            if (!string.IsNullOrWhiteSpace(dto.SessId)) SessionId = dto.SessId;
            return true;
        }

        public bool SendLog(string message, out string serverMessage)
        {
            serverMessage = null;
            EnsureReady();
            if (string.IsNullOrWhiteSpace(SessionId))
            {
                serverMessage = "Session missing. Please login again.";
                return false;
            }

            var payload = new PayloadBuilder("log")
                .WithContext(_appName, _ownerId, SessionId)
                .WithValue("message", message)
                .WithValue("pcuser", Environment.UserName)
                .Compile();

            string resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "log", out _);
            if (string.IsNullOrWhiteSpace(resp))
            {
                serverMessage = "Log request failed (no response).";
                return false;
            }
            if (resp[0] != '{')
            {
                serverMessage = resp.Trim();
                return false;
            }

            var dto = JsonConvert.DeserializeObject<DtoBasic>(resp);
            if (dto == null)
            {
                serverMessage = "Invalid server response";
                return false;
            }
            if (!dto.Success)
            {
                serverMessage = dto.Msg ?? "Log failed";
                return false;
            }
            LastMessage = dto.Msg;
            serverMessage = dto.Msg;
            return true;
        }

        public bool RetrieveFile(string fileId, out byte[] fileBytes, out string serverMessage)
        {
            fileBytes = null;
            serverMessage = null;
            EnsureReady();
            if (string.IsNullOrWhiteSpace(SessionId))
            {
                serverMessage = "Session missing. Please login again.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(fileId))
            {
                serverMessage = "Invalid file id.";
                return false;
            }

            var payload = new PayloadBuilder("file")
                .WithContext(_appName, _ownerId, SessionId)
                .WithValue("fileid", fileId)
                .Compile();

            string resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "file", out _);
            if (string.IsNullOrWhiteSpace(resp))
            {
                serverMessage = "Download request failed (no response).";
                return false;
            }
            if (resp[0] != '{')
            {
                serverMessage = resp.Trim();
                return false;
            }

            var dto = JsonConvert.DeserializeObject<DtoData>(resp);
            if (dto == null)
            {
                serverMessage = "Invalid server response";
                return false;
            }

            LastMessage = dto.Msg;
            LastMessage1 = dto.Msg;

            if (!dto.Success)
            {
                serverMessage = dto.Msg ?? "Download failed";
                return false;
            }
            if (string.IsNullOrWhiteSpace(dto.B64Data))
            {
                serverMessage = "File content missing";
                return false;
            }
            try
            {
                fileBytes = Convert.FromBase64String(dto.B64Data);
                serverMessage = dto.Msg ?? "Download successful";
                return true;
            }
            catch (FormatException)
            {
                serverMessage = "Invalid file encoding (base64)";
                return false;
            }
        }

        public bool GetOnlineClients(out List<OnlineUser> users, out string serverMessage)
        {
            users = null;
            serverMessage = null;
            EnsureReady();
            if (string.IsNullOrWhiteSpace(SessionId))
            {
                serverMessage = "Session missing. Please login again.";
                return false;
            }

            var payload = new PayloadBuilder("fetchonline")
                .WithContext(_appName, _ownerId, SessionId)
                .Compile();

            string resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "fetchonline", out _);
            if (string.IsNullOrWhiteSpace(resp))
            {
                serverMessage = "Request failed. Please try again.";
                return false;
            }
            if (resp[0] != '{')
            {
                serverMessage = resp.Trim();
                return false;
            }

            var dto = JsonConvert.DeserializeObject<DtoOnline>(resp);
            if (dto == null)
            {
                serverMessage = "Invalid server response.";
                return false;
            }
            if (!dto.Success)
            {
                serverMessage = dto.Msg ?? "Failed to fetch online users.";
                return false;
            }

            users = dto.UserList ?? new List<OnlineUser>();
            serverMessage = dto.Msg ?? "OK";
            return true;
        }

        public bool EnforceBan(string reason, out string serverMessage)
        {
            serverMessage = null;
            EnsureReady();
            if (string.IsNullOrWhiteSpace(SessionId))
            {
                serverMessage = "Session missing. Please login again.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(reason)) reason = "No reason provided";

            var payload = new PayloadBuilder("ban")
                .WithContext(_appName, _ownerId, SessionId)
                .WithValue("reason", reason)
                .Compile();

            string resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "ban", out _);
            if (string.IsNullOrWhiteSpace(resp))
            {
                serverMessage = "Request failed. Please try again.";
                return false;
            }
            if (resp[0] != '{')
            {
                serverMessage = resp.Trim();
                return false;
            }

            var dto = JsonConvert.DeserializeObject<DtoBasic>(resp);
            if (dto == null)
            {
                serverMessage = "Invalid server response";
                return false;
            }

            LastMessage = dto.Msg;
            LastMessage1 = dto.Msg;

            if (!dto.Success)
            {
                serverMessage = dto.Msg ?? "Ban failed";
                return false;
            }

            serverMessage = dto.Msg ?? "Banned";
            return true;
        }

        public void TerminateSession()
        {
            EnsureReady();
            var payload = new PayloadBuilder("logout")
                .WithContext(_appName, _ownerId, SessionId)
                .Compile();

            string resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "logout", out _);
            var dto = JsonConvert.DeserializeObject<DtoBasic>(resp);
            if (dto == null || !dto.Success) throw new Exception(dto?.Msg ?? "Logout Error");

            SessionId = null;
            Initialized = false;
            UserPermissions.Clear();
            Console.WriteLine("Logged out successfully");
        }

        public void UpdateUsername(string newUsername)
        {
            EnsureReady();
            if (string.IsNullOrWhiteSpace(newUsername)) throw new Exception("New username cannot be empty");

            var payload = new PayloadBuilder("changeusername")
                .WithContext(_appName, _ownerId, SessionId)
                .WithValue("newUsername", newUsername)
                .Compile();

            string resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "changeusername", out _);
            var dto = JsonConvert.DeserializeObject<DtoBasic>(resp);
            if (dto == null || !dto.Success) throw new Exception(dto?.Msg ?? "Change username Error");

            SessionId = null;
            Initialized = false;
            Console.WriteLine("Username changed successfully, user logged out.");
        }

        public bool VerifyBlacklist(out string serverMessage)
        {
            serverMessage = null;
            EnsureReady();
            if (string.IsNullOrWhiteSpace(SessionId))
            {
                serverMessage = "Session missing. Please login again.";
                return false;
            }

            var payload = new PayloadBuilder("checkblacklist")
                .WithContext(_appName, _ownerId, SessionId)
                .WithValue("hwid", HardwareIdentifier.Fetch())
                .Compile();

            string resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "checkblacklist", out _);
            if (string.IsNullOrWhiteSpace(resp))
            {
                serverMessage = "Request failed. Please try again.";
                return false;
            }
            if (resp[0] != '{')
            {
                serverMessage = resp.Trim();
                return false;
            }

            var dto = JsonConvert.DeserializeObject<DtoBasic>(resp);
            if (dto == null)
            {
                serverMessage = "Invalid server response";
                return false;
            }

            LastMessage = dto.Msg;
            LastMessage1 = dto.Msg;

            if (!dto.Success)
            {
                serverMessage = dto.Msg ?? "Client is blacklisted";
                return false;
            }

            serverMessage = dto.Msg ?? "Client is not blacklisted";
            return true;
        }

        public bool TriggerPasswordReset(string username, string email)
        {
            EnsureReady();
            var payload = new PayloadBuilder("forgot")
                .WithContext(_appName, _ownerId, SessionId)
                .WithValue("username", username)
                .WithValue("email", email)
                .Compile();

            string resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "forgot", out _);
            var dto = JsonConvert.DeserializeObject<DtoBasic>(resp);

            if (dto == null || !dto.Success)
            {
                RisponceCollection = dto?.Msg ?? "Failed";
                return false;
            }

            Console.WriteLine("Reset email sent successfully");
            return true;
        }

        public bool ApplyUpgrade(string username, string licenseKey)
        {
            EnsureReady();
            var payload = new PayloadBuilder("upgrade")
                .WithContext(_appName, _ownerId, SessionId)
                .WithValue("username", username)
                .WithValue("key", licenseKey)
                .Compile();

            string resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "upgrade", out _);
            var dto = JsonConvert.DeserializeObject<DtoUpgrade>(resp);

            if (dto == null || !dto.Success)
            {
                RisponceCollection = dto?.Msg ?? "Upgrade Error";
                return false;
            }

            Console.WriteLine("Upgrade successful: " + (dto.Upgraded != null && dto.Upgraded.Count > 0 ? dto.Upgraded[0].name : "Unknown"));
            return true;
        }

        public string FetchGlobalVariable(string varKey)
        {
            RisponceCollection = "";
            EnsureReady();
            if (string.IsNullOrWhiteSpace(SessionId))
            {
                RisponceCollection = "Session missing. Please login again.";
                return null;
            }
            if (string.IsNullOrWhiteSpace(varKey))
            {
                RisponceCollection = "Invalid variable key.";
                return null;
            }

            var payload = new PayloadBuilder("var")
                .WithContext(_appName, _ownerId, SessionId)
                .WithValue("varid", varKey)
                .Compile();

            string resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "var", out _);
            if (string.IsNullOrWhiteSpace(resp) || resp[0] != '{')
            {
                RisponceCollection = "Invalid server response.";
                return null;
            }

            var dto = JsonConvert.DeserializeObject<DtoBasic>(resp);
            if (dto == null || !dto.Success)
            {
                RisponceCollection = dto?.Msg ?? "Failed to fetch variable.";
                return null;
            }

            RisponceCollection = "OK";
            return dto.Msg;
        }

        public string FetchUserVariable(string varName)
        {
            RisponceCollection = "";
            EnsureReady();
            if (string.IsNullOrWhiteSpace(SessionId))
            {
                RisponceCollection = "Session missing. Please login again.";
                return null;
            }
            if (string.IsNullOrWhiteSpace(varName))
            {
                RisponceCollection = "Invalid variable name.";
                return null;
            }

            var payload = new PayloadBuilder("getvar")
                .WithContext(_appName, _ownerId, SessionId)
                .WithValue("var", varName)
                .Compile();

            string resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "getvar", out _);
            if (string.IsNullOrWhiteSpace(resp) || resp[0] != '{')
            {
                RisponceCollection = resp?.Trim() ?? "Request failed.";
                return null;
            }

            var dto = JsonConvert.DeserializeObject<DtoVar>(resp);
            if (dto == null || !dto.Success)
            {
                RisponceCollection = dto?.Msg ?? "Failed to get variable.";
                return null;
            }

            RisponceCollection = dto.Msg ?? "OK";
            return dto.VarData;
        }

        public bool UpdateUserVariable(string varName, string value)
        {
            RisponceCollection = "";
            EnsureReady();
            if (string.IsNullOrWhiteSpace(SessionId))
            {
                RisponceCollection = "Session missing. Please login again.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(varName))
            {
                RisponceCollection = "Invalid variable name.";
                return false;
            }

            var payload = new PayloadBuilder("setvar")
                .WithContext(_appName, _ownerId, SessionId)
                .WithValue("var", varName)
                .WithValue("data", value ?? string.Empty)
                .Compile();

            string resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "setvar", out _);
            if (string.IsNullOrWhiteSpace(resp) || resp[0] != '{')
            {
                RisponceCollection = resp?.Trim() ?? "Request failed.";
                return false;
            }

            var dto = JsonConvert.DeserializeObject<DtoBasic>(resp);
            if (dto == null)
            {
                RisponceCollection = "Invalid server response.";
                return false;
            }

            RisponceCollection = dto.Msg ?? (dto.Success ? "OK" : "Failed");
            LastMessage = RisponceCollection;
            LastMessage1 = RisponceCollection;

            return dto.Success;
        }

        public bool TransmitChatMessage(string message, string channel, out string serverMessage)
        {
            serverMessage = null;
            EnsureReady();
            if (string.IsNullOrWhiteSpace(SessionId))
            {
                serverMessage = "Session missing. Please login again.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                serverMessage = "Message cannot be empty.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(channel))
            {
                serverMessage = "Invalid channel.";
                return false;
            }

            var payload = new PayloadBuilder("chatsend")
                .WithContext(_appName, _ownerId, SessionId)
                .WithValue("message", message)
                .WithValue("channel", channel)
                .Compile();

            string resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "chatsend", out _);
            if (string.IsNullOrWhiteSpace(resp))
            {
                serverMessage = "Request failed. Please try again.";
                return false;
            }
            if (resp[0] != '{')
            {
                serverMessage = resp.Trim();
                LastResponseMessage = serverMessage;
                return false;
            }

            var dto = JsonConvert.DeserializeObject<DtoChat>(resp);
            if (dto == null)
            {
                serverMessage = "Invalid server response.";
                return false;
            }

            LastResponseMessage = dto.Msg;

            if (!dto.Success)
            {
                if (dto.ErrCode == 403 && dto.RemainingSec > 0)
                {
                    serverMessage = $"Muted till {dto.MutedTime} (wait {dto.MutedHuman})";
                    LastResponseMessage = serverMessage;
                    return false;
                }
                serverMessage = dto.Msg ?? "Failed to send message.";
                return false;
            }

            serverMessage = dto.Msg ?? "Message sent.";
            return true;
        }

        public Task<List<ChatMessage>> RetrieveChatHistory(string channel)
        {
            EnsureReady();
            LastResponseMessage = null;
            if (string.IsNullOrWhiteSpace(SessionId))
            {
                LastResponseMessage = "Session missing. Please login again.";
                return Task.FromResult(new List<ChatMessage>());
            }
            if (string.IsNullOrWhiteSpace(channel))
            {
                LastResponseMessage = "Invalid channel.";
                return Task.FromResult(new List<ChatMessage>());
            }

            var payload = new PayloadBuilder("chatfetch")
                .WithContext(_appName, _ownerId, SessionId)
                .WithValue("channel", channel)
                .Compile();

            string resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "chatfetch", out _);
            if (string.IsNullOrWhiteSpace(resp))
            {
                LastResponseMessage = "Request failed. Please try again.";
                return Task.FromResult(new List<ChatMessage>());
            }
            if (resp[0] != '{')
            {
                LastResponseMessage = resp.Trim();
                return Task.FromResult(new List<ChatMessage>());
            }

            var dto = JsonConvert.DeserializeObject<DtoChatHistory>(resp);
            if (dto == null)
            {
                LastResponseMessage = "Invalid server response.";
                return Task.FromResult(new List<ChatMessage>());
            }

            if (!dto.Success)
            {
                LastResponseMessage = dto.Msg ?? "Failed to fetch chat messages.";
                return Task.FromResult(new List<ChatMessage>());
            }

            LastResponseMessage = dto.Msg ?? "OK";
            return Task.FromResult(dto.Log ?? new List<ChatMessage>());
        }

        public bool ReportTampering(string reason)
        {
            EnsureReady();
            if (string.IsNullOrWhiteSpace(SessionId)) return false;
            if (string.IsNullOrWhiteSpace(reason)) reason = "Tampering Detected";

            var payload = new PayloadBuilder("tamper")
                .WithContext(_appName, _ownerId, SessionId)
                .WithValue("hwid", HardwareIdentifier.Fetch())
                .WithValue("reason", reason)
                .Compile();

            string resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "tamper", out _);
            var dto = JsonConvert.DeserializeObject<DtoBasic>(resp);

            if (dto == null || !dto.Success)
            {
                RisponceCollection = dto?.Msg ?? "Tamper report failed";
                return false;
            }

            return true;
        }

        public bool CheckFeaturePermission(string feature)
        {
            if (string.IsNullOrEmpty(feature) || UserPermissions == null) return false;
            return UserPermissions.Contains(feature);
        }
    }

    internal class PayloadBuilder
    {
        private readonly NameValueCollection _nvc;

        public PayloadBuilder(string actionType)
        {
            _nvc = new NameValueCollection { ["type"] = actionType };
        }

        public PayloadBuilder WithContext(string appName, string ownerId, string sessionId)
        {
            _nvc["name"] = appName;
            _nvc["ownerid"] = ownerId;
            if (!string.IsNullOrEmpty(sessionId))
                _nvc["sessionid"] = sessionId;
            return this;
        }

        public PayloadBuilder WithValue(string key, string value)
        {
            if (value != null)
                _nvc[key] = value;
            return this;
        }

        public NameValueCollection Compile() => _nvc;
    }

    internal class NetworkAgent
    {
        public static string Post(string url, NameValueCollection payload, string encKey, string actionType, out string signature)
        {
            signature = string.Empty;
            try
            {
                using (var client = new WebClient { Proxy = null })
                {
                    client.Headers.Add("User-Agent", "AuthVaultixClient/1.0");
                    ServicePointManager.ServerCertificateValidationCallback += SecureSslValidation;

                    byte[] responseBytes = client.UploadValues(url, payload);
                    string rawResponse = Encoding.UTF8.GetString(responseBytes);
                    signature = client.ResponseHeaders["signature"];

                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                    if (!VaultixCrypto.Verify(rawResponse, signature, actionType, encKey))
                    {
                        Diagnostics.Crash("Signature verification failed. Request tampered");
                        return null;
                    }
                    return rawResponse;
                }
            }
            catch (WebException wex)
            {
                if (wex.Response is HttpWebResponse resp && resp.StatusCode == (HttpStatusCode)429)
                    Diagnostics.Crash("You're connecting too fast, slow down.");
                else
                    Diagnostics.Crash("Connection failure or network error.");
                return null;
            }
        }

        private static bool SecureSslValidation(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {
            if ((!cert.Issuer.Contains("Cloudflare") && !cert.Issuer.Contains("Google") && !cert.Issuer.Contains("Let's Encrypt")) || errors != SslPolicyErrors.None)
            {
                Diagnostics.Crash("SSL assertion failed. Possible MITM or proxy.");
                return false;
            }
            return true;
        }
    }

    internal static class VaultixCrypto
    {
        public static bool Verify(string payload, string serverSig, string type, string key)
        {
            if (type == "log" || type == "file") return true;
            if (string.IsNullOrEmpty(serverSig)) return false;

            string signingKey = (type == "init") ? key.Substring(17, 64) : key;
            string localSig = GenerateHmac(signingKey, payload);
            return CryptographicEquals(localSig, serverSig);
        }

        private static string GenerateHmac(string key, string data)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        private static bool CryptographicEquals(string a, string b)
        {
            if (a.Length != b.Length) return false;
            int r = 0;
            for (int i = 0; i < a.Length; i++) r |= a[i] ^ b[i];
            return r == 0;
        }

        public static string FileHash(string path)
        {
            using (var sha = SHA256.Create())
            using (var fs = File.OpenRead(path))
            {
                return BitConverter.ToString(sha.ComputeHash(fs)).Replace("-", "").ToLower();
            }
        }
    }

    internal static class HardwareIdentifier
    {
        public static string Fetch()
        {
            string raw = string.Join("|", Environment.MachineName, Environment.UserName, Environment.UserDomainName, Environment.OSVersion.VersionString, Environment.Is64BitOperatingSystem ? "x64" : "x86", Environment.Version.ToString(), CultureInfo.CurrentCulture.Name, WindowsIdentity.GetCurrent().User.Value);
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
                var sb = new StringBuilder();
                foreach (byte b in bytes) sb.Append(b.ToString("X2"));
                string hex = sb.ToString();

                var formatted = new StringBuilder();
                for (int i = 0; i < hex.Length; i += 4)
                {
                    if (i > 0) formatted.Append("-");
                    formatted.Append(hex.Substring(i, Math.Min(4, hex.Length - i)));
                }
                return formatted.ToString();
            }
        }
    }

    internal static class Diagnostics
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        public static void Crash(string exceptionDetail)
        {
            try { File.AppendAllText("auth_diagnostics.txt", $"[{DateTime.Now}] FATAL: {exceptionDetail}\n"); } catch { }

            AllocConsole();
            var stdOut = Console.OpenStandardOutput();
            using (var writer = new StreamWriter(stdOut) { AutoFlush = true })
            {
                Console.SetOut(writer);
                Console.SetError(writer);
                Console.Title = "System Halt";
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("=======================================");
                Console.WriteLine("SUBSYSTEM FAILURE");
                Console.WriteLine(exceptionDetail);
                Console.WriteLine("=======================================");
                Console.ResetColor();
                Thread.Sleep(3000);
            }
            Environment.Exit(1);
        }
    }

    public static class AntiTamper
    {
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isDebuggerPresent);

        private static readonly string[] BadProcesses = { 
            "dnspy", "x64dbg", "x32dbg", "ollydbg", "cheatengine", "wireshark", 
            "httpdebugger", "fiddler", "processhacker", "scylla", "megadumper" 
        };

        public static void Check()
        {
            if (Debugger.IsAttached)
                Trigger("Debugger Attached");

            bool isDebuggerPresent = false;
            CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref isDebuggerPresent);
            if (isDebuggerPresent)
                Trigger("Remote Debugger Detected");

            foreach (var process in Process.GetProcesses())
            {
                if (BadProcesses.Any(p => process.ProcessName.ToLower().Contains(p)))
                    Trigger($"Suspicious Process: {process.ProcessName}");
            }
        }

        private static void Trigger(string reason)
        {
            // If we are logged in, try to report it.
            // Note: LoginForm.Client or equivalent needs to be accessible.
            // We'll let the MainForm handle the reporting if possible, 
            // or we can pass the client instance here.
            
            // For now, let's throw an exception that MainForm can catch 
            // or just define a static event.
            OnTamperDetected?.Invoke(reason);
        }

        public static event Action<string> OnTamperDetected;
    }
}
