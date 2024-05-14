using System;
using UnityEngine;

public class Manager_Level : MonoBehaviour
{
    public static Action OnLevelGeneration;
    public static Action OnLevelStart;
    public static Action<bool> OnLevelEnd; //bool : is win or lose
    public static Action OnLevelRetry;

    public static Action<int> OnSendCurrentLevel;

    [SerializeField] private string m_levelSaveKey = "Level";

    private int m_currentLevelIndex;


    private void Start()
    {
        LoadLevel();
    }

    private void OnEnable()
    {
        Manager_GameState.OnSendCurrentGameState += OnSendCurrentGameState;
        LevelGenerator.OnLevelGenerated += OnLevelGenerated;
        LevelFiguresController.OnLevelCleared += OnLevelCleared;
        LevelFiguresController.OnNoMoreMovesLeft += OnNoMoreMovesLeft;
        UI_Button_Retry.OnRetryButtonClicked += OnRetryButtonClicked;
    }

    private void OnDisable()
    {
        Manager_GameState.OnSendCurrentGameState -= OnSendCurrentGameState;
        LevelGenerator.OnLevelGenerated -= OnLevelGenerated;
        LevelFiguresController.OnLevelCleared -= OnLevelCleared;
        LevelFiguresController.OnNoMoreMovesLeft -= OnNoMoreMovesLeft;
        UI_Button_Retry.OnRetryButtonClicked -= OnRetryButtonClicked;
    }

    public static void GenerateNewGrid()
    {
        OnLevelGeneration?.Invoke();
    }

    private void OnSendCurrentGameState(GameState state)
    {
        if (state == GameState.TitleScreen)
            OnLevelGeneration?.Invoke();
    }

    private void OnLevelGenerated()
    {
        OnLevelStart?.Invoke();
    }

    private void LoadLevel()
    {
        if (PlayerPrefs.HasKey(m_levelSaveKey))
        {
            m_currentLevelIndex = PlayerPrefs.GetInt(m_levelSaveKey);
        }
        else
        {
            m_currentLevelIndex = 1;
            SaveLevel();
        }

        OnSendCurrentLevel?.Invoke(m_currentLevelIndex);
    }

    private void SaveLevel()
    {
        PlayerPrefs.SetInt(m_levelSaveKey, m_currentLevelIndex);
    }


    private void OnLevelCleared()
    {
        m_currentLevelIndex++;
        OnSendCurrentLevel?.Invoke(m_currentLevelIndex);
        OnLevelEnd?.Invoke(true);

        OnSendCurrentLevel?.Invoke(m_currentLevelIndex);

        SaveLevel();
    }

    private void OnNoMoreMovesLeft()
    {
        m_currentLevelIndex = 1;
        OnSendCurrentLevel?.Invoke(m_currentLevelIndex);
        OnLevelEnd?.Invoke(false);

        OnSendCurrentLevel?.Invoke(m_currentLevelIndex);

        SaveLevel();
    }

    private void OnRetryButtonClicked()
    {
        OnLevelRetry?.Invoke();
    }
}