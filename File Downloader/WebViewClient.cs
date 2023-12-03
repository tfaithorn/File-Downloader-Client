using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace File_Downloader
{
    public class WebViewClient : DownloaderClient
    {
        private WebView2 webView;
        private string platformUrl;
        private string currentCountryId = "0";
        private TextConsole textConsole;
        private string loginUrl;
        public string username;
        public string password;
        public Method formMethod;
        public bool overrideFiles;
        public bool stopOnError;
        public string folderDestinationPath;
        private TaskCompletionSource<bool> downloadFinishedEvent;
        private string currFullPath;
        public bool downloadAsImage;


        private const string emulationPath = "/emulation";
        //System.EventHandler<CoreWebView2DownloadStartingEventArgs> currEventHandler;

        public WebViewClient(WebView2 webView2)
        { 
            this.webView = webView2;
            this.webView.Source = new Uri("https://google.com");
            Task.Run(Initialize);
        }

        private async void SetUpWebView()
        {
            var op = new Microsoft.Web.WebView2.Core.CoreWebView2EnvironmentOptions("--disable-web-security");
            var env = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, null, op);
            await this.webView.EnsureCoreWebView2Async(env);
        }

        public void SetTextConsole(TextConsole textConsole)
        {
            this.textConsole = textConsole;
        }

        private async Task Initialize()
        {
            var op = new CoreWebView2EnvironmentOptions("--disable-web-security");
            var env = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, null, op);
            await webView.EnsureCoreWebView2Async(env);
        }

        private async Task<bool> Login()
        {
            try
            {
                webView.Source = new Uri(platformUrl);
                await WaitForNavigation();

                //wait for the login to occur
                await WaitForNavigation();

                return true;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                textConsole.WriteLine(e.Message);
                return false;
            }
        }

        public override async Task<bool> DownloadFileTaskAsync(Uri uri, ExcelRow row)
        {
            var reqPlatformUrl = ExtractPlatformUrl(uri.OriginalString);
            if (reqPlatformUrl != platformUrl)
            {
                this.platformUrl = reqPlatformUrl;
                await Login();
                currentCountryId = "0";
            }

            if (row.countryID != currentCountryId)
            {
                await EmulateCountry(row.countryID);
                currentCountryId = row.countryID;
            }

            if (CheckIfFileExists(row.path, row.filename))
            {
                return true;
            }

            try
            {
                //Note: extra slashes are used on later .net versions to allow file paths with over 255 characters (will require windows flag set)
                string fullPath = folderDestinationPath + row.path + "\\" + row.filename;
                string directory = Path.GetDirectoryName(fullPath);

                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                //terrible workaround to get access to fullPath in the callback
                System.EventHandler<CoreWebView2DownloadStartingEventArgs> currEventHandler = (s, e) =>
                {
                    e.Handled = true;
                    e.ResultFilePath = fullPath;
                    downloadFinishedEvent?.TrySetResult(true);
                };

                currFullPath = fullPath;
                webView.CoreWebView2.DownloadStarting += currEventHandler;

                if (formMethod == Method.POST)
                {
                    List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();

                    if (row.payload != null)
                    {
                        string[] splitPayload = row.payload.Split('&');

                        foreach (string parameter in splitPayload)
                        {
                            string[] keyValueSplit = parameter.Split('=');
                            string key = keyValueSplit[0];
                            string value = keyValueSplit[1];
                            parameters.Add(new KeyValuePair<string, string>(key, value));
                        }
                    }

                    var formContent = new FormUrlEncodedContent(parameters);
                    var formStream = formContent.ReadAsStreamAsync().Result;

                    var request = webView.CoreWebView2.Environment.CreateWebResourceRequest(uri.OriginalString, "POST", formStream, "Content-Type: application/x-www-form-urlencoded");
                    webView.CoreWebView2.NavigateWithWebResourceRequest(request);
                    var response = await WaitForNavigation();

                    if (!response)
                    {
                        return false;
                    }
                }

                if (formMethod == Method.GET)
                {
                    //NOTE: Still not working. Need to wait for request before starting new one of the file names get mixed up
                    await webView.ExecuteScriptAsync($@"fetch('{uri}').then(function(t) {{
                        return t.blob().then((b)=>{{
                            var a = document.createElement('a');
                            a.href = URL.createObjectURL(b);
                            a.setAttribute('download', 'defaultname');
                            a.click();
                        }}
                        );
                    }});console.log('javascript is executing?');
                    ");

                    downloadFinishedEvent = new TaskCompletionSource<bool>();
                    await downloadFinishedEvent.Task;

                    webView.CoreWebView2.DownloadStarting -= currEventHandler;
                }

                return true;
            }
            catch (Exception e)
            {
                textConsole.WriteLine(uri.OriginalString);
                textConsole.WriteLine("Error:" + e.Message);
                return false;
            }

        }

        protected override async Task<bool> EmulateCountry(string countryId)
        {
            textConsole.WriteLine("Changing Country:" + currentCountryId + " to " + countryId);
            var emulationUrl = platformUrl + emulationPath;

            await Task.Delay(1000);
            await webView.ExecuteScriptAsync(
                    $@"
                    console.log('[data-country-id=""{countryId}""]');
                    const countryLink = document.querySelector('[data-country-id=""{countryId}""]');
                    countryLink.click();");
            await WaitForNavigation();

            return true;
        }


        private void CoreWebView2_DownloadStarting(object sender, CoreWebView2DownloadStartingEventArgs e)
        {
            e.ResultFilePath = "C:\\Lorna";
            textConsole.WriteLine("Event Handler called?");
            e.Handled = true;
        }

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

        public async Task<bool> WaitForWebResourceRequested()
        {
            var tcs = new TaskCompletionSource<bool>();

            EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2WebResourceRequestedEventArgs> navHandler = null;

            navHandler = (sender, args) =>
            {
                webView.CoreWebView2.WebResourceRequested -= navHandler;
                args.Request.Headers.SetHeader("origin", platformUrl);
                tcs.SetResult(true);
            };

            webView.CoreWebView2.WebResourceRequested += navHandler;

            return await tcs.Task;
        }

        public async Task<bool> GenerateScreenshot(Uri uri, ExcelRow row)
        {

            try
            {
                string fullPath = destinationPath + row.path + "\\" + row.filename;
                string directory = Path.GetDirectoryName(fullPath);

                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                webView.Width = 1200;
                webView.Height = 1500;

                await webView.ExecuteScriptAsync($@"window.location = ""{uri.OriginalString}""");
                await WaitForNavigation();

                dynamic clip = new JObject();
                clip.x = 0;
                clip.y = 0;
                clip.width = 1200;
                clip.height = 1500;
                clip.scale = 1;

                dynamic settings = new JObject();
                settings.format = "png";
                settings.clip = clip;
                settings.fromSurface = true;
                settings.captureBeyondViewport = true;

                var p = settings.ToString(Newtonsoft.Json.Formatting.None);
                var devData = await webView.CoreWebView2.CallDevToolsProtocolMethodAsync("Page.captureScreenshot", p);
                var imgData = (string)((dynamic)JObject.Parse(devData)).data;
                var ms = new MemoryStream(Convert.FromBase64String(imgData));
                var bitmap = (Bitmap)System.Drawing.Image.FromStream(ms);
                bitmap.Save(fullPath);

                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                textConsole.WriteLine("Error:" + e.Message);
                return false;
            }
        }
    }
}
