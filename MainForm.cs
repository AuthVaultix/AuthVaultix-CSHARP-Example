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
            return DateTimeOffset.FromUnixTimeSeconds(long.Parse(unix)).DateTime;
        }
        public string GetTimeLeft()
        {
            var sub = LoginForm.Client.CurrentUser.subscriptions[0];
            long expiry = long.Parse(sub.expiry);

            DateTime expDate =DateTimeOffset.FromUnixTimeSeconds(expiry).DateTime;

            TimeSpan diff = expDate - DateTime.Now;

            return $"{diff.Days} Days {diff.Hours} Hours";
        }
        private void MainForm_Load(object sender, EventArgs e)
        {

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
                var onlineUsers = LoginForm.Client.FetchOnline();

                if (onlineUsers != null)
                {
                    onlineUsersField.Items.Clear();
                    foreach (var user in onlineUsers)
                    {
                        onlineUsersField.Items.Add(user.credential);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Status: " + ex.Message);
            }
        }

        private void sendLogDataBtn_Click(object sender, EventArgs e)
        {
            try
            {
                LoginForm.Client.Log(logDataField.Text);
                MessageBox.Show(LoginForm.Client.LastMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Log error: " + ex.Message);
            }
        }

        private void banBtn_Click(object sender, EventArgs e)
        {
            try
            {
                LoginForm.Client.Ban("Testing ban");
                MessageBox.Show(LoginForm.Client.LastMessage);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Status: " + ex.Message);
            }
        }

        private void checkSessionBtn_Click(object sender, EventArgs e)
        {
            try
            {
                LoginForm.Client.Check();
                MessageBox.Show(LoginForm.Client.LastMessage);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Status: " + ex.Message);
            }
        }

        private void CheackBlacklistBtn_Click(object sender, EventArgs e)
        {
            try
            {
                LoginForm.Client.CheckBlacklist();
                MessageBox.Show(LoginForm.Client.LastMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Status: " + ex.Message);
            }
        }

        private void fetchGlobalVariableBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string value = LoginForm.Client.GetGlobalVar(globalVariableField.Text);
                MessageBox.Show("Value: " + value);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void downloadFileBtn_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] fileBytes = LoginForm.Client.Download("");
                File.WriteAllBytes(filePathField.Text + "\\" + fileExtensionField.Text, fileBytes);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Download error: " + ex.Message);
            }
        }

        private void fetchUserVarBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string value = LoginForm.Client.GetVar(varField.Text);
                MessageBox.Show("Value: " + value);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Status: " + ex.Message);
            }
        }

        private void setUserVarBtn_Click(object sender, EventArgs e)
        {
            try
            {
                LoginForm.Client.SetVar(varField.Text, varDataField.Text);
                MessageBox.Show(LoginForm.Client.LastMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Status: " + ex.Message);
            }
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
                bool ok = await LoginForm.Client.ChatSend(chatMsgField.Text, chatchannel);

                if (ok)
                {
                    chatroomGrid.Rows.Insert(0,LoginForm.Client.CurrentUser.username,chatMsgField.Text,DateTime.Now.ToString());
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
                chatroomGrid.Rows.Insert(0,"AuthVaultix","No channel selected",DateTime.Now);return;
            }

            try
            {
                var messages = await LoginForm.Client.ChatFetch(chatchannel);

                chatroomGrid.Rows.Clear();

                if (messages == null || messages.Count == 0)
                {
                    chatroomGrid.Rows.Insert(0,"AuthVaultix","No chat messages",DateTime.Now);return;
                }

                foreach (var msg in messages)
                {
                    chatroomGrid.Rows.Insert(0,msg.author,msg.message,DateTimeOffset.FromUnixTimeSeconds(msg.timestamp).ToLocalTime().DateTime);
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
