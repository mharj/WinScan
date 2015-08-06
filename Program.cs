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
            ulong size = 0L;
            ulong files = 0L;
            ulong dirs = 0L;
            DirectoryLookup dl = new DirectoryLookup();
            Stack<string> directoryList = new Stack<string>();   
            if (args.Length == 0)
            {
                Console.WriteLine("Please give path to scan");
                return;
            }
            string startDirectory = GetWindowsPhysicalPath(args[0]);
            directoryList.Push(@startDirectory);

            while (directoryList.Count != 0)
            {
                string directory = directoryList.Pop();
                try
                {
                    dl.SetDirectory(directory);
                    FileData file;
                    while ((file = dl.GetNextFile()) != null)
                    {
                        if (file.FileName.Equals(".snapshot") || file.FileName.Equals("..") || file.FileName.Equals("."))
                        {
                            // skip
                        }
                        else
                        {
                            if (file.IsDirectory == true)
                            {
                                directoryList.Push(directory + @"\" + file.FileName);
                                dirs++;
                            }
                            else
                            {
                                files++;
                                size += file.FileSize;
                            }
                        }
                    }
                    dl.close();
                }
                catch (UnauthorizedAccessException ex)
                {
#if DEBUG
                    Console.WriteLine(ex.Message);
#endif            
                }
                
            }
            Console.WriteLine(startDirectory + "= files: " + files + " dirs: " + dirs + " size: " + size);
        }
        
        protected static string GetWindowsPhysicalPath(string path)
        {
            StringBuilder builder = new StringBuilder(255);
            NativeMethods.GetShortPathName(path, builder, builder.Capacity);
            path = builder.ToString();
            uint result = NativeMethods.GetLongPathName(path, builder, builder.Capacity);
            if (result > 0 && result < builder.Capacity)
            {
                builder[0] = char.ToLower(builder[0]);
                return builder.ToString(0, (int)result);
            }
            if (result > 0)
            {
                builder = new StringBuilder((int)result);
                result = NativeMethods.GetLongPathName(path, builder, builder.Capacity);
                builder[0] = char.ToLower(builder[0]);
                return builder.ToString(0, (int)result);
            }
            return null;
        }
    }
}
