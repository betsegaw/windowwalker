using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowWalker.Components
{
    /// <summary>
    /// Class that represents the state of the desktops windows
    /// </summary>
    class OpenWindows
    {
        IEnumerable<Tuple<string,string>> Blacklist = (new List<Tuple<string, string>> () { 
            new Tuple<string, string> ( null, "ShellExperienceHost.EXE".ToUpper() ),
            new Tuple<string, string> ( null, "ApplicationFrameHost.EXE".ToUpper() ),
            new Tuple<string, string> ( "Program Manager".ToUpper(), "Explorer.EXE".ToUpper() )
            }).AsEnumerable();

        #region Delegates

        /// <summary>
        /// Delegate handler for open windows updates
        /// </summary>
        public delegate void OpenWindowsUpdateHandler(object sender, Window.WindowListUpdateEventArgs e);

        /// <summary>
        /// Event raised when there is an update to the list of open windows
        /// </summary>
        public event OpenWindowsUpdateHandler OnOpenWindowsUpdate;

        #endregion


        #region Members

        /// <summary>
        /// List of all the open windows
        /// </summary>
        private List<Window> windows = new List<Window>();

        /// <summary>
        /// An instance of the class OpenWindows
        /// </summary>
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

        /// <summary>
        /// An instance property of this class that makes sure that
        /// the first instance gets created and that all the requests
        /// end up at that one instance
        /// </summary>
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
        
        #endregion

        /// <summary>
        /// Updates the list of open windows
        /// </summary>
        public void UpdateOpenWindowsList()
        {
            this.windows.Clear();
            
            new Task(() =>
                {
                    InteropAndHelpers.CallBackPtr callbackptr = new InteropAndHelpers.CallBackPtr(WindowEnumerationCallBack);
                    InteropAndHelpers.EnumWindows(callbackptr, 0);
                }
            ).Start();
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
            Window newWindow = new Window(hwnd);

            if (windows.Select(x => x.Title).Contains(newWindow.Title))
            {
                if (newWindow.ProcessName.ToLower().Equals("applicationframehost.exe"))
                {
                    windows.Remove(windows.Where(x => x.Title == newWindow.Title).First());
                    return false;
                }
                else
                {
                    return true;
                }

            }

            if (newWindow.Visible && !newWindow.ProcessName.ToLower().Equals("iexplore.exe") ||
                (newWindow.ProcessName.ToLower().Equals("iexplore.exe") && newWindow.ClassName == "TabThumbnailWindow")) 
            {
                this.windows.Add(newWindow);

                if (OnOpenWindowsUpdate != null)
                {
                    this.OnOpenWindowsUpdate(this, new Window.WindowListUpdateEventArgs());
                }
            }

            return true;
        }
    }
}
