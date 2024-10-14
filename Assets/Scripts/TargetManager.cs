using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public struct TrialConditions
{
    public float amplitude;     // Distance from the center
    public float width;         // Size of the target (main and distractors)
    public float EWToW_Ratio;   // Ratio of effective width to target size for distractor placement
}
public class StudySettings
{
    public List<float> widths;
    public List<float> amplitudes;
    public List<float> EWToW_Ratios;
    public int repetitions;
    public CursorType cursorType;

    public StudySettings(List<float> widths, List<float> amplitudes, List<float> EWToW_Ratios, int repetitions, CursorType cursorType)
    {
        this.widths = widths;
        this.amplitudes = amplitudes;
        this.EWToW_Ratios = EWToW_Ratios;
        this.repetitions = repetitions;
        this.cursorType = cursorType;
    }
    // Default constructor with 1 repetition
    public StudySettings(List<float> widths, List<float> amplitudes, List<float> EWToW_Ratios, CursorType cursorType)
    {
        this.widths = widths;
        this.amplitudes = amplitudes;
        this.EWToW_Ratios = EWToW_Ratios;
        this.repetitions = 1;
        this.cursorType = cursorType;
    }
}

public class TargetManager : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private List<float> targetSizes;
    [SerializeField] private List<float> targetAmplitudes;
    [SerializeField] private int numTargets;
    //[SerializeField] private ContactFilter2D contactFilter;

    //private List<Collider2D> targetColliders = new List<Collider2D>();
    private GameObject[] targetObjects;

    private Vector2 screenCentre;
    private Camera mainCamera;
    private GameObject centerTargetObject;
    private CursorType chosenCursor;

    // Hard coding values now, should be serializable in the future
    private StudySettings studySettings;
    List<TrialConditions> trialSequence;
    private int currentTrialIndex = 0;

    private void Start()
    {
        InitializeStudySettings();
        CreateSequenceOfTrials();

        mainCamera = Camera.main;
        screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
        chosenCursor = GameManager.Instance.selectedCursor;

        SetCursor(CursorType.PointCursor);
        SpawnStartTarget();
    }

    private void Update()
    {
        GetAllTargets();    // this puts all targets inside targetObjects array
        int numTargetsOnScreen = targetObjects.Length;

        // If 0 targets on screen, this means theat the start target was clicked.
        if (numTargetsOnScreen == 0)
        {
            SetCursor(chosenCursor);
            SpawnTargets();
            Debug.Log("Current trial number: " + currentTrialIndex++);
        }
        else if (numTargetsOnScreen == numTargets - 1)      // After targets are spawned, assuming only the main target was clicked
        {
            DestroyAllTargets();
            SetCursor(CursorType.PointCursor);              // Reset to "point cursor" only for start target selection 
            SpawnStartTarget();
        }

        if(currentTrialIndex >= trialSequence.Count)
        {
            SceneManager.LoadScene("End");
        }
    }

    // Only used once, but since this may be modified in the future to include custom settings I have made it its own function
    private void InitializeStudySettings()
    {
        studySettings = new StudySettings(
            new List<float> { 1f, 2f},             // Widths
            new List<float> { 50f, 100f},          // Amplitudes
            new List<float> { 1.5f, 2f},           // EW to W ratios
            GameManager.Instance.selectedCursor         // cursorType
        );
    }

    // Only used once, but since this may be modified in the future to include custom settings I have made it its own function
    private void CreateSequenceOfTrials()
    {
        trialSequence = new List<TrialConditions>();
        for (int i = 0; i < studySettings.repetitions; i++)
        {
            foreach (float EW in studySettings.EWToW_Ratios)
            {
                foreach (float size in studySettings.widths)
                {
                    foreach (float amp in studySettings.amplitudes)
                    {

                        trialSequence.Add(new TrialConditions
                        {
                            amplitude = amp,
                            width = size,
                            EWToW_Ratio = EW,
                        });
                    }
                }
            }
        }
        trialSequence = YatesShuffle<TrialConditions>(trialSequence);
    }

    private void SpawnStartTarget()
    {
        Vector3 screenPoint = new Vector3(screenCentre.x, screenCentre.y, 10f);  // Adjust 10f to be the distance from the camera
        Vector3 worldCenter = mainCamera.ScreenToWorldPoint(screenPoint);

        centerTargetObject = Instantiate(target, worldCenter, Quaternion.identity, transform);
        centerTargetObject.transform.localScale = Vector3.one * 0.5f;

        if (centerTargetObject.TryGetComponent(out Target centerTarget))
        {
            Debug.Log("Changing target color to Red");
            centerTarget.ChangeColor(Color.red);
        }
    }
    private void SpawnTargets()
    {
        SpawnMainTarget(trialSequence[currentTrialIndex]);
        SpawnOtherTargets();
    }


    private void SpawnMainTarget(TrialConditions trial)
    {
        // Calculate main target position based on amplitude
        Vector3 mainTargetPosition = mainCamera.ScreenToWorldPoint(
            new Vector3(screenCentre.x + trial.amplitude, screenCentre.y, 10f));

        // Instantiate the main target
        GameObject mainTargetObject = Instantiate(target, mainTargetPosition, Quaternion.identity, transform);
        mainTargetObject.transform.localScale = Vector3.one * trial.width;

        if (mainTargetObject.TryGetComponent(out Target mainTarget))
        {
            Debug.Log("Setting main target color to Red");
            mainTarget.ChangeColor(Color.red);  // Set the main target color to red
        }
    }

    private void SpawnOtherTargets()
    {
        List<Vector3> points = GenerateRandomPoints();
        List<float> randomSizes = GenerateRandomSizes();

        // Spawn random targets
        for (int i = 0; i < numTargets - 1; i++)
        {
            GameObject targetObject = Instantiate(target, points[i], Quaternion.identity, transform);
            targetObject.transform.localScale = Vector3.one * randomSizes[i];
        }
    }

    List<Vector3> GenerateRandomPoints()
    {
        List<Vector3> pointList = new();
        for (int i = 0; i < numTargets - 1; i++)
        {
            float randomX = Random.Range(0, Screen.width);
            float randomY = Random.Range(0, Screen.height);
            float z = 10f;
            Vector3 randomScreenPoint = new(randomX, randomY, z);
            Vector3 randomWorldPoint = mainCamera.ScreenToWorldPoint(randomScreenPoint);
            pointList.Add(randomWorldPoint);
        }
        return pointList;
    }

    List<float> GenerateRandomSizes()
    {
        List<float> sizes = new();
        for (int i = 0; i < numTargets - 1; i++)
        {
            int randomIndex = Random.Range(0, targetSizes.Count);
            sizes.Add(targetSizes[randomIndex]);
        }

        return sizes;
    }
    private static List<T> YatesShuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
        return list;
    }

    private void GetAllTargets()
    {
        targetObjects = GameObject.FindGameObjectsWithTag("Target");
    }

    private void DestroyAllTargets()
    {
        foreach (GameObject target in targetObjects)
        {
            Destroy(target);
        }
    }
    private void SetCursor(CursorType cursorType)
    {
        GameManager.Instance.selectedCursor = cursorType;
    }

}
