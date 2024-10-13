using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string participantID;
    public CursorType selectedCursor;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // This line makes it persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void StartStudy()
    { 
        SceneManager.LoadScene("Study");
    }
}

public enum CursorType
{
    Null,
    BubbleCursor,
    PointCursor
}

/*
public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button startStudyButton;
    [SerializeField] private TextMeshProUGUI errorMessage;
    [SerializeField] private GameObject canvas;

    [SerializeField] private GameObject bubbleCursor;
    [SerializeField] private GameObject pointCursor;

    private string activeCursorType;
    private string participantID;

    private void Awake()
    {
        errorMessage.text = string.Empty; // Clear any initial error messages
        SetActiveCursor(bubbleCursor); // Default cursor on start
    }

    private void Start()
    {
        // Hook up the button click event to a method
        startStudyButton.onClick.AddListener(OnStartStudyClicked);
    }

    // This method gets called when a user clicks "Start Study"
    private void OnStartStudyClicked()
    {
        participantID = inputField.text;

        // Check if both a cursor type is selected and participant ID is entered
        if (string.IsNullOrEmpty(participantID) || string.IsNullOrEmpty(activeCursorType))
        {
            errorMessage.text = "Error: Please select a cursor type and enter a valid Participant ID.";
            return;
        }

        // If both conditions are met, start the study
        StartStudy(participantID, activeCursorType);
    }

    public void StartStudy(string pID, string cursorType)
    {
        // Start the study with the given values
        Debug.Log("Study Started with Participant ID: " + pID + " and Cursor Type: " + cursorType);

        // Hide the canvas once the study starts
        canvas.SetActive(false);
    }

    // Method to switch the active cursor
    public void SetActiveCursor(GameObject cursor)
    {
        bubbleCursor.SetActive(false);
        pointCursor.SetActive(false);

        cursor.SetActive(true);

        // Set the cursor type based on which one is active
        activeCursorType = cursor == bubbleCursor ? "BubbleCursor" : "PointCursor";
    }

    // UI button for switching between bubble and point cursors
    public void OnBubbleCursorSelected()
    {
        SetActiveCursor(bubbleCursor);
    }

    public void OnPointCursorSelected()
    {
        SetActiveCursor(pointCursor);
    }
}
*/