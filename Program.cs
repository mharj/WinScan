using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinScan
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectoryLookup dl = new DirectoryLookup();
            Stack<string> directoryList = new Stack<string>();   
            if (args.Length == 0)
            {
                Console.WriteLine("Please give path to scan");
                return;
            }
            string startDirectory = GetWindowsPhysicalPath(args[0]);
            directoryList.Push(@startDirectory);
            
            while ( directoryList.Count != 0 )
            {
                string directory = directoryList.Pop();
                try
                {
                    dl.SetDirectory(directory);
                    FileData file;
                    while ( (file = dl.GetNextFile()) != null )
                    {
                        if ( file.FileName.Equals(".snapshot") || file.FileName.Equals("..") || file.FileName.Equals("."))
                        {
                            // skip
                        } 
                        else
                        { 
                            if (file.IsDirectory == true)
                            {
                                directoryList.Push(directory + @"\" + file.FileName);
                            }
                            else
                            {
                                
                            }
                        }
                    }
                    dl.close();
                } 
                catch ( UnauthorizedAccessException ex)
                {
                    Console.WriteLine(ex.getMessage());
                }
        }
    }
}
