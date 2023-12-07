using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.CompilerServices;

namespace File_Downloader
{

    class FileVerifier
    {
        public TextConsole textConsole;
        public FileVerifier(TextConsole textConsole)
        {
            this.textConsole = textConsole;
        }

        public void CheckFolders(string folderPath)
        {
            if (folderPath == "")
            {
                return;
            }

            DirectoryInfo dir = new DirectoryInfo(folderPath);
            textConsole.WriteLine("The folders below are empty:");
            FolderGoDownLevel(dir);
        }

        private void FolderGoDownLevel(DirectoryInfo folder)
        {
            DirectoryInfo[] folderList = folder.GetDirectories();
            foreach (var folderInfo in folderList)
            {
                string folderName = folderInfo.Name;
                try
                {
                    if (folderInfo.GetDirectories().Length == 0)
                    {
                        if (folderInfo.GetFiles().Length == 0)
                        {
                            textConsole.WriteLine(folderInfo.FullName);
                        }
                    }
                    else
                    {
                        FolderGoDownLevel(folderInfo);
                    }
                }
                catch (Exception e)
                {
                    textConsole.WriteLine("Error Occurred in folder:"+folderName);
                    textConsole.WriteLine(e.Message);
                }
            }
        }

        public void ListFiles(string folderPath)
        {
            if (folderPath == "")
            {
                return;
            }

            textConsole.WriteLine("The files in the folder are listed below:");
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            FileGoDownLevel(dir);
        }

        private void FileGoDownLevel(DirectoryInfo folder)
        {
            DirectoryInfo[] folderList = folder.GetDirectories();

            foreach (var folderInfo in folderList)
            {
                FileInfo[] files = folderInfo.GetFiles();
                if (files.Length > 0)
                {
                    foreach (FileInfo file in files)
                    {
                        textConsole.WriteLine(folderInfo.FullName + "/" + file.Name);
                    }

                }
                
                if (folderInfo.GetDirectories().Length != 0)
                {
                    FileGoDownLevel(folderInfo);
                }
            }

        }
    }
}
