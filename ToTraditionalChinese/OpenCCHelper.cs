using System;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;


namespace ToTraditionalChinese
{
    public static class OpenCCHelper
    {
        private static string extractionDir;
        /*
        [DllImport("opencc.dll", EntryPoint = "opencc_open")]
        private static extern IntPtr opencc_open(string configFileName);

        [DllImport("opencc.dll", EntryPoint = "opencc_convert_utf8")]
        private static extern IntPtr opencc_convert_utf8(IntPtr opencc, IntPtr input, long length);
        */
        private static OpenCCDllLoader loader;
        private static IntPtr opencc;

        public static string OpenCC(this string text)
        {
            try
            {
                int len = Encoding.UTF8.GetByteCount(text);
                byte[] buffer = new byte[len + 1];
                Encoding.UTF8.GetBytes(text, 0, text.Length, buffer, 0);
                IntPtr inStr = Marshal.AllocHGlobal(buffer.Length);
                try
                {
                    Marshal.Copy(buffer, 0, inStr, buffer.Length);
                    IntPtr outStr = loader.opencc_convert_utf8(opencc, inStr, -1);
                    try
                    {
                        int outLen = 0;
                        while (Marshal.ReadByte(outStr, outLen) != 0) ++outLen;
                        byte[] outBuffer = new byte[outLen];
                        Marshal.Copy(outStr, outBuffer, 0, outBuffer.Length);
                        return Encoding.UTF8.GetString(outBuffer);
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(outStr);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(inStr);
                }
            }
            finally
            {
                //Marshal.FreeHGlobal(opencc);
            }
        }
        public static void ExtractOpenCCFiles()
        {
            extractionDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "opencc");
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            string[] resourceNames = assembly.GetManifestResourceNames();
            foreach (string resourceName in resourceNames)
            {
                if (resourceName.Contains("opencc"))
                {
                    string[] parts = resourceName.Split('.');
                    string fileName = "";
                    if (parts.Length >= 2)
                    {
                        string fileNamePart = parts[parts.Length - 2];
                        string extensionPart = parts[parts.Length - 1];
                        fileName = $"{fileNamePart}.{extensionPart}";
                    }
                    string targetPath = Path.Combine(extractionDir, fileName);
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    using (FileStream fileStream = File.Create(targetPath))
                    {
                        stream.CopyTo(fileStream);
                    }
                    if (resourceName.Contains("opencc.dll"))
                    {
                        Console.WriteLine(resourceName + " " + targetPath);
                        loader = new OpenCCDllLoader();
                        loader.LoadOpenCC(targetPath);
                    }
                }
            }
        }
        public static void InitialOpenCC(string config)
        {
            ExtractOpenCCFiles();
            var configFile = Path.Combine(extractionDir, $"{config}.json");
            if (!File.Exists(configFile))
            {
                throw new FileNotFoundException("設定檔找不到", configFile);
            }
            opencc = loader.opencc_open(configFile);
        }
        public static void DisposeOpenCC()
        {
            Marshal.FreeHGlobal(opencc);
            loader.UnloadOpenCC();
        }
    }
}
