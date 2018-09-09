using Microsoft.Win32.SafeHandles;
using System;
using static MiniTerm.NativeApi;

namespace MiniTerm
{
    /// <summary>
    /// Utility functions around the new Pseudo Console APIs
    /// </summary>
    static class PseudoConsole
    {
        internal static IntPtr CreatePseudoConsole(SafeFileHandle inputReadSide, SafeFileHandle outputWriteSide)
        {
            var createResult = NativeApi.CreatePseudoConsole(new COORD { X = 100, Y = 100 }, inputReadSide, outputWriteSide, 0, out IntPtr hPC);
            if(createResult != 0)
            {
                throw new InvalidOperationException("Could not create psuedo console. Error Code " + createResult);
            }
            return hPC;
        }
    }
}
