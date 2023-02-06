using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using WK.Libraries.BetterFolderBrowserNS;


namespace File_Downloader
{
    public partial class Form1 : Form
    {
        private TextConsole textConsole;
        private DownloaderClient downloaderClient;
        private FileVerifier fileVerifier;
        private bool cancelDownload = false;
        private List<ExcelRow> excelResults = new List<ExcelRow>();

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            downloaderClient = new DownloaderClient();
            textConsole = new TextConsole(richTextBox1);
            fileVerifier = new FileVerifier(textConsole);
            downloaderClient.SetTextConsole(textConsole);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var loginForm = new LoginForm(this.downloaderClient);
            loginForm.SetTextConsole(textConsole);
            loginForm.SetLoginLabel(label2);
            loginForm.SetUsernameLabel(label6);
            loginForm.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            var betterFolderBrowser = new BetterFolderBrowser();
            betterFolderBrowser.Multiselect = false;

            if (betterFolderBrowser.ShowDialog() == DialogResult.OK)
            {
                string path = betterFolderBrowser.SelectedPath;
                label3.Text = betterFolderBrowser.SelectedPath;
                downloaderClient.SetDestinationPath(betterFolderBrowser.SelectedPath);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDiablog = new OpenFileDialog();
            string excelFilePath;

            if (fileDiablog.ShowDialog() == DialogResult.OK)
            {
                textConsole.WriteLine("Loading Spreadsheet...");
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
                    bool rowContainsError = false;

                    //read workbook into memory
                    string itemID = ((Excel.Range)xlWorksheet.Cells[i + 2, 1]).Value.ToString();
                    string filename = ((Excel.Range)xlWorksheet.Cells[i + 2, 2]).Value.ToString();
                    string relativeDestinationPath = Convert.ToString(((Excel.Range)xlWorksheet.Cells[i + 2, 3]).Value);
                    string countryID = ((Excel.Range)xlWorksheet.Cells[i + 2, 4]).Value.ToString();
                    string payload = (Excel.Range)xlWorksheet.Cells[i + 2, 5] != null ? Convert.ToString(((Excel.Range)xlWorksheet.Cells[i + 2, 5]).Value) : null;

                    relativeDestinationPath = downloaderClient.SanitizeFolderPath(relativeDestinationPath);
                    filename = downloaderClient.SanitizeFileName(filename);

                    //check if the filename contains illegal characters
                    foreach (char invalidCharacter in downloaderClient.invalidFileCharacters)
                    {
                        if (filename.IndexOf(invalidCharacter,0) != -1)
                        {
                            string errorText = "Warning: '" + filename + "' contains an invalid character '" + invalidCharacter + "', which will be removed.";
                            if (!rowContainsError)
                            {
                                rowContainsError = true;
                                textConsole.UpdateLine(errorText);
                            }
                            else {
                                textConsole.WriteLine(errorText);
                            }
                        }
                    }

                    //check if multiple files with the same name have the same path
                    foreach (ExcelRow row in excelResults)
                    {
                        if (row.filename == filename && row.path == relativeDestinationPath)
                        {
                            string errorText = "Warning: a file called '" + filename + "' will already be copied to the path '" + relativeDestinationPath;
                            if (!rowContainsError)
                            {
                                textConsole.UpdateLine(errorText);
                            }
                            else {
                                textConsole.WriteLine(errorText);
                            }

                            rowContainsError = true;
                        }
                    }

                    //if row contains errors write a new line instead of updating existing
                    if (rowContainsError)
                    {
                        textConsole.WriteLine("Reading row: " + (i + 1) + " / " + (cellCount - 1));
                    }
                    else {
                        textConsole.UpdateLine("Reading row: " + (i + 1) + " / " + (cellCount - 1));
                    }

                    excelResults.Add(new ExcelRow(itemID, filename, relativeDestinationPath, countryID, payload));

                }

                WB.Close();
                textConsole.WriteLine("Worksheet loaded");

                label5.Text = excelFilePath;
            }
        }

        private async void submit_click(object sender, EventArgs e)
        {
            try
            {
                cancelDownload = false;
                downloaderClient.stopOnError = stopOnErrorBox.Checked;
                downloaderClient.overrideFiles = redownloadFiles.Checked;
                //downloaderClient.downloadAsImage = downloadAsImageCheckBox.Checked;

                if (comboBox1.Text == "GET")
                {
                    downloaderClient.formMethod = DownloaderClient.Method.GET;
                }
                else
                {
                    downloaderClient.formMethod = DownloaderClient.Method.POST;
                }

                textConsole.WriteLine("Starting Download...");

                if (downloaderClient.downloadAsImage)
                {
                    await DownloadAsImage();
                }
                else
                {
                    await DownloadAsFiles();
                }
             
                textConsole.WriteLine("Download Complete");

                cancelDownload = false;
                return;
            }
            catch (Exception exception) {
                textConsole.WriteLine("Error:" + exception.Message);
            }
        }

        private async Task DownloadAsFiles()
        {
            for (int i = 0; i < excelResults.Count; i++)
            {
                if (cancelDownload)
                {
                    cancelDownload = false;
                    return;
                }

                //create download url for file
                Uri uri = new Uri(excelResults[i].itemID);

                if (excelResults[i].path == null)
                {
                    cancelDownload = false;
                    return;
                }

                textConsole.WriteLine((i + 1) + " / " + excelResults.Count + " - " + uri.OriginalString);

                var downloadStatus = await downloaderClient.DownloadFileTaskAsync(uri, excelResults[i]);

                if (downloadStatus == false && downloaderClient.stopOnError)
                {
                    cancelDownload = true;
                }
            }
        }

        private async Task  DownloadAsImage()
        {
            for (int i = 0; i < excelResults.Count; i++)
            {
                if (cancelDownload)
                {
                    cancelDownload = false;
                    return;
                }

                //create download url for file
                Uri uri = new Uri(excelResults[i].itemID);

                if (excelResults[i].path == null)
                {
                    cancelDownload = false;
                    return;
                }

                textConsole.WriteLine((i + 1) + " / " + excelResults.Count + " - " + uri.OriginalString);

                var downloadStatus = await downloaderClient.DownloadImageTaskAsync(uri, excelResults[i]);

                if (downloadStatus == false && downloaderClient.stopOnError)
                {
                    cancelDownload = true;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }


        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
          
        }

        private void label1_Click_1(object sender, EventArgs e)
        { 
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            var betterFolderBrowser = new BetterFolderBrowser();
            betterFolderBrowser.Multiselect = false;

            if (betterFolderBrowser.ShowDialog() == DialogResult.OK)
            {
                fileVerifier.CheckFolders(betterFolderBrowser.SelectedPath);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var betterFolderBrowser = new BetterFolderBrowser();
            betterFolderBrowser.Multiselect = false;

            if (betterFolderBrowser.ShowDialog() == DialogResult.OK)
            {
                fileVerifier.ListFiles(betterFolderBrowser.SelectedPath);
            }
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        //private void downloadAsImageCheckBox_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (downloadAsImageCheckBox.Checked)
        //    {
        //        screenshotOptions.Enabled = true;
        //    }
        //    else
        //    {
        //        screenshotOptions.Enabled = false;
        //    }
        //}

        private void screenshotOptions_Click(object sender, EventArgs e)
        {
            var screenshotOptions = new ScreenshotOptions(this.downloaderClient);
            screenshotOptions.Show();
        }
    }
}
