using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleCursor : MonoBehaviour
{
    [SerializeField] private ContactFilter2D contactFilter;

    private Camera mainCam;
    private Collider2D detectedCollider = new();
    private Collider2D previousDetectedCollider = new();
    private Target closest_target = null;
    private float radius;
    private List<Collider2D> targetColliders;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
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

        GetAllTargets();  
        Target closest_target = null;
        Target second_closest_target = null;

        float min_distance = 0f;

        foreach(Collider2D collider in targetColliders)
        {
            if (!collider.TryGetComponent(out Target target))
            {
                Debug.LogWarning("Not a valid target.");
                continue;
            }

            float distance = GetDistanceFromTarget(collider);
            if (distance < min_distance)
            {
                second_closest_target = closest_target;
                closest_target = target;
            }

        }
    }

    private float GetDistanceFromTarget(Collider2D collider)
    {
        Vector2 closest_point_on_target = Physics2D.ClosestPoint((Vector2) transform.position, collider);
        return Vector2.Distance((Vector2) transform.position, closest_point_on_target);
    }

    private void GetAllTargets()
    {
        // Unity uses (0,0) for bottom-left of the screen
        Vector2 topLeftScreen = new Vector2 (0, Screen.height);
        Vector2 bottomRightScreen = new Vector2 (Screen.width, 0);
        Physics2D.OverlapArea(topLeftScreen, bottomRightScreen, contactFilter, targets);
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
