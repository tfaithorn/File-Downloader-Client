//using System.Drawing;
//using System.Drawing.Imaging;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace File_Downloader
{
    public class DownloaderClient
    {
        private const string loginPath = "/admin/login-next.php";
        private const string emulationPath = "/emulation";

        public readonly char[] invalidFileCharacters;
        public readonly char[] invalidFolderCharacters;
        private HttpClient client;
        private HttpClientHandler handler;
        private WebView2 webView2;
        private string platformUrl;
        private string destinationPath;
        private TextConsole textConsole;
        private string currentCountryID;
        public bool overrideFiles;
        public Method formMethod;
        public bool stopOnError;
        public bool downloadAsImage;
        private string loginUrl;

        private string username;
        private string password;

        public enum Method
        {
            GET,
            POST
        }
        public DownloaderClient()
        {
            this.handler = new HttpClientHandler();
            //this.webView2 = webView2;
            this.platformUrl = "";
            this.destinationPath = "";
            this.currentCountryID = "0";
            this.formMethod = Method.GET;
            this.overrideFiles = false;
            this.stopOnError = false;
            this.downloadAsImage = false;

            //Set handler for HttpClient on initialization
            this.client = new HttpClient(handler);

            //Add header to prevent platform from rejecting request
            this.client.DefaultRequestHeaders.Add("User-Agent", "Custom");

            //10 minutes (to match platform timer)
            this.client.Timeout = new System.TimeSpan(0,10,0);

            //Note: To prevent country emulation blocking when it returns 302
            this.handler.AllowAutoRedirect = false;

            var cookieContainer = new CookieContainer();
            this.handler.CookieContainer = cookieContainer;

            this.invalidFileCharacters = new char[] { ':', '/', '\\', '*', '?', '>', '<', '|' };
            this.invalidFolderCharacters = new char[] { '#', '%', '&', '{', '}', '\\', '/', '<', '>', '*', '?', '$', '!', '\'', '"', ':', '@', '|', '`', '\t', '.' };

        }

        private async void SetUpWebView()
        {
            var op = new Microsoft.Web.WebView2.Core.CoreWebView2EnvironmentOptions("--disable-web-security");
            var env = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, null, op);
            await this.webView2.EnsureCoreWebView2Async(env);
        }

        public void SelectCountry(string countryID)
        {

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("countryId", countryID)
            });

            var result = client.PostAsync(new Uri(platformUrl + emulationPath), formContent);

            currentCountryID = countryID;

            textConsole.WriteLine("Current Country" + currentCountryID);
            return;

        }

        public bool SetPlatformUrl(string url)
        {
            platformUrl = url;
            return true;
        }

        public bool SetDestinationPath(string path)
        {
            destinationPath = path;
            textConsole.WriteLine("Destination Path:" + destinationPath);
            return true;
        }

        public string GetPlatformUrl()
        {
            return platformUrl;
        }

        public string SanitizeFileName(string filename)
        {
            if (filename == null || filename == "")
            {
                return "";
            }

            filename = filename.Trim();

            foreach (char c in invalidFileCharacters)
            {
                filename = filename.Replace(c.ToString(), "");
            }

            return filename;
        }

        public string SanitizeFolderPath(string path)
        {
            if (path == null || path == "")
            {
                return "";
            }

            //remove slash at the start of the path
            if (path[0] == '\\')
            {
                path.Remove(0, 1);
            }

            var strArray = path.Split('\\');
            var newPath = "";

            for (int i = 0; i < strArray.Length; i++)
            {
                string sanitizedName = SanitizeFolderName(strArray[i]);
                newPath += "\\" + sanitizedName;
            }

            return newPath;
        }

        public string SanitizeFolderName(string foldername)
        {
            if (foldername == null || foldername == "")
            {
                return "";
            }

            foldername = foldername.Trim();

            foreach (char c in invalidFolderCharacters)
            {
                foldername = foldername.Replace(c.ToString(), "");
            }

            return foldername;
        }

        public bool SetTextConsole(TextConsole textConsoleObj)
        {
            textConsole = textConsoleObj;
            return true;
        }

        public void SetLoginCredentials(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public async Task<bool> SetCookies()
        {
            loginUrl = platformUrl + loginPath;
            Uri loginUri = new Uri(loginUrl);
            var baseAddress = new Uri(platformUrl);

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("submitFrm", "true")
            });

            return await LoginHttpClient(formContent);
        }

        private async Task CheckPlatformUrl(string url)
        {
            var uri = new Uri(url);
            var newUrl = uri.Scheme + "://" + uri.Host;
            if (platformUrl != newUrl)
            {
                platformUrl = newUrl;
                currentCountryID = "0";
                textConsole.WriteLine("New platform URL: " + platformUrl);
                await SetCookies();
            }
        }

        private async Task<bool> LoginHttpClient(FormUrlEncodedContent formContent)
        {
            try
            {

                handler.CookieContainer.Add(new Uri(platformUrl), new Cookie("CookieName", "cookie_value"));

                await client.PostAsync(new Uri(platformUrl + loginPath), formContent);
                var cookeJar = handler.CookieContainer.GetCookies(new Uri(platformUrl));

                foreach (Cookie cookie in cookeJar)
                {
                    //check if staySignedIn cookie exists (can't see a better way to determine if you are logged in)
                    if (cookie.Name == "staysignedin")
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return false;
            }
        }

        private async Task<bool> LoginWebView(FormUrlEncodedContent formContent)
        {
            try
            {
                webView2.Source = new Uri(platformUrl);
                var formStream = formContent.ReadAsStreamAsync().Result;
                var request = webView2.CoreWebView2.Environment.CreateWebResourceRequest(loginUrl, "POST", formStream, "Content-Type: application/x-www-form-urlencoded");
                webView2.CoreWebView2.NavigateWithWebResourceRequest(request);
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

        public void CreateRelativePath(string path)
        {

            //if path is empty return
            if (path == "" || path == null || path == "\\")
            {
                return;
            }

            //if a forward slash has been added to the start of the string by mistake remove it
            if (path[0] == '\\')
            {
                path.Remove(0, 1);
            }

            //delimit string using slashes

            var strArray = path.Split('\\');
            string newPath = "";

            //check if a folder exists. If it doesn't create it
            for (int i = 0; i < strArray.Length; i++)
            {

                if (!Directory.Exists((destinationPath + newPath)))
                {
                    textConsole.WriteLine("creating directory: " + newPath);
                    try { Directory.CreateDirectory((destinationPath + newPath)); }
                    catch (ArgumentException e)
                    {
                        textConsole.WriteLine(e.Message);
                        return;
                    }
                }
            }

            return;
        }

        public async Task<bool> DownloadFileTaskAsync(Uri uri, ExcelRow row)
        {
            await CheckPlatformUrl(uri.OriginalString);

            if (CheckIfFileExists(row.path, row.filename))
            {
                return true;
            }

            if (row.countryID != currentCountryID)
            {
                await EmulateCountryFile(row.countryID);
            }

            return await GenerateFile(uri, row);
        }

        public async Task<bool> DownloadImageTaskAsync(Uri uri, ExcelRow row)
        {
            await CheckPlatformUrl(uri.OriginalString);
            if (CheckIfFileExists(row.path, row.filename))
            {
                return true;
            }

            if (row.countryID != currentCountryID)
            {
                await EmulateCountryImage(row.countryID);
            }
            return await GenerateScreenshot(uri, row);
        }

        private bool CheckIfFileExists(string path, string filename)
        {
            if (!overrideFiles && File.Exists(destinationPath + "\\" + path + "\\" + filename))
            {
                textConsole.WriteLine("Last file Skipped as the file already exists");
                return true;
            }

            return false;
        }

        public async Task<bool> EmulateCountryFile(string countryID)
        {
            textConsole.WriteLine("Changing Country:" + currentCountryID + " to " + countryID);

            Uri countryUri = new Uri(platformUrl + emulationPath);

            var formContent = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("countryId", countryID.ToString())
            });

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, countryUri);

            request.Content = formContent;

            try
            {
                var response = await client.SendAsync(request);
                currentCountryID = countryID;
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return false;
            }
        }

        private async Task<bool> EmulateCountryImage(string countryID)
        {
            try
            {
                string emulationUrl = platformUrl + emulationPath;

                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("countryId", countryID)
                });

                //Note: A Post require is much easier with plain javascript than with webview2's method "NavigateWithWebResourceRequest" which is a little buggy.
                //Also, there are issues with it not setting origin & referrer headers which was leading to CORS errors.
                string javascript = @"fetch(""{emulationUrl}"", {
                                      ""headers"": {
                                        ""accept"": ""text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"",
                                        ""accept-language"": ""en-US,en;q=0.9"",
                                        ""cache-control"": ""max-age=0"",
                                        ""content-type"": ""application/x-www-form-urlencoded"",
                                        ""sec-ch-ua"": ""\"" Not;A Brand\"";v=\""99\"", \""Microsoft Edge\"";v=\""103\"", \""Chromium\"";v=\""103\"", \""Microsoft Edge WebView2\"";v=\""103\"""",
                                        ""sec-ch-ua-mobile"": ""?0"",
                                        ""sec-ch-ua-platform"": ""\""Windows\"""",
                                        ""sec-fetch-dest"": ""document"",
                                        ""sec-fetch-mode"": ""navigate"",
                                        ""sec-fetch-site"": ""same-origin"",
                                        ""sec-fetch-user"": ""?1"",
                                        ""upgrade-insecure-requests"": ""1""
                                      },
                                      ""referrer"": ""{referrerUrl}/dashboard/"",
                                      ""referrerPolicy"": ""strict-origin-when-cross-origin"",
                                      ""body"": ""countryId=1"",
                                      ""method"": ""POST"",
                                      ""mode"": ""cors"",
                                      ""credentials"": ""include""
                                    });";

                javascript = javascript.Replace("{emulationUrl}", emulationUrl).Replace("{referrerUrl}", platformUrl);

                await webView2.ExecuteScriptAsync(javascript);
                //buffer to wait for form to be processed
                await Task.Delay(1000);
                currentCountryID = countryID;
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

        public async Task<bool> WaitForNavigation()
        {
            var navCompletionSource = new TaskCompletionSource<bool>();
            EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs> navCompleteHandler = null;

            navCompleteHandler = (s, e) =>
            {
                webView2.CoreWebView2.NavigationCompleted -= navCompleteHandler;
                navCompletionSource.SetResult(true);
            };

            webView2.CoreWebView2.NavigationCompleted += navCompleteHandler;

            return await navCompletionSource.Task;
        }


        public async Task<bool> WaitForWebResourceRequested()
        {
            var tcs = new TaskCompletionSource<bool>();

            EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2WebResourceRequestedEventArgs> navHandler = null;

            navHandler = (sender, args) =>
            {
                webView2.CoreWebView2.WebResourceRequested -= navHandler;
                args.Request.Headers.SetHeader("origin", platformUrl);
                tcs.SetResult(true);
            };

            webView2.CoreWebView2.WebResourceRequested += navHandler;

            return await tcs.Task;
        }

        public async Task<bool> GenerateFile(Uri uri, ExcelRow row)
        {
            try
            {
                //Note: extra slashes are used on later .net versions to allow file paths with over 255 characters (will require windows flag set)
                string fullPath = "\\\\?\\" + destinationPath + row.path + "\\" + row.filename;
                string directory = Path.GetDirectoryName(fullPath);

                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

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

                    var response = await client.PostAsync(uri, new FormUrlEncodedContent(parameters));

                    if (!response.IsSuccessStatusCode)
                    {
                        textConsole.WriteLine("Error: " + uri);
                        return false;
                    }
                    var stream = await response.Content.ReadAsStreamAsync();
                    FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.ReadWrite);
                    await stream.CopyToAsync(fs);
                }

                if (formMethod == Method.GET)
                {
                    using (var s = await client.GetStreamAsync(uri))
                    {
                        FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.ReadWrite);
                        await s.CopyToAsync(fs);
                        fs.Close();
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                textConsole.WriteLine("Error:" + e.Message);
                textConsole.WriteLine("Was Try Catch Error?");
                return false;
            }
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

                webView2.Width = 1200;
                webView2.Height = 1500;

                await webView2.ExecuteScriptAsync($@"window.location = ""{uri.OriginalString}""");
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
                var devData = await webView2.CoreWebView2.CallDevToolsProtocolMethodAsync("Page.captureScreenshot", p);
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
