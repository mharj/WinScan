using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinScan
{
    class Logger
    {
        private static Logger instance = null;
        private StreamWriter w = null;

        private Logger()
        {
            w = File.AppendText("log.txt");
        }

        public void Log(string msg)
        {
            w.WriteLine("{0} {1} : {2}",DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(),msg);
            w.Flush();
        }

        public static Logger GetInstance()
        {
            if (instance == null)
            {
                instance = new Logger();
            }
            return instance;
        }

    }
}
