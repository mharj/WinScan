using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinScan
{
    class FileData
    {
        private const int FILE_ATTRIBUTE_DIRECTORY = 0x10;
        public string FileName = null;
        public DateTime CreationTime;
        public DateTime LastAccessTime;
        public DateTime LastWriteTime;
        public UInt64 FileSize;
        public bool IsDirectory = false;
        public static FileData fromFindData(WIN32_FIND_DATA wfd)
        {
            FileData ret = new FileData();
            ret.FileName = wfd.cFileName;
            ret.FileSize = (UInt64)((wfd.nFileSizeHigh * (2 ^ 32)) + wfd.nFileSizeLow);
            ret.CreationTime = FileData.convertToDateTime(wfd.ftCreationTime);
            ret.LastAccessTime = FileData.convertToDateTime(wfd.ftLastAccessTime);
            ret.LastWriteTime = FileData.convertToDateTime(wfd.ftLastWriteTime);
            if ((wfd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) != 0)
            {
                ret.IsDirectory = true;
            }
            return ret;
        }
        private static DateTime convertToDateTime(System.Runtime.InteropServices.ComTypes.FILETIME time)
        {
            ulong high = (ulong)time.dwHighDateTime;
            uint low = (uint)time.dwLowDateTime;
            high = high << 32;
            return DateTime.FromFileTime((long)(high | (ulong)low));
        }
    }
}
