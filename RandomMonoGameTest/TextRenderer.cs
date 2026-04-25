using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace RandomMonoGameTest;

/// <summary>
/// Renders text using a built-in bitmap font atlas.
/// This avoids System.Drawing and is cross-platform.
/// </summary>
public class TextRenderer
{
    private readonly Texture2D _fontTexture;
    private readonly Dictionary<char, Rectangle> _glyphMap;
    private const int GlyphWidth = 5;
    private const int GlyphHeight = 7;
    private const int GlyphSpacing = 1;
    private const int LineSpacing = 2;
    private const int Scale = 2;

    /// <summary>
    /// Initializes a new instance of the TextRenderer class.
    /// </summary>
    /// <param name="graphicsDevice">The graphics device used to create the font atlas.</param>
    public TextRenderer(GraphicsDevice graphicsDevice)
    {
        Dictionary<char, string[]> glyphDefinitions = GetGlyphDefinitions();
        _glyphMap = new Dictionary<char, Rectangle>(glyphDefinitions.Count);

        int atlasWidth = glyphDefinitions.Count * (GlyphWidth + GlyphSpacing);
        int atlasHeight = GlyphHeight;
        _fontTexture = new Texture2D(graphicsDevice, atlasWidth, atlasHeight);

        Color[] pixels = new Color[atlasWidth * atlasHeight];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.Transparent;
        }

        int index = 0;
        foreach (KeyValuePair<char, string[]> glyph in glyphDefinitions)
        {
            int xOffset = index * (GlyphWidth + GlyphSpacing);
            for (int y = 0; y < GlyphHeight; y++)
            {
                string row = glyph.Value[y];
                for (int x = 0; x < GlyphWidth; x++)
                {
                    if (row[x] == '1')
                    {
                        pixels[y * atlasWidth + xOffset + x] = Color.White;
                    }
                }
            }

            _glyphMap[glyph.Key] = new Rectangle(xOffset, 0, GlyphWidth, GlyphHeight);
            index++;
        }

