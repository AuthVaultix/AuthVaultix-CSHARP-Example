using AuthVaultix;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class LoginForm : Form
    {

        public static AuthVaultixClient Client = new AuthVaultixClient(
            appName: "",
            ownerId: "",
            secret: "",
            version: ""
        );

        public LoginForm()
        {
            InitializeComponent();
            Drag.MakeDraggable(this);
            string initMsg;
            if (!Client.Init(out initMsg))
            {
                MessageBox.Show(initMsg, "Initialization Failed");
                this.Close(); 
                return;
            }
        }

        private void LoginBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string loginMsg;
                if (!Client.Login(userFild.Text, pasFild.Text, out loginMsg))
                {
                    MessageBox.Show(loginMsg, "Login Failed");
                    return;
                }
                // Login successful
                MainForm main = new MainForm();
                main.Show();
                this.Hide();
            }
            catch (Exception ex) { MessageBox.Show("" + ex.Message); }

        }

        private void RegisterBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string regMsg;
                if (!Client.Register(userFild.Text, pasFild.Text, keyFild.Text, out regMsg, emailFild.Text))
                {
                    MessageBox.Show(regMsg, "Registration Failed");
                    return;
                }
                // Register successful
                MainForm main = new MainForm();
                main.Show();
                this.Hide();
            }
            catch (Exception ex) { MessageBox.Show("" + ex.Message); }

        }

        private void LicenceBitn_Click(object sender, EventArgs e)
        {
            try
            {
                string licMsg;
                if (!Client.LicenseLogin(keyFild.Text, out licMsg))
                {
                    MessageBox.Show(licMsg, "License Login Failed");
                    return;
                }

                MainForm main = new MainForm();
                main.Show();
                this.Hide();
            }
            catch (Exception ex){MessageBox.Show("" + ex.Message);}
        }

        private void upgradeBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string upgradeMsg;
                if (!Client.Upgrade(userFild.Text, keyFild.Text, out upgradeMsg))
                {
                    MessageBox.Show(upgradeMsg, "Upgrade Failed");
                    return;
                }
                MessageBox.Show("Upgrade successful!", "Success");
            }
            catch (Exception ex){MessageBox.Show("" + ex.Message);}
        }

        private void forgotBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string msg;
                if (!Client.ForgotPassword(userFild.Text, keyFild.Text, out msg))
                {
                    MessageBox.Show(msg, "Forgot Password Failed");
                    return;
                }
                MessageBox.Show("Reset email sent successfully", "Success");
            }
            catch (Exception ex){MessageBox.Show("" + ex.Message);}
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void minBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
