using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Networking.UnityWebRequest;

public class BubbleCursor : MonoBehaviour
{
    [SerializeField] private ContactFilter2D contactFilter;

    private Camera mainCam;

    private Collider2D[] detectedColliders = null;  // only for debugging, should not need it here because only one collider should be detected
    private Collider2D detectedCollider = new();
    private Collider2D previousDetectedCollider = new();

    private Collider2D closestCollider = null;
    private Collider2D secondClosestCollider = null;

    private float radius;
    private List<Collider2D> targetColliders;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        GetAllColliders();
    }

    // Update is called once per frame
    void Update()
    {
        //Get Mouse Position on screen, and get the corresponding position in a Vector3 World Co-Ordinate
        Vector3 mousePosition = Input.mousePosition;

        //Change the z position so that cursor does not get occluded by the camera
        mousePosition.z += 9f;
        mousePosition.x = Mathf.Clamp(mousePosition.x, 0f, Screen.width);
        mousePosition.y = Mathf.Clamp(mousePosition.y, 0f, Screen.height);
        transform.position = mainCam.ScreenToWorldPoint(mousePosition);
        
        // For now, assuming 10 targets are always on screen for each task. 
        if (targetColliders.Count == 10)
        {
            // Following Grossman and Balakrishnan's (2005) algorithm to set radius
            GetClosestColliders();
            float containmentDistanceFromClosest = GetContainmentDistanceFromCollider(closestCollider);
            float intersectingDistanceFromSecondClosest = GetIntersectingDistanceFromCollider(secondClosestCollider);
            radius = Mathf.Min(containmentDistanceFromClosest, intersectingDistanceFromSecondClosest);

            // According to algorithm, the above radius is guaranteed to return exactly one collider, so use below line instead if there are no bugs in code
            // detectedCollider = Physics2D.OverlapCircle(transform.position, radius);

            int count = Physics2D.OverlapCircle(transform.position, radius, contactFilter, detectedColliders);

            //Detect how many targets
            //Change previous target back to default colour
            if (count < 1)
            {
                UnHoverPreviousTarget();
                return;
            }
            else if (count > 1)
            {
                UnHoverPreviousTarget();
                Debug.LogWarning("Too many targets in area");
                return;
            }
            else
            {
                detectedCollider = detectedColliders[0];
                UnHoverPreviousTarget(detectedCollider);
                HoverTarget(detectedCollider);
            }

            //On Mouse Click, select the closest target
            if (Input.GetMouseButtonDown(0))
            {
                SelectTarget(detectedCollider);
            }

            previousDetectedCollider = detectedCollider;
        }
    }
    // Intersecting Distance = shortest distance between cursor and collider
    private float GetIntersectingDistanceFromCollider(Collider2D collider)
    {
        return GetDistanceFromCollider(collider);
    }

    // Containment Distance = longest distance between cursor and collider (i.e shortest distance + 2*radius)
    private float GetContainmentDistanceFromCollider(Collider2D collider)
    {
        if (collider.TryGetComponent(out Target target))
        {
            return GetDistanceFromCollider(collider) + (2 * target.transform.localScale[0]);
        }
        else
        {
            Debug.LogWarning("Not a valid target.");
            return 0f;
        }
    }

    private void GetClosestColliders()
    {
        float minDistance = Mathf.Infinity;
        float secondMinDistance = Mathf.Infinity;

        foreach (Collider2D collider in targetColliders)
        {
            float distance = GetDistanceFromCollider(collider);

            if (distance > minDistance && distance < secondMinDistance)
            {
                secondClosestCollider = collider;
            }
            if (distance <= minDistance)
            {
                secondClosestCollider = closestCollider;
                closestCollider = collider;
            }
        }
    }

    private float GetDistanceFromCollider(Collider2D collider)
    {
        Vector2 closest_point_on_target = Physics2D.ClosestPoint((Vector2) transform.position, collider);
        return Vector2.Distance((Vector2) transform.position, closest_point_on_target);
    }

    private void GetAllColliders()
    {
        // Unity uses (0,0) for bottom-left of the screen
        Vector2 topLeftScreen = new Vector2 (0, Screen.height);
        Vector2 bottomRightScreen = new Vector2 (Screen.width, 0);
        Physics2D.OverlapArea(topLeftScreen, bottomRightScreen, contactFilter, targetColliders);

        if (targetColliders.Count != 10) { Debug.Log("There is an error with fetching colliders."); }
    } 

    private void HoverTarget(Collider2D collider)
    {
        if (collider.TryGetComponent(out Target target))
        {
            target.OnHoverEnter();
        }
        else
        {
            Debug.LogWarning("Not a valid Target?");
        }
    }

    private void UnHoverPreviousTarget()
    {
        if (previousDetectedCollider != null)
        {
            if (previousDetectedCollider.TryGetComponent(out Target t))
            {
                t.OnHoverExit();
            }
        }
    }

    private void UnHoverPreviousTarget(Collider2D collider)
    {
        //Checking if the target detected in previous and current frame are the same
        //If target changes, change the previous target back to default colour
        if (previousDetectedCollider != null && collider != previousDetectedCollider)
        {
            if (previousDetectedCollider.TryGetComponent(out Target t))
            {
                t.OnHoverExit();
            }
        }
    }



    void SelectTarget(Collider2D collider)
    {
        if (collider.TryGetComponent(out Target target))
        {
            target.OnSelect();
        }
    }
}
