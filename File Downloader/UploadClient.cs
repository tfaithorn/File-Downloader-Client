using Microsoft.Office.Interop.Excel;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Excel = Microsoft.Office.Interop.Excel;

namespace File_Downloader
{
    public partial class UploadClient : Form
    {
        private TextConsole textConsole;
        private WebView2 webView;
        private string baseFolderPath;

        public UploadClient()
        {
            InitializeComponent();
            textConsole = new TextConsole(richTextBox1);
        }

        private void UploadClient_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDiablog = new OpenFileDialog();
            string excelFilePath;

            if (fileDiablog.ShowDialog() == DialogResult.OK)
            {
                excelFilePath = fileDiablog.FileName;

                //create a instance for the Excel object  
                Excel.Application oExcel = new Excel.Application();

                //pass path to the workbook object  
                Excel.Workbook WB = oExcel.Workbooks.Open(excelFilePath);

                //set worksheet to use later
                Excel._Worksheet xlWorksheet = WB.Sheets[1];

                int cellCount = xlWorksheet.UsedRange.Rows.Count;
                textConsole.WriteLine("Reading row: 0 /" + cellCount);

                for (int i = 0; i < cellCount - 1; i++)
                {
                    string url = ((Excel.Range)xlWorksheet.Cells[i + 2, 1]).Value.ToString();
                    string key = ((Excel.Range)xlWorksheet.Cells[i + 2, 1]).Value.ToString();
                    string filePath = ((Excel.Range)xlWorksheet.Cells[i + 2, 2]).Value.ToString();



                    textConsole.WriteLine("Reading row: " + (i + 1) + " / " + (cellCount - 1));
                }
            }
        }

        //private async Task<bool> UploadFileAsync(string url, string key, string filePath)
        //{
        //    List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
        //    var response = await client.PostAsync(uri, new FormUrlEncodedContent(parameters));


        //    return true;
        //}

        public async Task<bool> WaitForNavigation()
        {
            var navCompletionSource = new TaskCompletionSource<bool>();
            EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs> navCompleteHandler = null;

            navCompleteHandler = (s, e) =>
            {
                webView.CoreWebView2.NavigationCompleted -= navCompleteHandler;
                navCompletionSource.SetResult(true);
            };

            webView.CoreWebView2.NavigationCompleted += navCompleteHandler;

            return await navCompletionSource.Task;

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
