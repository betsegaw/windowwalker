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
            List<int> matchIndexes = new List<int>();

            searchText = searchText.ToLower();
            text = text.ToLower();

            // Create a grid to march matches like
            // eg.
            //   a b c a d e c f g
            // a x     x
            // c     x       x
            bool[,] matches = new bool[text.Length, searchText.Length];
            for (int firstIndex = 0; firstIndex < text.Length; firstIndex++ )
            {
                for(int secondIndex = 0; secondIndex < searchText.Length; secondIndex++)
                {
                    matches[firstIndex, secondIndex] =
                        searchText[secondIndex] == text[firstIndex] ?
                        true :
                        false;
                }
            }

            // use this table to get all the possible matches
            List<List<int>> allMatches = FuzzyMatching.GetAllMatchIndexes(matches);

            // return the score that is the max 
            int maxScore = allMatches.Count > 0 ? FuzzyMatching.CalculateScoreForMatches(allMatches[0]) : 0 ;
            List<int> bestMatch = allMatches.Count > 0 ? allMatches[0] : new List<int>();

            foreach(var match in allMatches)
            {
                int score = FuzzyMatching.CalculateScoreForMatches(match);
                if (score > maxScore)
                {
                    bestMatch = match;
                    maxScore = score;
                }
            }

            return bestMatch;
        }

        public static List<List<int>> GetAllMatchIndexes(bool[,] matches)
        {
            List<List<int>> results = new List<List<int>>();

            for (int secondIndex = 0; secondIndex < matches.GetLength(1); secondIndex++)
            {
                int p = 4;
                for (int firstIndex = 0; firstIndex < matches.GetLength(0); firstIndex++)
                {
                    if (secondIndex == 0 && matches[firstIndex,secondIndex])
                    {
                        results.Add(new List<int> { firstIndex });
                    }
                    else if (matches[firstIndex, secondIndex])
                    {
                        var tempList = results.Where(x => x.Count == secondIndex && x[x.Count-1] < firstIndex).Select(x => x.ToList()).ToList();
                        
                        foreach(var pathSofar  in tempList)
                        {
                            pathSofar.Add(firstIndex);
                        }

                        results.AddRange(tempList);
                    }
                }

                results.Where(x => x.Count == secondIndex + 1);
            }

            return results.Where(x => x.Count == matches.GetLength(1)).ToList();
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

        [TestMethod]
        public void RealWorldProgramManager()
        {
            List<int> result = FuzzyMatching.FindBestFuzzyMatch("Program Manager", "pr");
            List<int> expected = new List<int>() { 0, 1 };

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
