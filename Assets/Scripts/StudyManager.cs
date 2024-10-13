using UnityEngine;

public class Study : MonoBehaviour
{
    [SerializeField] private GameObject bubbleCursor;
    [SerializeField] private GameObject pointCursor;

    private void Start()
    {
        if (pointCursor != null) pointCursor.SetActive(false);
        if (bubbleCursor != null) bubbleCursor.SetActive(false);

        SetupCursor(GameManager.Instance.selectedCursor);
    }

    private void SetupCursor(CursorType cursorType)
    {
        if (cursorType == CursorType.BubbleCursor)
        {
            bubbleCursor.SetActive(true);
            pointCursor.SetActive(false);
        }
        else if (cursorType == CursorType.PointCursor)
        {
            bubbleCursor.SetActive(false);
            pointCursor.SetActive(true);
        }
    }
}
