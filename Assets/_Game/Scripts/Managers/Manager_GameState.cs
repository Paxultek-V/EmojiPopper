using System;
using System.Collections;
using UnityEngine;

public enum GameState
{
    TitleScreen,
    Gameplay,
    Gameover,
    Victory
}

public class Manager_GameState : MonoBehaviour
{
    public static Action<GameState> OnSendCurrentGameState;
    public static Action OnNoMoreMovesPossible;
    public static Action OnForfeit;

    [SerializeField] private float m_delayBeforeLoadingNewLevelAfterWin = 2f;

    private GameState m_currentGameState;


    private void OnEnable()
    {
        Manager_Level.OnLevelStart += OnLevelStart;
        Manager_Level.OnLevelEnd += OnLevelEnd;
        Manager_Level.OnLevelRetry += OnLevelRetry;

        UI_Button_Forfeit.OnForfeitButtonPressed += OnForfeitButtonPressed;
    }

    private void OnDisable()
    {
        Manager_Level.OnLevelStart -= OnLevelStart;
        Manager_Level.OnLevelEnd -= OnLevelEnd;
        Manager_Level.OnLevelRetry -= OnLevelRetry;
        
        UI_Button_Forfeit.OnForfeitButtonPressed -= OnForfeitButtonPressed;
    }

    private void Start()
    {
        m_currentGameState = GameState.TitleScreen;
        BroadcastCurrentGameState();
    }


    private void OnForfeitButtonPressed()
    {
        OnForfeit?.Invoke();
        
        m_currentGameState = GameState.Gameover;

        BroadcastCurrentGameState();
    }

    private void OnLevelStart()
    {
        m_currentGameState = GameState.Gameplay;
        BroadcastCurrentGameState();
    }


    private void OnLevelEnd(bool isWin)
    {
        if (isWin)
        {
            m_currentGameState = GameState.Victory;
            StartCoroutine(LoadNextLevelCoroutine());
        }
        else
        {
            m_currentGameState = GameState.Gameover;
            OnNoMoreMovesPossible?.Invoke();
        }

        BroadcastCurrentGameState();
    }

    private IEnumerator LoadNextLevelCoroutine()
    {
        yield return new WaitForSeconds(m_delayBeforeLoadingNewLevelAfterWin);
        LoadNextLevel();
    }

    private void LoadNextLevel()
    {
        m_currentGameState = GameState.TitleScreen;
        BroadcastCurrentGameState();
    }

    private void OnLevelRetry()
    {
        m_currentGameState = GameState.TitleScreen;
        BroadcastCurrentGameState();
    }

    private void BroadcastCurrentGameState()
    {
        OnSendCurrentGameState?.Invoke(m_currentGameState);
    }
}