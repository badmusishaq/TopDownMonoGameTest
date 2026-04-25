using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace RandomMonoGameTest;

/// <summary>
/// The main game class that orchestrates all game components.
/// Handles initialization, updating, and drawing of all game elements.
/// </summary>
public class Game1 : Game
{
    /// <summary>
    /// The graphics device manager for managing graphics settings.
    /// </summary>
    private GraphicsDeviceManager _graphics;

    /// <summary>
    /// The SpriteBatch used for drawing 2D graphics.
    /// </summary>
    private SpriteBatch _spriteBatch;

    /// <summary>
    /// A 1x1 white pixel texture used for drawing simple shapes.
    /// </summary>
    private Texture2D _pixelTexture;

    /// <summary>
    /// The dynamic text renderer used for UI text drawing.
    /// </summary>
    private TextRenderer _textRenderer;

    /// <summary>
    /// The player object that handles player movement and actions.
    /// </summary>
    private Player _player;

    /// <summary>
    /// The list of active bullets in the game.
    /// </summary>
    private List<Bullet> _bullets;

    /// <summary>
    /// The list of active enemies in the game.
    /// </summary>
    private List<Enemy> _enemies;

    /// <summary>
    /// The game manager that handles game state, scoring, and win/lose conditions.
    /// </summary>
    private GameManager _gameManager;

    /// <summary>
    /// The sound manager that handles all audio playback.
    /// </summary>
    private SoundManager _soundManager;

    /// <summary>
    /// The UI manager that handles drawing user interface elements.
    /// </summary>
    private GameUI _gameUI;

    /// <summary>
    /// The time of the last bullet fired, used for rate limiting.
    /// </summary>
    private TimeSpan _lastBulletTime;

    /// <summary>
    /// The cooldown time between bullet shots.
    /// </summary>
    private TimeSpan _bulletCooldown = TimeSpan.FromMilliseconds(200);

    /// <summary>
    /// The time of the last enemy spawn, used for controlling spawn rate.
    /// </summary>
    private TimeSpan _lastEnemySpawnTime;

    /// <summary>
    /// The cooldown time between enemy spawns.
    /// </summary>
    private TimeSpan _enemySpawnCooldown = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Initializes a new instance of the Game1 class.
    /// Sets up the graphics device and content root directory.
    /// </summary>
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    /// <summary>
    /// Initializes game components and sets up initial game state.
    /// Called once when the game starts.
    /// </summary>
    protected override void Initialize()
    {
        // Initialize collections
        _bullets = new List<Bullet>();
        _enemies = new List<Enemy>();

        // Initialize managers
        _gameManager = GameManager.Instance;
        _soundManager = SoundManager.Instance;

        // Subscribe to game state changes
        _gameManager.OnGameStateChanged += OnGameStateChanged;

        // Start the game
        _gameManager.StartGame();

        base.Initialize();
    }

    /// <summary>
    /// Loads all game content including textures, fonts, and sounds.
    /// Called once after Initialize.
    /// </summary>
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Create a 1x1 white pixel texture for drawing shapes
        _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
        _pixelTexture.SetData(new[] { Color.White });

        // Initialize the bitmap-based text renderer for cross-platform UI text.
        _textRenderer = new TextRenderer(GraphicsDevice);

        // Load sounds from the Content/Sounds folder.
        _soundManager.LoadContent(Content.RootDirectory);

