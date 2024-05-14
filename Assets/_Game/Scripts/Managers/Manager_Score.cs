using System;
using UnityEngine;

public class Manager_Score : MonoBehaviour
{
    public static Action<float> OnSendCurrentScore;

    [SerializeField] private string m_scoreSaveKey = "Score";

    private float m_currentScore;

    private void OnEnable()
    {
        Figure.OnFigurePop += OnFigurePop;
        Manager_Level.OnLevelEnd += OnLevelEnd;
        Manager_Level.OnLevelRetry += OnLevelRetry;
    }

    private void OnDisable()
    {
        Figure.OnFigurePop -= OnFigurePop;
        Manager_Level.OnLevelEnd -= OnLevelEnd;
        Manager_Level.OnLevelRetry -= OnLevelRetry;
    }

    private void Start()
    {
        LoadScore();
    }

    private void LoadScore()
    {
        if (PlayerPrefs.HasKey(m_scoreSaveKey))
        {
            m_currentScore = PlayerPrefs.GetFloat(m_scoreSaveKey);
        }
        else
        {
            m_currentScore = 0;
            SaveScore();
        }

        OnSendCurrentScore?.Invoke(m_currentScore);
    }

    private void SaveScore()
    {
        PlayerPrefs.SetFloat(m_scoreSaveKey, m_currentScore);
    }

    private void OnFigurePop(float figureScore)
    {
        m_currentScore += figureScore;

        OnSendCurrentScore?.Invoke(m_currentScore);
    }

    private void OnLevelRetry()
    {
        m_currentScore = 0;

        SaveScore();

        OnSendCurrentScore?.Invoke(m_currentScore);
    }

    private void OnLevelEnd(bool isWin)
    {
        if (isWin)
        {
            SaveScore();

            OnSendCurrentScore?.Invoke(m_currentScore);
        }
        else
        {
            m_currentScore = 0;

            SaveScore();
        }
    }
}