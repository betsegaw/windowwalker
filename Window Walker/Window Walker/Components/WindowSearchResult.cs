using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowWalker.Components
{
    /// <summary>
    /// Contains search result windows with each window including the reason why the result was included
    /// </summary>
    public class WindowSearchResult
    {
        /// <summary>
        /// The actual window reference for the search result
        /// </summary>
        public Window ResultWindow
        {
            get;
            private set;
        }

        /// <summary>
        /// The list of indexes of the matching characters for the search in the title window
        /// </summary>
        public List<int> SearchMatchesInTitle
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public WindowSearchResult(Window window, List<int> matches)
        {
            this.ResultWindow = window;
            this.SearchMatchesInTitle = matches;
        }
    }
}
