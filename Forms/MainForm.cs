using AuthVaultix;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Drag.MakeDraggable(this);
            timer1.Interval = 5000; // 5 sec (testing)
            timer1.Start();

            // Subscribe to tamper detection
            AntiTamper.OnTamperDetected += (reason) =>
            {
                // Trigger the API report
                LoginForm.Client.Tamper(reason);
                MessageBox.Show($"Tamper Detected: {reason}\nYou have been banned.", "Security Violation", MessageBoxButtons.OK, MessageBoxIcon.Error);
               // Environment.Exit(0);
            };
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Fetches the user's "level" variable from the server. This is used to determine user access rights (e.g., VIP status). The server stores this variable per account.Based on its value, we can enable or restrict certain features.
            string level = LoginForm.Client.GetVar("level");

            if (level == "vip")
            {
                MessageBox.Show("⚠ Your update support has expired.\n\nPlease renew your subscription to continue receiving updates and support.", "Support Expired", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                //MessageBox.Show("🔒 Not a VIP user.");
            }

            userDataField.Items.Add($"Username: {LoginForm.Client.CurrentUser.username}");
            userDataField.Items.Add($"License: {LoginForm.Client.CurrentUser.subscriptions[0].key}"); ;
            userDataField.Items.Add($"Expires: {LoginForm.Client.CurrentUser.subscriptions[0].ExpiryFormatted}");
            userDataField.Items.Add($"Subscription: {LoginForm.Client.CurrentUser.subscriptions[0].subscription}");
            userDataField.Items.Add($"IP: {LoginForm.Client.CurrentUser.ip}");
            userDataField.Items.Add($"HWID: {LoginForm.Client.CurrentUser.hwid}");
            userDataField.Items.Add($"Creation Date: {LoginForm.Client.CurrentUser.CreationDateFormatted}");
            userDataField.Items.Add($"Last Login: {LoginForm.Client.CurrentUser.LastLoginFormatted}");
            userDataField.Items.Add($"Time Left: {LoginForm.Client.CurrentUser.subscriptions[0].TimeLeft}");
            
            // Add feature permission checks
            bool hasVip = LoginForm.Client.CheckFeaturePermission("VIP");
            bool haspremium = LoginForm.Client.CheckFeaturePermission("premium");
            bool hasEsp = LoginForm.Client.CheckFeaturePermission("ESP");

            userDataField.Items.Add($"VIP Feature: {(hasVip ? "Access Granted" : "Access Denied")}");
            userDataField.Items.Add($"premium Feature: {(haspremium ? "Access Granted" : "Access Denied")}");
            userDataField.Items.Add($"ESP Feature: {(hasEsp ? "Access Granted" : "Access Denied")}");

            Console.WriteLine($"[+] VIP Feature: {(hasVip ? "Access Granted! VIP menu loaded." : "Access Denied! Please buy VIP package.")}");
            Console.WriteLine($"[+] premium Feature: {(haspremium ? "Activated successfully!" : "Locked! Upgrade required.")}");
            Console.WriteLine($"[+] ESP Feature: {(hasEsp ? "Activated successfully!" : "Locked! Upgrade required.")}");

            try
            {
                List<OnlineUser> onlineUsers;
                string msg;

                if (!LoginForm.Client.FetchOnline(out onlineUsers, out msg))
                {
                    MessageBox.Show(msg, "Error"); // ✅ message show karo
                    return;

                }
                onlineUsersField.Items.Clear();
                foreach (var user in onlineUsers)
                    onlineUsersField.Items.Add(user.credential);
            }
            catch (Exception ex) { Console.WriteLine("Status: " + ex.Message); }
        }

        private void sendLogDataBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string logMsg;
                if (!LoginForm.Client.Log(logDataField.Text, out logMsg))
                {
                    MessageBox.Show(logMsg);
                }
                else { MessageBox.Show(logMsg); }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Log error: " + ex.Message);
            }
        }

        private void banBtn_Click(object sender, EventArgs e)
        {
            string msg;
            if (LoginForm.Client.Ban("Cheating detected", out msg))
            {
                MessageBox.Show(msg, "Banned");
                MessageBox.Show("Please reopen this program");
                Environment.Exit(0);
                return;
            }
            else { MessageBox.Show(msg, "Ban Failed"); }
        }

        private void checkSessionBtn_Click(object sender, EventArgs e)
        {
            if (LoginForm.Client.Check())
            { MessageBox.Show(LoginForm.Client.RisponceCollection); return; }
        }

        private void CheackBlacklistBtn_Click(object sender, EventArgs e)
        {

            string msg;
            if (!LoginForm.Client.CheckBlacklist(out msg))
            {
                MessageBox.Show(msg);
                return;
            }
            else { MessageBox.Show(msg); }
        }
        public static string GetPattern(string value)
        {
            return LoginForm.Client.GetGlobalVar(value);
        }

        private void fetchGlobalVariableBtn_Click(object sender, EventArgs e)
        {
            string val = LoginForm.Client.GetGlobalVar(globalVariableField.Text);

            if (val == null)
            {
                MessageBox.Show(LoginForm.Client.RisponceCollection);
                return;
            }

            MessageBox.Show("Global var value: " + val);
        }


        private void downloadFileBtn_Click(object sender, EventArgs e)
        {
            byte[] bytes;
            string msg;

            if (!LoginForm.Client.Download("EC5FF376", out bytes, out msg))
            {
                MessageBox.Show(msg, "Download Failed");
                return;
            }
            if (bytes == null || bytes.Length == 0) { MessageBox.Show("File data empty", "Error"); return; }

            try
            {
                string fullPath = Path.Combine(filePathField.Text, fileExtensionField.Text);
                File.WriteAllBytes(fullPath, bytes);
                MessageBox.Show("Downloaded " + bytes.Length + " bytes", "Success");
            }
            catch (Exception ex) { MessageBox.Show("File save error: " + ex.Message, "Error"); }
        }

        private void fetchUserVarBtn_Click(object sender, EventArgs e)
        {
            string val = LoginForm.Client.GetVar(varField.Text);

            if (val == null)
            {
                MessageBox.Show(LoginForm.Client.RisponceCollection);
                return;
            }

            MessageBox.Show(val);
        }

        private void setUserVarBtn_Click(object sender, EventArgs e)
        {
            if (!LoginForm.Client.SetVar(varField.Text, varDataField.Text))
            {
                MessageBox.Show(LoginForm.Client.RisponceCollection);
                return;
            }

            MessageBox.Show(LoginForm.Client.RisponceCollection);
        }


        private void closeBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        string chatchannel = "test";
        private async void sendMsgBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string msg;
                bool ok = LoginForm.Client.ChatSend(chatMsgField.Text, chatchannel, out msg);

                if (ok)
                {
                    chatroomGrid.Rows.Insert(0, LoginForm.Client.CurrentUser.username, chatMsgField.Text, DateTime.Now.ToString());
                    chatMsgField.Clear();
                }
                else
                {
                    MessageBox.Show(LoginForm.Client.LastResponseMessage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 15000; // 15 seconds

            
            AntiTamper.Check(); // Run anti-tamper check

            if (string.IsNullOrWhiteSpace(chatchannel))
            {
                timer1.Stop();
                chatroomGrid.Rows.Clear();
                chatroomGrid.Rows.Insert(0, "AuthVaultix", "No channel selected", DateTime.Now); return;
            }

            try
            {
                var messages = await LoginForm.Client.ChatFetch(chatchannel);

                chatroomGrid.Rows.Clear();

                if (!string.IsNullOrEmpty(LoginForm.Client.LastResponseMessage) &&
                    LoginForm.Client.LastResponseMessage != "OK")
                {
                    Console.WriteLine(LoginForm.Client.LastResponseMessage, "Chat Error");
                    timer1.Stop();
                    return;
                }


                if (messages == null || messages.Count == 0)
                {
                    chatroomGrid.Rows.Insert(0, "AuthVaultix", "No chat messages", DateTime.Now); return;
                }

                foreach (var msg in messages)
                {
                    chatroomGrid.Rows.Insert(0, msg.author, msg.message, DateTimeOffset.FromUnixTimeSeconds(msg.timestamp).ToLocalTime().DateTime);
                }
            }
            catch (Exception ex)
            {
               
                timer1.Stop();  // if signature / session / network fail 
                Console.WriteLine("Chat error: " + ex.Message);
            }
        }

        private void btnVIP_Click(object sender, EventArgs e)
        {
            if (!LoginForm.Client.CheckFeaturePermission("VIP"))
            {
                MessageBox.Show("VIP locked! Upgrade required.");
                return;
            }  
            MessageBox.Show("VIP Activated!"); // actual ESP logic
        }

        private async void btnCustomAction_Click(object sender, EventArgs e)
        {
            try
            {
                btnCustomAction.Text = "Listening for Web Action...";
                btnCustomAction.Enabled = false;

                // Listen for "MyCustomAction" button trigger in background
                await Task.Run(() =>
                {
                    LoginForm.Client.Button("MyCustomAction");
                });

                MessageBox.Show("Web Button 'MyCustomAction' was clicked from the Website Dashboard!", "Web Action Triggered", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Web Button Error");
            }
            finally
            {
                btnCustomAction.Text = "Listen Web Custom Action";
                btnCustomAction.Enabled = true;
            }
        }
    }
}
