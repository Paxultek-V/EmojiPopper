using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI_Button_Bomb : MonoBehaviour
{
    public static Action<UI_Button_Bomb, bool> OnBombSelected;
    public static Action<UI_Button_Bomb> OnBombUsed;

    [SerializeField] private TMP_Text m_bombImage = null;

    [SerializeField] private Color m_fullColor = Color.white;

    [SerializeField] private Color m_transparentColor = Color.white;

    [SerializeField] private float m_startAppearScale = 0.4f;
    [SerializeField] private float m_appearAnimationDuration = 0.2f;


    private bool m_isSelected;
    private Tweener tweenerSelection;
    private Tweener tweenerNoHintPreview;


    private void OnEnable()
    {
        OnBombSelected += ManageSelection;
        UI_BombConfirmationPanel.OnConfirmBombSelection += OnConfirmBombSelection;
        UI_BombConfirmationPanel.OnCancelBombSelection += OnCancelBombSelection;
        LevelFiguresController.OnNoHintAvailable += OnNoHintAvailable;
        Figure.OnFigureClicked += OnFigureClicked;
        Manager_Level.OnLevelGeneration += OnLevelGeneration;
    }

    private void OnDisable()
    {
        OnBombSelected -= ManageSelection;
        UI_BombConfirmationPanel.OnConfirmBombSelection -= OnConfirmBombSelection;
        UI_BombConfirmationPanel.OnCancelBombSelection -= OnCancelBombSelection;
        LevelFiguresController.OnNoHintAvailable -= OnNoHintAvailable;
        Figure.OnFigureClicked -= OnFigureClicked;
        Manager_Level.OnLevelGeneration -= OnLevelGeneration;
    }

    // Called by bomb manager
    public void Initialize()
    {
        transform.localScale = Vector3.one * m_startAppearScale;
        transform.DOScale(Vector3.one, m_appearAnimationDuration).SetEase(Ease.OutBounce);

        m_isSelected = false;
        m_bombImage.color = m_fullColor;
    }

    private void OnNoHintAvailable()
    {
        tweenerNoHintPreview = transform
            .DOScale(Vector3.one * 1.2f, 0.3f)
            .SetLoops(4, LoopType.Yoyo)
            .OnComplete(() => tweenerNoHintPreview.Restart(true, 2f));
    }

    private void OnConfirmBombSelection()
    {
        if (m_isSelected)
        {
            OnBombUsed?.Invoke(this);
            Destroy(gameObject);
        }
        else
            ResetVisuals();
    }

    private void OnCancelBombSelection()
    {
        m_isSelected = false;
        ResetVisuals();
    }


    /// <summary>
    /// Called by button
    /// </summary>
    public void ToggleBomb()
    {
        m_isSelected = !m_isSelected;

        OnBombSelected?.Invoke(this, m_isSelected);
    }

    private void ManageSelection(UI_Button_Bomb uiButtonBomb, bool currentBombSelectedState)
    {
        if (uiButtonBomb != this && m_isSelected)
            m_isSelected = false;

        ManageImageTransparency(uiButtonBomb, currentBombSelectedState);
    }

    private void ManageImageTransparency(UI_Button_Bomb uiButtonBombCurrent, bool currentBombSelectedState)
    {
        ResetVisuals();

        if (currentBombSelectedState == false)
            return;

        if (uiButtonBombCurrent == this)
            tweenerSelection = transform.DOScale(1.33f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        else
            m_bombImage.color = m_transparentColor;
    }

    private void ResetVisuals()
    {
        m_bombImage.color = m_fullColor;
        tweenerSelection.Kill();
        KillHintBombTweener();
    }

    private void OnFigureClicked(Figure clickedFigure)
    {
        KillHintBombTweener();
    }

    private void KillHintBombTweener()
    {
        tweenerNoHintPreview.Kill();
        transform.localScale = Vector3.one;
    }

    private void OnLevelGeneration()
    {
        KillHintBombTweener();
    }
}