using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomMonoGameTest;

/// <summary>
/// Represents a bullet projectile in the game.
/// Handles bullet movement and lifetime.
/// </summary>
public class Bullet
{
    /// <summary>
    /// The current position of the bullet on the screen.
    /// </summary>
    public Vector2 Position { get; private set; }

    /// <summary>
    /// The speed at which the bullet moves (pixels per second).
    /// </summary>
    public float Speed { get; set; } = 400f;

    /// <summary>
    /// The size of the bullet (width and height in pixels).
    /// </summary>
    public Vector2 Size { get; set; } = new Vector2(5, 10);

    /// <summary>
    /// The color of the bullet.
    /// </summary>
    public Color Color { get; set; } = Color.Yellow;

    /// <summary>
    /// Indicates whether the bullet is still active (not off-screen).
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// The texture used to draw the bullet.
    /// </summary>
    private Texture2D _texture;

    /// <summary>
    /// Initializes a new instance of the Bullet class.
    /// </summary>
    /// <param name="texture">The texture to use for drawing the bullet.</param>
    /// <param name="startPosition">The initial position of the bullet.</param>
    public Bullet(Texture2D texture, Vector2 startPosition)
    {
        _texture = texture;
        Position = startPosition;
    }

    /// <summary>
    /// Updates the bullet's position and checks if it's still on screen.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    /// <param name="screenBounds">The bounds of the game screen.</param>
    public void Update(GameTime gameTime, Rectangle screenBounds)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Move bullet upward
        Position = new Vector2(Position.X, Position.Y - Speed * deltaTime);

        // Deactivate if off-screen
        if (Position.Y < -Size.Y)
        {
            IsActive = false;
        }
    }

    /// <summary>
    /// Draws the bullet on the screen.
    /// </summary>
    /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        if (IsActive)
        {
            spriteBatch.Draw(_texture, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), Color);
        }
    }

    /// <summary>
    /// Resets the bullet for reuse by the object pool.
    /// </summary>
    /// <param name="startPosition">The new starting position for the bullet.</param>
    public void Reset(Vector2 startPosition)
    {
        Position = startPosition;
        IsActive = true;
    }

    /// <summary>
    /// Gets the bounding rectangle of the bullet for collision detection.
    /// </summary>
    /// <returns>The bounding rectangle of the bullet.</returns>
    public Rectangle GetBounds()
    {
        return new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
    }
}