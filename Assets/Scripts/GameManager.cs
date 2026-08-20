using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int score = 0;
    private bool isGameOver = false;

    private void Awake()
    {
        Instance = this;
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log(score);
    }

    public void GameOver()
    {

    }

    public void Restart()
    {

    }
}
