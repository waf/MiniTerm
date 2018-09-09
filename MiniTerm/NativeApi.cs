using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace MiniTerm
{
    static class NativeApi
    {
        internal const uint PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE = 0x00020016;
        internal const uint EXTENDED_STARTUPINFO_PRESENT = 0x00080000;

        internal const int STD_OUTPUT_HANDLE = -11;
        internal const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        internal const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

        [StructLayout(LayoutKind.Sequential)]
        internal struct COORD
        {
            internal short X;
            internal short Y;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct STARTUPINFOEX
        {
            internal STARTUPINFO StartupInfo;
            internal IntPtr lpAttributeList;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct STARTUPINFO
        {
            internal Int32 cb;
            internal string lpReserved;
            internal string lpDesktop;
            internal string lpTitle;
            internal Int32 dwX;
            internal Int32 dwY;
            internal Int32 dwXSize;
            internal Int32 dwYSize;
            internal Int32 dwXCountChars;
            internal Int32 dwYCountChars;
            internal Int32 dwFillAttribute;
            internal Int32 dwFlags;
            internal Int16 wShowWindow;
            internal Int16 cbReserved2;
            internal IntPtr lpReserved2;
            internal IntPtr hStdInput;
            internal IntPtr hStdOutput;
            internal IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_INFORMATION
        {
            internal IntPtr hProcess;
            internal IntPtr hThread;
            internal int dwProcessId;
            internal int dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SECURITY_ATTRIBUTES
        {
            internal int nLength;
            internal IntPtr lpSecurityDescriptor;
            internal int bInheritHandle;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern SafeFileHandle GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleMode(SafeFileHandle hConsoleHandle, uint mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetConsoleMode(SafeFileHandle handle, out uint mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int CreatePseudoConsole(COORD size, SafeFileHandle hInput, SafeFileHandle hOutput, uint dwFlags, out IntPtr phPC);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int CreatePseudoConsole(COORD size, IntPtr hInput, IntPtr hOutput, uint dwFlags, out IntPtr phPC);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int ResizePseudoConsole(IntPtr hPC, COORD size);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int ClosePseudoConsole(IntPtr hPC);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool InitializeProcThreadAttributeList(
            IntPtr lpAttributeList, int dwAttributeCount, int dwFlags, ref IntPtr lpSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UpdateProcThreadAttribute(
            IntPtr lpAttributeList, uint dwFlags, IntPtr Attribute, IntPtr lpValue,
            IntPtr cbSize, IntPtr lpPreviousValue, IntPtr lpReturnSize);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool CreatePipe(ref IntPtr hReadPipe, ref IntPtr hWritePipe, IntPtr lpPipeAttributes, int nSize);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool CreatePipe(out SafeFileHandle hReadPipe, out SafeFileHandle hWritePipe, IntPtr lpPipeAttributes, int nSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool ReadFile(IntPtr hFile, [Out] byte[] lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool ReadFile(SafeFileHandle hFile, [Out] byte[] lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool WriteFile(SafeFileHandle hFile, byte[] lpBuffer,
           uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten,
           IntPtr lpOverlapped);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CreateProcess(
            string lpApplicationName, string lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes,
            ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags,
            IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref STARTUPINFOEX lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteProcThreadAttributeList(IntPtr lpAttributeList);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr hObject);
    }
}
