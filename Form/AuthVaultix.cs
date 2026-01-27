using Cryptographic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;


namespace AuthVaultix
{
    public class api
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetCurrentProcess();

        // Import the required Atom Table functions from kernel32.dll
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern ushort GlobalAddAtom(string lpString);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern ushort GlobalFindAtom(string lpString);

        public string name, ownerid, version, path, seed;
        public api(string name, string ownerid, string version, string path = null)
        {
            if (ownerid.Length != 10)
            {
                Process.Start("https://AuthVaultix.com/win/app/");
                Thread.Sleep(2000);
                error("Application not setup correctly. Please watch the YouTube video for setup.");
                TerminateProcess(GetCurrentProcess(), 1);
            }

            this.name = name;

            this.ownerid = ownerid;

            this.version = version;

            this.path = path;
        }

        #region structures
        [DataContract]
        private class response_structure
        {
            [DataMember]
            public bool success { get; set; }

            [DataMember]
            public bool newSession { get; set; }

            [DataMember]
            public string sessionid { get; set; }

            [DataMember]
            public string contents { get; set; }

            [DataMember]
            public string response { get; set; }

            [DataMember]
            public string message { get; set; }

            [DataMember]
            public string ownerid { get; set; }

            [DataMember]
            public string download { get; set; }

            [DataMember(IsRequired = false, EmitDefaultValue = false)]
            public user_data_structure info { get; set; }

            [DataMember(IsRequired = false, EmitDefaultValue = false)]
            public app_data_structure appinfo { get; set; }

            [DataMember]
            public List<msg> messages { get; set; }

            [DataMember]
            public List<users> users { get; set; }

        }

        public class msg
        {
            public string message { get; set; }
            public string author { get; set; }
            public string timestamp { get; set; }
        }

        public class users
        {
            public string credential { get; set; }
        }

        [DataContract]
        private class user_data_structure
        {
            [DataMember]
            public string username { get; set; }

            [DataMember]
            public string ip { get; set; }
            [DataMember]
            public string hwid { get; set; }
            [DataMember]
            public string createdate { get; set; }
            [DataMember]
            public string lastlogin { get; set; }
            [DataMember]
            public List<Data> subscriptions { get; set; } // array of subscriptions (basically multiple user ranks for user with individual expiry dates
        }

        [DataContract]
        private class app_data_structure
        {
            [DataMember]
            public string numUsers { get; set; }
            [DataMember]
            public string numOnlineUsers { get; set; }
            [DataMember]
            public string numKeys { get; set; }
            [DataMember]
            public string version { get; set; }
            [DataMember]
            public string customerPanelLink { get; set; }
            [DataMember]
            public string downloadLink { get; set; }
        }
        #endregion
        private static string sessionid, enckey;
        bool initialized;
        public async Task init()
        {
            Random random = new Random();
            int length = random.Next(5, 51);
            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                char randomChar = (char)random.Next(32, 127);
                sb.Append(randomChar);
            }

            seed = sb.ToString();
            checkAtom();

            var values_to_upload = new NameValueCollection
            {
                ["type"] = "init",
                ["ver"] = version,
                ["hash"] = checksum(Process.GetCurrentProcess().MainModule.FileName),
                ["name"] = name,
                ["ownerid"] = ownerid
            };

            if (!string.IsNullOrEmpty(path))
            {
                values_to_upload.Add("token", File.ReadAllText(path));
                values_to_upload.Add("thash", TokenHash(path));
            }

            var response = await req(values_to_upload);

            if (response == "AuthVaultix_Invalid")
            {
                error("Application not found");
                TerminateProcess(GetCurrentProcess(), 1);
            }

            var json = response_decoder.string_to_generic<response_structure>(response);
            if (json.ownerid == ownerid)
            {
                load_response_struct(json);
                if (json.success)
                {
                    sessionid = json.sessionid;
                    initialized = true;
                }
                else if (json.message == "invalidver")
                {
                    app_data.downloadLink = json.download;
                }
            }
            else
            {
                TerminateProcess(GetCurrentProcess(), 1);
            }
        }

#pragma warning disable IDE0052
        private System.Threading.Timer atomTimer;
