using System.Collections.Generic;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance { get; private set; }

    [SerializeField] private int maxEntries = 10;

    private List<int> highScores = new List<int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void AddScore(int score)
    {
        highScores.Add(score);
        highScores.Sort((a, b) => b.CompareTo(a));

        if (highScores.Count > maxEntries)
        {
            highScores.RemoveAt(highScores.Count - 1);
        }

        if (highScores.Contains(score))
        {
            int rank = highScores.IndexOf(score) + 1;
            Debug.Log($"Diem {score} cua ban xep hang {rank}");
        }
    }
}
