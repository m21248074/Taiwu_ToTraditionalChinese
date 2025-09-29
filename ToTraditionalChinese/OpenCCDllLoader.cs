using System;
using System.Runtime.InteropServices;


namespace ToTraditionalChinese
{
    public class OpenCCDllLoader
    {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern IntPtr GetProcAddress(IntPtr lib, String funcName);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern bool FreeLibrary(IntPtr lib);

        private delegate IntPtr opencc_open_delegate(string configFileName);

        private delegate IntPtr opencc_convert_utf8_delegate(IntPtr opencc, IntPtr input, long length);

        private IntPtr openccModuleHandle = IntPtr.Zero;

        private opencc_open_delegate OpenCC_Open_Func;
        private opencc_convert_utf8_delegate OpenCC_Convert_UTF8_Func;

        public bool LoadOpenCC(string openccDllFilePath)
        {
            openccModuleHandle = LoadLibrary(openccDllFilePath);
            if (openccModuleHandle == IntPtr.Zero)
            {
                Console.WriteLine("錯誤：無法加載 opencc.dll。請確認它在執行路徑中。");
                return false;
            }

            IntPtr openPtr = GetProcAddress(openccModuleHandle, "opencc_open");
            if (openPtr == IntPtr.Zero)
            {
                Console.WriteLine("錯誤：找不到 opencc_open 函數。");
                return false;
            }
            OpenCC_Open_Func = (opencc_open_delegate)Marshal.GetDelegateForFunctionPointer(openPtr, typeof(opencc_open_delegate));

            IntPtr convertPtr = GetProcAddress(openccModuleHandle, "opencc_convert_utf8");
            if (convertPtr == IntPtr.Zero)
            {
                Console.WriteLine("錯誤：找不到 opencc_convert_utf8 函數。");
                return false;
            }
            OpenCC_Convert_UTF8_Func = (opencc_convert_utf8_delegate)Marshal.GetDelegateForFunctionPointer(convertPtr, typeof(opencc_convert_utf8_delegate));

            Console.WriteLine("已加載 opencc.dll");
            return true;
        }

        public void UnloadOpenCC()
        {
            if (openccModuleHandle != IntPtr.Zero)
            {
                FreeLibrary(openccModuleHandle);
                openccModuleHandle = IntPtr.Zero;
                OpenCC_Open_Func = null;
                OpenCC_Convert_UTF8_Func = null;
                Console.WriteLine("opencc.dll 已釋放。");
            }
        }

        public IntPtr opencc_open(string configFileName)
        {
            if (OpenCC_Open_Func == null)
                throw new InvalidOperationException("OpenCC 尚未成功加載。請先調用 LoadOpenCC()。");
            return OpenCC_Open_Func(configFileName);
        }

        public IntPtr opencc_convert_utf8(IntPtr opencc, IntPtr input, long length)
        {
            if (OpenCC_Convert_UTF8_Func == null)
                throw new InvalidOperationException("OpenCC 尚未成功加載。請先調用 LoadOpenCC()。");
            return OpenCC_Convert_UTF8_Func(opencc, input, length);
        }
    }
}
