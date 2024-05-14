using TMPro;
using UnityEngine;

public class UI_Text_GameoverMessage : MonoBehaviour
{
    [SerializeField] private TMP_Text m_gameoverMessageText = null;

    [SerializeField] private string m_noMorePossibleMoveLeftMessage = "";
    [SerializeField] private string m_forfeitMessage = "";

    private void OnEnable()
    {
        Manager_GameState.OnForfeit += OnForfeit;
        Manager_GameState.OnNoMoreMovesPossible += OnNoMoreMovesPossible;
    }

    private void OnDisable()
    {
        Manager_GameState.OnForfeit -= OnForfeit;
        Manager_GameState.OnNoMoreMovesPossible -= OnNoMoreMovesPossible;
    }

    private void OnForfeit()
    {
        m_gameoverMessageText.text = m_forfeitMessage;
    }

    private void OnNoMoreMovesPossible()
    {
        m_gameoverMessageText.text = m_noMorePossibleMoveLeftMessage;
    }
}