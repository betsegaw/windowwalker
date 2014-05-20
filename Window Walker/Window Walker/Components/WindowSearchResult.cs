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
        /// The list of indexes of the matching characters for the search in the 
        /// name of the process
        /// </summary>
        public List<int> SearchMatchesInProcessName
        {
            get;
            private set;
        }

        /// <summary>
        /// A score indicating how well this matches what we are looking for
        /// </summary>
        public int Score
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public WindowSearchResult(Window window, List<int> matchesInTitle, List<int> matchesInProcessName)
        {
            this.ResultWindow = window;
            this.SearchMatchesInTitle = matchesInTitle;
            this.SearchMatchesInProcessName = matchesInProcessName;
            this.CalculateScore();
        }

        /// <summary>
        /// Calculates the score for how closely this window matches the search string
        /// </summary>
        /// <remarks>
        /// Higher Score is better
        /// </remarks>
        private void CalculateScore()
        {
            this.Score =
                FuzzyMatching.CalculateScoreForMatches(this.SearchMatchesInProcessName) >
                FuzzyMatching.CalculateScoreForMatches(this.SearchMatchesInTitle) ?
                FuzzyMatching.CalculateScoreForMatches(this.SearchMatchesInProcessName) :
                FuzzyMatching.CalculateScoreForMatches(this.SearchMatchesInTitle);

        }
    }
}
