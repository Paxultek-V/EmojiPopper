using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Bombs : MonoBehaviour
{
    [SerializeField] private UI_Button_Bomb m_bombPrefab = null;

    [SerializeField] private Transform m_bombParent = null;

    [SerializeField] private int m_initialBombCount = 3;

    [SerializeField] private float m_delayBewteenSpawnAtStart = 0.2f;

    private UI_Button_Bomb m_uiButtonBombBuffer;
    private List<UI_Button_Bomb> m_uiButtonBombList = new List<UI_Button_Bomb>();

    public static Manager_Bombs Instance;

    public bool HasBombLeft
    {
        get => m_uiButtonBombList.Count > 0;
    }


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        Manager_Level.OnLevelRetry += OnLevelRetry;
        UI_Button_Bomb.OnBombUsed += OnBombUsed;
    }

    private void OnDisable()
    {
        Manager_Level.OnLevelRetry += OnLevelRetry;
        UI_Button_Bomb.OnBombUsed -= OnBombUsed;
    }

    private IEnumerator Start()
    {
        yield return InitializeBombsCoroutine();
    }

    private IEnumerator InitializeBombsCoroutine()
    {
        while (m_uiButtonBombList.Count < m_initialBombCount)
        {
            yield return new WaitForSeconds(m_delayBewteenSpawnAtStart);
            GainBomb();
        }

        if (m_uiButtonBombList.Count > 3)
        {
            UI_Button_Bomb bomb = m_uiButtonBombList[0];
            m_uiButtonBombList.RemoveAt(0);
            Destroy(bomb.gameObject);
        }
    }

    private void OnLevelRetry()
    {
        StartCoroutine(InitializeBombsCoroutine());
    }

    private void OnBombUsed(UI_Button_Bomb uiButtonBomb)
    {
        m_uiButtonBombList.Remove(uiButtonBomb);
    }

    private void GainBomb()
    {
        m_uiButtonBombBuffer = Instantiate(m_bombPrefab, m_bombParent);
        m_uiButtonBombBuffer.Initialize();
        m_uiButtonBombList.Add(m_uiButtonBombBuffer);
    }
}