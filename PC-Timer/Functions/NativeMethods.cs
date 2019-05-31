#region Header

//----------------------------------------------------------------------
// 
// Project is available at https://github.com/Hundhausen
// This Project is licensed under the GNU General Public License v3.0
//
// Date: 2019-05-26
// User: Hundhausen
//
//----------------------------------------------------------------------

#endregion

using System.Runtime.InteropServices;
using PC_Timer.Form;

namespace PC_Timer.Functions
{
    /// <summary>
    /// Import of (native) DLLs
    /// </summary>
    public class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern FrmPcTimerMain.EXECUTION_STATE SetThreadExecutionState(FrmPcTimerMain.EXECUTION_STATE esFlags);
    }
}
//----------------------------------------------------------------------
// Project is available at https://github.com/Hundhausen
//----------------------------------------------------------------------