using AuthVaultix;
using Loader;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AuthVaultix
{
    public partial class Login : Form
    {
  
        public static api AuthVaultixApp = new api(
                name: "loly",
                ownerid: "5d36476ca4",
                version: "1.0"
        );


        public Login()
        {
            InitializeComponent();
            Drag.MakeDraggable(this);
            AuthVaultixApp.init();
        }



        private async void Login_Load(object sender, EventArgs e)
        {
           
        }
        

        private async void loginBtn_Click_1(object sender, EventArgs e)
        {
            await AuthVaultixApp.login(usernameField.Text, passwordField.Text);
            if (AuthVaultixApp.response.success)
            {
                Main main = new Main();
                main.Show();
                this.Hide();
            }
            else
                MessageBox.Show("Status: " + AuthVaultixApp.response.message);
        }

        private async void registerBtn_Click(object sender, EventArgs e)
        {
            string email = this.emailField.Text;
            if (email == "Email (leave blank if none)")
            { // default value
                email = null;
            }

            await AuthVaultixApp.register(usernameField.Text, passwordField.Text, keyField.Text, email);
            if (AuthVaultixApp.response.success)
            {
                Main main = new Main();
                main.Show();
                this.Hide();
            }
            else
                MessageBox.Show("Status: " + AuthVaultixApp.response.message);
        }

        private async void licenseBtn_Click(object sender, EventArgs e)
        {
            await AuthVaultixApp.license(keyField.Text);
            if (AuthVaultixApp.response.success)
            {
                Main main = new Main();
                main.Show();
                this.Hide();
            }
            else
               MessageBox.Show("Status: " + AuthVaultixApp.response.message);
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void minBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private async void resetBtn_Click(object sender, EventArgs e)
        {
            await AuthVaultixApp.forgot(usernameField.Text, emailField.Text);
            MessageBox.Show("Status: " + AuthVaultixApp.response.message);
        }

        private async void UpgradeBtn_Click(object sender, EventArgs e)
        {
            await AuthVaultixApp.upgrade(usernameField.Text, keyField.Text); // success is set to false so people can't press upgrade then press login and skip logging in. it doesn't matter, since you shouldn't take any action on succesfull upgrade anyways. the only thing that needs to be done is the user needs to see the message from upgrade function
            MessageBox.Show("Status: " + AuthVaultixApp.response.message);
            // don't login, because they haven't authenticated. this is just to extend expiry of user with new key.
        }
    }
}
