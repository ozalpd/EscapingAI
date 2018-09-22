using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("HUD")]
    public Text healthText;
    public Text livesText;
    public Text scoreText;
    public Text highScoreText;

    [Tooltip("Use only one of the sliders damage or health")]
    public Slider damageSlider;
    private Image _imgDamageFillArea;
    public Color damageMinColor = Color.yellow;
    public Color damageMaxColor = Color.red;

    [Tooltip("Use only one of the sliders damage or health")]
    public Slider healthSlider;
    private Image _imgHealthFillArea;
    public Color healthMinColor = Color.red;
    public Color healthMaxColor = Color.green;


    [Header("Menu Items")]
    public Button pauseButton;
    public Button resumeButton;
    public Image pauseMenu;

    public Text gameStateText;

    [Header("Settings")]
    public Slider musicVolumeSlider;
    public Slider sFXVolumeSlider;


    public float MusicVolume
    {
        get { return GameSettings.MusicVolume; }
        set { GameSettings.MusicVolume = value; }
    }

    public float SfxVolume
    {
        get { return GameSettings.SfxVolume; }
        set { GameSettings.SfxVolume = value; }
    }

    private void Awake()
    {
        if (damageSlider != null)
            _imgDamageFillArea = damageSlider.fillRect.GetComponent<Image>();

        if (healthSlider != null)
            _imgHealthFillArea = healthSlider.fillRect.GetComponent<Image>();
    }

    private void Start()
    {
        if (damageSlider != null)
        {
            if (damageMaxColor == damageMinColor)
                _imgDamageFillArea.color = damageMaxColor;
            GameManager_DamageChanged(0, GameManager.maxDamage);
            GameManager.DamageChanged += GameManager_DamageChanged;
        }

        if (healthSlider != null)
        {

            if (healthMaxColor == healthMinColor)
                _imgHealthFillArea.color = healthMaxColor;
            GameManager_HealthChanged(GameManager.Health, GameManager.maxHealth);
            GameManager.HealthChanged += GameManager_HealthChanged;
        }

        if (livesText != null)
        {
            GameManager_LivesChanged(GameManager.Lives);
            GameManager.LivesChanged += GameManager_LivesChanged;
        }

        if (scoreText != null)
        {
            GameManager_ScoreChanged(GameManager.Score);
            GameManager.ScoreChanged += GameManager_ScoreChanged;
        }

        if (highScoreText != null)
        {
            GameManager_HighScoreChanged(GameManager.HighScore);
            GameManager.HighScoreChanged += GameManager_HighScoreChanged;
        }


        //updateUI(GameManager.GameState);
        GameManager.GameStateChanged += updateUI;
        GameManager.GameState = GameState.Running;

        if (musicVolumeSlider != null)
            musicVolumeSlider.value = MusicVolume;

        if (sFXVolumeSlider != null)
            sFXVolumeSlider.value = SfxVolume;
    }

    private void GameManager_HealthChanged(float health, float maxHealth)
    {
        healthSlider.value = health / maxHealth;
        if (healthMaxColor != healthMinColor)
            _imgHealthFillArea.color = Color.Lerp(healthMinColor, healthMaxColor, health / maxHealth);

        if (healthText != null)
            healthText.text = health.ToString("00");
    }

    //Its better way to unsubscribe events inside from the GameManager
    //void OnDestroy()
    //{
    //    GameManager.DamageChanged -= GameManager_DamageChanged;
    //    GameManager.LivesChanged -= GameManager_LivesChanged;
    //    GameManager.ScoreChanged -= GameManager_ScoreChanged;
    //    GameManager.HighScoreChanged -= GameManager_HighScoreChanged;
    //    GameManager.GameStateChanged -= updateUI;
    //}

    private void GameManager_DamageChanged(float damage, float maxDamage)
    {
        damageSlider.value = damage / maxDamage;
        if (damageMaxColor != damageMinColor)
            _imgDamageFillArea.color = Color.Lerp(damageMinColor, damageMaxColor, damage / maxDamage);
    }

    private void GameManager_LivesChanged(int lives)
    {
        livesText.text = string.Format("{0} {1}", GameManager.Lives, GameManager.Lives > 1 ? "LIVES" : "LIFE");
    }

    private void GameManager_ScoreChanged(int score)
    {
        scoreText.text = string.Format("SCORE: {0}", GameManager.Score);
    }

    private void GameManager_HighScoreChanged(int score)
    {
        highScoreText.text = string.Format("HIGH SCORE: {0}", GameManager.HighScore);
    }

    public void PauseGame()
    {
        GameManager.GameState = GameState.Paused;
    }

    public void RestartLevel()
    {
        //GameManager.RestartLevel();
        StartCoroutine(GameManager.RestartLevelAsync());
    }

    public void ResumeGame()
    {
        GameManager.GameState = GameState.Running;
    }

    private void updateUI(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Over:
                if (gameStateText != null)
                    gameStateText.text = "GAME OVER";
                break;

            case GameState.Paused:
                if (gameStateText != null)
                    gameStateText.text = "GAME PAUSED";
                break;

            case GameState.Running:
                break;

            default:
                break;
        }

        //TODO: Do sth for GameState.NotStarted

        if (pauseButton != null)
            pauseButton.gameObject.SetActive(gameState == GameState.Running);
        if (resumeButton != null)
            resumeButton.gameObject.SetActive(gameState == GameState.Paused);
        if (pauseMenu != null)
            pauseMenu.gameObject.SetActive(gameState != GameState.Running);
    }

}