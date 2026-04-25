using Microsoft.Xna.Framework;
using System;

namespace RandomMonoGameTest;

/// <summary>
/// Manages the overall game state, including timer, scoring, and win/lose conditions.
/// Implements the Singleton pattern to ensure only one instance exists.
/// </summary>
public class GameManager
{
    /// <summary>
    /// The single instance of the GameManager.
    /// </summary>
    private static GameManager _instance;

    /// <summary>
    /// Gets the singleton instance of the GameManager.
    /// </summary>
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameManager();
            }
            return _instance;
        }
    }

    /// <summary>
    /// Represents the different states the game can be in.
    /// </summary>
    public enum GameState
    {
        Playing,
        Won,
        Lost
    }

    /// <summary>
    /// The current state of the game.
    /// </summary>
    public GameState CurrentState { get; private set; } = GameState.Playing;

    /// <summary>
    /// The total time allowed for the game (2 minutes).
    /// </summary>
    private readonly TimeSpan _gameDuration = TimeSpan.FromMinutes(2);

    /// <summary>
    /// The time when the game started.
    /// </summary>
    private TimeSpan _startTime;

    /// <summary>
    /// The current game time elapsed.
    /// </summary>
    public TimeSpan CurrentTime { get; private set; }

    /// <summary>
    /// The number of enemies the player must kill to win.
    /// </summary>
    private const int _requiredKills = 20;

    /// <summary>
    /// The current number of enemies killed by the player.
    /// </summary>
    public int EnemiesKilled { get; private set; } = 0;

    /// <summary>
    /// The current score of the player.
    /// </summary>
    public int Score { get; private set; } = 0;

    /// <summary>
    /// Event raised when the game state changes.
    /// </summary>
    public event Action<GameState> OnGameStateChanged;

    /// <summary>
    /// Private constructor to prevent instantiation from outside.
    /// </summary>
    private GameManager() { }

    /// <summary>
    /// Starts a new game, resetting all values.
    /// </summary>
    public void StartGame()
    {
        CurrentState = GameState.Playing;
        _startTime = TimeSpan.Zero;
        CurrentTime = TimeSpan.Zero;
        EnemiesKilled = 0;
        Score = 0;
        OnGameStateChanged?.Invoke(CurrentState);
    }

    /// <summary>
    /// Updates the game manager's state.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public void Update(GameTime gameTime)
    {
        if (CurrentState != GameState.Playing)
            return;

        CurrentTime = gameTime.TotalGameTime - _startTime;

        // Check win condition
        if (EnemiesKilled >= _requiredKills)
        {
            CurrentState = GameState.Won;
            OnGameStateChanged?.Invoke(CurrentState);
        }
        // Check lose condition
        else if (CurrentTime >= _gameDuration)
        {
            CurrentState = GameState.Lost;
            OnGameStateChanged?.Invoke(CurrentState);
        }
    }

    /// <summary>
    /// Increments the enemy kill count and updates the score.
    /// </summary>
    public void EnemyKilled()
    {
        EnemiesKilled++;
        Score += 10; // 10 points per enemy
    }

    /// <summary>
    /// Gets the remaining time in the game.
    /// </summary>
    /// <returns>The remaining time as a TimeSpan.</returns>
    public TimeSpan GetRemainingTime()
    {
        return _gameDuration - CurrentTime;
    }

    /// <summary>
    /// Gets the formatted time string for display.
    /// </summary>
    /// <returns>A formatted string showing minutes and seconds.</returns>
    public string GetFormattedTime()
    {
        TimeSpan time = CurrentState == GameState.Playing ? GetRemainingTime() : CurrentTime;
        return $"{(int)time.TotalMinutes}:{time.Seconds:D2}";
    }

    /// <summary>
    /// Sets the current game state.
    /// </summary>
    /// <param name="newState">The new game state to set.</param>
    public void SetGameState(GameState newState)
    {
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }

    /// <summary>
    /// Checks if the game is currently in a playing state.
    /// </summary>
    /// <returns>True if the game is playing, false otherwise.</returns>
    public bool IsPlaying()
    {
        return CurrentState == GameState.Playing;
    }
}