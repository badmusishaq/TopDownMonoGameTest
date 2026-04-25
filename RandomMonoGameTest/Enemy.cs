using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomMonoGameTest;

/// <summary>
/// Represents an enemy in the game.
/// Handles enemy movement and collision detection.
/// </summary>
public class Enemy
{
    /// <summary>
    /// The current position of the enemy on the screen.
    /// </summary>
    public Vector2 Position { get; private set; }

    /// <summary>
    /// The speed at which the enemy moves downward (pixels per second).
    /// </summary>
    public float Speed { get; set; } = 100f;

    /// <summary>
    /// The size of the enemy (width and height in pixels).
    /// </summary>
    public Vector2 Size { get; set; } = new Vector2(20, 20);

    /// <summary>
    /// The color of the enemy.
    /// </summary>
    public Color Color { get; set; } = Color.Red;

    /// <summary>
    /// Indicates whether the enemy is still active (not destroyed or off-screen).
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// The texture used to draw the enemy.
    /// </summary>
    private Texture2D _texture;

    /// <summary>
    /// Initializes a new instance of the Enemy class.
    /// </summary>
    /// <param name="texture">The texture to use for drawing the enemy.</param>
    /// <param name="startPosition">The initial position of the enemy.</param>
    public Enemy(Texture2D texture, Vector2 startPosition)
    {
        _texture = texture;
        Position = startPosition;
    }

    /// <summary>
    /// Updates the enemy's position and checks if it's still on screen.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    /// <param name="screenBounds">The bounds of the game screen.</param>
    public void Update(GameTime gameTime, Rectangle screenBounds)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Move enemy downward
        Position = new Vector2(Position.X, Position.Y + Speed * deltaTime);

        // Deactivate if off-screen
        if (Position.Y > screenBounds.Height)
        {
            IsActive = false;
        }
    }

    /// <summary>
    /// Draws the enemy on the screen.
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
    /// Gets the bounding rectangle of the enemy for collision detection.
    /// </summary>
    /// <returns>The bounding rectangle of the enemy.</returns>
    public Rectangle GetBounds()
    {
        return new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
    }
}