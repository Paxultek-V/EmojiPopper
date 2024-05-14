using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelFiguresController : MonoBehaviour
{
    public static Action OnNoMoreMovesLeft;
    public static Action OnLevelCleared;
    public static Action<Figure> OnClickFigureBomb;
    public static Action OnBombExplode;
    public static Action OnNoHintAvailable;
    public static Action<Figure> OnShowHintFigure;
    public static Action<Figure> OnPopLastUniqueDifferentFigure;
    public static Action<int> OnPopFigures;
    public static Action<bool> OnPlayerCanInteractWithFigures;

    [SerializeField] private int m_minAdjacentFigureToPopCount = 2;
    [SerializeField] private int m_minAdjacentFigureToHintCount = 3;
    [SerializeField] private float m_figureFallDuration = 0.25f;
    [SerializeField] private float m_figureRowMovementDuration = 0.25f;
    [SerializeField] private float m_delayBetweenFigurePop = 0.075f;
    [SerializeField] private float m_delayBeforeFigurePop = 0.1f;

    private Figure[,] m_figureGrid;
    private List<Figure> m_openList = new List<Figure>();
    private List<Figure> m_sameTypeAdjacentFigureList = new List<Figure>();
    private List<Figure> m_figureInBombRangeList = new List<Figure>();
    private List<Figure> m_differentTypeFigureRemainingList = new List<Figure>();
    private Coroutine m_tryPopFigureCoroutine;
    private Coroutine m_destroyFiguresByBombCoroutine;
    private bool m_isRestructuringColumn = false;
    private bool m_isMovingColumn = false;
    private bool m_canInteractWithFigures = false;


    //max column index
    private int m_maxXPosition
    {
        get => m_figureGrid.GetLength(0) - 1;
    }

    //max row index
    private int m_maxYPosition
    {
        get => m_figureGrid.GetLength(1) - 1;
    }


    private void OnEnable()
    {
        Manager_GameState.OnSendCurrentGameState += OnSendCurrentGameState;
        LevelGenerator.OnSendFiguresGrid += OnSendFiguresGrid;
        Figure.OnFigureClicked += OnFigureClicked;
        UI_BombConfirmationPanel.OnConfirmBombSelection += OnConfirmBombSelection;
        UI_BombConfirmationPanel.OnCancelBombSelection += OnCancelBombSelection;
        Manager_Hint.OnShowHint += ShowHint;
    }

    private void OnDisable()
    {
        Manager_GameState.OnSendCurrentGameState -= OnSendCurrentGameState;
        LevelGenerator.OnSendFiguresGrid -= OnSendFiguresGrid;
        Figure.OnFigureClicked -= OnFigureClicked;
        UI_BombConfirmationPanel.OnConfirmBombSelection -= OnConfirmBombSelection;
        UI_BombConfirmationPanel.OnCancelBombSelection -= OnCancelBombSelection;
        Manager_Hint.OnShowHint -= ShowHint;
    }


    private void OnSendCurrentGameState(GameState state)
    {
        m_canInteractWithFigures = (state == GameState.Gameplay);
    }

    private void OnSendFiguresGrid(Figure[,] figureArray)
    {
        m_figureGrid = figureArray.Clone() as Figure[,];

        if (CheckGameoverCondition())
            OnNoMoreMovesLeft?.Invoke();
    }

    private void OnFigureClicked(Figure clickedFigure)
    {
        switch (Manager_ClickMode.Instance.CurrentClickMode)
        {
            case ClickMode.Pop:
                TryPopFigure(clickedFigure);
                break;
            case ClickMode.Bomb:
                TryBombFigure(clickedFigure);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    // RESTRUCTURE GRID SECTION
    private IEnumerator RestructureGrid()
    {
        for (int j = 0; j < m_figureGrid.GetLength(0); j++)
            ReorderColumn(j);

        if (m_isRestructuringColumn)
            yield return new WaitForSeconds(m_figureFallDuration);

        for (int i = 0; i < m_figureGrid.GetLength(0); i++)
            if (m_figureGrid[i, 0] == null)
                MoveColumn(i);

        if (m_isMovingColumn)
            yield return new WaitForSeconds(m_figureRowMovementDuration);
    }

    private void ReorderColumn(int columnIndex)
    {
        for (int i = 0; i < m_figureGrid.GetLength(1); i++)
        {
            if (m_figureGrid[columnIndex, i] == null)
            {
                Figure figureToRePosition = FindNextFigureInColumn(columnIndex, i);

                if (figureToRePosition != null)
                {
                    m_isRestructuringColumn = true;
                    RepositionFigure(figureToRePosition, columnIndex, i, m_figureFallDuration);
                }
            }
        }
    }

    private Figure FindNextFigureInColumn(int columnIndex, int startingIndex)
    {
        for (int i = startingIndex; i < m_figureGrid.GetLength(1); i++)
        {
            if (m_figureGrid[columnIndex, i] != null)
                return m_figureGrid[columnIndex, i];
        }

        return null;
    }

    private void MoveColumn(int columnIndex)
    {
        if (columnIndex + 1 > m_figureGrid.GetLength(0))
            return;

        Figure figureToRePosition = FindNextFigureInRow(0, columnIndex);

        if (figureToRePosition != null)
        {
            int columnToMoveIndex = figureToRePosition.XPos;

            m_isMovingColumn = true;

            for (int i = 0; i < m_figureGrid.GetLength(1); i++)
            {
                if (m_figureGrid[columnToMoveIndex, i] != null)
                    RepositionFigure(m_figureGrid[columnToMoveIndex, i], columnIndex, i, m_figureRowMovementDuration);
            }
        }
    }

    private Figure FindNextFigureInRow(int rowIndex, int startingIndex)
    {
        for (int i = startingIndex; i < m_figureGrid.GetLength(0); i++)
        {
            if (m_figureGrid[i, rowIndex] != null)
                return m_figureGrid[i, rowIndex];
        }

        return null;
    }

    private void RepositionFigure(Figure figureToRePosition, int xPos, int yPos, float movementDuration)
    {
        m_figureGrid[figureToRePosition.XPos, figureToRePosition.YPos] = null; // Set to null the old position in grid

        bool isMovingX = figureToRePosition.XPos != xPos;
        bool isMovingy = figureToRePosition.YPos != yPos;
        figureToRePosition.XPos = xPos;
        figureToRePosition.YPos = yPos;
        figureToRePosition.MoveFigureToPosition(movementDuration, isMovingX, isMovingy);

        m_figureGrid[xPos, yPos] = figureToRePosition; // Set the figure to the new position in grid
    }


    // POP SECTION
    private void TryPopFigure(Figure clickedFigure)
    {
        if (m_canInteractWithFigures == false)
            return;

        if (m_tryPopFigureCoroutine != null)
            StopCoroutine(m_tryPopFigureCoroutine);

        m_tryPopFigureCoroutine = StartCoroutine(TryPopFigureCoroutine(clickedFigure));
    }

    private IEnumerator TryPopFigureCoroutine(Figure clickedFigure)
    {
        m_canInteractWithFigures = false;
        OnPlayerCanInteractWithFigures?.Invoke(m_canInteractWithFigures);

        m_isRestructuringColumn = false;
        m_isMovingColumn = false;

        m_openList.Clear();
        m_sameTypeAdjacentFigureList.Clear();

        FillSameTypeAdjacentFigureList(clickedFigure);

        yield return DestroyFiguresCoroutine();

        yield return RestructureGrid();

        if (CheckLevelClearedCondition())
        {
            yield return new WaitForSeconds(0.5f);
            OnLevelCleared?.Invoke();
            yield break;
        }

        if (CheckGameoverCondition())
        {
            OnNoMoreMovesLeft?.Invoke();
            yield break;
        }

        m_canInteractWithFigures = true;
        OnPlayerCanInteractWithFigures?.Invoke(m_canInteractWithFigures);
    }

    private void FillSameTypeAdjacentFigureList(Figure clickedFigure)
    {
        FillOpenListWithSameFigureTypeNeighbors(clickedFigure);

        while (m_openList.Count > 0)
        {
            Figure nextFigure = m_openList[0];
            m_openList.Remove(nextFigure);
            FillOpenListWithSameFigureTypeNeighbors(nextFigure);
        }
    }

    private void FillOpenListWithSameFigureTypeNeighbors(Figure currentFigure)
    {
        m_sameTypeAdjacentFigureList.Add(currentFigure);

        AddNeighborOfSameTypeToOpenList(currentFigure, currentFigure.XPos + 1, currentFigure.YPos);
        AddNeighborOfSameTypeToOpenList(currentFigure, currentFigure.XPos - 1, currentFigure.YPos);
        AddNeighborOfSameTypeToOpenList(currentFigure, currentFigure.XPos, currentFigure.YPos + 1);
        AddNeighborOfSameTypeToOpenList(currentFigure, currentFigure.XPos, currentFigure.YPos - 1);
    }

    private void AddNeighborOfSameTypeToOpenList(Figure currentFigure, int x, int y)
    {
        if (IsPositionInGrid(x, y) == false)
            return;

        if (m_figureGrid[x, y] == null)
            return;

        if (!DoesFigureHaveNeighborOfSameTypeAtPosition(currentFigure, x, y))
            return;

        if (m_sameTypeAdjacentFigureList.Contains(m_figureGrid[x, y]))
            return;

        if (m_openList.Contains(m_figureGrid[x, y]))
            return;

        m_openList.Add(m_figureGrid[x, y]);
    }

    private bool DoesFigureHaveNeighborOfSameTypeAtPosition(Figure currentFigure, int x, int y)
    {
        if (IsPositionInGrid(x, y) == false)
            return false;

        if (m_figureGrid[x, y] == null)
            return false;

        return m_figureGrid[x, y].Type == currentFigure.Type;
    }

    private IEnumerator DestroyFiguresCoroutine()
    {
        yield return new WaitForSeconds(m_delayBeforeFigurePop);

        OnPopFigures?.Invoke(m_sameTypeAdjacentFigureList.Count);

        if (m_sameTypeAdjacentFigureList.Count >= m_minAdjacentFigureToPopCount)
        {
            for (int i = 0; i < m_sameTypeAdjacentFigureList.Count; i++)
            {
                m_figureGrid[m_sameTypeAdjacentFigureList[i].XPos, m_sameTypeAdjacentFigureList[i].YPos] = null;

                m_sameTypeAdjacentFigureList[i].Kill(true);

                yield return new WaitForSeconds(m_delayBetweenFigurePop);
            }
        }
    }


    // HINT SECTION
    private void ShowHint()
    {
        for (int i = 0; i < m_figureGrid.GetLength(0); i++)
        {
            for (int j = 0; j < m_figureGrid.GetLength(1); j++)
            {
                if (m_figureGrid[j, i] == null)
                    continue;

                m_openList.Clear();
                m_sameTypeAdjacentFigureList.Clear();

                FillSameTypeAdjacentFigureList(m_figureGrid[j, i]);

                if (m_sameTypeAdjacentFigureList.Count >= m_minAdjacentFigureToHintCount)
                {
                    for (int k = 0; k < m_sameTypeAdjacentFigureList.Count; k++)
                        OnShowHintFigure?.Invoke(m_sameTypeAdjacentFigureList[k]);

                    return;
                }
            }
        }

        OnNoHintAvailable?.Invoke();
    }

    // BOMB SECTION
    private void TryBombFigure(Figure clickedFigure)
    {
        if (m_canInteractWithFigures == false)
            return;

        OnClickFigureBomb?.Invoke(clickedFigure);

        m_figureInBombRangeList.Clear();

        FillFiguresInBombRangeList(clickedFigure);

        ToggleBombPreviewOnFigures(true);
    }

    private void ToggleBombPreviewOnFigures(bool state)
    {
        for (int i = 0; i < m_figureInBombRangeList.Count; i++)
        {
            m_figureInBombRangeList[i].ToggleBombPreview(state);
        }
    }

    private void OnConfirmBombSelection()
    {
        if (m_destroyFiguresByBombCoroutine != null)
            StopCoroutine(m_destroyFiguresByBombCoroutine);

        m_destroyFiguresByBombCoroutine = StartCoroutine(DestroyFiguresByBombCoroutine());
    }

    private IEnumerator DestroyFiguresByBombCoroutine()
    {
        for (int i = 0; i < m_figureInBombRangeList.Count; i++)
        {
            m_figureGrid[m_figureInBombRangeList[i].XPos, m_figureInBombRangeList[i].YPos] = null;

            m_figureInBombRangeList[i].Kill(true);
        }

        OnBombExplode?.Invoke();

        yield return new WaitForSeconds(0.5f);

        yield return RestructureGrid();

        if (CheckLevelClearedCondition())
        {
            OnLevelCleared?.Invoke();
            yield break;
        }

        if (CheckGameoverCondition())
        {
            OnNoMoreMovesLeft?.Invoke();
            yield break;
        }

        m_canInteractWithFigures = true;
        OnPlayerCanInteractWithFigures?.Invoke(m_canInteractWithFigures);
    }

    private void OnCancelBombSelection()
    {
        ToggleBombPreviewOnFigures(false);
        m_canInteractWithFigures = true;
    }

    private void FillFiguresInBombRangeList(Figure selectedFigure)
    {
        m_figureInBombRangeList.Add(selectedFigure);

        AddFiguresInBombRangeToList(selectedFigure.XPos - 1, selectedFigure.YPos + 1);
        AddFiguresInBombRangeToList(selectedFigure.XPos, selectedFigure.YPos + 1);
        AddFiguresInBombRangeToList(selectedFigure.XPos + 1, selectedFigure.YPos + 1);

        AddFiguresInBombRangeToList(selectedFigure.XPos - 1, selectedFigure.YPos);
        AddFiguresInBombRangeToList(selectedFigure.XPos + 1, selectedFigure.YPos);

        AddFiguresInBombRangeToList(selectedFigure.XPos - 1, selectedFigure.YPos - 1);
        AddFiguresInBombRangeToList(selectedFigure.XPos, selectedFigure.YPos - 1);
        AddFiguresInBombRangeToList(selectedFigure.XPos + 1, selectedFigure.YPos - 1);
    }

    private void AddFiguresInBombRangeToList(int x, int y)
    {
        if (IsPositionInGrid(x, y) == false)
            return;

        if (m_figureGrid[x, y] == null)
            return;

        m_figureInBombRangeList.Add(m_figureGrid[x, y]);
    }


    // GAME FLOW SECTION
    private bool CheckGameoverCondition()
    {
        if (Manager_Bombs.Instance.HasBombLeft)
            return false;

        bool hasNeighborOfSameType = false;

        for (int i = 0; i < m_figureGrid.GetLength(0); i++)
        {
            for (int j = 0; j < m_figureGrid.GetLength(1); j++)
            {
                if (m_figureGrid[j, i] != null)
                {
                    hasNeighborOfSameType =
                    (
                        DoesFigureHaveNeighborOfSameTypeAtPosition(m_figureGrid[j, i], m_figureGrid[j, i].XPos + 1,
                            m_figureGrid[j, i].YPos) ||
                        DoesFigureHaveNeighborOfSameTypeAtPosition(m_figureGrid[j, i], m_figureGrid[j, i].XPos - 1,
                            m_figureGrid[j, i].YPos) ||
                        DoesFigureHaveNeighborOfSameTypeAtPosition(m_figureGrid[j, i], m_figureGrid[j, i].XPos,
                            m_figureGrid[j, i].YPos + 1) ||
                        DoesFigureHaveNeighborOfSameTypeAtPosition(m_figureGrid[j, i], m_figureGrid[j, i].XPos,
                            m_figureGrid[j, i].YPos - 1)
                    );

                    if (hasNeighborOfSameType)
                    {
                        return !hasNeighborOfSameType;
                    }
                }
            }
        }

        return !hasNeighborOfSameType;
    }

    private bool CheckLevelClearedCondition()
    {
        m_differentTypeFigureRemainingList.Clear();

        for (int i = 0; i < m_figureGrid.GetLength(0); i++)
        {
            for (int j = 0; j < m_figureGrid.GetLength(1); j++)
            {
                if (m_figureGrid[j, i] != null)
                {
                    m_differentTypeFigureRemainingList.Add(m_figureGrid[j, i]);

                    if (m_differentTypeFigureRemainingList.Count > 4)
                        return false;
                }
            }
        }

        if (DoesListContainsUniqueFigureTypes() == true)
        {
            for (int i = 0; i < m_differentTypeFigureRemainingList.Count; i++)
            {
                OnPopLastUniqueDifferentFigure?.Invoke(m_differentTypeFigureRemainingList[i]);
            }

            return true;
        }

        return false;
    }

    private bool DoesListContainsUniqueFigureTypes()
    {
        if (m_differentTypeFigureRemainingList.Count == 0 || m_differentTypeFigureRemainingList.Count == 1)
            return true;

        List<FigureType> m_typeList = new List<FigureType>();

        m_typeList.Add(m_differentTypeFigureRemainingList[0].Type);

        for (int i = 1; i < m_differentTypeFigureRemainingList.Count; i++)
        {
            if (m_typeList.Contains(m_differentTypeFigureRemainingList[i].Type))
            {
                ShowList(m_typeList);
                return false;
            }

            m_typeList.Add(m_differentTypeFigureRemainingList[i].Type);
        }

        ShowList(m_typeList);
        return true;
    }

    private void ShowList(List<FigureType> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Debug.Log(list[i]);
        }
    }

    private bool IsPositionInGrid(int x, int y)
    {
        return !(x < 0 || x > m_maxXPosition ||
                 y < 0 || y > m_maxYPosition);
    }
}