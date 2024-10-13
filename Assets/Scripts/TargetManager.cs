using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {

        mainCamera = Camera.main;
        screenCentre = new Vector2(Screen.width/2, Screen.height / 2);
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
        }
        else if (numTargetsOnScreen == numTargets - 1)      // After targets are spawned, assuming only the main target was clicked
        {
            DestroyAllTargets();                            
            SetCursor(CursorType.PointCursor);              // Reset to "point cursor" only for start target selection 
            SpawnStartTarget();
        }
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
        List<Vector3> points = GenerateRandomPoints();
        List<float> randomSizes = GenerateRandomSizes();
        for (int i = 0; i < numTargets; i++)
        {
            GameObject targetObject = Instantiate(target, points[i], Quaternion.identity, transform);
            targetObject.transform.localScale = Vector3.one * randomSizes[i];

            if (i == numTargets - 1 && targetObject.TryGetComponent(out Target mainTarget))
            { 
                Debug.Log("Changing main target color to Red");
                mainTarget.ChangeColor(Color.red);
            }
        }
    }

    private void DestroyAllTargets()
    {
        foreach (GameObject target in targetObjects)
        {
            Destroy(target);
        }
    }

    /*
    private void CheckCenterTargetClicked()
    {
        Vector3 mousePosition = Input.mousePosition;

        //Change the z position so that cursor does not get occluded by the camera
        mousePosition.z += 9f;
        mousePosition.x = Mathf.Clamp(mousePosition.x, 0f, Screen.width);
        mousePosition.y = Mathf.Clamp(mousePosition.y, 0f, Screen.height);
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        // If a collider is hit and it matches the center target
        if (hit.collider != null && hit.collider.gameObject == centerTargetObject && !centerTargetClicked)
        {
            centerTargetClicked = true;
            Destroy(centerTargetObject);  // Remove the center target after clicking
            GameManager.Instance.selectedCursor = chosen;
            SpawnTargets();  // Spawn the remaining targets
        }
    }
    */

    List<Vector3> GenerateRandomPoints()
    {
        List<Vector3> pointList = new();
        for (int i = 0; i < numTargets-1; i++)
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
        for (int i = 0; i < numTargets-1; i++)
        {
            int randomIndex = Random.Range(0, targetSizes.Count);
            sizes.Add(targetSizes[randomIndex]);
        }

        return sizes;
    }

    /*
    private void GetAllColliders()
    {
        // Unity uses (0,0) for bottom-left of the screen
        Vector2 topLeftScreen = new Vector2(0, Screen.height);
        Vector2 bottomRightScreen = new Vector2(Screen.width, 0);

        Vector2 topLeftWorld = mainCamera.ScreenToWorldPoint(new Vector3(topLeftScreen.x, topLeftScreen.y, 10));
        Vector2 bottomRightWorld = mainCamera.ScreenToWorldPoint(new Vector3(bottomRightScreen.x, bottomRightScreen.y, 10));

        Physics2D.OverlapArea(topLeftWorld, bottomRightWorld, contactFilter, targetColliders);

        //Debug.Log(targetColliders.Count);
    }
    */

    private void GetAllTargets()
    {
        targetObjects = GameObject.FindGameObjectsWithTag("Target");
    }

    private void SetCursor(CursorType cursorType)
    {
        GameManager.Instance.selectedCursor = cursorType;
    }

}
