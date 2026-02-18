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
        }

        public static DateTime UnixToDate(string unix)
        {
            if (long.TryParse(unix, out long unixTime))
            {
                return DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        public string GetTimeLeft()
        {
            var sub = LoginForm.Client.CurrentUser.subscriptions[0];
            long expiry = long.Parse(sub.expiry);

            DateTime expDate = DateTimeOffset.FromUnixTimeSeconds(expiry).DateTime;

            TimeSpan diff = expDate - DateTime.Now;

            return $"{diff.Days} Days {diff.Hours} Hours";
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Fetches the user's "level" variable from the server. This is used to determine user access rights (e.g., VIP status). The server stores this variable per account.Based on its value, we can enable or restrict certain features.
            string level = LoginForm.Client.GetVar("level");

            if (level == "vip")
            {
                MessageBox.Show("⚠ Your update support has expired.\n\nPlease renew your subscription to continue receiving updates and support.","Support Expired",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            else
            {
                //MessageBox.Show("🔒 Not a VIP user.");
            }

            userDataField.Items.Add($"Username: {LoginForm.Client.CurrentUser.username}");
            userDataField.Items.Add($"License: {LoginForm.Client.CurrentUser.subscriptions[0].key}");
            userDataField.Items.Add($"Expires: {LoginForm.Client.CurrentUser.subscriptions[0].expiry}");
            userDataField.Items.Add($"Subscription: {LoginForm.Client.CurrentUser.subscriptions[0].subscription}");
            userDataField.Items.Add($"IP: {LoginForm.Client.CurrentUser.ip}");
            userDataField.Items.Add($"HWID: {LoginForm.Client.CurrentUser.hwid}");
            userDataField.Items.Add($"Creation Date: {UnixToDate(LoginForm.Client.CurrentUser.createdate)}");
            userDataField.Items.Add($"Last Login: {UnixToDate(LoginForm.Client.CurrentUser.lastlogin)}");
            userDataField.Items.Add($"Time Left: {GetTimeLeft()}");
            try
            {
                List<OnlineUser> onlineUsers;
                string msg;

                if (!LoginForm.Client.FetchOnline(out onlineUsers, out msg))
                {
                    return;
                }
                onlineUsersField.Items.Clear();
                foreach (var user in onlineUsers)
                onlineUsersField.Items.Add(user.credential);
            }
            catch (Exception ex){MessageBox.Show("Status: " + ex.Message);}
        }

        private void sendLogDataBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string logMsg;
                if (!LoginForm.Client.Log(logDataField.Text, out logMsg))
                {
                    MessageBox.Show("Log failed: " + logMsg);
                }
                else { MessageBox.Show("Log failed: " + logMsg); }
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
                Application.Exit(); // better than Environment.Exit(0) for WinForms
                return;
            }
            else{MessageBox.Show(msg, "Ban Failed");}
        }

        private void checkSessionBtn_Click(object sender, EventArgs e)
        {
            if (LoginForm.Client.Check())
            {MessageBox.Show(LoginForm.Client.RisponceCollection);return;}
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
            byte[] bytes; string msg;
            if (!LoginForm.Client.Download("", out bytes, out msg))
            {
                File.WriteAllBytes(filePathField.Text + "\\" + fileExtensionField.Text, bytes);
                MessageBox.Show(msg, "Download Failed");
                return;
            }
            else
            {
                File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\test.png", bytes);
                MessageBox.Show("Downloaded " + bytes.Length + " bytes", "Success");
            }
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
                bool ok =  LoginForm.Client.ChatSend(chatMsgField.Text, chatchannel, out msg);

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
                // if signature / session / network fail 
                timer1.Stop();
                MessageBox.Show("Chat error: " + ex.Message);
            }
        }
    }
}
