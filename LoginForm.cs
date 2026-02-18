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
            Client.Init();
        }

        private void LoginBtn_Click(object sender, EventArgs e)
        {

            try
            {
                if (!Client.Login(userFild.Text, pasFild.Text))
                {
                    MessageBox.Show(Client.RisponceCollection, "Login Failed");
                    return;
                }

                // Login successful
                MainForm main = new MainForm();
                main.Show();
                this.Hide();
            }
            catch (Exception ex){MessageBox.Show(ex.Message, "Error");}
        }

        private void RegisterBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Client.Register(userFild.Text, pasFild.Text, keyFild.Text, emailFild.Text))
                {
                    MessageBox.Show(Client.RisponceCollection, "Registration Failed");
                    return;

                }
                // Register successful
                MainForm main = new MainForm();
                main.Show();
                this.Hide();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
        }

        private void LicenceBitn_Click(object sender, EventArgs e)
        {
            try
            {

                if (!Client.LicenseLogin(keyFild.Text))
                {
                    MessageBox.Show(Client.RisponceCollection, "License Login Failed");
                    return;
                }

                MainForm main = new MainForm();
                main.Show();
                this.Hide();
            }
            catch (Exception ex) { MessageBox.Show("" + ex.Message); }
        }

        private void upgradeBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Client.Upgrade(userFild.Text, keyFild.Text))
                {
                    MessageBox.Show(Client.RisponceCollection, "Upgrade Failed");
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
                if (!Client.ForgotPassword(userFild.Text, keyFild.Text))
                {
                    MessageBox.Show(Client.RisponceCollection, "Forgot Password Failed");
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