        // Initialize player
        Vector2 playerStartPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight - 50);
        _player = new Player(_pixelTexture, playerStartPosition);

        // Initialize UI
        _gameUI = new GameUI(_spriteBatch, _textRenderer, _graphics);
    }

    /// <summary>
    /// Updates the game state each frame.
    /// Handles input, updates all game objects, and checks for collisions.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
        // Handle exit input
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Update game manager
        _gameManager.Update(gameTime);

        // Only update game objects if playing
        if (_gameManager.IsPlaying())
        {
            // Handle restart input
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                RestartGame();
                return;
            }

            // Update player
            _player.Update(Keyboard.GetState(), gameTime, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));

            // Handle shooting
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && gameTime.TotalGameTime - _lastBulletTime > _bulletCooldown)
            {
                ShootBullet(gameTime);
            }

            // Spawn enemies
            if (gameTime.TotalGameTime - _lastEnemySpawnTime > _enemySpawnCooldown)
            {
                SpawnEnemy(gameTime);
            }

            // Update bullets
            for (int i = _bullets.Count - 1; i >= 0; i--)
            {
                _bullets[i].Update(gameTime, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
                if (!_bullets[i].IsActive)
                {
                    _bullets.RemoveAt(i);
                }
            }

            // Update enemies
            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                _enemies[i].Update(gameTime, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
                if (!_enemies[i].IsActive)
                {
                    _enemies.RemoveAt(i);
                }
            }

            // Check collisions
            CheckCollisions();
        }
        else
        {
            // Handle restart input on end screen
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                RestartGame();
            }
        }

        base.Update(gameTime);
    }

    /// <summary>
    /// Draws all game elements on the screen.
    /// Called each frame after Update.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        // Only draw game objects if playing
        if (_gameManager.IsPlaying())
        {
            // Draw player
            _player.Draw(_spriteBatch);

            // Draw bullets
            foreach (var bullet in _bullets)
            {
                bullet.Draw(_spriteBatch);
            }

            // Draw enemies
            foreach (var enemy in _enemies)
            {
                enemy.Draw(_spriteBatch);
            }
        }

        // Draw UI (always draw UI)
        _gameUI.Draw(_gameManager);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    /// <summary>
    /// Creates and shoots a new bullet from the player's position.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    private void ShootBullet(GameTime gameTime)
    {
        Vector2 bulletPosition = new Vector2(_player.Position.X + _player.Size.X / 2 - 2.5f, _player.Position.Y);
        Bullet newBullet = new Bullet(_pixelTexture, bulletPosition);
        _bullets.Add(newBullet);
        _lastBulletTime = gameTime.TotalGameTime;

        // Play shoot sound
        _soundManager.PlayShootSound();
    }

    /// <summary>
    /// Creates and spawns a new enemy at a random horizontal position.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    private void SpawnEnemy(GameTime gameTime)
    {
        int randomX = Random.Shared.Next(0, _graphics.PreferredBackBufferWidth - 20);
        Vector2 enemyPosition = new Vector2(randomX, 0);
        Enemy newEnemy = new Enemy(_pixelTexture, enemyPosition);
        _enemies.Add(newEnemy);
        _lastEnemySpawnTime = gameTime.TotalGameTime;
    }

    /// <summary>
    /// Checks for collisions between bullets and enemies, and between player and enemies.
    /// </summary>
    private void CheckCollisions()
    {
        // Check bullet-enemy collisions
        for (int i = _bullets.Count - 1; i >= 0; i--)
        {
            for (int j = _enemies.Count - 1; j >= 0; j--)
            {
                if (_bullets[i].IsActive && _enemies[j].IsActive &&
                    _bullets[i].GetBounds().Intersects(_enemies[j].GetBounds()))
                {
                    // Destroy bullet and enemy
                    _bullets[i].IsActive = false;
                    _enemies[j].IsActive = false;

                    // Update score
                    _gameManager.EnemyKilled();

                    // Play sound
                    _soundManager.PlayEnemyDestroyedSound();

                    break;
                }
            }
        }

        // Check player-enemy collisions
        for (int i = _enemies.Count - 1; i >= 0; i--)
        {
            if (_enemies[i].IsActive && _player.GetBounds().Intersects(_enemies[i].GetBounds()))
            {
                // Game over
                _gameManager.SetGameState(GameManager.GameState.Lost);
                _soundManager.PlayGameOverSound();
                break;
            }
        }
    }

    /// <summary>
    /// Restarts the game by resetting all game objects and state.
    /// </summary>
    private void RestartGame()
    {
        // Clear all game objects
        _bullets.Clear();
        _enemies.Clear();

        // Reset player position
        _player.Position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight - 50);

        // Reset game manager
        _gameManager.StartGame();

        // Reset timers
        _lastBulletTime = TimeSpan.Zero;
        _lastEnemySpawnTime = TimeSpan.Zero;
    }

    /// <summary>
    /// Handles game state changes from the GameManager.
    /// </summary>
    /// <param name="newState">The new game state.</param>
    private void OnGameStateChanged(GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.Won)
        {
            _soundManager.PlayVictorySound();
        }
        else if (newState == GameManager.GameState.Lost)
        {
            _soundManager.PlayGameOverSound();
        }
    }
}
