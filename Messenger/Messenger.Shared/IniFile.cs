using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Shared
{
    public class IniFile
    {
        private readonly string path;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder value, int size, string filePath);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int WritePrivateProfileString(string section, string key, string value, string filePath);

        public IniFile(string fileNameOrPath)
        {
            // Если передан просто имя файла, кладём рядом с EXE
            if (!Path.IsPathRooted(fileNameOrPath))
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileNameOrPath);
            else
                path = fileNameOrPath;
        }

        public string Read(string section, string key, string defaultValue = "")
        {
            var sb = new StringBuilder(512);
            GetPrivateProfileString(section, key, defaultValue, sb, sb.Capacity, path);
            return sb.ToString();
        }

        public void Write(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, path);
        }

        public bool KeyExists(string section, string key)
        {
            return !string.IsNullOrEmpty(Read(section, key));
        }
    }
}
