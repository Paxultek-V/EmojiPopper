using DG.Tweening;
using TMPro;
using UnityEngine;

public class PopScore : MonoBehaviour
{
    [SerializeField] private TMP_Text m_scoreText = null;

    [SerializeField] private Vector3 m_textMovePosition = Vector3.zero;

    [SerializeField] private Color m_transparentColor = Color.white;
    
    [SerializeField] private Color m_invisibleColor = Color.white;

    [SerializeField] private Color m_fullColor = Color.white;

    [SerializeField] private float m_animationDuration = 0.5f;

    private Vector3 m_startPosition;


    public void Initalize(float score)
    {
        m_scoreText.text = "+" + score;

        m_startPosition = transform.position;
        m_scoreText.transform.DOMove(m_startPosition + m_textMovePosition, m_animationDuration).SetEase(Ease.Linear);

        m_scoreText.color = m_transparentColor;
        m_scoreText.DOColor(m_fullColor, m_animationDuration).SetEase(Ease.Linear).OnComplete(() =>
            m_scoreText.DOColor(m_invisibleColor, m_animationDuration).SetEase(Ease.Linear));
    }
}