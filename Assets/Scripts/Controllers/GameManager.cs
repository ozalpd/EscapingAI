using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    NotStarted = 0,
    Running = 1,
    Paused = 2,
    Over = 3
}

/// <summary>
/// Static game controller class not complicated enough to be called as 'Manager'
/// </summary>
/// <remarks>
/// Do not use SceneManager.LoadScene directly, instead use GameController's LoadScene methods
/// </remarks>
public static class GameManager
{
    public const int startingLives = 1;
    public const float maxDamage = 100;
    public const float maxHealth = 100;

    public delegate void GameStateChange(GameState gameState);

    public delegate void DamageChange(float damage, float maxDamage);
    public delegate void HealthChange(float health, float maxHealth);
    public delegate void LivesChange(int lives);
    public delegate void ScoreChange(int score);

    public delegate void PlayerMove(AbstractPlayerController player, Vector3 movement);

    public static GameState GameState
    {
        get { return _state; }
        set
        {
            if (_state != value)
            {
                _state = value;
                if (GameStateChanged != null)
                    GameStateChanged(_state);

                switch (_state)
                {
                    case GameState.NotStarted:
                        Debug.LogWarning("This is default state and does not intended to set from another GameState!");
                        break;

                    case GameState.Running:
                        //TODO:Set UserControls enabled
                        Time.timeScale = 1;
                        break;

                    case GameState.Paused:
                        //TODO:Set UserControls disabled
                        Time.timeScale = 0;
                        break;

                    case GameState.Over:
                        //TODO:Set UserControls disabled
                        Time.timeScale = 0;
                        break;
                }
            }
        }
    }
    private static GameState _state;
    public static event GameStateChange GameStateChanged;


    public static float Damage
    {
        get { return _damage; }
        set
        {
            if (!Mathf.Approximately(_damage, value))
            {
                _damage = value > 0 ? value : 0f;

                if (maxDamage <= _damage)
                {
                    Lives--;
                    if (GameState != GameState.Over)
                    {
                        _damage = 0;
                        Health = maxHealth;
                    }
                }

                if (DamageChanged != null)
                    DamageChanged(_damage, maxDamage);
            }
        }
    }
    private static float _damage;
    public static event DamageChange DamageChanged;


    public static float Health
    {
        get
        {
            if (_health == null)
                _health = maxHealth;
            return _health.Value;
        }
        set
        {
            if (!Mathf.Approximately(_health.Value, value))
            {
                _health = value < maxHealth ? value : maxHealth;

                if (_health <= 0)
                {
                    Lives--;
                    if (GameState != GameState.Over)
                    {
                        _health = maxHealth;
                        Damage = 0;
                    }
                }

                if (HealthChanged != null)
                    HealthChanged(_health.Value, maxHealth);
            }
        }
    }
    private static float? _health;
    public static event HealthChange HealthChanged;

    public static int Lives
    {
        get
        {
            if (_lives == null)
                _lives = startingLives;
            return _lives.Value;
        }
        set
        {
            if (_lives != value)
            {
                _lives = value;

                if (LivesChanged != null)
                    LivesChanged(_lives.Value);

                if (_lives <= 0)
                    GameState = GameState.Over;
            }
        }
    }
    private static int? _lives;
    public static event LivesChange LivesChanged;


    public static int Score
    {
        get { return _score; }
        set
        {
            if (_score != value)
            {
                _score = value;
                HighScore = _score;
                if (ScoreChanged != null)
                    ScoreChanged(_score);
            }
        }
    }
    private static int _score;
    public static event ScoreChange ScoreChanged;


    public static int HighScore
    {
        get
        {
            if (_highScore == null)
            {
                _highScore = PlayerPrefs.GetInt("HighScore", 0);
            }
            return _highScore.Value;
        }
        set
        {
            if (value > (_highScore ?? 0))
            {
                _highScore = value;
                PlayerPrefs.SetInt("HighScore", _highScore.Value);

                if (HighScoreChanged != null)
                    HighScoreChanged(_highScore.Value);
            }
        }
    }
    private static int? _highScore;
    public static event ScoreChange HighScoreChanged;


    /// <summary>
    /// Loads the Scene by its index in Build Settings or by its name.
    /// </summary>
    /// <param name="sceneBuildIndex">Index of the Scene in the Build Settings to load.</param>
    public static void LoadScene(int sceneBuildIndex)
    {
        ReleaseSubscribers();
        ResetMetrics();
        SceneManager.LoadScene(sceneBuildIndex);
    }

    /// <summary>
    /// Loads the Scene by its index in Build Settings or by its name.
    /// </summary>
    /// <param name="sceneName">Name or path of the Scene to load.</param>
    public static void LoadScene(string sceneName)
    {
        ReleaseSubscribers();
        ResetMetrics();
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Loads the Scene asynchronously in the background by its index in Build Settings or by its name.
    /// </summary>
    /// <param name="sceneBuildIndex">Index of the Scene in the Build Settings to load.</param>
    /// <returns></returns>
    public static IEnumerator LoadSceneAsync(int sceneBuildIndex)
    {
        ReleaseSubscribers();
        ResetMetrics();
        var asyncState = SceneManager.LoadSceneAsync(sceneBuildIndex);
        while (!asyncState.isDone)
        {
            yield return null;
        }
    }

    /// <summary>
    /// Loads the Scene asynchronously in the background by its index in Build Settings or by its name.
    /// </summary>
    /// <param name="sceneName">Name or path of the Scene to load.</param>
    /// <returns></returns>
    public static IEnumerator LoadSceneAsync(string sceneName)
    {
        ReleaseSubscribers();
        ResetMetrics();
        var asyncState = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncState.isDone)
        {
            yield return null;
        }
    }

    public static void PlayerMoving(AbstractPlayerController player, Vector3 movement)
    {
        if (OnPlayerMoving != null)
            OnPlayerMoving(player, movement);
    }
    public static event PlayerMove OnPlayerMoving;
    //public static event PlayerMove OnPlayerMoved;//

    public static void PlayerStopped(AbstractPlayerController player)
    {
        if (OnPlayerStopped != null)
            OnPlayerStopped(player, Vector3.zero);
    }
    public static event PlayerMove OnPlayerStopped;


    public static void RestartLevel()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        LoadScene(sceneIndex);
    }

    public static IEnumerator RestartLevelAsync()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        var asyncState = SceneManager.LoadSceneAsync(sceneIndex);

        ReleaseSubscribers();
        ResetMetrics();
        while (!asyncState.isDone)
        {
            yield return null;
        }
    }

    private static void ReleaseSubscribers()
    {
        GameStateChanged = null;
        DamageChanged = null;
        HealthChanged = null;
        HighScoreChanged = null;
        LivesChanged = null;
        OnPlayerMoving = null;
        ScoreChanged = null;
    }

    private static void ResetMetrics()
    {
        _damage = 0;
        _health = null;
        _lives = null;
        _score = 0;
        _state = GameState.NotStarted;
    }

    private static void RestartLevelByObjectPool()
    {
        ObjectPool.ClearAllPools();

        //TODO: Find objects which should be reset

        var savedTransforms = Object.FindObjectsOfType<SaveTransform>();
        foreach (var t in savedTransforms)
        {
            if (t.IsSaved)
                t.RestoreTransform();
        }

        ResetMetrics();
    }
}
