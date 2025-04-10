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
using UnityEditor;
using UnityEngine;

namespace FronkonGames.PaletteTool
{
  /// <summary> Color palette search tool. </summary>
  public partial class PaletteTool : EditorWindow
  {
    /// <summary> Converts an HTML color string to a Unity Color struct </summary>
    /// <param name="htmlColor">HTML color string (e.g. "#FF00FF" or "FF00FF")</param>
    /// <returns>Unity Color struct</returns>
    public static Color ParseHtmlColor(string htmlColor)
    {
      var (r, g, b) = ParseHtmlString(htmlColor);

      return new Color(r / 255.0f, g / 255.0f, b / 255.0f);
    }

    /// <summary> Converts an HTML color string to an RGB integer tuple (0-255) </summary>
    /// <param name="htmlColor">HTML color string</param>
    /// <returns>(R, G, B) tuple with integer values</returns>
    public static (int R, int G, int B) HtmlToRgbInt(string htmlColor)
    {
      var (r, g, b) = ParseHtmlString(htmlColor);
      return (r, g, b);
    }

    /// <summary>
    /// Converts an HTML color string to a normalized float tuple (0.0f-1.0f)
    /// </summary>
    /// <param name="htmlColor">HTML color string</param>
    /// <returns>(R, G, B) tuple with float values</returns>
    public static (float R, float G, float B) HtmlToRgbFloat(string htmlColor)
    {
      var (r, g, b) = ParseHtmlString(htmlColor);
      return (r / 255.0f, g / 255.0f, b / 255.0f);
    }

    private static (byte R, byte G, byte B) ParseHtmlString(string htmlColor)
    {
      if (string.IsNullOrEmpty(htmlColor))
        throw new FormatException("Invalid HTML color string");

      string hex = htmlColor.TrimStart('#');

      if (hex.Length == 3)
        hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}";
      else if (hex.Length != 6)
        throw new FormatException("Invalid HTML color format. Must be 3 or 6 characters long after #");

      try
      {
        byte r = Convert.ToByte(hex.Substring(0, 2), 16);
        byte g = Convert.ToByte(hex.Substring(2, 2), 16);
        byte b = Convert.ToByte(hex.Substring(4, 2), 16);

        return (r, g, b);
      }
      catch (FormatException)
      {
        throw new FormatException("Invalid hexadecimal value in color string");
      }
    }
  }
}
