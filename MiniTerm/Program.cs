using Microsoft.Win32.SafeHandles;
using System;
using System.Threading.Tasks;
using static MiniTerm.NativeApi;

namespace MiniTerm
{
    /// <summary>
    /// C# version of:
    /// https://blogs.msdn.microsoft.com/commandline/2018/08/02/windows-command-line-introducing-the-windows-pseudo-console-conpty/
    /// https://docs.microsoft.com/en-us/windows/console/creating-a-pseudoconsole-session
    ///
    /// System Requirements:
    /// As of September 2018, requires Windows 10 with the "Windows Insider Program" installed for Redstone 5.
    /// Also requires the Windows Insider Preview SDK: https://www.microsoft.com/en-us/software-download/windowsinsiderpreviewSDK
    /// </summary>
    static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // newer versions of the windows console support interpreting virtual terminal sequences, we just have to opt-in
                Terminal.EnableVirtualTerminalSequenceProcessing();

                RunTerminal();
            }
            catch (InvalidOperationException e)
            {
                Console.Error.WriteLine(e.Message);
                throw;
            }
        }

        private static void RunTerminal()
        {
            if (!CreatePipe(out SafeFileHandle inputReadSide, out SafeFileHandle inputWriteSide, IntPtr.Zero, 0))
            {
                throw new InvalidOperationException("failed to create input pipe");
            }
            if (!CreatePipe(out SafeFileHandle outputReadSide, out SafeFileHandle outputWriteSide, IntPtr.Zero, 0))
            {
                throw new InvalidOperationException("failed to create output pipe");
            }

            // set up a background task to copy all pseudo console output to stdout
            Task.Run(() => Terminal.CopyPipeToOutput(outputReadSide));

            IntPtr hPC = PseudoConsole.CreatePseudoConsole(inputReadSide, outputWriteSide);
            var process = Process.Start(hPC, "cmd.exe", (IntPtr)PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE);

            // prompt for stdin input and send the result to the pipe.
            // blocks until the user types "exit"
            Terminal.CopyInputToPipe(inputWriteSide);

            Process.CleanUp(process);
            ClosePseudoConsole(hPC);
            inputReadSide.Dispose();
            inputWriteSide.Dispose();
            outputReadSide.Dispose();
            outputWriteSide.Dispose();
        }
    }
}