#pragma warning restore IDE0052
        void checkAtom()
        {
            atomTimer = new System.Threading.Timer(_ =>
            {
                ushort foundAtom = GlobalFindAtom(seed);
                if (foundAtom == 0)
                {
                    TerminateProcess(GetCurrentProcess(), 1);
                }
            }, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }

        public static string TokenHash(string tokenPath)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var s = File.OpenRead(tokenPath))
                {
                    byte[] bytes = sha256.ComputeHash(s);
                    return BitConverter.ToString(bytes).Replace("-", string.Empty);
                }
            }
        }

        public void CheckInit()
        {
            if (!initialized)
            {
                error("You must run the function AuthVaultixApp.init(); first");
                TerminateProcess(GetCurrentProcess(), 1);
            }
        }

        public string expirydaysleft()
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
            dtDateTime = dtDateTime.AddSeconds(long.Parse(user_data.subscriptions[0].expiry)).ToLocalTime();
            TimeSpan difference = dtDateTime - DateTime.Now;
            return Convert.ToString(difference.Days + " Days " + difference.Hours + " Hours Left");
        }

        public static DateTime UnixTimeToDateTime(long unixtime)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
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

        public async Task register(string username, string pass, string key, string email = "")
        {
            CheckInit();

            string hwid = HWID.Get();

            var values_to_upload = new NameValueCollection
            {
                ["type"] = "register",
                ["username"] = username,
                ["pass"] = pass,
                ["key"] = key,
                ["email"] = email,
                ["hwid"] = hwid,
                ["sessionid"] = sessionid,
                ["name"] = name,
                ["ownerid"] = ownerid
            };

            var response = await req(values_to_upload);
            var json = response_decoder.string_to_generic<response_structure>(response);
            if (json.ownerid == ownerid)
            {
                GlobalAddAtom(seed);
                GlobalAddAtom(ownerid);

                load_response_struct(json);
                if (json.success)
                    load_user_data(json.info);
            }
            else
            {
                TerminateProcess(GetCurrentProcess(), 1);
            }
        }

        public async Task forgot(string username, string email)
        {
            CheckInit();

            var values_to_upload = new NameValueCollection
            {
                ["type"] = "forgot",
                ["username"] = username,
                ["email"] = email,
                ["sessionid"] = sessionid,
                ["name"] = name,
                ["ownerid"] = ownerid
            };

            var response = await req(values_to_upload);

            var json = response_decoder.string_to_generic<response_structure>(response);
            load_response_struct(json);
        }

        public async Task login(string username, string pass)
        {
            CheckInit();

            string hwid = HWID.Get();

            var values_to_upload = new NameValueCollection
            {
                ["type"] = "login",
                ["username"] = username,
                ["pass"] = pass,
                ["hwid"] = hwid,
                ["sessionid"] = sessionid,
                ["name"] = name,
                ["ownerid"] = ownerid
            };

            var response = await req(values_to_upload);
            var json = response_decoder.string_to_generic<response_structure>(response);
            if (json.ownerid == ownerid)
            {
                GlobalAddAtom(seed);
                GlobalAddAtom(ownerid);

                load_response_struct(json);
                if (json.success)
                    load_user_data(json.info);
            }
            else
            {
                TerminateProcess(GetCurrentProcess(), 1);
            }
        }

        public async Task logout()
        {
            CheckInit();

            var values_to_upload = new NameValueCollection
            {
                ["type"] = "logout",
                ["sessionid"] = sessionid,
                ["name"] = name,
                ["ownerid"] = ownerid
            };

            var response = await req(values_to_upload);

            var json = response_decoder.string_to_generic<response_structure>(response);
            if (json.ownerid == ownerid)
            {
                load_response_struct(json);
            }
            else
            {
                TerminateProcess(GetCurrentProcess(), 1);
            }
        }

        public async Task upgrade(string username, string key)
        {
            CheckInit();

            var values_to_upload = new NameValueCollection
            {
                ["type"] = "upgrade",
                ["username"] = username,
                ["key"] = key,
                ["sessionid"] = sessionid,
                ["name"] = name,
                ["ownerid"] = ownerid
            };

            var response = await req(values_to_upload);

            var json = response_decoder.string_to_generic<response_structure>(response);
            if (json.ownerid == ownerid)
            {
                json.success = false;
                load_response_struct(json);
            }
            else
            {
                TerminateProcess(GetCurrentProcess(), 1);
            }
        }

        public async Task license(string key)
        {
            CheckInit();

            string hwid = HWID.Get();

            var values_to_upload = new NameValueCollection
            {
                ["type"] = "license",
                ["key"] = key,
                ["hwid"] = hwid,
                ["sessionid"] = sessionid,
                ["name"] = name,
                ["ownerid"] = ownerid
            };

            var response = await req(values_to_upload);

            var json = response_decoder.string_to_generic<response_structure>(response);

            if (json.ownerid == ownerid)
            {
                GlobalAddAtom(seed);
                GlobalAddAtom(ownerid);

                load_response_struct(json);
                if (json.success)
                    load_user_data(json.info);
            }
            else
            {
                TerminateProcess(GetCurrentProcess(), 1);
            }
        }

        public async Task check()
        {
            CheckInit();

            var values_to_upload = new NameValueCollection
            {
                ["type"] = "check",
                ["sessionid"] = sessionid,
                ["name"] = name,
                ["ownerid"] = ownerid
            };

            var response = await req(values_to_upload);

            var json = response_decoder.string_to_generic<response_structure>(response);
            if (json.ownerid == ownerid)
            {
                load_response_struct(json);
            }
            else
            {
                TerminateProcess(GetCurrentProcess(), 1);
            }
        }

        public async Task setvar(string var, string data)
        {
            CheckInit();

            var values_to_upload = new NameValueCollection
            {
                ["type"] = "setvar",
                ["var"] = var,
                ["data"] = data,
                ["sessionid"] = sessionid,
                ["name"] = name,
                ["ownerid"] = ownerid
            };

            var response = await req(values_to_upload);

            var json = response_decoder.string_to_generic<response_structure>(response);
            if (json.ownerid == ownerid)
            {
                load_response_struct(json);
            }
            else
            {
                TerminateProcess(GetCurrentProcess(), 1);
            }
        }

        public async Task<string> getvar(string var)
        {
            CheckInit();

            var values_to_upload = new NameValueCollection
            {
                ["type"] = "getvar",
                ["var"] = var,
                ["sessionid"] = sessionid,
                ["name"] = name,
                ["ownerid"] = ownerid
            };

            var response = await req(values_to_upload);

            var json = response_decoder.string_to_generic<response_structure>(response);
            if (json.ownerid == ownerid)
            {
                load_response_struct(json);
                if (json.success)
                    return json.response;
            }
            else
            {
                TerminateProcess(GetCurrentProcess(), 1);
            }
            return null;
        }

        public async Task ban(string reason = null)
        {
            CheckInit();

            var values_to_upload = new NameValueCollection
            {
                ["type"] = "ban",
                ["reason"] = reason,
                ["sessionid"] = sessionid,
                ["name"] = name,
                ["ownerid"] = ownerid
            };

            var response = await req(values_to_upload);

            var json = response_decoder.string_to_generic<response_structure>(response);
            if (json.ownerid == ownerid)
            {
                load_response_struct(json);
            }
            else
            {
                TerminateProcess(GetCurrentProcess(), 1);
            }
        }

        public async Task<string> var(string varid)
        {
            CheckInit();

            var values_to_upload = new NameValueCollection
            {
                ["type"] = "var",
                ["varid"] = varid,
                ["sessionid"] = sessionid,
                ["name"] = name,
                ["ownerid"] = ownerid
            };

            var response = await req(values_to_upload);

            var json = response_decoder.string_to_generic<response_structure>(response);
            if (json.ownerid == ownerid)
            {
                load_response_struct(json);
                if (json.success)
                    return json.message;
            }
            else
            {
                TerminateProcess(GetCurrentProcess(), 1);
            }
            return null;
        }

        public async Task<List<users>> fetchOnline()
        {
            CheckInit();

            var values_to_upload = new NameValueCollection
            {
                ["type"] = "fetchOnline",
                ["sessionid"] = sessionid,
                ["name"] = name,
                ["ownerid"] = ownerid
            };

            var response = await req(values_to_upload);

            var json = response_decoder.string_to_generic<response_structure>(response);
            load_response_struct(json);

            if (json.success)
                return json.users;
            return null;
        }

        public async Task fetchStats()
        {
            CheckInit();

            var values_to_upload = new NameValueCollection
            {
                ["type"] = "fetchStats",
                ["sessionid"] = sessionid,
                ["name"] = name,
                ["ownerid"] = ownerid
            };

            var response = await req(values_to_upload);

            var json = response_decoder.string_to_generic<response_structure>(response);
            load_response_struct(json);

            if (json.success)
                load_app_data(json.appinfo);
        }

        public async Task<List<msg>> chatget(string channelname)
        {
            CheckInit();

            var values_to_upload = new NameValueCollection
            {
                ["type"] = "chatget",
                ["channel"] = channelname,
                ["sessionid"] = sessionid,
                ["name"] = name,
                ["ownerid"] = ownerid
            };

            var response = await req(values_to_upload);

            var json = response_decoder.string_to_generic<response_structure>(response);
            load_response_struct(json);
            if (json.success)
            {
                return json.messages;
            }
            return null;
        }

        public async Task<bool> chatsend(string msg, string channelname)
        {
            CheckInit();

            var values_to_upload = new NameValueCollection
            {
                ["type"] = "chatsend",
                ["message"] = msg,
                ["channel"] = channelname,
                ["sessionid"] = sessionid,
                ["name"] = name,
                ["ownerid"] = ownerid
            };

            var response = await req(values_to_upload);

            var json = response_decoder.string_to_generic<response_structure>(response);
            load_response_struct(json);
            if (json.success)
                return true;
            return false;
        }

        public async Task<bool> checkblacklist()
        {
            CheckInit();
            string hwid = HWID.Get();

            var values_to_upload = new NameValueCollection
            {
                ["type"] = "checkblacklist",
                ["hwid"] = hwid,
                ["sessionid"] = sessionid,
                ["name"] = name,
                ["ownerid"] = ownerid
            };

            var response = await req(values_to_upload);

            var json = response_decoder.string_to_generic<response_structure>(response);
            if (json.ownerid == ownerid)
            {
                load_response_struct(json);
                if (json.success)
                    return true;
                else
                    return false;
            }
            else
            {
                TerminateProcess(GetCurrentProcess(), 1);
            }
            return true; // return yes blacklisted if the OwnerID is spoofed
        }

        public async Task<byte[]> download(string fileid)
        {
            CheckInit();

            var values_to_upload = new NameValueCollection
            {

                ["type"] = "file",
                ["fileid"] = fileid,
                ["sessionid"] = sessionid,
                ["name"] = name,
                ["ownerid"] = ownerid
            };

            var response = await req(values_to_upload);

            var json = response_decoder.string_to_generic<response_structure>(response);
            load_response_struct(json);
            if (json.success)
                return encryption.str_to_byte_arr(json.contents);
            return null;
        }
        /// <summary>
        /// Logs the IP address,PC Name with a message, if a discord webhook is set up in the app settings, the log will get sent there and the dashboard if not set up it will only be in the dashboard
        /// </summary>
        /// <param name="message">Message</param>
        public async Task log(string message)
        {
            CheckInit();

            var values_to_upload = new NameValueCollection
            {
                ["type"] = "log",
                ["pcuser"] = Environment.UserName,
                ["message"] = message,
                ["sessionid"] = sessionid,
                ["name"] = name,
                ["ownerid"] = ownerid


            };

            await req(values_to_upload);
        }
        /// <summary>
        /// Change the username of a user, *User must be logged in*
        /// </summary>
        /// <param username="username">New username.</param>
        public async Task changeUsername(string username)
        {
            CheckInit();

            var values_to_upload = new NameValueCollection
            {
                ["type"] = "changeUsername",
                ["newUsername"] = username,
                ["sessionid"] = sessionid,
                ["name"] = name,
                ["ownerid"] = ownerid
            };

            var response = await req(values_to_upload);

            var json = response_decoder.string_to_generic<response_structure>(response);
            load_response_struct(json);
        }

        public static string checksum(string filename)
        {
            string result;
            using (MD5 md = MD5.Create())
            {
                using (FileStream fileStream = File.OpenRead(filename))
                {
                    byte[] value = md.ComputeHash(fileStream);
                    result = BitConverter.ToString(value).Replace("-", "").ToLowerInvariant();
                }
            }
            return result;
        }

        public static void error(string message)
        {
            string folder = @"Logs", file = Path.Combine(folder, "ErrorLogs.txt");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            if (!File.Exists(file))
            {
                using (FileStream stream = File.Create(file))
                {
                    File.AppendAllText(file, DateTime.Now + " > This is the start of your error logs file");
                }
            }

            File.AppendAllText(file, DateTime.Now + $" > {message}" + Environment.NewLine);

            System.Windows.MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(1);
        }

        private static async Task<string> req(NameValueCollection post_data)
        {
            try
            {
                if (FileCheck("authvaultix.com")) // change this url if you're using a custom domain
                {
                    error("File manipulation detected. Terminating process.");
                    Logger.LogEvent("File manipulation detected.");
                    TerminateProcess(GetCurrentProcess(), 1);
                    return "";
                }

                var formData = new List<KeyValuePair<string, string>>();
                foreach (string key in post_data)
                {
                    formData.Add(new KeyValuePair<string, string>(key, post_data[key]));
                }
                var content = new FormUrlEncodedContent(formData);

                var handler = new HttpClientHandler
                {
                    Proxy = null,
                    ServerCertificateCustomValidationCallback = (request, certificate, chain, sslPolicyErrors) =>
                    {
                        return assertSSL(request, certificate, chain, sslPolicyErrors);
                    }
                };


                using (var client = new HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromSeconds(20);

                    HttpResponseMessage response = await client.PostAsync("https://api.authvaultix.com/api/1.3/", content); // change this url if you're using a custom domain

                    if (!response.IsSuccessStatusCode)
                    {
                        switch (response.StatusCode)
                        {
                            case (HttpStatusCode)429: // Rate Limited
                                error("You're connecting too faster to loader, slow down");
                                Logger.LogEvent("You're connecting too faster to loader, slow down");
                                TerminateProcess(GetCurrentProcess(), 1);
                                break;
                            default:
                                error("Connection failure. Please try again, or contact us for help.");
                                Logger.LogEvent("Connection failure. Please try again, or contact us for help.");
                                TerminateProcess(GetCurrentProcess(), 1);
                                break;
                        }
                        return "";
                    }


                    string raw_response = await response.Content.ReadAsStringAsync();
                    var headers = new WebHeaderCollection();
                    if (response.Headers.TryGetValues("x-signature-ed25519", out IEnumerable<string> signatureValues))
                        headers["x-signature-ed25519"] = signatureValues.FirstOrDefault();

                    if (response.Headers.TryGetValues("x-signature-timestamp", out IEnumerable<string> timeStampValues))
                        headers["x-signature-timestamp"] = timeStampValues.FirstOrDefault();

                    sigCheck(raw_response, headers, post_data.Get(0));

                    Logger.LogEvent(raw_response + "\n");

                    return raw_response;


                }
            }
            catch (Exception ex)
            {
                error("Connection failure. Please try again, or contact us for help. Exception: " + ex.Message);
                Logger.LogEvent("Connection failure. Please try again, or contact us for help. Exception: " + ex.Message);
                TerminateProcess(GetCurrentProcess(), 1);
                return "";
            }
        }

        private static bool FileCheck(string domain)
        {
            try
            {
                var address = Dns.GetHostAddresses(domain);
                foreach (var addr in address)
                {
                    if (IPAddress.IsLoopback(addr) || IsPrivateIP(addr))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return true;

            }
        }

        private static bool IsPrivateIP(IPAddress ip)
        {
            byte[] bytes = ip.GetAddressBytes();
            // 10.0.0.0/8
            if (bytes[0] == 10)
                return true;
            // 172.16.0.0/12
            if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] < 32)
                return true;
            // 192.168.0.0/16
            if (bytes[0] == 192 && bytes[1] == 168)
                return true;
            return false;
        }

        private static bool assertSSL(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if ((!certificate.Issuer.Contains("Google Trust Services") && !certificate.Issuer.Contains("Let's Encrypt")) || sslPolicyErrors != SslPolicyErrors.None)
            {
                error("SSL assertion fail, make sure you're not debugging Network. Disable internet firewall on router if possible. & echo: & echo If not, ask the developer of the program to use custom domains to fix this.");
                Logger.LogEvent("SSL assertion fail, make sure you're not debugging Network. Disable internet firewall on router if possible. If not, ask the developer of the program to use custom domains to fix this.");
                return false;
            }
            return true;
        }

        private static void sigCheck(string resp, WebHeaderCollection headers, string type)
        {
            if (type == "log" || type == "file") // log doesn't return a response.
            {
                return;
            }

            try
            {
                string signature = headers["x-signature-ed25519"];
                string timestamp = headers["x-signature-timestamp"];

                // Try to parse the input string to a long Unix timestamp
                if (!long.TryParse(timestamp, out long unixTimestamp))
                {
                    error("Failed to parse the timestamp from the server. Please ensure your device's date and time settings are correct.");
                    TerminateProcess(GetCurrentProcess(), 1);
                }

                // Convert the Unix timestamp to a DateTime object (in UTC)
                DateTime timestampTime = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).UtcDateTime;

                // Get the current UTC time
                DateTime currentTime = DateTime.UtcNow;

                // Calculate the difference between the current time and the timestamp
                TimeSpan timeDifference = currentTime - timestampTime;

                // Check if the timestamp is within 20 seconds of the current time
                if (timeDifference.TotalSeconds > 20)
                {
                    error("Date/Time settings aren't synced on your device, please sync them to use the program");
                    TerminateProcess(GetCurrentProcess(), 1);
                }

                var byteSig = encryption.str_to_byte_arr(signature);
                var byteKey = encryption.str_to_byte_arr("ff243fca3a0362fc911804594685680463632790b9a38b31c66087302241d48d");
                // ... read the body from the request ...
                // ... add the timestamp and convert it to a byte[] ...
                string body = timestamp + resp;
                var byteBody = Encoding.Default.GetBytes(body);

                Console.Write(" Authenticating"); // there's also ... dots being created inside the CheckValid() function BELOW

                bool signatureValid = Ed25519.CheckValid(byteSig, byteBody, byteKey); // the ... dots in the console are from this function!
                if (!signatureValid)
                {
                    error("Signature checksum failed. Request was tampered with or session ended most likely. & echo: & echo Response: " + resp);
                    Logger.LogEvent(resp + "\n");
                    TerminateProcess(GetCurrentProcess(), 1);
                }
            }
            catch
            {
                error("Signature checksum failed. Request was tampered with or session ended most likely. & echo: & echo Response: " + resp);
                Logger.LogEvent(resp + "\n");
                TerminateProcess(GetCurrentProcess(), 1);
            }
        }

        #region app_data
        public app_data_class app_data = new app_data_class();

        public class app_data_class
        {
            public string numUsers { get; set; }
            public string numOnlineUsers { get; set; }
            public string numKeys { get; set; }
            public string version { get; set; }
            public string customerPanelLink { get; set; }
            public string downloadLink { get; set; }
        }

        private void load_app_data(app_data_structure data)
        {
            app_data.numUsers = data.numUsers;
            app_data.numOnlineUsers = data.numOnlineUsers;
            app_data.numKeys = data.numKeys;
            app_data.version = data.version;
            app_data.customerPanelLink = data.customerPanelLink;
        }
        #endregion

        #region user_data
        public user_data_class user_data = new user_data_class();

        public class user_data_class
        {
            public string username { get; set; }
            public string ip { get; set; }
            public string hwid { get; set; }
            public string createdate { get; set; }
            public string lastlogin { get; set; }
            public List<Data> subscriptions { get; set; } // array of subscriptions (basically multiple user ranks for user with individual expiry dates

            public DateTime CreationDate => AuthVaultix.api.UnixTimeToDateTime(long.Parse(createdate));
            public DateTime LastLoginDate => AuthVaultix.api.UnixTimeToDateTime(long.Parse(lastlogin));
        }
        public class Data
        {
            public string subscription { get; set; }
            public string expiry { get; set; }
            public string timeleft { get; set; }
            public string key { get; set; }

            public DateTime expiration
            {
                get
                {
                    return AuthVaultix.api.UnixTimeToDateTime(long.Parse(expiry));
                }
            }
        }

        private void load_user_data(user_data_structure data)
        {
            user_data.username = data.username;
            user_data.ip = data.ip;
            user_data.hwid = data.hwid;
            user_data.createdate = data.createdate;
            user_data.lastlogin = data.lastlogin;
            user_data.subscriptions = data.subscriptions; // array of subscriptions (basically multiple user ranks for user with individual expiry dates 
        }
        #endregion

        #region response_struct
        public response_class response = new response_class();

        public class response_class
        {
            public bool success { get; set; }
            public string message { get; set; }
        }

        private void load_response_struct(response_structure data)
        {
            response.success = data.success;
            response.message = data.message;
        }
        #endregion

        private json_wrapper response_decoder = new json_wrapper(new response_structure());
    }

    public static class Logger
    {
        public static bool IsLoggingEnabled { get; set; } = false; // Disabled by default
        public static void LogEvent(string content)
        {
            if (!IsLoggingEnabled)
            {
                //Console.WriteLine("Debug mode disabled."); // Optional: Message when logging is disabled
                return; // Exit the method if logging is disabled
            }

            string exeName = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location);

            string logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "AuthVaultix", "debug", exeName);
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            string logFileName = $"{DateTime.Now:MMM_dd_yyyy}_logs.txt";
            string logFilePath = Path.Combine(logDirectory, logFileName);

            try
            {
                // Redact sensitive fields - Add more if you would like. 
                content = RedactField(content, "sessionid");
                content = RedactField(content, "ownerid");
                content = RedactField(content, "app");
                content = RedactField(content, "version");
                content = RedactField(content, "fileid");
                content = RedactField(content, "webhooks");
                content = RedactField(content, "nonce");

                using (StreamWriter writer = File.AppendText(logFilePath))
                {
                    writer.WriteLine($"[{DateTime.Now}] [{AppDomain.CurrentDomain.FriendlyName}] {content}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging data: {ex.Message}");
            }
        }

        private static string RedactField(string content, string fieldName)
        {
            // Basic pattern matching to replace values of sensitive fields
            string pattern = $"\"{fieldName}\":\"[^\"]*\"";
            string replacement = $"\"{fieldName}\":\"REDACTED\"";

            return System.Text.RegularExpressions.Regex.Replace(content, pattern, replacement);
        }
    }

    public static class encryption
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetCurrentProcess();

        public static string HashHMAC(string enckey, string resp)
        {
            byte[] key = Encoding.UTF8.GetBytes(enckey);
            byte[] message = Encoding.UTF8.GetBytes(resp);
            var hash = new HMACSHA256(key);
            return byte_arr_to_str(hash.ComputeHash(message));
        }

        public static string byte_arr_to_str(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static byte[] str_to_byte_arr(string hex)
        {
            try
            {
                int NumberChars = hex.Length;
                byte[] bytes = new byte[NumberChars / 2];
                for (int i = 0; i < NumberChars; i += 2)
                    bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
                return bytes;
            }
            catch
            {
                api.error("The session has ended, open program again.");
                TerminateProcess(GetCurrentProcess(), 1);
                return null;
            }
        }

        public static string iv_key() =>
            Guid.NewGuid().ToString().Substring(0, 16);
    }

    public class json_wrapper
    {
        public static bool is_serializable(Type to_check) =>
            to_check.IsSerializable || to_check.IsDefined(typeof(DataContractAttribute), true);

        public json_wrapper(object obj_to_work_with)
        {
            current_object = obj_to_work_with;

            var object_type = current_object.GetType();

            serializer = new DataContractJsonSerializer(object_type);

            if (!is_serializable(object_type))
                throw new Exception($"the object {current_object} isn't a serializable");
        }

        public object string_to_object(string json)
        {
            var buffer = Encoding.Default.GetBytes(json);

            //SerializationException = session expired

            using (var mem_stream = new MemoryStream(buffer))
                return serializer.ReadObject(mem_stream);
        }

        public T string_to_generic<T>(string json) =>
            (T)string_to_object(json);

        private DataContractJsonSerializer serializer;

        private object current_object;
    }
}
public static class HWID
{
    public static string Get()
    {
        string cpu = GetWMI("Win32_Processor", "ProcessorId");
        string board = GetWMI("Win32_BaseBoard", "SerialNumber");
        string disk = GetWMI("Win32_PhysicalMedia", "SerialNumber");
        string guid = GetMachineGuid();
        string sid = WindowsIdentity.GetCurrent().User.Value;

        string raw = $"{cpu}|{board}|{disk}|{guid}|{sid}";
        return SHA256(raw);
    }

    static string GetWMI(string cls, string prop)
    {
        try
        {
            foreach (ManagementObject mo in new ManagementObjectSearcher("SELECT " + prop + " FROM " + cls).Get())
                return mo[prop]?.ToString()?.Trim() ?? "";
        }
        catch { }
        return "";
    }

    static string GetMachineGuid()
    {
        try
        {
            using (var rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography"))
                return rk.GetValue("MachineGuid")?.ToString() ?? "";
        }
        catch { return ""; }
    }

    static string SHA256(string input)
    {
        using (var sha = SHA256Managed.Create())
        {
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
