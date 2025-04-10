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
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FronkonGames.PaletteTool
{
  /// <summary>
  /// Color palette search tool for Unity.
  /// Provides a convenient way to browse, search, and use color palettes in your projects.
  /// </summary>
  public partial class PaletteTool : EditorWindow
  {
    /// <summary>
    /// Represents a color palette with five colors, name, favorites rating, and labels.
    /// Implements IComparable to allow sorting by favorites.
    /// </summary>
    public class Palette : IComparable<Palette>
    {
      /// <summary>Array of hex color codes in the palette.</summary>
      public string[] colors;

      /// <summary>Favorites rating (0-5) for sorting and display.</summary>
      public int favorites;

      /// <summary>Category labels for filtering and organization.</summary>
      public string[] labels;

      /// <summary>Display name of the palette.</summary>
      public string name;

      /// <summary>
      /// Compares palettes by their favorites rating for sorting.
      /// Higher-rated palettes appear first in sorted lists.
      /// </summary>
      /// <param name="other">The palette to compare with</param>
      /// <returns>Comparison result for sorting</returns>
      public int CompareTo(Palette other) => other.favorites.CompareTo(favorites);
    }

    /// <summary>Complete list of available palettes.</summary>
    internal List<Palette> palettes;

    /// <summary>Unique category labels across all palettes.</summary>
    internal List<string> labels;

    /// <summary>Cached method info for repainting inspector windows.</summary>
    private static MethodInfo repaintInspectors = null;

    /// <summary>
    /// Opens the Palette Tool window and loads available palettes.
    /// Searches for a palettes.csv file in the Assets directory and parses it.
    /// </summary>
    [MenuItem("Help/Fronkon Games/Tools/Palette Tool")]
    public static void ShowTool()
    {
      List<Palette> palettes = null;

      try
      {
        // Search for palette CSV files in the project
        string[] files = Directory.GetFiles("Assets/", "*.csv", SearchOption.AllDirectories);
        foreach (string file in files)
        {
          if (Path.GetFileName(file) == "palettes.csv")
          {
            string text = File.ReadAllText(file);
            palettes = ParseCSV(text);
            break;
          }
        }

        if (palettes != null && palettes.Count > 0)
        {
          // Sort palettes by favorites rating
          palettes.Sort();

          // Create and configure the tool window
          PaletteTool paletteTool = (PaletteTool)GetWindow(typeof(PaletteTool), false, "Palette Tool");
          paletteTool.palettes = new List<Palette>(palettes);
          paletteTool.filtered = new List<Palette>(palettes);
          paletteTool.minSize = new Vector2(800.0f, 600.0f);

          // Extract unique labels from all palettes for filtering
          paletteTool.labels = new();
          for (int i = 0; i < palettes.Count; ++i)
          {
            for (int j = 0; j < palettes[i].labels.Length; ++j)
            {
              if (paletteTool.labels.Contains(palettes[i].labels[j]) == false)
                paletteTool.labels.Add(palettes[i].labels[j]);
            }
          }
        }
        else
          Debug.LogWarning("File 'palettes.csv' incorrect or not found.");
      }
      catch (Exception ex)
      {
        var frame = new System.Diagnostics.StackTrace(ex, true).GetFrame(0);
        Debug.LogError($"Exception loading 'palettes.csv': {ex.Message} {frame.GetFileLineNumber()}:{frame.GetFileColumnNumber()}");
      }
    }

    /// <summary>
    /// Creates a texture filled with a single color.
    /// Used for rendering color swatches in the UI.
    /// </summary>
    /// <param name="width">Width of the texture in pixels</param>
    /// <param name="height">Height of the texture in pixels</param>
    /// <param name="col">Color to fill the texture with</param>
    /// <returns>A new texture filled with the specified color</returns>
    private Texture2D MakeTexture(int width, int height, Color col)
    {
      Color[] pix = new Color[width * height];

      // Fill the pixel array with the specified color
      for (int i = 0; i < pix.Length; ++i)
        pix[i] = col;

      // Create and configure the texture
      Texture2D result = new(width, height, TextureFormat.RGB24, false);
      result.SetPixels(pix);
      result.Apply();

      return result;
    }

    /// <summary>
    /// Forces all inspector windows to repaint.
    /// Uses reflection to access Unity's internal RepaintAllInspectors method.
    /// </summary>
    private void RepaintInspectors()
    {
      if (repaintInspectors == null)
      {
        // Find the InspectorWindow type and its RepaintAllInspectors method via reflection
        var inspectorWindow = typeof(EditorApplication).Assembly.GetType("UnityEditor.InspectorWindow");
        repaintInspectors = inspectorWindow.GetMethod("RepaintAllInspectors", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
      }

      // Invoke the method to repaint all inspector windows
      repaintInspectors.Invoke(null, null);
    }
  }
}
