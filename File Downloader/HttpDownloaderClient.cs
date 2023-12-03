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
    public class HttpDownloaderClient : DownloaderClient
    {
        public readonly char[] invalidFileCharacters;
        public readonly char[] invalidFolderCharacters;
        private HttpClient client;
        private HttpClientHandler handler;

        public HttpDownloaderClient()
        {
            this.handler = new HttpClientHandler();
            this.platformUrl = "";
            this.destinationPath = "";
            this.currentCountryID = "0";
            this.formMethod = Method.GET;
            this.overrideFiles = false;
            this.stopOnError = false;

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

        private bool CheckNewPlatformUrl(string url)
        {
            var uri = new Uri(url);
            var newUrl = uri.Scheme + "://" + uri.Host;
            if (platformUrl != newUrl)
            {
                platformUrl = newUrl;
                currentCountryID = "0";
                textConsole.WriteLine("New platform URL: " + platformUrl);
                return true;
            }
            return false;
        }

        private string GenerateLoginFormData()
        {
            var graphql = @"{
                                  ""variables"": {
                                    ""input"": {
                                      ""username"": ""{username}"",
                                      ""password"": ""{password}"",
                                      ""staySignedIn"": false,
                                      ""recaptchaToken"": null
                                    }
                                  },
                                  ""query"": ""mutation ($input: LoginInput!) {\n  login(input: $input)\n}\n""
                                }";

            graphql = graphql.Replace("{username}", username);
            graphql = graphql.Replace("{password}", password);

            return graphql;
        }

        private async Task<bool> LoginHttpClient()
        {
            try
            {
                //handler.CookieContainer.Add(new Uri(platformUrl), new Cookie("CookieName", "cookie_value"));
                var formData = GenerateLoginFormData();
                var postBody = new StringContent(formData, Encoding.UTF8, "application/json");
                textConsole.WriteLine(formData);

                var res = await client.PostAsync(new Uri(platformUrl + loginPath), postBody);

                var cookeJar = handler.CookieContainer.GetCookies(new Uri(platformUrl));

                foreach (Cookie cookie in cookeJar)
                {
                    textConsole.WriteLine("Key:" + cookie.Name +" Value:" + cookie.Value);

                    //check if staySignedIn cookie exists (can't see a better way to determine if you are logged in)
                    //if (cookie.Name == "staysignedin")
                    if (cookie.Name == "SESSIONID")
                    {
                        textConsole.WriteLine("SESSIONID:" + cookie.Value);

                        return true;
                    }
                }
                textConsole.WriteLine("Unable to log in");
                return false;
            }
            catch (HttpRequestException e)
            {
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

        public override async Task<bool> DownloadFileTaskAsync(Uri uri, ExcelRow row)
        {
            if (CheckNewPlatformUrl(uri.OriginalString))
            {
                await LoginHttpClient();
            }

            if (CheckIfFileExists(row.path, row.filename))
            {
                return true;
            }

            if (row.countryID != currentCountryID)
            {
                await EmulateCountry(row.countryID);
            }

            return await GenerateFile(uri, row);
        }

        protected override async Task<bool> EmulateCountry(string countryId)
        {
            textConsole.WriteLine("Changing Country:" + currentCountryID + " to " + countryId);

            Uri countryUri = new Uri(platformUrl + emulationPath);

            var formContent = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("countryId", countryId.ToString())
            });

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, countryUri);

            request.Content = formContent;

            try
            {
                var response = await client.SendAsync(request);
                currentCountryID = countryId;
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return false;
            }
        }

        public async Task<bool> GenerateFile(Uri uri, ExcelRow row)
        {
            try
            {
                //Note: extra slashes are used in later .net versions to allow file paths over 255 characters (but this will require settings a windows flag)
                string fullPath = destinationPath + row.path + "\\" + row.filename;
                string directory = Path.GetDirectoryName(fullPath);

                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (formMethod == Method.POST)
                {
                    textConsole.WriteLine("is posting?");
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

                    textConsole.WriteLine(response.ReasonPhrase);
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
                return false;
            }
        }
    }
}
