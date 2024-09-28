using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private GameObject areaCursor;
    [SerializeField] private GameObject pointCursor;

    // Start is called before the first frame update
    void Start()
    {
        SetActiveCursor(areaCursor);
    }

    // Update is called once per frame
    void Update()
    {
        // Toggle cursor using the 'C' key
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (areaCursor.activeSelf)
            {
                SetActiveCursor(pointCursor);
            }
            else
            {
                SetActiveCursor(areaCursor);
            }
        }
    }

    void SetActiveCursor(GameObject cursor)
    {
        areaCursor.SetActive(false);
        pointCursor.SetActive(false);

        cursor.SetActive(true);
    }
}
