using System;
using UnityEngine;

public class Manager_HapticFeedback : MonoBehaviour
{
    private void OnEnable()
    {
        Option.OnSendOptionState += OnSendOptionState;
        
        Manager_GameState.OnSendCurrentGameState += OnBroadcastGameState;
        Figure.OnFigurePop += OnFigurePop;
        UI_BombConfirmationPanel.OnConfirmBombSelection += PlayHeavyHaptic;
        Click_Controller.OnClick += OnClick;
    }

    private void OnDisable()
    {
        Option.OnSendOptionState -= OnSendOptionState;
        
        Manager_GameState.OnSendCurrentGameState -= OnBroadcastGameState;
        Figure.OnFigurePop -= OnFigurePop;
        UI_BombConfirmationPanel.OnConfirmBombSelection -= PlayHeavyHaptic;
        Click_Controller.OnClick -= OnClick;
    }

    private void OnSendOptionState(OptionType optionType, bool state)
    {
        if (optionType == OptionType.Vibration)
            Taptic.tapticOn = state;
    }
    
    private void OnClick(Vector3 cursorPosition)
    {
        PlayLightHaptic();
    }
    
    private void OnBroadcastGameState(GameState state)
    {
        switch (state)
        {
            case GameState.TitleScreen:
                break;
            case GameState.Gameplay:
                break;
            case GameState.Gameover:
                PlayHeavyHaptic();
                break;
            case GameState.Victory:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void OnFigurePop(float figureValue)
    {
        PlayLightHaptic();
    }

    private void PlayLightHaptic()
    {
        Taptic.Light();
    }

    private void PlayHeavyHaptic()
    {
        Taptic.Heavy();
    }
}