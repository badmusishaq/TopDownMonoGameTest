using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace RandomMonoGameTest;

/// <summary>
/// Represents the player character in the game.
/// Handles player movement, shooting, and collision detection.
/// </summary>
public class Player
{
    /// <summary>
    /// The current position of the player on the screen.
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    /// The speed at which the player moves (pixels per second).
    /// </summary>
    public float Speed { get; set; } = 200f;

    /// <summary>
    /// The size of the player sprite (width and height in pixels).
    /// </summary>
    public Vector2 Size { get; set; } = new Vector2(20, 20);

    /// <summary>
    /// The color of the player sprite.
    /// </summary>
    public Color Color { get; set; } = Color.Green;

    /// <summary>
    /// The texture used to draw the player.
    /// </summary>
    private Texture2D _texture;

    /// <summary>
    /// Initializes a new instance of the Player class.
    /// </summary>
    /// <param name="texture">The texture to use for drawing the player.</param>
    /// <param name="startPosition">The initial position of the player.</param>
    public Player(Texture2D texture, Vector2 startPosition)
    {
        _texture = texture;
        Position = startPosition;
    }

    /// <summary>
    /// Updates the player's position based on keyboard input.
    /// </summary>
    /// <param name="keyboardState">The current state of the keyboard.</param>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    /// <param name="screenBounds">The bounds of the game screen.</param>
    public void Update(KeyboardState keyboardState, GameTime gameTime, Rectangle screenBounds)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Vector2 newPosition = Position;

        // Handle movement input
        if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            newPosition.X -= Speed * deltaTime;
        if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            newPosition.X += Speed * deltaTime;
        if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            newPosition.Y -= Speed * deltaTime;
        if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            newPosition.Y += Speed * deltaTime;

        // Keep player within screen bounds
        newPosition.X = MathHelper.Clamp(newPosition.X, 0, screenBounds.Width - Size.X);
        newPosition.Y = MathHelper.Clamp(newPosition.Y, 0, screenBounds.Height - Size.Y);

        Position = newPosition;
    }

    /// <summary>
    /// Draws the player on the screen.
    /// </summary>
    /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), Color);
    }

    /// <summary>
    /// Gets the bounding rectangle of the player for collision detection.
    /// </summary>
    /// <returns>The bounding rectangle of the player.</returns>
    public Rectangle GetBounds()
    {
        return new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
    }
}