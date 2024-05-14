using UnityEngine;

public class UI_HeaderPosition_Controller : MonoBehaviour
{
    [SerializeField] private RectTransform m_header = null;

    [SerializeField] private Vector3 m_headerOffsetFromTopOfGrid = Vector3.zero;

    private void OnEnable()
    {
        LevelGenerator.OnSendGridSize += OnSendGridSize;
    }

    private void OnDisable()
    {
        LevelGenerator.OnSendGridSize -= OnSendGridSize;
    }

    private void OnSendGridSize(int colCount, int rowCount)
    {
        Vector3 highestMidlePosition = new Vector3((float)(colCount - 1) / 2f, (rowCount - 1), 0f);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(highestMidlePosition);

        Vector3 desiredPosition = new Vector3(screenPos.x, screenPos.y + m_headerOffsetFromTopOfGrid.y, 0f);
        m_header.position = desiredPosition;
    }
}