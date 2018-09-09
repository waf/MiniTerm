using System;
using System.Runtime.InteropServices;
using static MiniTerm.NativeApi;

namespace MiniTerm
{
	/// <summary>
	/// Support for starting and configuring processes.
	/// </summary>
	/// <remarks>
	/// Possible to replace with managed code? The key is being able to provide the PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE attribute
	/// </remarks>
	static class Process
    {
		/// <summary>
		/// Start and configure a process. The return value should be send to <see cref="CleanUp"/>
		/// </summary>
		/// <param name="hPC"></param>
		/// <param name="command"></param>
		/// <param name="attributes"></param>
		/// <returns></returns>
		internal static (STARTUPINFOEX, PROCESS_INFORMATION) Start(IntPtr hPC, string command, IntPtr attributes)
		{
			var startupInfo = ConfigureProcessThread(hPC, attributes);
			var processInfo = RunProcess(ref startupInfo, "cmd.exe");
			return (startupInfo, processInfo);
		}

		private static STARTUPINFOEX ConfigureProcessThread(IntPtr hPC, IntPtr attributes)
		{
			var startupInfo = new STARTUPINFOEX();
			startupInfo.StartupInfo.cb = Marshal.SizeOf(startupInfo);
			var lpSize = IntPtr.Zero;
			startupInfo.lpAttributeList = Marshal.AllocHGlobal(lpSize);

			var success = InitializeProcThreadAttributeList(IntPtr.Zero, 1, 0, ref lpSize);
			if (success || lpSize == IntPtr.Zero)
			{
				throw new InvalidOperationException("Could not calculate the number of bytes for the attribute list. " + Marshal.GetLastWin32Error());
			}

			success = InitializeProcThreadAttributeList(startupInfo.lpAttributeList, 1, 0, ref lpSize);
			if (!success)
			{
				throw new InvalidOperationException("Could not set up attribute list. " + Marshal.GetLastWin32Error());
			}

			success = UpdateProcThreadAttribute(
				startupInfo.lpAttributeList,
				0,
				attributes,
				hPC,
				(IntPtr)IntPtr.Size,
				IntPtr.Zero,
				IntPtr.Zero);
			if (!success)
			{
				throw new InvalidOperationException("Could not set pseudoconsole thread attribute. " + Marshal.GetLastWin32Error());
			}

			return startupInfo;
		}

		private static PROCESS_INFORMATION RunProcess(ref STARTUPINFOEX sInfoEx, string commandLine)
		{
			PROCESS_INFORMATION pInfo;
			var pSec = new SECURITY_ATTRIBUTES();
			pSec.nLength = Marshal.SizeOf(pSec);
			var tSec = new SECURITY_ATTRIBUTES();
			tSec.nLength = Marshal.SizeOf(tSec);
			var result = CreateProcess(null, commandLine, ref pSec, ref tSec, false, EXTENDED_STARTUPINFO_PRESENT, IntPtr.Zero, null, ref sInfoEx, out pInfo);
			if (!result)
			{
				throw new InvalidOperationException("Could not create process. " + Marshal.GetLastWin32Error());
			}

			return pInfo;
		}

		public static void CleanUp((STARTUPINFOEX, PROCESS_INFORMATION) process)
		{
			var (startupInfo, processInfo) = process;

			// Free the attribute list
			if (startupInfo.lpAttributeList != IntPtr.Zero)
			{
				DeleteProcThreadAttributeList(startupInfo.lpAttributeList);
				Marshal.FreeHGlobal(startupInfo.lpAttributeList);
			}

			// Close process and thread handles
			if (processInfo.hProcess != IntPtr.Zero)
			{
				CloseHandle(processInfo.hProcess);
			}
			if (processInfo.hThread != IntPtr.Zero)
			{
				CloseHandle(processInfo.hThread);
			}
		}
	}
}
