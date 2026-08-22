using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private float increaseSpeedInterval = 5f;
    [SerializeField] private float increaseSpeedAmount = 0.5f;

    private int score = 0;
    private float timer;
    private bool isGameOver = false;

    private Player player;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        player = FindAnyObjectByType<Player>();

        timer = increaseSpeedInterval;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.R)) Restart();
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            player.IncreaseSpeed(increaseSpeedAmount);

            timer = increaseSpeedInterval;
        }
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;
        score += amount;
        Debug.Log($"Score: {score}");
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        Debug.Log($"GAME OVER! Final Score: {score}");
        Time.timeScale = 0f;
    }

    public void OnPlayerDied()
    {
        GameOver();

        HighScoreManager.Instance.AddScore(score);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
