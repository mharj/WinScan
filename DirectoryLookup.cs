using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinScan
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct WIN32_FIND_DATA
    {
        public uint dwFileAttributes;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
        public uint nFileSizeHigh;
        public uint nFileSizeLow;
        public uint dwReserved0;
        public uint dwReserved1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string cFileName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public string cAlternateFileName;
    }
    
    class DirectoryLookup
    {
        public const int ERROR_SUCCESS = 0x0;
        public const int ERROR_INVALID_FUNCTION = 0x1;
        public const int ERROR_FILE_NOT_FOUND = 0x2;
        public const int ERROR_PATH_NOT_FOUND = 0x3;
        public const int ERROR_TOO_MANY_OPEN_FILES = 0x4;
        public const int ERROR_ACCESS_DENIED = 0x5;
        public const int ERROR_INVALID_NAME = 0x123;
        bool disposed = false;
        bool closed = false;
        IntPtr h;
        WIN32_FIND_DATA wfd = new WIN32_FIND_DATA();
        private string dir;
        private bool first = true;
        IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        public DirectoryLookup()
        {
            SetDirectory(dir);
        }

        ~DirectoryLookup()
        {
            close();
        }

        public void SetDirectory(string _dir)
        {
            dir = _dir;
            first = true;
        }

        public FileData GetNextFile()
        {
            if (first == true)
            {
                first = false;
                h = NativeMethods.FindFirstFile((@"\\?\" + dir + @"\*.*"), out wfd);
                int error = Marshal.GetLastWin32Error();
                if (h.Equals(INVALID_HANDLE_VALUE))
                {
                    switch (error)
                    {
                        case ERROR_ACCESS_DENIED: // soft exception
                            throw new UnauthorizedAccessException("ERROR: Access denied " + dir);
                        default:
                            throw new Exception("Handle error [code:" + error + "] " + dir);
                    }
                }
                closed = false;
            }
            else
            {
                if (!NativeMethods.FindNextFile(h, out wfd))
                {
                    return null;
                }
            }
            return FileData.fromFindData(wfd);
        }

        public void close()
        {
            if ( closed == false )
            {
                NativeMethods.FindClose(h);
                closed = true;
                dir = null;
            }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }
            close();
        }
    }
}
