using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRTTiming
{
    class Program
    {
        static void Main(string[] args)
        {
            string srtPath = getPath();

            if (srtPath != null)
            {
            Console.WriteLine("Seconds (##.###):");
            string sSec = Console.ReadLine();
            decimal dSec;
            if (decimal.TryParse(sSec, out dSec))
            {
                srtFile myFile = new srtFile();
                myFile.FilePath = srtPath;
                myFile.TmpFilePath = srtPath + ".tmp";
                myFile.BakFilePath = srtPath + ".bak";
                myFile.Seconds = dSec;
                myFile.AddSeconds();
            }
            else
                Console.WriteLine("Seconds is not in the right format...");
            }
        }

        private static string getPath()
        {
            Console.WriteLine("Full file path:");
            string srtpath = Console.ReadLine();
            if (File.Exists(srtpath))
                return srtpath;
            else
            {
                Console.WriteLine("Can not access file specified...");
                return null;
            }
        }
    }

    class srtFile
    {
        public string FilePath { get; set; }
        public string TmpFilePath { get; set; }
        public string BakFilePath { get; set; }
        public decimal Seconds { get; set; }

        public void AddSeconds()
        {
            if (File.Exists(this.TmpFilePath))
                File.Delete(this.TmpFilePath);

            if (File.Exists(this.BakFilePath))
                File.Delete(this.BakFilePath);

            StreamReader mySR = new StreamReader(this.FilePath, Encoding.GetEncoding("iso-8859-1"));
            StreamWriter mySW = new StreamWriter(this.TmpFilePath, false, Encoding.GetEncoding("iso-8859-1"));

            string line = "";
            while (line != null)
            {
                line = mySR.ReadLine();
                if (line != null)
                {
                    if (line.Contains("-->"))
                        line = doAdd(line.Substring(0, 12)) + " --> " + doAdd(line.Substring(17, 12));
                    mySW.WriteLine(line);
                }
            }

            mySR.Close();
            mySW.Close();
            mySR = null;
            mySW = null;

            renameFiles();
        }

        private string doAdd(string token)
        {
            int hh;
            int mm;
            int ss;
            int ms;
            string r;
            decimal time_seconds;

            int.TryParse(token.Substring(0, 2), out hh);
            int.TryParse(token.Substring(3, 2), out mm);
            int.TryParse(token.Substring(6, 2), out ss);
            int.TryParse(token.Substring(9, 3), out ms);

            time_seconds = hh * 3600 + mm * 60 + ss + (decimal)ms / 1000;
            time_seconds += this.Seconds;

            hh = (int)(time_seconds / 3600);
            time_seconds -= hh * 3600;
            mm = (int)(time_seconds / 60);
            time_seconds -= mm * 60;
            ss = (int)time_seconds;
            time_seconds -= ss;
            ms = (int)(time_seconds * 1000);

            r = string.Format("{0:0#}:{1:0#}:{2:0#},{3:00#}", hh, mm, ss, ms);

            return r;
        }

        private void renameFiles()
        {
            File.Move(this.FilePath, this.BakFilePath);
            File.Move(this.TmpFilePath, this.FilePath);
        }
    }
}
