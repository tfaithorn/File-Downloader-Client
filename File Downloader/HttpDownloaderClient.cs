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
        private HttpClient client;
        private HttpClientHandler handler;

        public HttpDownloaderClient()
        {
            this.handler = new HttpClientHandler();
            this.destinationPath = "";
            this.currentCountryID = "0";
            this.formMethod = Method.GET;
            this.overrideFiles = false;
            this.stopOnError = false;

            //Set handler for HttpClient on initialization
            this.client = new HttpClient(handler);

            //Add header to prevent platform from rejecting request
            this.client.DefaultRequestHeaders.Add("User-Agent", "custom");

            //10 minute timout to match platform timer
            this.client.Timeout = new System.TimeSpan(0,10,0);

            //flag needed to prevent country emulation preventing downloads (returns 302)
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

        public override async Task<bool> Login(string requestUrl)
        {
            this.platformUrl = ExtractPlatformUrl(requestUrl);

            try
            {
                var formData = @"{
                                  ""variables"": {
                                    ""input"": {
                                      ""username"": ""{username}"",
                                      ""password"": ""{password}"",
                                      ""staySignedIn"": false,
                                      ""recaptchaToken"": null
                                    }
                                  },
                                  ""query"": ""mutation ($input: LoginInput!) {\n  login(input: $input)\n}\n""
                                }"
                        .Replace("{username}", username)
                        .Replace("{password}", password);

                var postBody = new StringContent(formData, Encoding.UTF8, "application/json");
                var res = await client.PostAsync(new Uri(platformUrl + loginPath), postBody);
                var cookeJar = handler.CookieContainer.GetCookies(new Uri(platformUrl));

                foreach (Cookie cookie in cookeJar)
                {
                    if (cookie.Name == "SESSIONID")
                    {
                        textConsole.WriteLine("Logged into platform");
                        
                        //Submitting the form to go to World level seems to be required, but I have no explanation for why.
                        await EmulateCountry("0");
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
            if (CheckIfFileExists(row.path, row.filename))
            {
                return true;
            }

            if (row.countryID != currentCountryID)
            {
                await EmulateCountry(row.countryID);
            }

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

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);

                    request.Content = new FormUrlEncodedContent(parameters);

                    var response = await client.SendAsync(request);

                    if (!response.IsSuccessStatusCode) 
                    {
                        textConsole.WriteLine("Error: " + uri);
                        return false;
                    }

                    var stream = await response.Content.ReadAsStreamAsync();
                    FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.ReadWrite);
                    await stream.CopyToAsync(fs);
                    fs.Close();

                    //using (var response = await client.PostAsync(uri, new FormUrlEncodedContent(parameters)))
                    //{
                    //    if (!response.IsSuccessStatusCode)
                    //    {
                    //        textConsole.WriteLine("Error: " + uri);
                    //        return false;
                    //    }

                    //    var stream = await response.Content.ReadAsStreamAsync();
                    //    FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.ReadWrite);
                    //    await stream.CopyToAsync(fs);
                    //    fs.Close();
                    //}
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
    }
}
