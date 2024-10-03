using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Networking.UnityWebRequest;

public class BubbleCursor : MonoBehaviour
{
    [SerializeField] private ContactFilter2D contactFilter;
    [SerializeField] private GameObject secondaryCircle;

    private Camera mainCam;

    private List<Collider2D> detectedColliders = new List<Collider2D>();  // only for debugging, should not need it here because only one collider should be detected
    private Collider2D detectedCollider = new();
    private Collider2D previousDetectedCollider = new();

    private Collider2D closestCollider = null;
    private Collider2D secondClosestCollider = null;

    private float radius;
    private List<Collider2D> targetColliders = new List<Collider2D>();

    private SpriteRenderer spriteRenderer = null;

    private GameObject[] targets;
    private GameObject closestTarget; 
    private GameObject secondClosestTarget;
    private SpriteMask cursorMask;
    

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start called on Bubble");
    }

    private void Awake()
    {
        mainCam = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        cursorMask = GetComponent<SpriteMask>();
        Debug.Log("Awake called on Bubble");
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

        targets = GameObject.FindGameObjectsWithTag("Target");

        float minDistance = Mathf.Infinity, secondMinDistance = Mathf.Infinity;

        foreach (GameObject target in targets)
        {
            float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
            distanceToTarget = distanceToTarget - (target.transform.localScale.x)/2; //subtract the radius 

            if(distanceToTarget < minDistance)
            {
                minDistance = distanceToTarget;
                closestTarget = target;
            }
        }

        /*
        foreach (GameObject target in targets)
        {
            float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
            distanceToTarget = distanceToTarget - (target.transform.localScale.x) / 2; //subtract the radius 

            if (distanceToTarget < secondMinDistance && target != closestTarget)
            {
                secondMinDistance = distanceToTarget;
                secondClosestTarget = target;
            }
        }

        // Get containment distance to closest target
        float containmentDistanceFromClosest = Vector2.Distance(transform.position, closestTarget.transform.position) + (closestTarget.transform.localScale.x / 2);

        // Get intersection distance to second-closest target
        float intersectionDistanceFromSecondClosest = Vector2.Distance(transform.position, secondClosestTarget.transform.position) - (secondClosestTarget.transform.localScale.x / 2f);
        float scaleFactor = (intersectionDistanceFromSecondClosest > containmentDistanceFromClosest) ? 1.8f : 2f; 

        float lengthOfRadius = Mathf.Min(containmentDistanceFromClosest, intersectionDistanceFromSecondClosest);
        transform.localScale = new Vector2(lengthOfRadius * 2, lengthOfRadius * 2);
        radius = lengthOfRadius;
        */

        transform.localScale = new Vector2(minDistance * 2, minDistance * 2);
        radius = minDistance;

        if(cursorMask != null)
        {
            cursorMask.transform.localScale = new Vector2(radius*2, radius*2);
            Debug.Log(cursorMask.transform.localScale.x);
            Debug.Log(radius);
        }

        // According to algorithm, the above radius is guaranteed to return exactly one collider, so use below line instead if there are no bugs in code
        // detectedCollider = Physics2D.OverlapCircle(transform.position, radius);

        int count = Physics2D.OverlapCircle(transform.position, radius, contactFilter, detectedColliders);

        if (closestTarget != null && secondaryCircle != null)
        {
            Vector2 closestPointOnTarget = Physics2D.ClosestPoint((Vector2)transform.position, closestTarget.GetComponent<Collider2D>());
            // Position the secondary circle at the same position as the closest target
            secondaryCircle.transform.position = closestPointOnTarget;

            // Adjust the size of the secondary circle to fully cover the closest target
            float circleRadius = closestTarget.transform.localScale.x *2;
            secondaryCircle.transform.localScale = Vector2.Lerp(secondaryCircle.transform.localScale, new Vector2(circleRadius, circleRadius), Time.deltaTime * 5);
        }

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
        /*
        GetAllColliders();

        // For now, assuming 10 targets are always on screen for each task. 
        if (targetColliders.Count > 1)
        {
            // Following Grossman and Balakrishnan's (2005) algorithm to set radius
            GetClosestColliders();
            float containmentDistanceFromClosest = GetContainmentDistanceFromCollider(closestCollider);
            float intersectingDistanceFromSecondClosest = GetIntersectingDistanceFromCollider(secondClosestCollider);
            radius = Mathf.Min(containmentDistanceFromClosest, intersectingDistanceFromSecondClosest);
            if (spriteRenderer != null)
            {
                Debug.Log("Radius set to: " + radius);
                float scaleFactor = radius / spriteRenderer.bounds.extents.x; 
                spriteRenderer.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
            }

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

        */
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

        Collider2D newClosestCollider = null;
        Collider2D newSecondClosestCollider = null;

        foreach (Collider2D collider in targetColliders)
        {
            float distance = GetDistanceFromCollider(collider);

            if (distance > minDistance && distance < secondMinDistance)
            {
                newSecondClosestCollider = collider;
            }
            if (distance <= minDistance)
            {
                newSecondClosestCollider = newClosestCollider;
                newClosestCollider = collider;
            }
        }

        if (newClosestCollider.TryGetComponent(out Target newClosestTarget) && newSecondClosestCollider.TryGetComponent(out Target newSecondClosestTarget))
        {
            if((closestCollider != null && secondClosestCollider != null) 
                && closestCollider.TryGetComponent(out Target oldClosestTarget) 
                && secondClosestCollider.TryGetComponent(out Target oldSecondClosestTarget))
            {
                oldClosestTarget.ChangeColor(Color.white);
                oldSecondClosestTarget.ChangeColor(Color.white);
            }

            newClosestTarget.ChangeColor(Color.yellow);
            newSecondClosestTarget.ChangeColor(Color.blue);
        }

        closestCollider = newClosestCollider;
        secondClosestCollider = newSecondClosestCollider;
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

        Vector2 topLeftWorld = mainCam.ScreenToWorldPoint(new Vector3(topLeftScreen.x, topLeftScreen.y, 10));
        Vector2 bottomRightWorld = mainCam.ScreenToWorldPoint(new Vector3(bottomRightScreen.x, bottomRightScreen.y, 10));

        Physics2D.OverlapArea(topLeftWorld, bottomRightWorld, contactFilter, targetColliders);

        Debug.Log(targetColliders.Count);
        if (targetColliders.Count == 0) { Debug.Log("There is an error with fetching colliders."); }
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
