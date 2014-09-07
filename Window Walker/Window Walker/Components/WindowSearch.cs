using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

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
        /// </summary
        private List<WindowSearchResult> searchMatches;

        /// <summary>
        /// Singleton pattern
        /// </summary>
        private static WindowSearchController instance;

        #endregion

        #region Events

        /// <summary>
        /// Delegate handler for open windows updates
        /// </summary>
        public delegate void SearchResultUpdateHandler(object sender, Window.WindowListUpdateEventArgs e);

        /// <summary>
        /// Event raised when there is an update to the list of open windows
        /// </summary>
        public event SearchResultUpdateHandler OnSearchResultUpdate;

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
                searchText = value.ToLower().Trim();
                this.SearchTextUpdated();
            }
        }

        /// <summary>
        /// Gets the open window search results
        /// </summary>
        public List<WindowSearchResult> SearchMatches
        {
            get 
            { return new List<WindowSearchResult>(searchMatches); }
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
        private WindowSearchController()
        {
            this.searchText = string.Empty;
            OpenWindows.Instance.OnOpenWindowsUpdate += OpenWindowsUpdateHandler;
        }

        /// <summary>
        /// Event handler for when the search text has been updated
        /// </summary>
        public void SearchTextUpdated()
        {
            this.SyncOpenWindowsWithModelAsync();
        }

        #region Event Handlers

        public void OpenWindowsUpdateHandler(object sender, Window.WindowListUpdateEventArgs e)
        {
            this.SyncOpenWindowsWithModelAsync();
        }

        /// <summary>
        /// Syncs the open windows with the OpenWindows Model
        /// </summary>
        private async void SyncOpenWindowsWithModelAsync()
        {
            System.Diagnostics.Debug.Print("Syncing WindowSearch result with OpenWindows Model");

            List<Window> snapshotOfOpenWindows = OpenWindows.Instance.Windows;

            if (this.SearchText == string.Empty)
            {
                var windows = 
                    (snapshotOfOpenWindows.Where(x => !string.IsNullOrEmpty(x.Title)).ToList());

                this.searchMatches = new List<WindowSearchResult>();

                foreach(var window in windows)
                {
                    this.searchMatches.Add(new WindowSearchResult(window, new List<int>(), new List<int>()));
                }
            }
            else
            {
                this.searchMatches = await FuzzySearchOpenWindowsAsync(snapshotOfOpenWindows);                    
            }

            if (this.OnSearchResultUpdate != null)
            {
                this.OnSearchResultUpdate(this, new Window.WindowListUpdateEventArgs());
            }
        }

        private Task<List<WindowSearchResult>> FuzzySearchOpenWindowsAsync(List<Window> openWindows)
        {
            return Task.Run(
                () =>
                    this.FuzzySearchOpenWindows(openWindows)
                );
        }

        private List<WindowSearchResult> FuzzySearchOpenWindows(List<Window> openWindows)
        {
            List<WindowSearchResult> result = new List<WindowSearchResult>();

            string shortcut = SettingsManager.Instance.GetShortcut(this.SearchText);
            string searchString = (shortcut != null) ? shortcut : this.searchText;

            foreach(var window  in openWindows)
            {
                var titleMatch = FuzzyMatching.FindBestFuzzyMatch(window.Title, searchString);
                var processMatch = FuzzyMatching.FindBestFuzzyMatch(window.ProcessName, searchString);

                if ((titleMatch.Count != 0 || processMatch.Count != 0) &&
                         window.Title.Length != 0)
                {
                    var temp = new WindowSearchResult(window, titleMatch, processMatch);
                    result.Add(temp);
                }
            }

            System.Diagnostics.Debug.Print("Found " + result.Count + " windows that match the search text");

            return result;
        }

        #endregion
    }
}
