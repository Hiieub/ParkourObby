using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Camera & Respawn")]
    [SerializeField] private CinemachineOrbitalFollow cinemachineOrbitalFollow;
    private Vector3 currentRespawnPosition;

    [Header("Level Stats")]
    public int TotalCoins { get; private set; }
    public int DeathCount { get; private set; }
    public float LevelTime { get; private set; }

    [Header("Flagpole Progression")]
    public int CurrentStage { get; private set; } = 0;
    public int TotalStages { get; private set; } = 4;
    public float ProgressRatio { get; private set; } = 0f;

    private readonly List<Checkpoint> registeredCheckpoints = new List<Checkpoint>();
    private FinishTrigger finishTrigger;
    private Checkpoint finishCheckpoint;
    private bool hasFinishCheckpoint = false;

    // State Machine
    private GameStateMachine stateMachine;
    public GameState CurrentState => stateMachine != null ? stateMachine.CurrentStateType : GameState.Init;
    public bool IsPlaying => CurrentState == GameState.Playing;
    public bool IsPaused => CurrentState == GameState.Paused;
    public bool IsVictory => CurrentState == GameState.Victory;

    // Events
    public event Action<GameState, GameState> OnGameStateChanged;
    public event Action<float, int, int> OnProgressUpdated; // (ratio, stage, totalStages)
    public event Action<float> OnTimerUpdated;              // (time)
    public event Action<int, int> OnStageChanged;           // (currentStage, totalStages)
    public event Action<int> OnCoinChanged;
    public event Action OnPlayerRespawned;
    public event Action<float, int, int> OnLevelWon;        // (time, coins, stars)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        SetupStateMachine();
    }

    private void SetupStateMachine()
    {
        stateMachine = new GameStateMachine();
        stateMachine.RegisterState(new InitState(this));
        stateMachine.RegisterState(new PlayingState(this));
        stateMachine.RegisterState(new PausedState(this));
        stateMachine.RegisterState(new VictoryState(this));
        //stateMachine.RegisterState(new GameOverState(this));

        stateMachine.OnStateChanged += (oldState, newState) =>
        {
            OnGameStateChanged?.Invoke(oldState, newState);
        };
    }

    private void Start()
    {
        TotalCoins = 0;
        DeathCount = 0;
        LevelTime = 0f;

        FindAndRegisterCheckpoints();

        stateMachine.Initialize(GameState.Init);
        stateMachine.ChangeState(GameState.Playing);
    }

    private void Update()
    {
        stateMachine?.Update();

        if (IsPlaying)
        {
            OnTimerUpdated?.Invoke(LevelTime);
        }
    }

    public void FindAndRegisterCheckpoints()
    {
        registeredCheckpoints.Clear();
        Checkpoint[] cps = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
        registeredCheckpoints.AddRange(cps);

        finishTrigger = FindAnyObjectByType<FinishTrigger>();

        registeredCheckpoints.Sort((a, b) =>
        {
            if (a.OrderIndex != b.OrderIndex)
                return a.OrderIndex.CompareTo(b.OrderIndex);
            return a.transform.position.z.CompareTo(b.transform.position.z);
        });
        
        for (int i = 0; i < registeredCheckpoints.Count; i++)
        {
            registeredCheckpoints[i].OrderIndex = i + 1;
        }
       
        hasFinishCheckpoint = false;
        finishCheckpoint = null;
        if (finishTrigger != null && registeredCheckpoints.Count > 0)
        {
            Checkpoint lastCp = registeredCheckpoints[registeredCheckpoints.Count - 1];
            if (lastCp.transform.IsChildOf(finishTrigger.transform) ||
                finishTrigger.transform.IsChildOf(lastCp.transform) ||
                Vector3.Distance(lastCp.transform.position, finishTrigger.transform.position) < 8f)
            {
                hasFinishCheckpoint = true;
                finishCheckpoint = lastCp;
            }
        }
      
        if (hasFinishCheckpoint)
        {
            TotalStages = registeredCheckpoints.Count;
        }
        else
        {
            TotalStages = registeredCheckpoints.Count > 0 ? (registeredCheckpoints.Count + 1) : 4;
        }

        CurrentStage = 0;
        ProgressRatio = 0f;

        OnStageChanged?.Invoke(CurrentStage, TotalStages);
        OnProgressUpdated?.Invoke(ProgressRatio, CurrentStage, TotalStages);
    }

    public void OnCheckpointReached(Checkpoint cp)
    {
        int flagpoleNumber = cp.OrderIndex > 0 ? cp.OrderIndex : (registeredCheckpoints.IndexOf(cp) + 1);
        if (flagpoleNumber <= 0) return;

        if (flagpoleNumber > CurrentStage)
        {
            CurrentStage = Mathf.Clamp(flagpoleNumber, 0, TotalStages);
            ProgressRatio = TotalStages > 0 ? (float)CurrentStage / TotalStages : 0f;

            OnStageChanged?.Invoke(CurrentStage, TotalStages);
            OnProgressUpdated?.Invoke(ProgressRatio, CurrentStage, TotalStages);
        }

        SetCheckpoint(cp.transform.position + Vector3.up);

        if (flagpoleNumber >= TotalStages || (finishCheckpoint != null && cp == finishCheckpoint))
        {
            OnLevelFinished();
        }
    }

    private void ChangeState(GameState newState)
    {
        stateMachine?.ChangeState(newState);
    }

    public void PauseGame()
    {
        ChangeState(GameState.Paused);
    }

    public void ResumeGame()
    {
        ChangeState(GameState.Playing);
    }

    public void TickLevelTime(float delta)
    {
        LevelTime += delta;
    }

    public void AddCoin(int amount)
    {
        TotalCoins += amount;
        OnCoinChanged?.Invoke(TotalCoins);
    }

    public void SetCheckpoint(Vector3 newPosition)
    {
        currentRespawnPosition = newPosition;
    }

    public void ReSpawnPlayer(Player player)
    {
        DeathCount++;
        OnPlayerRespawned?.Invoke();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayRespawn();
        }

        CharacterController cc = player.GetComponent<CharacterController>();

        if (cc != null)
        {
            cc.enabled = false;

            player.transform.position = currentRespawnPosition;
            player.transform.rotation = Quaternion.identity;

            if (cinemachineOrbitalFollow != null)
            {
                cinemachineOrbitalFollow.HorizontalAxis.Value = 0f;
                cinemachineOrbitalFollow.VerticalAxis.Value = 17.5f;
            }

            cc.enabled = true;
        }
        else
        {
            player.transform.position = currentRespawnPosition;
        }
    }

    public void OnLevelFinished()
    {
        if (IsVictory) return;

        CurrentStage = TotalStages;
        ProgressRatio = 1.0f;
        OnStageChanged?.Invoke(CurrentStage, TotalStages);
        OnProgressUpdated?.Invoke(1.0f, CurrentStage, TotalStages);

        if (finishCheckpoint != null)
        {
            finishCheckpoint.Activate();
        }
        else if (hasFinishCheckpoint && registeredCheckpoints.Count > 0)
        {
            registeredCheckpoints[registeredCheckpoints.Count - 1].Activate();
        }

        if (finishTrigger != null)
        {
            finishTrigger.TriggerFinish();
        }

        ChangeState(GameState.Victory);
    }

    public void ProcessVictory()
    {
        string currentLevel = DataManager.SelectedLevel;

        LevelConfigSO config = DataManager.Database != null
            ? DataManager.Database.GetLevelById(currentLevel)
            : null;

        int stars = config != null
            ? config.CalculateStars(DeathCount, LevelTime)
            : 3;

        DataManager.SetStars(currentLevel, stars);
        DataManager.AddCoins(TotalCoins);
        DataManager.TrySetBestTime(currentLevel, LevelTime);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayWin();
        }

        OnLevelWon?.Invoke(LevelTime, TotalCoins, stars);
    }
}
