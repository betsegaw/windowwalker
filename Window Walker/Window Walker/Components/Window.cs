using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WindowWalker.Components
{
    /// <summary>
    /// Represents a specific open window
    /// </summary>
    public class Window
    {
        #region Constants

        /// <summary>
        /// Maximum size of a file name
        /// </summary>
        private const int MaximumFileNameLength = 1000;

        #endregion

        #region Static members

        /// <summary>
        /// The list of owners of a window so that we don't have to
        /// constantly query for the process owning a specific window
        /// </summary>
        private static Dictionary<IntPtr,string> handlesToProcessCache = new Dictionary<IntPtr,string>();

        /// <summary>
        /// The list of icons from process so that we don't have to keep
        /// loading them from disk
        /// </summary>
        private static Dictionary<uint, ImageSource> processIdsToIconsCache = new Dictionary<uint, ImageSource>();

        #endregion

        #region Private Members

        /// <summary>
        /// The handle to the window
        /// </summary>
        private IntPtr hwnd;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the title of the window (the string displayed at the top of the window)
        /// </summary>
        public string Title
        {
            get 
            {
                int sizeOfTitle = InteropAndHelpers.GetWindowTextLength(this.hwnd);
                if (sizeOfTitle++ > 0)
                {
                    StringBuilder titleBuffer = new StringBuilder(sizeOfTitle);
                    InteropAndHelpers.GetWindowText(this.hwnd, titleBuffer, sizeOfTitle);
                    return titleBuffer.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets the handle to the window
        /// </summary>
        public IntPtr Hwnd
        {
            get { return hwnd; }
        }

        /// <summary>
        /// Returns the name of the process
        /// </summary>
        public String ProcessName
        {
            get
            {
                if (Window.handlesToProcessCache.Count > 700)
                {
                    Window.handlesToProcessCache.Clear();
                }

                if (!Window.handlesToProcessCache.ContainsKey(this.Hwnd))
                {
                    uint processId = 0;
                    InteropAndHelpers.GetWindowThreadProcessId(this.Hwnd, out processId);
                    IntPtr processHandle = InteropAndHelpers.OpenProcess(InteropAndHelpers.ProcessAccessFlags.AllAccess, true, (int)processId);
                    StringBuilder processName = new StringBuilder(Window.MaximumFileNameLength);
                    if (InteropAndHelpers.GetProcessImageFileName(processHandle, processName, Window.MaximumFileNameLength) != 0)
                    {
                        Window.handlesToProcessCache.Add(this.Hwnd, processName.ToString().Split('\\').Reverse().ToArray()[0]);
                    }
                    else
                    {
                        Window.handlesToProcessCache.Add(this.Hwnd, string.Empty);
                    }
                }

                return Window.handlesToProcessCache[this.hwnd];
            }
        }

        /// <summary>
        /// Returns the name of the class for the window represented
        /// </summary>
        public String ClassName
        {
            get
            {
                StringBuilder WindowClassName = new StringBuilder(300);
                InteropAndHelpers.GetClassName(this.Hwnd, WindowClassName, WindowClassName.MaxCapacity);

                return WindowClassName.ToString();
            }
        }   

        public ImageSource WindowIcon
        {
            get
            {
                uint processId = 0;
                InteropAndHelpers.GetWindowThreadProcessId(this.Hwnd, out processId);
                
                if (!Window.processIdsToIconsCache.ContainsKey(processId))
                {
                    try
                    { 
                        Process process = Process.GetProcessById((int)processId);
                        Icon tempIcon = Icon.ExtractAssociatedIcon(process.Modules[0].FileName);
                        Window.processIdsToIconsCache.Add(processId,Imaging.CreateBitmapSourceFromHIcon(
                            tempIcon.Handle,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions()));
                    }
                    catch
                    {
                        BitmapImage failedImage = new BitmapImage(new Uri(@"Images\failedIcon.jpg",UriKind.Relative));
                        Window.processIdsToIconsCache.Add(processId, failedImage);
                    }
                }

                return Window.processIdsToIconsCache[processId];
            }
        }

        /// <summary>
        /// Is the window visible (might return false if it is a hidden IE tab)
        /// </summary>
        public bool Visible
        {
            get
            {
                return InteropAndHelpers.IsWindowVisible(this.Hwnd);
            }
        }

        /// <summary>
        /// Returns true if the window is minimized
        /// </summary>
        public bool Minimized
        {
            get
            {
                return this.GetWindowSizeState() == WindowSizeState.Minimized;
            }
        }
        #endregion

        #region Constructors
        
        /// <summary>
        /// Initializes a new Window representation
        /// </summary>
        /// <param name="hwnd">the handle to the window we are representing</param>
        public Window(IntPtr hwnd)
        {
            // TODO: Add verification as to whether the window handle is valid
            this.hwnd = hwnd;
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// Highlights a window to help the user identify the window that has been selected
        /// </summary>
        public void HighlightWindow()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Switches dekstop focus to the window
        /// </summary>
        public void SwitchToWindow()
        {
            // The following block is necessary because
            // 1) There is a weird flashing behaviour when trying
            //    to use ShowWindow for switching tabs in IE
            // 2) SetForegroundWindow fails on minimized windows

            if (this.ProcessName.ToLower().Equals("iexplore.exe") || !this.Minimized)
            {
                InteropAndHelpers.SetForegroundWindow(this.Hwnd);
            }
            else
            {
                InteropAndHelpers.ShowWindow(this.Hwnd, InteropAndHelpers.ShowWindowCommands.Restore);
            }

            InteropAndHelpers.FlashWindow(this.Hwnd, true);
        }

        public override string ToString()
        {
            return this.Title + " (" + this.ProcessName.ToUpper() + ")";
        }

        public WindowSizeState GetWindowSizeState()
        {
            InteropAndHelpers.WINDOWPLACEMENT placement = new InteropAndHelpers.WINDOWPLACEMENT();
            InteropAndHelpers.GetWindowPlacement(this.Hwnd,out placement);

            switch (placement.ShowCmd)
            {
                case InteropAndHelpers.ShowWindowCommands.Normal:
                    return WindowSizeState.Normal;
                case InteropAndHelpers.ShowWindowCommands.Minimize:
                case InteropAndHelpers.ShowWindowCommands.ShowMinimized:
                    return WindowSizeState.Minimized;
                case InteropAndHelpers.ShowWindowCommands.Maximize: // No need for ShowMaximized here since its also of value 3
                    return WindowSizeState.Maximized;
                default:
                    // throw new Exception("Don't know how to handle window state = " + placement.ShowCmd);
                    return WindowSizeState.Unknown;
            }
        }

        #endregion

        #region Classes

        /// <summary>
        /// Event args for a window list update event
        /// </summary>
        public class WindowListUpdateEventArgs : EventArgs
        {

        }

        #endregion

        #region Enums

        public enum WindowSizeState
        {
            Normal,
            Minimized,
            Maximized,
            Unknown
        }

        #endregion
    }
}
