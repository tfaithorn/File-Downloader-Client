using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace File_Downloader
{
    public partial class LoginForm : Form
    {
        private TextConsole textConsole;
        private Label loginLabel;
        private Label usernameLabel;
        private DownloaderClient downloaderClient;

        public LoginForm(DownloaderClient downloaderClient)
        {
            InitializeComponent();
            this.Text = "Login";
            this.downloaderClient = downloaderClient;
        }

        public void SetTextConsole(TextConsole textConsoleObj)
        {
            this.textConsole = textConsoleObj;
        }

        public void SetLoginLabel(Label label)
        {
            loginLabel = label;    
        }

        public void SetUsernameLabel(Label label)
        {
            usernameLabel = label;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;
            downloaderClient.SetLoginCredentials(username, password);
            usernameLabel.Text = username;

            var form1 = new Form1();
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
