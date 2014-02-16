using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowWalker.Components
{
    /// <summary>
    /// Class containing static methods for accessing all open windows
    /// </summary>
    class OpenWindows
    {
        #region Members

        /// <summary>
        /// List of all the open windows
        /// </summary>
        private List<Window> windows = new List<Window>();

        private static OpenWindows instance;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of all open windows
        /// </summary>
        public List<Window> Windows
        {
            get { return new List<Window>(windows); }
        }

        public static OpenWindows Instance
        {
            get 
            { 
                if (instance == null)
                {
                    instance = new OpenWindows();
                }

                return instance;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor to make sure there is never
        /// more than one instance of this class
        /// </summary>
        private OpenWindows()
        {

        }

        /// <summary>
        /// Updates the list of open windows
        /// </summary>
        public void UpdateOpenWindowsList()
        {
            this.windows.Clear();
            InteropAndHelpers.CallBackPtr callbackptr = new InteropAndHelpers.CallBackPtr(WindowEnumerationCallBack);
            InteropAndHelpers.EnumWindows(callbackptr, 0);
        }

        /// <summary>
        /// Call back method for window enumeration
        /// </summary>
        /// <param name="hwnd">The handle to the current window being enumerated</param>
        /// <param name="lParam">Value being passed from the caller (we don't use this but might come in handy
        /// in the future</param>
        /// <returns>true to make sure to contiue enumeration</returns>
        public bool WindowEnumerationCallBack(IntPtr hwnd, IntPtr lParam)
        {
            this.windows.Add(new Window(hwnd));
            return true;
        }

        #endregion
    }
}
