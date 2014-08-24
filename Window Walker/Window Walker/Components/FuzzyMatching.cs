using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowWalker.Components
{
    /// <summary>
    /// Class housing fuzzy matching methods
    /// </summary>
    public class FuzzyMatching
    {
        /// <summary>
        /// Finds the best match (the one with the most
        /// number of letters adjecent to each other) and 
        /// returns the index location of each of the letters
        /// of the matches
        /// </summary>
        /// <param name="text">The text to search inside of</param>
        /// <param name="searchText">the text to search for</param>
        /// <returns>The best fuzzy match</returns>
        public static List<int> FindBestFuzzyMatch(string text, string searchText, int searchStartIndex = 0)
        {
            List<int> bestResult = null;
            int bestResultScore = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == searchText[0])
                {
                    // The charcter at index i matches the first character in the search string
                    List<int> currentResult = new List<int> { i };

                    // Loop through trying to find all of the rest of the characters in the search string
                    int currentNeedleIndex = 1;
                    for (int j = (i + 1); j < searchText.Length && currentNeedleIndex < searchText.Length; j++)
                    {
                        if (text[j] == searchText[currentNeedleIndex])
                        {
                            // We found the character. Add the index and then increment it so we start looking
                            // for the next character in the search string
                            currentResult.Add(j);
                            currentNeedleIndex++;
                        }
                    }

                    if (currentNeedleIndex == searchText.Length)
                    {
                        // We got through the entire string
                        int currentResultScore = CalculateScoreForMatches(currentResult);

                        if (bestResult == null || currentResultScore < bestResultScore)
                        {
                            // This result is better than the best match we have so far
                            bestResult = currentResult;
                            bestResultScore = currentResultScore;
                        }
                    }
                }
            }

            return (bestResult != null) ? bestResult : new List<int>();
        }

        /// <summary>
        /// Calculates the score for a string
        /// </summary>
        /// <param name="matches">the index of the matches</param>
        /// <returns>an integer representing the score</returns>
        public static int CalculateScoreForMatches(List<int> matches)
        {
            // Note: The matches are in ascending order to the score is simply the index of
            // the last letter minus the index of the first letter.
            return (matches.Count > 1) ? (matches[matches.Count -1] - matches[0]) : -10000;
        }
    }

    #region Tests

    [TestClass]
    public class FuzzyMatchingUnitTest
    {
        [TestMethod]
        public void SimpleMatching()
        {
            List<int> result = FuzzyMatching.FindBestFuzzyMatch("watsapp hellow", "hello");
            List<int> expected = new List<int>() { 8, 9, 10, 11, 12 };

            Assert.IsTrue(FuzzyMatchingUnitTest.IsEqual(expected, result));
        }

        [TestMethod]
        public void NoResult()
        {
            List<int> result = FuzzyMatching.FindBestFuzzyMatch("what is going on?", "whatsx goin on?");
            List<int> expected = new List<int>();

            Assert.IsTrue(FuzzyMatchingUnitTest.IsEqual(expected, result));
        }

        [TestMethod]
        public void ZeroLengthSearchString()
        {
            List<int> result = FuzzyMatching.FindBestFuzzyMatch("whatsapp hellow", "");
            List<int> expected = new List<int>();

            Assert.IsTrue(FuzzyMatchingUnitTest.IsEqual(expected, result));
        }

        [TestMethod]
        public void ZeroLengthText()
        {
            List<int> result = FuzzyMatching.FindBestFuzzyMatch("", "hello");
            List<int> expected = new List<int>();

            Assert.IsTrue(FuzzyMatchingUnitTest.IsEqual(expected, result));
        }

        [TestMethod]
        public void ZeroLengthInputs()
        {
            List<int> result = FuzzyMatching.FindBestFuzzyMatch("", "");
            List<int> expected = new List<int>();

            Assert.IsTrue(FuzzyMatchingUnitTest.IsEqual(expected, result));
        }

        [TestMethod]
        public void BestMatch()
        {
            List<int> result = FuzzyMatching.FindBestFuzzyMatch("aaacaab", "ab");
            List<int> expected = new List<int>() { 5, 6 };

            Assert.IsTrue(FuzzyMatchingUnitTest.IsEqual(expected, result));
        }

        [TestMethod]
        public void RealWorldProgramManager()
        {
            List<int> result = FuzzyMatching.FindBestFuzzyMatch("Program Manager", "pr");
            List<int> expected = new List<int>() { 0, 1 };

            Assert.IsTrue(FuzzyMatchingUnitTest.IsEqual(expected, result));
        }

        [TestMethod]
        public void BestScoreTest()
        {
            int score = FuzzyMatching.CalculateScoreForMatches(new List<int>(){1, 2, 3, 4});
            Assert.IsTrue(score == -3);
        }

        private static bool IsEqual(List<int> list1, List<int> list2)
        {
            if (list1.Count != list2.Count)
            {
                return false;
            }

            for (int i = 0; i < list1.Count; i++ )
            {
                if (list1[i] != list2[i])
                {
                    return false;
                }
            }

            return true;
        }
    }

#endregion
}
