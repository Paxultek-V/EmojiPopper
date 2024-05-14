using System;
using UnityEngine;

public class Manager_Hint : MonoBehaviour
{
    public static Action OnShowHint;

    [SerializeField] private float m_timeBeforeShowingHint = 5f;


    private float m_timer;
    private bool m_isHintDisplayed;


    private void OnEnable()
    {
        Figure.OnFigureClicked += OnFigureClicked;
    }

    private void OnDisable()
    {
        Figure.OnFigureClicked -= OnFigureClicked;
    }


    private void Update()
    {
        ManageHint();
    }

    private void ManageHint()
    {
        if(m_isHintDisplayed)
            return;
        
        m_timer += Time.deltaTime;

        if (m_timer > m_timeBeforeShowingHint)
        {
            m_isHintDisplayed = true;
            OnShowHint?.Invoke();
        }
    }

    private void OnFigureClicked(Figure clickedFigure)
    {
        m_timer = 0;
        m_isHintDisplayed = false;
    }
    
}
