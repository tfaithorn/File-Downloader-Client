using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace File_Downloader
{
    /// <summary>
    /// Base class for HTTP & Webview clients
    /// </summary>
    public abstract class DownloaderClient
    {
        public bool overrideFiles;
        public string folderDestinationPath;
        public TextConsole textConsole;
        public string destinationPath;
        protected string currentCountryID;
        public bool stopOnError;
        public Method formMethod;
        public static char[] invalidFileCharacters = new char[] { ':', '/', '\\', '*', '?', '>', '<', '|' };
        public static char[] invalidFolderCharacters = new char[] { '#', '%', '&', '{', '}', '\\', '/', '<', '>', '*', '?', '$', '!', '\'', '"', ':', '@', '|', '`', '\t', '.' };
        protected string loginUrl;
        protected string platformUrl;
        protected const string loginPath = "/login/graphql";
        protected const string emulationPath = "/emulation";

        public enum Method
        {
            GET,
            POST
        }

        protected string username;
        protected string password;

        public abstract Task<bool> Login(string requestUrl);
        public abstract Task<bool> DownloadFileTaskAsync(Uri uri, ExcelRow row);
        protected abstract Task<bool> EmulateCountry(string countryId);

        protected bool CheckIfFileExists(string path, string filename)
        {
            if (!overrideFiles && File.Exists(folderDestinationPath + "\\" + path + "\\" + filename))
            {
                textConsole.WriteLine("Last file Skipped as the file already exists");
                return true;
            }

            return false;
        }

        public void SetLoginCredentials(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        protected string ExtractPlatformUrl(string url)
        {
            var uri = new Uri(url);
            this.platformUrl = uri.Host;
            return "https://" + uri.Host;
        }

        public bool SetTextConsole(TextConsole textConsole)
        {
            this.textConsole = textConsole;
            return true;
        }

        public bool SetDestinationPath(string path)
        {
            destinationPath = path;
            textConsole.WriteLine("Destination Path:" + destinationPath);
            return true;
        }

        public static string SanitizeFolderPath(string path)
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

        public static string SanitizeFolderName(string foldername)
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

        public static string SanitizeFileName(string filename)
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
    }
}
