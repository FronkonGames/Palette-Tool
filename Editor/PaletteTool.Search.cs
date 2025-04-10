////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Martin Bustos @FronkonGames <fronkongames@gmail.com>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of
// the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using UnityEditor;

namespace FronkonGames.PaletteTool
{
  /// <summary> Color palette search tool. </summary>
  public partial class PaletteTool : EditorWindow
  {
    /// <summary>Previous search text used to avoid redundant searches.</summary>
    private string previousSearchText, searchText;

    /// <summary>Current page index in the paginated results.</summary>
    private int pageCurrent = 0;

    /// <summary>Total number of pages based on filtered results and palettes per page.</summary>
    private int pageCount = 0;

    /// <summary>List of palettes that match the current search criteria.</summary>
    private List<Palette> filtered = new();

    /// <summary>Number of palettes to display per page.</summary>
    private const int PalettesPerPage = 5;

    /// <summary>Maximum Levenshtein distance allowed for fuzzy matching.</summary>
    private const int MaxDistance = 1;

    /// <summary>
    /// Searches through all palettes to find matches based on the current search text.
    /// Uses fuzzy matching with Levenshtein distance to find approximate matches.
    /// Searches through palette names and labels.
    /// </summary>
    private void SearchPalettes()
    {
      // Reset to first page when performing a new search
      pageCurrent = 0;

      // Only perform search if the search text has changed
      if (previousSearchText != searchText)
      {
        filtered.Clear();

        // Store matches with their distance score for sorting
        List<(int, Palette)> matches = new();

        // Split search text into individual terms for multi-term searching
        string[] searches = searchText.ToLower().Split(' ', ',', '.', '-');
        for (int i = 0; i < searches.Length; ++i)
        {
          for (int j = 0; j < palettes.Count; ++j)
          {
            bool match = false;

            // Search through palette labels
            for (int k = 0; k < palettes[j].labels.Length && match == false; ++k)
            {
              // Calculate Levenshtein distance between search term and label
              int distance = LevenshteinDistance(searches[i], palettes[j].labels[k].ToLower());
              if (distance <= MaxDistance)
              {
                // Add palette to matches if not already included
                if (matches.Contains((distance, palettes[j])) == false)
                  matches.Add((distance, palettes[j]));
                match = true;
              }
            }

            // Search through words in palette name
            string[] words = palettes[j].name.ToLower().Split(' ', ',', '.', '-');
            for (int k = 0; k < words.Length && match == false; ++k)
            {
              // Calculate Levenshtein distance between search term and word
              int distance = LevenshteinDistance(searches[i], words[k]);
              if (distance <= MaxDistance)
              {
                // Add palette to matches if not already included
                if (matches.Contains((distance, palettes[j])) == false)
                  matches.Add((distance, palettes[j]));
                match = true;
              }
            }
          }
        }

        // Sort matches by distance (closest matches first)
        matches.Sort((a, b) => a.Item1.CompareTo(b.Item1));

        // Add sorted matches to filtered results
        foreach (var match in matches)
          filtered.Add(match.Item2);
      }

      // Update previous search text to avoid redundant searches
      previousSearchText = searchText;
    }

    /// <summary>
    /// Calculates the Levenshtein distance between two strings.
    /// The Levenshtein distance is the minimum number of single-character edits
    /// (insertions, deletions, or substitutions) required to change one string into another.
    /// Used for fuzzy string matching in the search functionality.
    /// </summary>
    /// <param name="s">First string to compare</param>
    /// <param name="t">Second string to compare</param>
    /// <returns>The Levenshtein distance between the two strings</returns>
    private static int LevenshteinDistance(string s, string t)
    {
      // Handle edge cases for empty strings
      if (string.IsNullOrEmpty(s) == true)
      {
        if (string.IsNullOrEmpty(t) == true)
          return 0;

        return t.Length;
      }

      if (string.IsNullOrEmpty(t) == true)
        return s.Length;

      // Initialize variables for the algorithm
      int n = s.Length;
      int m = t.Length;
      int[,] d = new int[n + 1, m + 1];

      // Initialize first row and column of the distance matrix
      for (int i = 0; i <= n; d[i, 0] = i++) ;
      for (int j = 1; j <= m; d[0, j] = j++) ;

      // Fill the distance matrix using dynamic programming
      for (int i = 1; i <= n; i++)
      {
        for (int j = 1; j <= m; j++)
        {
          // Calculate cost of substitution (0 if characters match, 1 otherwise)
          int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

          // Calculate minimum edit distance considering deletion, insertion, and substitution
          int min1 = d[i - 1, j] + 1;      // Deletion
          int min2 = d[i, j - 1] + 1;      // Insertion
          int min3 = d[i - 1, j - 1] + cost; // Substitution

          // Choose the minimum of the three operations
          d[i, j] = Math.Min(Math.Min(min1, min2), min3);
        }
      }

      // Return the final distance
      return d[n, m];
    }
  }
}