        _fontTexture.SetData(pixels);
    }

    /// <summary>
    /// Draws a string to the screen using the SpriteBatch.
    /// </summary>
    /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="position">The screen position.</param>
    /// <param name="color">The text color.</param>
    public void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
    {
        float x = position.X;
        float y = position.Y;

        foreach (char ch in text)
        {
            if (ch == '\r')
                continue;

            if (ch == '\n')
            {
                x = position.X;
                y += (GlyphHeight + LineSpacing) * Scale;
                continue;
            }

            if (ch == ' ')
            {
                x += (GlyphWidth + GlyphSpacing) * Scale;
                continue;
            }

            char upper = char.ToUpperInvariant(ch);
            if (_glyphMap.TryGetValue(upper, out Rectangle sourceRect))
            {
                Rectangle destination = new(
                    (int)x,
                    (int)y,
                    sourceRect.Width * Scale,
                    sourceRect.Height * Scale);

                spriteBatch.Draw(_fontTexture, destination, sourceRect, color);
                x += (sourceRect.Width + GlyphSpacing) * Scale;
            }
            else
            {
                x += (GlyphWidth + GlyphSpacing) * Scale;
            }
        }
    }

    /// <summary>
    /// Measures the width of the rendered text using the built-in font.
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <returns>The width in pixels.</returns>
    public float MeasureTextWidth(string text)
    {
        float width = 0f;
        float currentLineWidth = 0f;

        foreach (char ch in text)
        {
            if (ch == '\r')
                continue;

            if (ch == '\n')
            {
                width = Math.Max(width, currentLineWidth);
                currentLineWidth = 0f;
                continue;
            }

            if (ch == ' ')
            {
                currentLineWidth += (GlyphWidth + GlyphSpacing) * Scale;
                continue;
            }

            char upper = char.ToUpperInvariant(ch);
            if (_glyphMap.TryGetValue(upper, out Rectangle sourceRect))
            {
                currentLineWidth += (sourceRect.Width + GlyphSpacing) * Scale;
            }
            else
            {
                currentLineWidth += (GlyphWidth + GlyphSpacing) * Scale;
            }
        }

        return Math.Max(width, currentLineWidth);
    }

    private static Dictionary<char, string[]> GetGlyphDefinitions()
    {
        return new Dictionary<char, string[]>
        {
            ['A'] = new[]
            {
                "01110",
                "10001",
                "10001",
                "11111",
                "10001",
                "10001",
                "10001"
            },
            ['B'] = new[]
            {
                "11110",
                "10001",
                "10001",
                "11110",
                "10001",
                "10001",
                "11110"
            },
            ['C'] = new[]
            {
                "01110",
                "10001",
                "10000",
                "10000",
                "10000",
                "10001",
                "01110"
            },
            ['D'] = new[]
            {
                "11100",
                "10010",
                "10001",
                "10001",
                "10001",
                "10010",
                "11100"
            },
            ['E'] = new[]
            {
                "11111",
                "10000",
                "10000",
                "11110",
                "10000",
                "10000",
                "11111"
            },
            ['F'] = new[]
            {
                "11111",
                "10000",
                "10000",
                "11110",
                "10000",
                "10000",
                "10000"
            },
            ['G'] = new[]
            {
                "01110",
                "10001",
                "10000",
                "10111",
                "10001",
                "10001",
                "01110"
            },
            ['H'] = new[]
            {
                "10001",
                "10001",
                "10001",
                "11111",
                "10001",
                "10001",
                "10001"
            },
            ['I'] = new[]
            {
                "01110",
                "00100",
                "00100",
                "00100",
                "00100",
                "00100",
                "01110"
            },
            ['J'] = new[]
            {
                "00111",
                "00010",
                "00010",
                "00010",
                "10010",
                "10010",
                "01100"
            },
            ['K'] = new[]
            {
                "10001",
                "10010",
                "10100",
                "11000",
                "10100",
                "10010",
                "10001"
            },
            ['L'] = new[]
            {
                "10000",
                "10000",
                "10000",
                "10000",
                "10000",
                "10000",
                "11111"
            },
            ['M'] = new[]
            {
                "10001",
                "11011",
                "10101",
                "10101",
                "10001",
                "10001",
                "10001"
            },
            ['N'] = new[]
            {
                "10001",
                "11001",
                "10101",
                "10011",
                "10001",
                "10001",
                "10001"
            },
            ['O'] = new[]
            {
                "01110",
                "10001",
                "10001",
                "10001",
                "10001",
                "10001",
                "01110"
            },
            ['P'] = new[]
            {
                "11110",
                "10001",
                "10001",
                "11110",
                "10000",
                "10000",
                "10000"
            },
            ['Q'] = new[]
            {
                "01110",
                "10001",
                "10001",
                "10001",
                "10101",
                "10010",
                "01101"
            },
            ['R'] = new[]
            {
                "11110",
                "10001",
                "10001",
                "11110",
                "10100",
                "10010",
                "10001"
            },
            ['S'] = new[]
            {
                "01111",
                "10000",
                "10000",
                "01110",
                "00001",
                "00001",
                "11110"
            },
            ['T'] = new[]
            {
                "11111",
                "00100",
                "00100",
                "00100",
                "00100",
                "00100",
                "00100"
            },
            ['U'] = new[]
            {
                "10001",
                "10001",
                "10001",
                "10001",
                "10001",
                "10001",
                "01110"
            },
            ['V'] = new[]
            {
                "10001",
                "10001",
                "10001",
                "10001",
                "10001",
                "01010",
                "00100"
            },
            ['W'] = new[]
            {
                "10001",
                "10001",
                "10001",
                "10101",
                "10101",
                "11011",
                "10001"
            },
            ['X'] = new[]
            {
                "10001",
                "10001",
                "01010",
                "00100",
                "01010",
                "10001",
                "10001"
            },
            ['Y'] = new[]
            {
                "10001",
                "10001",
                "01010",
                "00100",
                "00100",
                "00100",
                "00100"
            },
            ['Z'] = new[]
            {
                "11111",
                "00001",
                "00010",
                "00100",
                "01000",
                "10000",
                "11111"
            },
            ['0'] = new[]
            {
                "01110",
                "10001",
                "10011",
                "10101",
                "11001",
                "10001",
                "01110"
            },
            ['1'] = new[]
            {
                "00100",
                "01100",
                "00100",
                "00100",
                "00100",
                "00100",
                "01110"
            },
            ['2'] = new[]
            {
                "01110",
                "10001",
                "00001",
                "00010",
                "00100",
                "01000",
                "11111"
            },
            ['3'] = new[]
            {
                "01110",
                "10001",
                "00001",
                "00110",
                "00001",
                "10001",
                "01110"
            },
            ['4'] = new[]
            {
                "00010",
                "00110",
                "01010",
                "10010",
                "11111",
                "00010",
                "00010"
            },
            ['5'] = new[]
            {
                "11111",
                "10000",
                "10000",
                "11110",
                "00001",
                "10001",
                "01110"
            },
            ['6'] = new[]
            {
                "01110",
                "10000",
                "10000",
                "11110",
                "10001",
                "10001",
                "01110"
            },
            ['7'] = new[]
            {
                "11111",
                "00001",
                "00010",
                "00100",
                "01000",
                "01000",
                "01000"
            },
            ['8'] = new[]
            {
                "01110",
                "10001",
                "10001",
                "01110",
                "10001",
                "10001",
                "01110"
            },
            ['9'] = new[]
            {
                "01110",
                "10001",
                "10001",
                "01111",
                "00001",
                "00001",
                "01110"
            },
            ['!'] = new[]
            {
                "00100",
                "00100",
                "00100",
                "00100",
                "00100",
                "00000",
                "00100"
            },
            [':'] = new[]
            {
                "00000",
                "00100",
                "00000",
                "00000",
                "00100",
                "00000",
                "00000"
            },
            ['/'] = new[]
            {
                "00001",
                "00010",
                "00100",
                "01000",
                "10000",
                "00000",
                "00000"
            }
        };
    }
}
