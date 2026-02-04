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
            appName: "Teamdeveloperxd",
            ownerId: "5d36476ca4",
            secret: "a7b1b6b29e179dd1435e1afe799c658080f853652714362c72239281e983932f",
            version: "1.0"
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
                var user = Client.Login(userFild.Text, pasFild.Text);

                MainForm main = new MainForm();
                main.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Status: " + ex.Message);
            }
        }

        private void RegisterBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var user = Client.Register(userFild.Text, pasFild.Text, keyFild.Text, emailFild.Text);

                MainForm main = new MainForm();
                main.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Status: " + ex.Message);
            }
        }

        private void LicenceBitn_Click(object sender, EventArgs e)
        {
            try
            {
                var user = Client.LicenseLogin(keyFild.Text);

                MainForm main = new MainForm();
                main.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Status: " + ex.Message);
            }
        }

        private void upgradeBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Client.Upgrade(userFild.Text, keyFild.Text);
                MessageBox.Show("Status: Upgraded successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Status: " + ex.Message);
            }
        }

        private void forgotBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Client.ForgotPassword(userFild.Text, emailFild.Text);

                MessageBox.Show("Reset email sent. Check your inbox.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Status: " + ex.Message);
            }
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
