using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_Downloader
{
    public class ExcelRow
    {
        public string itemID;
        public string filename;
        public string path;
        public string countryID;
        public string payload;

        public ExcelRow(string itemID, string filename, string path, string countryID, string payload = null)
        {
            this.itemID = itemID;
            this.filename = filename;
            this.path = path;
            this.countryID = countryID;
            this.payload = payload;
        }
    }
}
