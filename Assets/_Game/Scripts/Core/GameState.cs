using System;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Init,
    Playing,
    Paused,
    Victory
}

public interface IGameState
{
    GameState StateType { get; }
    void Enter();
    void Update();
    void Exit();
}

public class GameStateMachine
{
    private readonly Dictionary<GameState, IGameState> states = new Dictionary<GameState, IGameState>();
    public IGameState CurrentState { get; private set; }
    public GameState CurrentStateType => CurrentState != null ? CurrentState.StateType : GameState.Init;

    public event Action<GameState, GameState> OnStateChanged;

    public void RegisterState(IGameState state)
    {
        states[state.StateType] = state;
    }

    public void Initialize(GameState startingState)
    {
        if (states.TryGetValue(startingState, out IGameState state))
        {
            CurrentState = state;
            CurrentState.Enter();
        }
        else
        {
            Debug.LogWarning($"[GameStateMachine] Initial state {startingState} not registered!");
        }
    }

    public void ChangeState(GameState newState)
    {
        if (CurrentState != null && CurrentState.StateType == newState)
        {
            return;
        }

        if (states.TryGetValue(newState, out IGameState nextState))
        {
            GameState oldState = CurrentStateType;
            CurrentState?.Exit();
            CurrentState = nextState;
            CurrentState.Enter();
            OnStateChanged?.Invoke(oldState, newState);
        }
        else
        {
            Debug.LogWarning($"[GameStateMachine] State {newState} is not registered!");
        }
    }

    public void Update()
    {
        CurrentState?.Update();
    }
}

public class InitState : IGameState
{
    private readonly GameManager gameManager;
    public GameState StateType => GameState.Init;

    public InitState(GameManager gm) { gameManager = gm; }

    public void Enter()
    {
        Time.timeScale = 1f;
    }

    public void Update() { }
    public void Exit() { }
}

public class PlayingState : IGameState
{
    private readonly GameManager gameManager;
    public GameState StateType => GameState.Playing;

    public PlayingState(GameManager gm) { gameManager = gm; }

    public void Enter()
    {
        Time.timeScale = 1f;
    }

    public void Update()
    {
        gameManager.TickLevelTime(Time.deltaTime);
    }

    public void Exit() { }
}

public class PausedState : IGameState
{
    private readonly GameManager gameManager;
    public GameState StateType => GameState.Paused;

    public PausedState(GameManager gm) { gameManager = gm; }

    public void Enter()
    {
        Time.timeScale = 0f;
    }

    public void Update() { }

    public void Exit()
    {
        Time.timeScale = 1f;
    }
}

public class VictoryState : IGameState
{
    private readonly GameManager gameManager;
    public GameState StateType => GameState.Victory;

    public VictoryState(GameManager gm) { gameManager = gm; }

    public void Enter()
    {
        Time.timeScale = 1f;
        gameManager.ProcessVictory();
    }

    public void Update() { }
    public void Exit() { }
}

//public class GameOverState : IGameState
//{
//    private readonly GameManager gameManager;
//    public GameState StateType => GameState.GameOver;

//    public GameOverState(GameManager gm) { gameManager = gm; }

//    public void Enter()
//    {
//        Time.timeScale = 1f;
//    }

//    public void Update() { }
//    public void Exit() { }
//}
