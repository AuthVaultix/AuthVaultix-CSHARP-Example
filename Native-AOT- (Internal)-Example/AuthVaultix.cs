using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

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
        public UserInfo? CurrentUser => _core.CurrentUser;
        public UserInfo? UserData => _core.UserData;
        public bool UseFullKey => _core.UseFullKey;
        public string? SessionId => _core.SessionId;
        public bool Initialized => _core.Initialized;

        public bool Init() => _core.InitializeContext();
        public bool Login(string username, string password) => _core.AuthenticateUser(username, password);
        public bool Check() => _core.ValidateSession();
        public bool Register(string username, string password, string licenseKey, string email = "") => _core.RegisterAccount(username, password, licenseKey, email);
        public bool LicenseLogin(string licenseKey) => _core.LicenseAccess(licenseKey);
        public bool Log(string message, out string? serverMessage) => _core.SendLog(message, out serverMessage);
        public bool Download(string fileId, out byte[]? fileBytes, out string? serverMessage) => _core.RetrieveFile(fileId, out fileBytes, out serverMessage);
        public bool FetchOnline(out List<OnlineUser>? users, out string? serverMessage) => _core.GetOnlineClients(out users, out serverMessage);
        public bool Ban(string reason, out string? serverMessage) => _core.EnforceBan(reason, out serverMessage);
        public void Logout() => _core.TerminateSession();
        public void ChangeUsername(string newUsername) => _core.UpdateUsername(newUsername);
        public bool CheckBlacklist(out string? serverMessage) => _core.VerifyBlacklist(out serverMessage);
        public bool ForgotPassword(string username, string email) => _core.TriggerPasswordReset(username, email);
        public bool Upgrade(string username, string licenseKey) => _core.ApplyUpgrade(username, licenseKey);
        public string? GetGlobalVar(string varKey) => _core.FetchGlobalVariable(varKey);
        public string? GetVar(string varName) => _core.FetchUserVariable(varName);
        public bool SetVar(string varName, string value) => _core.UpdateUserVariable(varName, value);
        public bool ChatSend(string message, string channel, out string? serverMessage) => _core.TransmitChatMessage(message, channel, out serverMessage);
        public Task<List<ChatMessage>> ChatFetch(string channel) => _core.RetrieveChatHistory(channel);
        public bool Tamper(string reason) => _core.ReportTampering(reason);
        public bool CheckFeaturePermission(string feature) => _core.CheckFeaturePermission(feature);
    }

    public class OnlineUser
    {
        [JsonPropertyName("credential")] public string credential { get; set; } = string.Empty;
    }

    public class UserInfo
    {
        [JsonPropertyName("username")] public string username { get; set; } = string.Empty;
        [JsonPropertyName("ip")] public string ip { get; set; } = string.Empty;
        [JsonPropertyName("hwid")] public string hwid { get; set; } = string.Empty;
        [JsonPropertyName("createdate")] public string createdate { get; set; } = string.Empty;
        [JsonPropertyName("lastlogin")] public string lastlogin { get; set; } = string.Empty;
        [JsonPropertyName("subscriptions")] public Subscription[] subscriptions { get; set; } = Array.Empty<Subscription>();

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
        [JsonPropertyName("subscription")] public string subscription { get; set; } = string.Empty;
        [JsonPropertyName("key")] public string key { get; set; } = string.Empty;
        [JsonPropertyName("expiry")] public string expiry { get; set; } = string.Empty;
        [JsonPropertyName("timeleft")] public long timeleft { get; set; }

        private long? ExpiryTimestamp => long.TryParse(expiry, out long ts) ? ts : (long?)null;
        public DateTime? ExpiryDate => ExpiryTimestamp.HasValue ? DateTimeOffset.FromUnixTimeSeconds(ExpiryTimestamp.Value).LocalDateTime : null;
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
        [JsonPropertyName("author")] public string author { get; set; } = string.Empty;
        [JsonPropertyName("role")] public string role { get; set; } = string.Empty;
        [JsonPropertyName("message")] public string message { get; set; } = string.Empty;
        [JsonPropertyName("timestamp")] public long timestamp { get; set; }
    }

    public class AppInfo
    {
        [JsonPropertyName("version")] public string version { get; set; } = string.Empty;
        [JsonPropertyName("customerPanelLink")] public string customerPanelLink { get; set; } = string.Empty;
    }

    public class UpgradeUser
    {
        [JsonPropertyName("name")] public string name { get; set; } = string.Empty;
    }

    internal class DtoBasic
    {
        [JsonPropertyName("success")] public bool Success { get; set; }
        [JsonPropertyName("message")] public string? Msg { get; set; }
    }

    internal class DtoInit : DtoBasic
    {
        [JsonPropertyName("sessionid")] public string? SessId { get; set; }
        [JsonPropertyName("appinfo")] public AppInfo? AppInfo { get; set; }
    }

    internal class DtoAuth : DtoBasic
    {
        [JsonPropertyName("info")] public UserInfo? Profile { get; set; }
        [JsonPropertyName("sessionid")] public string? SessId { get; set; }
        [JsonPropertyName("permissions")] public List<string>? Permissions { get; set; }
    }

    internal class DtoData : DtoBasic
    {
        [JsonPropertyName("contents")] public string? B64Data { get; set; }
    }

    internal class DtoVar : DtoBasic
    {
        [JsonPropertyName("response")] public string? VarData { get; set; }
    }

    internal class DtoOnline : DtoBasic
    {
        [JsonPropertyName("users")] public List<OnlineUser>? UserList { get; set; }
    }

    internal class DtoChat : DtoBasic
    {
        [JsonPropertyName("code")] public int ErrCode { get; set; }
        [JsonPropertyName("remaining_seconds")] public int RemainingSec { get; set; }
        [JsonPropertyName("muted_until")] public string? MutedTime { get; set; }
        [JsonPropertyName("remaining_human")] public string? MutedHuman { get; set; }
    }

    internal class DtoChatHistory : DtoBasic
    {
        [JsonPropertyName("messages")] public List<ChatMessage>? Log { get; set; }
    }

    internal class DtoUpgrade : DtoBasic
    {
        [JsonPropertyName("users")] public List<UpgradeUser>? Upgraded { get; set; }
    }

    [JsonSerializable(typeof(DtoBasic))]
    [JsonSerializable(typeof(DtoInit))]
    [JsonSerializable(typeof(DtoAuth))]
    [JsonSerializable(typeof(DtoData))]
    [JsonSerializable(typeof(DtoVar))]
    [JsonSerializable(typeof(DtoOnline))]
    [JsonSerializable(typeof(DtoChat))]
    [JsonSerializable(typeof(DtoChatHistory))]
    [JsonSerializable(typeof(DtoUpgrade))]
    [JsonSerializable(typeof(UserInfo))]
    [JsonSerializable(typeof(Subscription))]
    [JsonSerializable(typeof(OnlineUser))]
    [JsonSerializable(typeof(ChatMessage))]
    [JsonSerializable(typeof(AppInfo))]
    [JsonSerializable(typeof(UpgradeUser))]
    internal partial class AuthVaultixJsonContext : JsonSerializerContext
    {
    }

    internal class AuthVaultixCore
    {
        private readonly string _appName;
        private readonly string _ownerId;
        private readonly string _secret;
        private readonly string _version;
        private readonly string _apiUrl = "https://authvaultix.com/api/1.0/";

        public string RisponceCollection { get; internal set; } = "";
        public string LastMessage1 { get; internal set; } = "";
        public string LastMessage { get; internal set; } = "";
        public string LastResponseMessage { get; internal set; } = "";
        public UserInfo? CurrentUser { get; internal set; }
        public UserInfo? UserData { get; internal set; }
        public bool UseFullKey { get; internal set; }
        public string? SessionId { get; internal set; }
        public bool Initialized { get; internal set; }
        public List<string> UserPermissions { get; internal set; } = new List<string>();

        private string _encryptionKey = string.Empty;

        public AuthVaultixCore(string appName, string ownerId, string secret, string version)
        {
            if (string.IsNullOrWhiteSpace(appName) || string.IsNullOrWhiteSpace(ownerId) || string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(version))
            {
                Diagnostics.Crash("Application not setup correctly. AppName, OwnerId, Secret, and Version are required.");
            }
            _appName = appName;
            _ownerId = ownerId;
            _secret = secret;
            _version = version;
        }

        private void EnsureReady()
        {
            if (!Initialized) Diagnostics.Crash("SDK not initialized. Call Client.Init() before using any API.");
        }

        public bool InitializeContext()
        {
            RisponceCollection = "Initialization failed1";
            if (Initialized) return true;

            string iv = Guid.NewGuid().ToString("N").Substring(0, 16);
            _encryptionKey = iv + "-" + _secret;
            string? processPath = Environment.ProcessPath;
            if (string.IsNullOrEmpty(processPath))
            {
                processPath = Path.Combine(AppContext.BaseDirectory, AppDomain.CurrentDomain.FriendlyName + ".exe");
            }
            string hash = VaultixCrypto.FileHash(processPath);
            
            var payload = new PayloadBuilder("init")
                .WithValue("ver", _version)
                .WithValue("enckey", iv)
                .WithValue("hash", hash)
                .WithValue("name", _appName)
                .WithValue("ownerid", _ownerId)
                .Compile();

            string? resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "init", out _);
            if (resp == "Authvaultix_Invalid") Diagnostics.Crash("App not found");
            if (resp == null) return false;

            var dto = JsonSerializer.Deserialize(resp, AuthVaultixJsonContext.Default.DtoInit);
            if (dto == null) Diagnostics.Crash("Invalid JSON");
            if (!dto.Success) Diagnostics.Crash(dto.Msg ?? "Initialization Failed");

            SessionId = dto.SessId;
            Initialized = true;
            return true;
        }

        public bool AuthenticateUser(string username, string password)
        {
            RisponceCollection = "";
            EnsureReady();

            var payload = new PayloadBuilder("login")
                .WithContext(_appName, _ownerId, SessionId!)
                .WithValue("username", username)
                .WithValue("pass", password)
                .WithValue("hwid", HardwareIdentifier.Fetch())
                .Compile();

            string? resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "login", out _);
            if (resp == null) return false;
            var dto = JsonSerializer.Deserialize(resp, AuthVaultixJsonContext.Default.DtoAuth);

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
            RisponceCollection = "";
            EnsureReady();
            if (string.IsNullOrWhiteSpace(SessionId)) Diagnostics.Crash("Session missing");

            var payload = new PayloadBuilder("check")
                .WithContext(_appName, _ownerId, SessionId!)
                .Compile();

            string? resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "check", out _);
            if (resp == null) Diagnostics.Crash("Connection failed");
            if (string.IsNullOrWhiteSpace(resp) || resp[0] != '{') Diagnostics.Crash("Invalid response format");

            var dto = JsonSerializer.Deserialize(resp, AuthVaultixJsonContext.Default.DtoBasic);
            if (dto == null) Diagnostics.Crash("Invalid JSON");
            if (!dto.Success) Diagnostics.Crash(dto.Msg ?? "Session check failed");

            RisponceCollection = dto.Msg ?? "OK";
            LastMessage = RisponceCollection;
            LastMessage1 = RisponceCollection;
            return true;
        }

        public bool RegisterAccount(string username, string password, string licenseKey, string email)
        {
            RisponceCollection = "";
            EnsureReady();

            var payload = new PayloadBuilder("register")
                .WithContext(_appName, _ownerId, SessionId!)
                .WithValue("username", username)
                .WithValue("pass", password)
                .WithValue("key", licenseKey)
                .WithValue("email", email)
                .WithValue("hwid", HardwareIdentifier.Fetch())
                .Compile();

            string? resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "register", out _);
            if (resp == null) return false;
            var dto = JsonSerializer.Deserialize(resp, AuthVaultixJsonContext.Default.DtoAuth);

            if (dto == null || !dto.Success)
            {
                RisponceCollection = dto?.Msg ?? "Registration failed";
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
                .WithContext(_appName, _ownerId, SessionId!)
                .WithValue("key", licenseKey)
                .WithValue("hwid", HardwareIdentifier.Fetch())
                .Compile();

            string? resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "license", out _);
            if (resp == null) return false;
            var dto = JsonSerializer.Deserialize(resp, AuthVaultixJsonContext.Default.DtoAuth);

            if (dto == null || !dto.Success)
            {
                RisponceCollection = dto?.Msg ?? "License login failed";
                return false;
            }

            CurrentUser = dto.Profile;
            UserPermissions = dto.Permissions ?? new List<string>();
            if (!string.IsNullOrWhiteSpace(dto.SessId)) SessionId = dto.SessId;
            return true;
        }

        public bool SendLog(string message, out string? serverMessage)
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

            string? resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "log", out _);
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

            var dto = JsonSerializer.Deserialize(resp, AuthVaultixJsonContext.Default.DtoBasic);
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
            LastMessage = dto.Msg ?? "";
            serverMessage = dto.Msg;
            return true;
        }

        public bool RetrieveFile(string fileId, out byte[]? fileBytes, out string? serverMessage)
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

            string? resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "file", out _);
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

            var dto = JsonSerializer.Deserialize(resp, AuthVaultixJsonContext.Default.DtoData);
            if (dto == null)
            {
                serverMessage = "Invalid server response";
                return false;
            }

            LastMessage = dto.Msg ?? "";
            LastMessage1 = dto.Msg ?? "";

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

        public bool GetOnlineClients(out List<OnlineUser>? users, out string? serverMessage)
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

            string? resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "fetchonline", out _);
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

            var dto = JsonSerializer.Deserialize(resp, AuthVaultixJsonContext.Default.DtoOnline);
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

        public bool EnforceBan(string reason, out string? serverMessage)
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

            string? resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "ban", out _);
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

            var dto = JsonSerializer.Deserialize(resp, AuthVaultixJsonContext.Default.DtoBasic);
            if (dto == null)
            {
                serverMessage = "Invalid server response";
                return false;
            }

            LastMessage = dto.Msg ?? "";
            LastMessage1 = dto.Msg ?? "";

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
                .WithContext(_appName, _ownerId, SessionId!)
                .Compile();

            string? resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "logout", out _);
            var dto = resp != null ? JsonSerializer.Deserialize(resp, AuthVaultixJsonContext.Default.DtoBasic) : null;
            if (dto == null || !dto.Success) throw new Exception(dto?.Msg ?? "Logout Error");

            SessionId = null;
            Initialized = false;
            UserPermissions.Clear();
        }

        public void UpdateUsername(string newUsername)
        {
            EnsureReady();
            if (string.IsNullOrWhiteSpace(newUsername)) throw new Exception("New username cannot be empty");

            var payload = new PayloadBuilder("changeusername")
                .WithContext(_appName, _ownerId, SessionId!)
                .WithValue("newUsername", newUsername)
                .Compile();

            string? resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "changeusername", out _);
            var dto = resp != null ? JsonSerializer.Deserialize(resp, AuthVaultixJsonContext.Default.DtoBasic) : null;
            if (dto == null || !dto.Success) throw new Exception(dto?.Msg ?? "Change username Error");

            SessionId = null;
            Initialized = false;
        }

        public bool VerifyBlacklist(out string? serverMessage)
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

            string? resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "checkblacklist", out _);
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

            var dto = JsonSerializer.Deserialize(resp, AuthVaultixJsonContext.Default.DtoBasic);
            if (dto == null)
            {
                serverMessage = "Invalid server response";
                return false;
            }

            LastMessage = dto.Msg ?? "";
            LastMessage1 = dto.Msg ?? "";

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
                .WithContext(_appName, _ownerId, SessionId!)
                .WithValue("username", username)
                .WithValue("email", email)
                .Compile();

            string? resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "forgot", out _);
            var dto = resp != null ? JsonSerializer.Deserialize(resp, AuthVaultixJsonContext.Default.DtoBasic) : null;

            if (dto == null || !dto.Success)
            {
                RisponceCollection = dto?.Msg ?? "Failed";
                return false;
            }

            return true;
        }

        public bool ApplyUpgrade(string username, string licenseKey)
        {
            EnsureReady();
            var payload = new PayloadBuilder("upgrade")
                .WithContext(_appName, _ownerId, SessionId!)
                .WithValue("username", username)
                .WithValue("key", licenseKey)
                .Compile();

            string? resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "upgrade", out _);
            if (resp == null) return false;
            var dto = JsonSerializer.Deserialize(resp, AuthVaultixJsonContext.Default.DtoUpgrade);

            if (dto == null || !dto.Success)
            {
                RisponceCollection = dto?.Msg ?? "Upgrade Error";
                return false;
            }

            return true;
        }

        public string? FetchGlobalVariable(string varKey)
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

            string? resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "var", out _);
            if (string.IsNullOrWhiteSpace(resp) || resp[0] != '{')
            {
                RisponceCollection = "Invalid server response.";
                return null;
            }

            var dto = JsonSerializer.Deserialize(resp, AuthVaultixJsonContext.Default.DtoBasic);
            if (dto == null || !dto.Success)
            {
                RisponceCollection = dto?.Msg ?? "Failed to fetch variable.";
                return null;
            }

            RisponceCollection = "OK";
            return dto.Msg;
        }

        public string? FetchUserVariable(string varName)
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

            string? resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "getvar", out _);
            if (string.IsNullOrWhiteSpace(resp) || resp[0] != '{')
            {
                RisponceCollection = resp?.Trim() ?? "Request failed.";
                return null;
            }

            var dto = JsonSerializer.Deserialize(resp, AuthVaultixJsonContext.Default.DtoVar);
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

            string? resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "setvar", out _);
            if (string.IsNullOrWhiteSpace(resp) || resp[0] != '{')
            {
                RisponceCollection = resp?.Trim() ?? "Request failed.";
                return false;
            }

            var dto = JsonSerializer.Deserialize(resp, AuthVaultixJsonContext.Default.DtoBasic);
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

        public bool TransmitChatMessage(string message, string channel, out string? serverMessage)
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

            string? resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "chatsend", out _);
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

            var dto = JsonSerializer.Deserialize(resp, AuthVaultixJsonContext.Default.DtoChat);
            if (dto == null)
            {
                serverMessage = "Invalid server response.";
                return false;
            }

            LastResponseMessage = dto.Msg ?? "";

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
            LastResponseMessage = "";
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

            string? resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "chatfetch", out _);
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

            var dto = JsonSerializer.Deserialize(resp, AuthVaultixJsonContext.Default.DtoChatHistory);
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

            string? resp = NetworkAgent.Post(_apiUrl, payload, _encryptionKey, "tamper", out _);
            if (resp == null) return false;
            var dto = JsonSerializer.Deserialize(resp, AuthVaultixJsonContext.Default.DtoBasic);

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
        private readonly Dictionary<string, string> _dict;

        public PayloadBuilder(string actionType)
        {
            _dict = new Dictionary<string, string> { ["type"] = actionType };
        }

        public PayloadBuilder WithContext(string appName, string ownerId, string sessionId)
        {
            _dict["name"] = appName;
            _dict["ownerid"] = ownerId;
            if (!string.IsNullOrEmpty(sessionId))
                _dict["sessionid"] = sessionId;
            return this;
        }

        public PayloadBuilder WithValue(string key, string value)
        {
            if (value != null)
                _dict[key] = value;
            return this;
        }

        public Dictionary<string, string> Compile() => _dict;
    }

    internal class NetworkAgent
    {
        private static readonly HttpClientHandler _handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = SecureSslValidation,
            Proxy = null,
            UseProxy = false
        };

        private static readonly HttpClient _httpClient = new HttpClient(_handler);

        public static string? Post(string url, Dictionary<string, string> payload, string encKey, string actionType, out string? signature)
        {
            signature = null;
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new FormUrlEncodedContent(payload)
                };
                request.Headers.Add("User-Agent", "AuthVaultixClient/1.0");

                using var response = _httpClient.Send(request);
                
                if (response.StatusCode == (HttpStatusCode)429)
                {
                    Diagnostics.Crash("You're connecting too fast, slow down.");
                    return null;
                }

                response.EnsureSuccessStatusCode();

                string rawResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (response.Headers.TryGetValues("signature", out var values))
                {
                    signature = values.FirstOrDefault();
                }

                if (!VaultixCrypto.Verify(rawResponse, signature, actionType, encKey))
                {
                    Diagnostics.Crash("Signature verification failed. Request tampered");
                    return null;
                }
                return rawResponse;
            }
            catch (Exception ex)
            {
                Diagnostics.Crash($"Connection failure or network error: {ex.Message}");
                return null;
            }
        }

        private static bool SecureSslValidation(HttpRequestMessage request, X509Certificate2? cert, X509Chain? chain, SslPolicyErrors errors)
        {
            if (cert == null || (!cert.Issuer.Contains("Cloudflare") && !cert.Issuer.Contains("Google") && !cert.Issuer.Contains("Let's Encrypt")) || errors != SslPolicyErrors.None)
            {
                Diagnostics.Crash("SSL assertion failed. Possible MITM or proxy.");
                return false;
            }
            return true;
        }
    }

    internal static class VaultixCrypto
    {
        public static bool Verify(string payload, string? serverSig, string type, string key)
        {
            if (type == "log" || type == "file") return true;
            if (string.IsNullOrEmpty(serverSig)) return false;

            string signingKey = (type == "init") ? key.Substring(17, 64) : key;
            string localSig = GenerateHmac(signingKey, payload);
            return CryptographicEquals(localSig, serverSig);
        }

        private static string GenerateHmac(string key, string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
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
            using var sha = SHA256.Create();
            using var fs = File.OpenRead(path);
            return BitConverter.ToString(sha.ComputeHash(fs)).Replace("-", "").ToLower();
        }
    }

    internal static class HardwareIdentifier
    {
        public static string Fetch()
        {
            string userSid = "UNKNOWN-SID-A8F3D1QZ";
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    userSid = WindowsIdentity.GetCurrent().User?.Value ?? "UNKNOWN-SID-A8F3D1QZ";
                }
            }
            catch { }

            string raw = string.Join("|", Environment.MachineName, Environment.UserName, Environment.UserDomainName, Environment.OSVersion.VersionString, Environment.Is64BitOperatingSystem ? "x64" : "x86", Environment.Version.ToString(), CultureInfo.CurrentCulture.Name, userSid);
            
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
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

    internal static class Diagnostics
    {
        [System.Diagnostics.CodeAnalysis.DoesNotReturn]
        public static void Crash(string exceptionDetail)
        {
            try { File.AppendAllText("auth_diagnostics.txt", $"[{DateTime.Now}] FATAL: {exceptionDetail}\n"); } catch { }

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("\n=======================================");
            Console.WriteLine("SUBSYSTEM FAILURE");
            Console.WriteLine(exceptionDetail);
            Console.WriteLine("=======================================");
            Console.ResetColor();

            // Show popup message to explain the failure to the user
            System.Windows.Forms.MessageBox.Show(
                $"A fatal error occurred:\n\n{exceptionDetail}\n\nThe application will now close.",
                "Subsystem Failure",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Error
            );

            Environment.Exit(1);
        }
    }

    public static class AntiTamper
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isDebuggerPresent);

        private static readonly string[] BadProcesses = {
            "dnspy", "x64dbg", "x32dbg", "ollydbg", "cheatengine", "wireshark",
            "httpdebugger", "fiddler", "processhacker", "scylla", "megadumper"
        };

        public static void Check()
        {
            if (Debugger.IsAttached)
                Trigger("Debugger Attached");

            if (OperatingSystem.IsWindows())
            {
                try
                {
                    bool isDebuggerPresent = false;
                    CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref isDebuggerPresent);
                    if (isDebuggerPresent)
                        Trigger("Remote Debugger Detected");
                }
                catch
                {
                    // Ignore P/Invoke errors on non-Windows or restricted environments
                }
            }

            try
            {
                foreach (var process in Process.GetProcesses())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(process.ProcessName)) continue;
                        if (BadProcesses.Any(p => process.ProcessName.Contains(p, StringComparison.OrdinalIgnoreCase)))
                            Trigger($"Suspicious Process: {process.ProcessName}");
                    }
                    catch
                    {
                        // Ignore access denied for system processes
                    }
                }
            }
            catch
            {
                // Ignore process enumeration failures
            }
        }

        private static void Trigger(string reason)
        {
            OnTamperDetected?.Invoke(reason);
        }

        public static event Action<string>? OnTamperDetected;
    }
}
