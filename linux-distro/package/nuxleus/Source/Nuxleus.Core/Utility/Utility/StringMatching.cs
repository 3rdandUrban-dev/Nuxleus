using System;
using System.Collections;
using System.Collections.Generic;

namespace Nuxleus.Utility
{

    public static class StringMatching
    {

        /// <summary>Returns true if both strings are close, the threshold is the token determining at which point both strings are considered too far away to be similar. This uses the Levenshtein distance algorithm to compute the distance between the two strings.</summary>
        /// <param name="x">First word.</param>
        /// <param name="y">Second word</param>
        /// <param name="threshold">Level above which two words should be considered not similar enough. The bigger that number is the greater chance you take to get quite unsimilar words.</param>
        /// <return>true if both words' distance is below or equals the threshold, false otherwise.</return>
        public static bool AreNeighbors(string x, string y, int threshold)
        {
            // if the length difference between the strings is greater
            // than the threshold, we don't even bother
            // This may miss some matches modulo extra spaces
            // but improve the performances in the general use case
            if (Math.Abs(x.Length - y.Length) > threshold)
            {
                return false;
            }

            // We want to test strings of equal lengths
            if (x.Length < y.Length)
            {
                x = x.PadRight(y.Length, ' ');
            }
            else if (y.Length < x.Length)
            {
                y = y.PadRight(x.Length, ' ');
            }

            int dist = ComputeEditDistance(x, y);

            // Take the case of:
            // x = "js"
            // y = "do"
            // threshold = 2
            // Then distance would be 2 but x and y are clearly different
            if ((dist == threshold) && (dist == x.Length) && (dist == y.Length))
            {
                return false;
            }

            return (dist <= threshold);
        }

        // See http://en.wikipedia.org/wiki/Levenshtein_distance
        private static int ComputeEditDistance(string w0, string w1)
        {
            if (w0.Length == 0) return w1.Length;
            if (w1.Length == 0) return w0.Length;



            int m = w0.Length;
            int n = w1.Length;
            int[,] d = new int[m + 1, n + 1];
            int cost;

            // Prepare the distance matrix
            for (int i = 0; i < m; i++)
            {
                d[i, 0] = i;
            }

            for (int j = 0; j < n; j++)
            {
                d[0, j] = j;
            }

            // Compute the distance matrix
            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    cost = 1;
                    if (w0[i - 1] == w1[j - 1])
                        cost = 0;

                    d[i, j] = System.Math.Min(System.Math.Min(d[i - 1, j] + 1,
                                          d[i, j - 1] + 1),
                                  d[i - 1, j - 1] + cost);
                }
            }

            return d[m, n];
        }
    }
}