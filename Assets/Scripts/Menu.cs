using UnityEngine;
using TMPro;

public class Menu : MonoBehaviour
{
    [SerializeField] private TMP_InputField participantIDInput;
    [SerializeField] private TextMeshProUGUI errorMessage;
    [SerializeField] private TextMeshProUGUI selectMessage;

    public void SelectBubbleCursor()
    {
        GameManager.Instance.selectedCursor = CursorType.BubbleCursor;
        selectMessage.text = "Selected: Bubble Cursor";
    }

    public void SelectPointCursor()
    {
        GameManager.Instance.selectedCursor = CursorType.PointCursor;
        selectMessage.text = "Selected: Point Cursor";
    }

    public void OnStartButtonPressed()
    {
        if (string.IsNullOrEmpty(participantIDInput.text))
        {
            errorMessage.text = "Please enter a Participant ID.";
            return;
        }

        else if (GameManager.Instance.selectedCursor == CursorType.Null)
        {
            errorMessage.text = "Please select a cursor type.";
            return;
        }

        else { Debug.Log("Participant ID: " + participantIDInput.text + "\nCursor Type: " + CursorType.BubbleCursor); }

        GameManager.Instance.participantID = participantIDInput.text;
        GameManager.Instance.StartStudy();
    }
}
