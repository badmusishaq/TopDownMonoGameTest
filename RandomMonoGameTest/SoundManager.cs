using Microsoft.Xna.Framework.Audio;
using System;
using System.IO;

namespace RandomMonoGameTest;

/// <summary>
/// Manages all sound effects in the game.
/// Implements the Singleton pattern to ensure only one instance exists.
/// </summary>
public class SoundManager
{
    /// <summary>
    /// The single instance of the SoundManager.
    /// </summary>
    private static SoundManager _instance;

    /// <summary>
    /// Gets the singleton instance of the SoundManager.
    /// </summary>
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SoundManager();
            }
            return _instance;
        }
    }

    /// <summary>
    /// Sound effect for shooting bullets.
    /// </summary>
    private SoundEffect _shootSound;

    /// <summary>
    /// Sound effect for enemy destruction.
    /// </summary>
    private SoundEffect _enemyDestroyedSound;

    /// <summary>
    /// Sound effect for game over (loss).
    /// </summary>
    private SoundEffect _gameOverSound;

    /// <summary>
    /// Sound effect for game victory.
    /// </summary>
    private SoundEffect _victorySound;

    /// <summary>
    /// Indicates whether sounds are enabled.
    /// </summary>
    public bool SoundEnabled { get; set; } = true;

    /// <summary>
    /// Private constructor to prevent instantiation from outside.
    /// </summary>
    private SoundManager() { }

    /// <summary>
    /// Loads all sound effects from the raw Content/Sounds folder.
    /// </summary>
    /// <param name="contentRoot">The root content folder name, typically "Content".</param>
    public void LoadContent(string contentRoot)
    {
        string contentDirectory = Path.Combine(AppContext.BaseDirectory, contentRoot, "Sounds");

        _shootSound = LoadSound(Path.Combine(contentDirectory, "shoot.wav"));
        _enemyDestroyedSound = LoadSound(Path.Combine(contentDirectory, "enemy_destroyed.wav"));
        _gameOverSound = LoadSound(Path.Combine(contentDirectory, "game_over.wav"));
        _victorySound = LoadSound(Path.Combine(contentDirectory, "victory.wav"));
    }

    /// <summary>
    /// Loads a sound effect from a WAV file.
    /// </summary>
    /// <param name="path">The file system path to the WAV file.</param>
    /// <returns>The loaded SoundEffect, or null if the file was missing or invalid.</returns>
    private SoundEffect LoadSound(string path)
    {
        if (!File.Exists(path))
            return null;

        using FileStream stream = File.OpenRead(path);
        return SoundEffect.FromStream(stream);
    }

    /// <summary>
    /// Plays the shooting sound effect.
    /// </summary>
    public void PlayShootSound()
    {
        if (SoundEnabled && _shootSound != null)
        {
            _shootSound.Play();
        }
    }

    /// <summary>
    /// Plays the enemy destruction sound effect.
    /// </summary>
    public void PlayEnemyDestroyedSound()
    {
        if (SoundEnabled && _enemyDestroyedSound != null)
        {
            _enemyDestroyedSound.Play();
        }
    }

    /// <summary>
    /// Plays the game over sound effect.
    /// </summary>
    public void PlayGameOverSound()
    {
        if (SoundEnabled && _gameOverSound != null)
        {
            _gameOverSound.Play();
        }
    }

    /// <summary>
    /// Plays the victory sound effect.
    /// </summary>
    public void PlayVictorySound()
    {
        if (SoundEnabled && _victorySound != null)
        {
            _victorySound.Play();
        }
    }
}