using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text finalScoreText;

    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gameOverPanel;

    [SerializeField] private Button playButton;
    [SerializeField] private Button restartButton;

    public event EventHandler OnPlayClicked;
    public event EventHandler OnRestartClicked;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ShowStartPanel();

        playButton.onClick.AddListener(OnPlayButtonPressed);
        restartButton.onClick.AddListener(OnRestartButtonPressed);
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
        gameOverPanel.SetActive(false);

        scoreText.gameObject.SetActive(false);

        highScoreText.text = $"High Score: {HighScoreManager.Instance.GetHighScore()}";
        highScoreText.gameObject.SetActive(true);
    }

    public void ShowGameOverPanel(int score, int bestScore)
    {
        gameOverPanel.SetActive(true);
        scoreText.gameObject.SetActive(false);
        finalScoreText.text = $"Score: {score}\n Best Score: {bestScore}";
    }

    private void OnPlayButtonPressed()
    {
        startPanel.SetActive(false);
        scoreText.gameObject.SetActive(true);
        highScoreText.gameObject.SetActive(false);
        OnPlayClicked?.Invoke(this, new EventArgs());
    }

    private void OnRestartButtonPressed()
    {
        OnRestartClicked?.Invoke(this, new EventArgs());
    }
}
