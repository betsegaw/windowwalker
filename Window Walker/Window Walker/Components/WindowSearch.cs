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
                searchText = value.ToLower();
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
                    this.searchMatches.Add(new WindowSearchResult(window, new List<int>()));
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

            foreach(var window  in openWindows)
            {
                if ((WindowSearchController.IsFuzzyMatch(this.searchText.ToLower(), window.Title.ToLower()) != null || WindowSearchController.IsFuzzyMatch(this.searchText.ToLower(), window.ProcessName.ToLower()) != null) &&
                         window.Title.Length != 0)
                {
                    var temp = new WindowSearchResult(window, WindowSearchController.IsFuzzyMatch(this.searchText.ToLower(), window.Title.ToLower()));
                    result.Add(temp);
                }
            }

            return result;
        }

        #endregion

        #region Static Methods

        private static List<int> IsFuzzyMatch(string searchText, string text)
        {
            int searchStartIndex = 0;
            int letterIndex;
            List<int> matchIndexes = new List<int>();

            foreach (char letter in searchText)
            {
                if (searchStartIndex >= text.Length)
                {
                    return null;
                }
                else
                {
                    letterIndex = text.IndexOf(letter, searchStartIndex);

                    if (letterIndex != -1)
                    {
                        searchStartIndex = letterIndex + 1;
                        matchIndexes.Add(letterIndex);
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return matchIndexes;
        }

        #endregion
    }
}
