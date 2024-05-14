using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    public static Action<Figure[,]> OnSendFiguresGrid;
    public static Action<int, int> OnSendGridSize;
    public static Action OnLevelGenerated;

    [SerializeField] private Transform m_figureParent = null;

    [SerializeField] private List<FigureSet> m_figureSetList = null;

    [SerializeField] private FigureSet m_figureSetOnFirstLevel = null;

    [SerializeField] private Figure m_figurePrefab = null;

    [SerializeField] private float m_timeToGenerateLevel = 1f;

    [SerializeField] private int m_columnCount = 9;

    [SerializeField] private int m_rowCount = 9;

    [SerializeField] private bool m_isDebug = false;

    private FigureSet m_currentFigureSet;
    private Figure m_figureBuffer;
    private Vector3 m_figurePosition;
    private Figure[,] m_figureGrid;
    private int m_debugIndex;

    private float m_delayBetweenEachFigureSpawn
    {
        get => m_timeToGenerateLevel / (m_columnCount * m_rowCount);
    }

    private void OnEnable()
    {
        Manager_Level.OnLevelGeneration += OnLevelGeneration;
    }

    private void OnDisable()
    {
        Manager_Level.OnLevelGeneration -= OnLevelGeneration;
    }

    private void Start()
    {
        OnSendGridSize?.Invoke(m_columnCount, m_rowCount);
    }

    private void OnLevelGeneration()
    {
        StartCoroutine(GenerateLevel());
    }

    private IEnumerator GenerateLevel()
    {
        ClearCurrentGrid();

        if (Manager_FirstTimePlaying.Instance.IsFirstTimePlaying())
            m_currentFigureSet = m_figureSetOnFirstLevel;
        else
            m_currentFigureSet = GetRandomFigureSet();

        m_figureGrid = new Figure[m_columnCount, m_rowCount];

        for (int i = 0; i < m_rowCount; i++)
        {
            for (int j = 0; j < m_columnCount; j++)
            {
                m_figurePosition = new Vector3(j, i, 0f);
                m_figureBuffer = Instantiate(m_figurePrefab, m_figurePosition, Quaternion.identity, m_figureParent);

                m_figureBuffer.Initialize(m_currentFigureSet, j, i);

                m_figureGrid[j, i] = m_figureBuffer;

                yield return new WaitForSeconds(m_delayBetweenEachFigureSpawn);
            }
        }

        OnSendFiguresGrid?.Invoke(m_figureGrid);

        OnLevelGenerated?.Invoke();
    }

    private void ClearCurrentGrid()
    {
        if (m_figureGrid == null)
            return;

        for (int i = 0; i < m_figureGrid.GetLength(0); i++)
        {
            for (int j = 0; j < m_figureGrid.GetLength(1); j++)
            {
                if (m_figureGrid[j, i] != null)
                    m_figureGrid[j, i].Kill(false);

                m_figureGrid[j, i] = null;
            }
        }
    }

    private FigureSet GetRandomFigureSet()
    {
        int randomIndex = Random.Range(0, m_figureSetList.Count);

        if (m_isDebug)
        {
            randomIndex = m_debugIndex;
            m_debugIndex++;

            if (m_debugIndex >= m_figureSetList.Count)
                m_debugIndex = 0;
        }

        return m_figureSetList[randomIndex];
    }
}