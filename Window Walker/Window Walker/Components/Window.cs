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
    class Window
    {
        #region Private Members

        /// <summary>
        /// The title of the window (the string displayed at the top of the window)
        /// </summary>
        private string title;

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
            get { return title; }
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

        public Window(IntPtr hwnd)
        {
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
            throw new NotImplementedException();
        }

        #endregion
    }
}
