using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using SDColor = System.Drawing.Color;

namespace RandomMonoGameTest;

/// <summary>
/// Creates texture-based text output from a system font.
/// This allows drawing text without requiring a SpriteFont asset.
/// </summary>
public class TextRenderer : IDisposable
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly Font _font;
    private readonly Dictionary<string, Texture2D> _textureCache = new();

    /// <summary>
    /// Initializes a new instance of the TextRenderer class.
    /// </summary>
    /// <param name="graphicsDevice">The graphics device used to create textures.</param>
    /// <param name="fontFamily">The system font family to use for text rendering.</param>
    /// <param name="fontSize">The font size in pixels.</param>
    public TextRenderer(GraphicsDevice graphicsDevice, string fontFamily = "Arial", float fontSize = 16f)
    {
        _graphicsDevice = graphicsDevice;
        _font = new Font(fontFamily, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
    }

    /// <summary>
    /// Draws a string to the screen using the SpriteBatch.
    /// </summary>
    /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="position">The screen position.</param>
    /// <param name="color">The text color.</param>
    public void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, Microsoft.Xna.Framework.Color color)
    {
        Texture2D texture = GetTextTexture(text, color);
        spriteBatch.Draw(texture, position, Microsoft.Xna.Framework.Color.White);
    }

    /// <summary>
    /// Measures the width of the rendered text using the current font.
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <returns>The width in pixels.</returns>
    public float MeasureTextWidth(string text)
    {
        using Bitmap sizeBitmap = new(1, 1);
        using Graphics sizeGraphics = Graphics.FromImage(sizeBitmap);
        sizeGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        SizeF size = sizeGraphics.MeasureString(text, _font);
        return size.Width;
    }

    /// <summary>
    /// Gets a cached texture for a text string and color combination.
    /// </summary>
    /// <param name="text">The text to render.</param>
    /// <param name="color">The text color.</param>
    /// <returns>A Texture2D containing the rendered text.</returns>
    private Texture2D GetTextTexture(string text, Microsoft.Xna.Framework.Color color)
    {
        string cacheKey = $"{text}|{color.PackedValue}";
        if (_textureCache.TryGetValue(cacheKey, out Texture2D cachedTexture))
        {
            return cachedTexture;
        }

        Texture2D newTexture = CreateTextTexture(text, color);
        _textureCache[cacheKey] = newTexture;
        return newTexture;
    }

    /// <summary>
    /// Creates a new Texture2D containing the rendered text.
    /// </summary>
    private Texture2D CreateTextTexture(string text, Microsoft.Xna.Framework.Color color)
    {
        if (string.IsNullOrEmpty(text))
        {
            Texture2D empty = new Texture2D(_graphicsDevice, 1, 1);
            empty.SetData(new[] { Microsoft.Xna.Framework.Color.Transparent });
            return empty;
        }

        using Bitmap measurementBitmap = new(1, 1);
        using Graphics measurementGraphics = Graphics.FromImage(measurementBitmap);
        measurementGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        SizeF stringSize = measurementGraphics.MeasureString(text, _font);

        int width = Math.Max(1, (int)Math.Ceiling(stringSize.Width));
        int height = Math.Max(1, (int)Math.Ceiling(stringSize.Height));

        using Bitmap renderBitmap = new(width, height, PixelFormat.Format32bppArgb);
        using Graphics renderGraphics = Graphics.FromImage(renderBitmap);
        renderGraphics.Clear(System.Drawing.Color.Transparent);
        renderGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        using SolidBrush brush = new(System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B));
        renderGraphics.DrawString(text, _font, brush, new PointF(0, 0));

        Microsoft.Xna.Framework.Color[] pixels = new Microsoft.Xna.Framework.Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                System.Drawing.Color pixel = renderBitmap.GetPixel(x, y);
                pixels[y * width + x] = new Microsoft.Xna.Framework.Color(pixel.R, pixel.G, pixel.B, pixel.A);
            }
        }

        Texture2D texture = new(_graphicsDevice, width, height, false, SurfaceFormat.Color);
        texture.SetData(pixels);
        return texture;
    }

    /// <summary>
    /// Releases all textures used by the text renderer.
    /// </summary>
    public void Dispose()
    {
        foreach (Texture2D texture in _textureCache.Values)
        {
            texture.Dispose();
        }

        _textureCache.Clear();
        _font.Dispose();
    }
}
