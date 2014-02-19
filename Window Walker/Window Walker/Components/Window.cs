using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowWalker.Components
{
    /// <summary>
    /// Represents a specific open window
    /// </summary>
    public class Window
    {
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
            InteropAndHelpers.SetForegroundWindow(this.hwnd);
        }

        public override string ToString()
        {
            return this.Title;
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
    }
}
