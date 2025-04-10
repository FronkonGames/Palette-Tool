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
using System.Collections.Generic;
using UnityEditor;

namespace FronkonGames.PaletteTool
{
  /// <summary> Color palette search tool. </summary>
  public partial class PaletteTool : EditorWindow
  {
    /// <summary>
    /// Parses a CSV file containing palette data and converts it to a list of Palette objects.
    /// 
    /// Expected CSV format:
    /// Header row, followed by data rows with 9 columns:
    /// ID, Name, Color1, Color2, Color3, Color4, Color5, Favorites, Labels
    /// 
    /// Labels are pipe-separated (|) values.
    /// </summary>
    /// <param name="text">The CSV text content to parse</param>
    /// <returns>A list of Palette objects created from the CSV data</returns>
    private static List<Palette> ParseCSV(string text)
    {
      List<Palette> palettes = new();

      // Split the CSV into lines
      string[] lines = text.Split('\n');

      // Skip the header row (index 0) and process each data row
      for (int i = 1; i < lines.Length; ++i)
      {
        // Skip empty lines and comment lines (starting with #)
        if (string.IsNullOrEmpty(lines[i]) == true || lines[i].StartsWith('#') == true)
          continue;

        string line = lines[i].Trim();

        // Split the line into columns
        string[] parts = line.Trim().Split(',');

        // Validate that we have the expected number of columns
        if (parts.Length != 9)
          continue;

        // Create a new Palette object from the CSV data
        palettes.Add(new Palette
        {
          name = parts[1].Replace("\"", "").Trim(),  // Remove quotes and trim whitespace from name
          colors = new[]
          {
          parts[2].Trim(),  // Color 1 (hex code)
          parts[3].Trim(),  // Color 2 (hex code)
          parts[4].Trim(),  // Color 3 (hex code)
          parts[5].Trim(),  // Color 4 (hex code)
          parts[6].Trim()   // Color 5 (hex code)
        },
          favorites = int.Parse(parts[7].Trim()),  // Favorites rating (0-5)
          labels = parts[8].Trim().Split('|')      // Split labels by pipe character
        });
      }

      //Debug.Log($"Parsed {palettes.Count} palettes.");
      return palettes;
    }
  }
}
