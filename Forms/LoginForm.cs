using AuthVaultix;
using System;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Client
{
    public partial class LoginForm : Form
    {


        public static AuthVaultixClient Client = new AuthVaultixClient(
            appName: "",
            secret: "",
            version: "1.0",
         //"Your_Path_Here" // token path here
        );

        public LoginForm()
        {
            InitializeComponent();
            Drag.MakeDraggable(this);
            if (!Client.Init())
            {
                MessageBox.Show(Client.RisponceCollection);
                return;
            }
        }

        private async void webLoginBtn_Click(object sender, EventArgs e)
        {
            try
            {
                webLoginBtn.Text = "Waiting for handshake...";
                webLoginBtn.Enabled = false;

                if (!await Client.WebLogin())
                {
                    MessageBox.Show(Client.RisponceCollection, "Login Failed");
                    webLoginBtn.Text = "Web Log In";
                    webLoginBtn.Enabled = true;
                    return;
                }

                // Login successful
                MainForm main = new MainForm();
                main.Show();
                this.Hide();
            }
            catch (Exception ex) 
            { 
                MessageBox.Show(ex.Message, "Error"); 
                webLoginBtn.Text = "Web Log In";
                webLoginBtn.Enabled = true;
            }
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
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
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
            catch (Exception ex) { MessageBox.Show("" + ex.Message); }
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
            catch (Exception ex) { MessageBox.Show("" + ex.Message); }
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
