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
        /// <returns></returns>
        public static List<int> FindBestFuzzyMatch(string text, string searchText, int searchStartIndex = 0)
        {
            int letterIndex;
            List<int> matchIndexes = new List<int>();

            text = text.ToLower();
            searchText = searchText.ToLower();

            foreach (char letter in searchText)
            {
                if (searchStartIndex >= text.Length)
                {
                    return new List<int>();
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
                        return new List<int>();
                    }
                }
            }

            return matchIndexes;
        }

        /// <summary>
        /// Calculates the score for a string
        /// </summary>
        /// <param name="matches">the index of the matches</param>
        /// <returns>an integer representing the score</returns>
        public static int CalculateScoreForMatches(List<int> matches)
        {
            var score = 0;

            for (int currentIndex = 1; currentIndex < matches.Count; currentIndex++)
            {
                var previousIndex = currentIndex - 1;

                score -= (matches[currentIndex] - matches[previousIndex]) *
                    (matches[currentIndex] - matches[previousIndex]);
            }

            return score;
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
