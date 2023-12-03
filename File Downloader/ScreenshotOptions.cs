using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;

namespace File_Downloader
{
    public partial class ScreenshotOptions : Form
    {
        DownloaderClient downloaderClient;

        public ScreenshotOptions(WebViewClient downloaderClient)
        {
            InitializeComponent();
            string platformUrl = downloaderClient.GetPlatformUrl();
            this.downloaderClient = downloaderClient;

            Uri outUri;
            optionsWebView.BringToFront();

            if (Uri.TryCreate(platformUrl, UriKind.Absolute, out outUri))
            {
                optionsWebView.Source = new Uri(platformUrl);
            }
            else
            {
                //optionsWebView.Source = new Uri("https://www.youtube.com/watch?v=dQw4w9WgXcQ&autoplay=1");
                Console.WriteLine("Video loaded?");
            }

            textBox1.Text = platformUrl;
        }

        private void ScreenshotOptions_Load(object sender, EventArgs e)
        {

        }

        private void optionsWebView_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Uri outUri;

            if (Uri.TryCreate(textBox1.Text, UriKind.Absolute, out outUri))
            {
                Console.WriteLine("Text Changed?");
                optionsWebView.Source = outUri;
            }
        }

        private void optionsWebView_Click_1(object sender, EventArgs e)
        {

        }
    }
}
