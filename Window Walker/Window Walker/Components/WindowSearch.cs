using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowWalker.Components
{
    /// <summary>
    /// Responsible for searching and finding matches for the strings provided.
    /// Essentially the UI independent model of the application
    /// </summary>
    class WindowSearchController
    {
        #region Members

        /// <summary>
        /// the current search text
        /// </summary>
        private string searchText;

        /// <summary>
        /// Open window search results
        /// </summary>
        private List<Window> searchMatches;

        /// <summary>
        /// Singleton pattern
        /// </summary>
        private static WindowSearchController instance;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current search text
        /// </summary>
        public string SearchText
        {
            get { return searchText; }
            set 
            { 
                searchText = value;
                this.SearchTextUpdated();
            }
        }

        /// <summary>
        /// Gets the open window search results
        /// </summary>
        public List<Window> SearchMatches
        {
            get { return new List<Window>(searchMatches); }
        }

        /// <summary>
        /// Singleton Pattern
        /// </summary>
        public static WindowSearchController Instance
        {
            get
            {
                if (WindowSearchController.instance == null)
                {
                    WindowSearchController.instance = new WindowSearchController();
                }

                return WindowSearchController.instance;
            }

        }

        #endregion

        /// <summary>
        /// Initializes the search controller object
        /// </summary>
        public WindowSearchController()
        {
            this.SearchText = string.Empty;
        }

        /// <summary>
        /// Event handler for when the search text has been updated
        /// </summary>
        public void SearchTextUpdated()
        {
            OpenWindows.Instance.UpdateOpenWindowsList();
            List<Window> snapshotOfOpenWindows = OpenWindows.Instance.Windows;

            if (this.SearchText == string.Empty)
            {
                this.searchMatches = (from singleWindow in snapshotOfOpenWindows
                                      where singleWindow.Title != string.Empty
                                      select singleWindow).ToList<Window>();
            }
            else
            {
                this.searchMatches =    (List<Window>)
                                        from singleWindow in snapshotOfOpenWindows
                                        where singleWindow.Title.Contains(this.searchText)
                                        select singleWindow;
            }
        }
    }
}
