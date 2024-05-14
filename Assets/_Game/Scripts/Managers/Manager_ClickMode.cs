using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClickMode
{
    Pop,
    Bomb
}

public class Manager_ClickMode : MonoBehaviour
{
    private ClickMode m_currentClickMode;

    public ClickMode CurrentClickMode
    {
        get => m_currentClickMode;
    }
    
    public static Manager_ClickMode Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        Manager_GameState.OnSendCurrentGameState += OnSendCurrentGameState;
        UI_Button_Bomb.OnBombSelected += OnBombSelected;
        UI_BombConfirmationPanel.OnConfirmBombSelection += OnConfirmBombSelection;
        UI_BombConfirmationPanel.OnCancelBombSelection += OnCancelBombSelection;
    }

    private void OnDisable()
    {
        Manager_GameState.OnSendCurrentGameState -= OnSendCurrentGameState;
        UI_Button_Bomb.OnBombSelected -= OnBombSelected;
        UI_BombConfirmationPanel.OnConfirmBombSelection -= OnConfirmBombSelection;
        UI_BombConfirmationPanel.OnCancelBombSelection -= OnCancelBombSelection;
    }

    private void OnConfirmBombSelection()
    {
        m_currentClickMode = ClickMode.Pop;
    }
    
    private void OnCancelBombSelection()
    {
        m_currentClickMode = ClickMode.Pop;
    }

    private void OnSendCurrentGameState(GameState state)
    {
        if(state == GameState.TitleScreen)
            m_currentClickMode = ClickMode.Pop;
    }

    private void OnBombSelected(UI_Button_Bomb uiButtonBomb, bool currentBombSelectedState)
    {
        if (currentBombSelectedState == true)
            m_currentClickMode = ClickMode.Bomb;
        else
            m_currentClickMode = ClickMode.Pop;
    }
    
}