using System;
using UnityEngine;

public class Manager_Level : MonoBehaviour
{
    public static Action OnLevelGeneration;
    public static Action OnLevelStart;
    public static Action<bool> OnLevelEnd; //bool : is win or lose
    public static Action OnLevelRetry;
    public static Action<int> OnSendTotalLevelCleared;

    public static Action<int> OnSendCurrentLevel;

    [SerializeField] private string m_levelSaveKey = "Level";
    [SerializeField] private string m_totalLevelClearedSaveKey = "LevelCleared";

    private int m_currentLevelIndex;
    private int m_totalLevelCleared;

    private void Start()
    {
        LoadLevel();
        LoadTotalLevelCleared();
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
        else if (state == GameState.Gameplay)
        {
            // YSO SDK
            Debug.Log("game started : " + m_totalLevelCleared);
            //YsoCorp.GameUtils.YCManager.instance.OnGameStarted(m_totalLevelCleared);
        }
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

    private void LoadTotalLevelCleared()
    {
        if (PlayerPrefs.HasKey(m_totalLevelClearedSaveKey))
        {
            m_totalLevelCleared = PlayerPrefs.GetInt(m_totalLevelClearedSaveKey);
        }
        else
        {
            m_totalLevelCleared = 0;
            SaveTotalLevelCleared();
        }

        OnSendTotalLevelCleared?.Invoke(m_totalLevelCleared);
    }

    private void SaveTotalLevelCleared()
    {
        PlayerPrefs.SetInt(m_totalLevelClearedSaveKey, m_totalLevelCleared);
    }

    private void OnLevelCleared()
    {
        m_currentLevelIndex++;
        m_totalLevelCleared++;
        OnSendCurrentLevel?.Invoke(m_currentLevelIndex);
        OnSendTotalLevelCleared?.Invoke(m_totalLevelCleared);
        OnLevelEnd?.Invoke(true);

        OnSendCurrentLevel?.Invoke(m_currentLevelIndex);

        SaveLevel();
        SaveTotalLevelCleared();
        Debug.Log("WIN");
        //YsoCorp.GameUtils.YCManager.instance.OnGameFinished(true);
    }

    private void OnNoMoreMovesLeft()
    {
        m_currentLevelIndex = 1;
        OnSendCurrentLevel?.Invoke(m_currentLevelIndex);
        OnLevelEnd?.Invoke(false);

        OnSendCurrentLevel?.Invoke(m_currentLevelIndex);

        SaveLevel();
        Debug.Log("LOSE");
        //YsoCorp.GameUtils.YCManager.instance.OnGameFinished(false);
    }

    private void OnRetryButtonClicked()
    {
        OnLevelRetry?.Invoke();
    }
}