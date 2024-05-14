using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Figure : MonoBehaviour
{
    public static Action<Figure> OnFigureClicked;
    public static Action<float> OnFigurePop;

    [SerializeField] private Transform m_visualTransform = null;

    [SerializeField] private TMP_Text m_text = null;

    [SerializeField] private PopScore m_popScorePrefab = null;

    [SerializeField] private GameObject m_destroyFx = null;

    [SerializeField] private float m_appearScale = 0.3f;

    [SerializeField] private float m_scoreValue = 100f;

    [SerializeField] private Color m_fullColor = Color.white;

    [SerializeField] private Color m_bombDestroyColor = Color.white;

    private FigureType m_type;
    private Tweener tweenerBombPreview;
    private Tweener tweenerHintPreview;
    private int m_xPos;
    private int m_yPos;

    public FigureType Type
    {
        get => m_type;
    }

    public int XPos
    {
        get => m_xPos;
        set => m_xPos = value;
    }

    public int YPos
    {
        get => m_yPos;
        set => m_yPos = value;
    }


    private void OnEnable()
    {
        LevelFiguresController.OnShowHintFigure += OnShowHintFigure;
        LevelFiguresController.OnClickFigureBomb += OnClickFigureBomb;
        LevelFiguresController.OnPopLastUniqueDifferentFigure += OnPopLastUniqueDifferentFigure;
        
        OnFigureClicked += StopHint;
    }

    private void OnDisable()
    {
        LevelFiguresController.OnShowHintFigure -= OnShowHintFigure;
        LevelFiguresController.OnClickFigureBomb -= OnClickFigureBomb;
        LevelFiguresController.OnPopLastUniqueDifferentFigure -= OnPopLastUniqueDifferentFigure;
        
        OnFigureClicked -= StopHint;
    }

    public void Initialize(FigureSet figureSet, int xPos, int yPos)
    {
        DetermineRandomFigure();

        m_xPos = xPos;
        m_yPos = yPos;

        m_text.text = figureSet.figureSetDictionary[m_type];

        m_visualTransform.localScale = Vector3.one * m_appearScale;
        m_visualTransform.DOScale(Vector3.one, 0.3f);
    }

    public void MoveFigureToPosition(float movementDuration, bool isMovingX, bool isMovingY)
    {
        if (isMovingX)
            transform.DOMove(new Vector3(XPos, YPos, 0f), movementDuration).SetEase(Ease.Linear);
        else if (isMovingY)
            transform.DOMove(new Vector3(XPos, YPos, 0f), movementDuration).SetEase(Ease.OutBounce);
    }

    private void DetermineRandomFigure()
    {
        m_type = (FigureType)Random.Range(0, Enum.GetNames(typeof(FigureType)).Length);
    }

    /// <summary>
    /// Called by button
    /// </summary>
    public void ClickOnFigure()
    {
        OnFigureClicked?.Invoke(this);
    }

    public void Kill(bool isKilledInGame)
    {
        if (isKilledInGame)
        {
            transform.DOScale(Vector3.one * 1.1f, 0.05f)
                .OnComplete(() => transform.DOScale(Vector3.one * 0.7f, 0.1f)
                    .OnComplete(PopFigure));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void PopFigure()
    {
        PopScore popScore = Instantiate(m_popScorePrefab, transform.position, Quaternion.identity);
        popScore.Initalize(m_scoreValue);
        OnFigurePop?.Invoke(m_scoreValue);
        //Instantiate(m_popSfx, transform.position, Quaternion.identity);
        Instantiate(m_destroyFx, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }


    // BOMB FIGURE
    public void ToggleBombPreview(bool state)
    {
        if (tweenerBombPreview != null)
            tweenerBombPreview.Kill();

        if (state)
            tweenerBombPreview = m_text.DOColor(m_bombDestroyColor, 0.5f).SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        else
            m_text.color = m_fullColor;
    }
    
    private void OnClickFigureBomb(Figure clickedFigure)
    {
        ToggleBombPreview(false);
    }

    
    // REMAINING FIGURE IN LEVEL SECTION
    private void OnPopLastUniqueDifferentFigure(Figure figure)
    {
        if (figure == this)
            Kill(true);
    }

    // HINT SECTION
    private void OnShowHintFigure(Figure hintFigure)
    {
        if (hintFigure == this)
        {
            if (tweenerHintPreview != null)
                tweenerHintPreview.Kill();

            tweenerHintPreview = m_text.transform
                .DOScale(Vector3.one * 1.2f, 0.3f)
                .SetLoops(4, LoopType.Yoyo)
                .OnComplete(() => tweenerHintPreview.Restart(true, 2f));
        }
    }

    private void StopHint(Figure clickedFigure)
    {
        if (tweenerHintPreview != null)
        {
            tweenerHintPreview.Kill();
            m_text.transform.localScale = Vector3.one;
        }
    }
}