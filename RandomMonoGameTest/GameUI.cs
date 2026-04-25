using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace RandomMonoGameTest;

/// <summary>
/// Handles all user interface elements, including the end game screen.
/// </summary>
public class GameUI
{
    /// <summary>
    /// The SpriteBatch used for drawing UI elements.
    /// </summary>
    private SpriteBatch _spriteBatch;

    /// <summary>
    /// The dynamic text renderer used for drawing text to textures.
    /// </summary>
    private TextRenderer _textRenderer;

    /// <summary>
    /// The graphics device manager for screen information.
    /// </summary>
    private GraphicsDeviceManager _graphics;

    /// <summary>
    /// A texture used for drawing UI overlays.
    /// </summary>
    private Texture2D _overlayTexture;

    /// <summary>
    /// Initializes a new instance of the GameUI class.
    /// </summary>
    /// <param name="spriteBatch">The SpriteBatch for drawing.</param>
    /// <param name="textRenderer">The text renderer for UI text.</param>
    /// <param name="graphics">The graphics device manager.</param>
    public GameUI(SpriteBatch spriteBatch, TextRenderer textRenderer, GraphicsDeviceManager graphics)
    {
        _spriteBatch = spriteBatch;
        _textRenderer = textRenderer;
        _graphics = graphics;

        // Create overlay texture
        _overlayTexture = new Texture2D(_spriteBatch.GraphicsDevice, 1, 1);
        _overlayTexture.SetData(new[] { new Color(0, 0, 0, 128) });
    }

    /// <summary>
    /// Draws the game UI, including the end screen if the game is over.
    /// </summary>
    /// <param name="gameManager">The game manager containing game state information.</param>
    public void Draw(GameManager gameManager)
    {
        if (_textRenderer == null) return;

        if (gameManager.CurrentState == GameManager.GameState.Playing)
        {
            DrawPlayingUI(gameManager);
        }
        else
        {
            DrawEndScreen(gameManager);
        }
    }

    /// <summary>
    /// Draws the UI elements during gameplay.
    /// </summary>
    /// <param name="gameManager">The game manager containing current game stats.</param>
    private void DrawPlayingUI(GameManager gameManager)
    {
        string timeText = $"Time: {gameManager.GetFormattedTime()}";
        string scoreText = $"Score: {gameManager.Score}";
        string killsText = $"Enemies Killed: {gameManager.EnemiesKilled}/20";

        Vector2 timePosition = new Vector2(10, 10);
        Vector2 scorePosition = new Vector2(10, 40);
        Vector2 killsPosition = new Vector2(10, 70);

        _textRenderer.DrawText(_spriteBatch, timeText, timePosition, Color.White);
        _textRenderer.DrawText(_spriteBatch, scoreText, scorePosition, Color.White);
        _textRenderer.DrawText(_spriteBatch, killsText, killsPosition, Color.White);
    }

    /// <summary>
    /// Draws the end game screen with final statistics.
    /// </summary>
    /// <param name="gameManager">The game manager containing final game stats.</param>
    private void DrawEndScreen(GameManager gameManager)
    {
        // Semi-transparent overlay
        _spriteBatch.Draw(_overlayTexture, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);

        // End screen text
        string resultText = gameManager.CurrentState == GameManager.GameState.Won ? "VICTORY!" : "GAME OVER";
        string statsText = $"Enemies Killed: {gameManager.EnemiesKilled}\n" +
                          $"Time Played: {gameManager.GetFormattedTime()}\n" +
                          $"Final Score: {gameManager.Score}\n\n" +
                          $"Press R to Restart or ESC to Exit";

        Color resultColor = gameManager.CurrentState == GameManager.GameState.Won ? Color.Green : Color.Red;

        Vector2 resultPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2 - _textRenderer.MeasureTextWidth(resultText) / 2,
                                           _graphics.PreferredBackBufferHeight / 2 - 100);
        Vector2 statsPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2 - _textRenderer.MeasureTextWidth(statsText) / 2,
                                          _graphics.PreferredBackBufferHeight / 2 - 20);

        _textRenderer.DrawText(_spriteBatch, resultText, resultPosition, resultColor);
        _textRenderer.DrawText(_spriteBatch, statsText, statsPosition, Color.White);
    }
}