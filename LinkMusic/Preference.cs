using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace LinkMusic
{
    class Preference
    {
        public const string FILE_NAME = @"config.ini";
        public static string FILE_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AdbAudioController", FILE_NAME);
        /// <summary>
        /// 为INI文件中指定的节点取得字符串
        /// </summary>
        /// <param name="lpAppName">欲在其中查找关键字的节点名称</param>
        /// <param name="lpKeyName">欲获取的项名</param>
        /// <param name="lpDefault">指定的项没有找到时返回的默认值</param>
        /// <param name="lpReturnedString">指定一个字串缓冲区，长度至少为nSize</param>
        /// <param name="nSize">指定装载到lpReturnedString缓冲区的最大字符数量</param>
        /// <param name="lpFileName">INI文件完整路径</param>
        /// <returns>复制到lpReturnedString缓冲区的字节数量，其中不包括那些NULL中止字符</returns>
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        /// <summary>
        /// 修改INI文件中内容
        /// </summary>
        /// <param name="lpApplicationName">欲在其中写入的节点名称</param>
        /// <param name="lpKeyName">欲设置的项名</param>
        /// <param name="lpString">要写入的新字符串</param>
        /// <param name="lpFileName">INI文件完整路径</param>
        /// <returns>非零表示成功，零表示失败</returns>
        [DllImport("kernel32")]
        private static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString, string lpFileName);

        public static string ReadValue(string Section, string Key, string Def)
        {
            if (!File.Exists(FILE_PATH))
            {
                return Def;
            }
            StringBuilder sb = new StringBuilder(1024);
            GetPrivateProfileString(Section, Key, Def, sb, 1024, FILE_PATH);
            return sb.ToString();
        }

        public static void SaveValue(string Section, string key, string value)
        {
            if (!File.Exists(FILE_PATH))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FILE_PATH));
                File.Create(FILE_PATH);
            }
            WritePrivateProfileString(Section, key, value, FILE_PATH);
        }
    }
}
