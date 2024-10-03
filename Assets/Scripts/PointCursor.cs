using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCursor : MonoBehaviour
{
    private Camera mainCam;
    private Collider2D detectedCollider = null;
    private Collider2D previousDetectedCollider = new();

    void Start()
    {
        Debug.Log("Start called on Point");
    }

    private void Awake()
    {
        mainCam = Camera.main;
        Debug.Log("Awake called on Point");
    }

    void Update()
    {
        //Get Mouse Position on screen, and get the corresponding position in a Vector3 World Co-Ordinate
        Vector3 mousePosition = Input.mousePosition;

        //Change the z position so that cursor does not get occluded by the camera
        mousePosition.z += 9f;
        mousePosition.x = Mathf.Clamp(mousePosition.x, 0f, Screen.width);
        mousePosition.y = Mathf.Clamp(mousePosition.y, 0f, Screen.height);

        transform.position = mainCam.ScreenToWorldPoint(mousePosition);

        // Casting a ray straight down, below the cursor
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);

        //Checking if cursor is hovering over targets
        if(hit.collider != null)
        {
            detectedCollider = hit.collider;
            UnHoverPreviousTarget(detectedCollider);
            HoverTarget(detectedCollider);
        }
        else
        {
            UnHoverPreviousTarget();
        }

        // If cursor is clicked, select the target
        if (Input.GetMouseButtonDown(0))
        {
            SelectTarget(detectedCollider);
        }

        previousDetectedCollider = detectedCollider;
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
