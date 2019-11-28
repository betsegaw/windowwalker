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
    public class SearchResult
    {
        /// <summary>
        /// The actual window reference for the search result
        /// </summary>
        public Window Result
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
        /// The type of match (shortcut, fuzzy or nothing)
        /// </summary>
        public SearchType SearchResultMatchType
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
        /// The source of where the best score was found
        /// </summary>
        public TextType BestScoreSource
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SearchResult(Window window, List<int> matchesInTitle, List<int> matchesInProcessName, SearchType matchType)
        {
            this.Result = window;
            this.SearchMatchesInTitle = matchesInTitle;
            this.SearchMatchesInProcessName = matchesInProcessName;
            this.SearchResultMatchType = matchType;
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
            if (FuzzyMatching.CalculateScoreForMatches(this.SearchMatchesInProcessName) >
                FuzzyMatching.CalculateScoreForMatches(this.SearchMatchesInTitle))
            {
                this.Score = FuzzyMatching.CalculateScoreForMatches(this.SearchMatchesInProcessName);
                this.BestScoreSource = TextType.ProcessName;
            }
            else
            {
                this.Score = FuzzyMatching.CalculateScoreForMatches(this.SearchMatchesInTitle);
                this.BestScoreSource = TextType.WindowTitle;
            }
        }

        /// <summary>
        /// The type of text that a string represents
        /// </summary>
        public enum TextType
        {
            ProcessName,
            WindowTitle
        }

        /// <summary>
        /// The type of search
        /// </summary>
        public enum SearchType
        {
            /// <summary>
            /// the search string is empty, which means all open windows are
            /// going to be returned
            /// </summary>
            Empty,

            /// <summary>
            /// Regular fuzzy match search
            /// </summary>
            Fuzzy,

            /// <summary>
            /// The user has entered text that has been matched to a shortcut
            /// and the shortcut is now being searched
            /// </summary>
            Shortcut
        }
    }
}
