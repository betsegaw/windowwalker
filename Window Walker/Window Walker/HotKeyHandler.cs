using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace WindowWalker
{
    /// <summary>
    /// This class handles all hotkey related activities
    /// </summary>
    /// <remarks>Large pieces of this code were retrived from 
    /// http://www.dreamincode.net/forums/topic/323708-global-hotkeys-for-wpf-applications-c%23/
    /// </remarks>
    class HotKeyHandler
    {
        private IntPtr hwnd;

        /// <summary>
        /// Constructor for the class
        /// </summary>
        /// <param name="hwnd">The handle to the window we are registering the key for</param>
        public HotKeyHandler(Visual window)
        {
            this.hwnd = new WindowInteropHelper((System.Windows.Window)window).Handle;

            var source = PresentationSource.FromVisual(window) as HwndSource;  

            if (source == null)
            { 
                throw new Exception("Could not create hWnd source from window.");  
            }

            source.AddHook(WndProc);  
        }

        /// <summary>
        /// Call back function to detect when the hot key has been called
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)  

        {  

            return IntPtr.Zero;  
        } 

    }
}
