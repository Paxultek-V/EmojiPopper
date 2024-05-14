using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UI_BombConfirmationPanel : MonoBehaviour
{
    public static Action OnConfirmBombSelection;
    public static Action OnCancelBombSelection;


    [SerializeField] private CanvasGroup m_panelCanvasGroup = null;

    [SerializeField] private Vector3 m_positionOffset = Vector3.zero;

    [SerializeField] private float m_startAppearScale = 0.4f;
    [SerializeField] private float m_appearAnimationDuration = 0.2f;

    private void OnEnable()
    {
        LevelFiguresController.OnClickFigureBomb += OnClickFigureBomb;
    }

    private void OnDisable()
    {
        LevelFiguresController.OnClickFigureBomb -= OnClickFigureBomb;
    }

    private void Start()
    {
        TogglePanelInteraction(false);
    }

    private void OnClickFigureBomb(Figure selectedFigure)
    {
        transform.position = selectedFigure.gameObject.transform.position + m_positionOffset;

        TogglePanel(true, true);
    }

    private void TogglePanel(bool state, bool withAnimations)
    {
        if (withAnimations)
        {
            Vector3 targetScale = state ? Vector3.one : Vector3.one * m_startAppearScale;
            
            transform.localScale = state ? Vector3.one * m_startAppearScale : Vector3.one;
            
            transform.DOScale(targetScale, m_appearAnimationDuration).SetEase(state ? Ease.OutBounce : Ease.InBounce)
                .OnComplete(()=> TogglePanelInteraction(state));
        }
        else
        {
            TogglePanelInteraction(state);
        }
    }

    private void TogglePanelInteraction(bool state)
    {
        m_panelCanvasGroup.alpha = state ? 1 : 0;
        m_panelCanvasGroup.interactable = state;
        m_panelCanvasGroup.blocksRaycasts = state;
    }

    // Called by button
    public void ConfirmSelection()
    {
        TogglePanel(false, false);
        OnConfirmBombSelection?.Invoke();
    }

    // Called by button
    public void CancelSelection()
    {
        TogglePanel(false, false);
        OnCancelBombSelection?.Invoke();
    }
}