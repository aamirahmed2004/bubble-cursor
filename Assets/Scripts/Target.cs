using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private SpriteRenderer sprite;
    private bool onSelect;
    private Color originalColor;

    void Start()
    {
        Debug.Log("Start called on Target");
        sprite = GetComponent<SpriteRenderer>();
        originalColor = sprite.color;  // Store the original color
    }

    public void OnHighlight()
    {
        // Add your highlight logic here if needed
    }

    public void OnHoverEnter()
    {
        if (onSelect) return;
        sprite.color = Color.yellow;
    }

    public void OnHoverExit()
    {
        if (onSelect) return;
        sprite.color = originalColor;  // Revert to the original color (white by default)
    }

    public void OnSelect()
    {
        onSelect = true;
        sprite.color = Color.green;
        StartCoroutine(DestroyGameObject(0.1f));
    }

    public void ChangeColor(Color color)
    {
        if (sprite == null) 
            sprite = GetComponent<SpriteRenderer>();

        sprite.color = color;
        originalColor = color;  // Update the original color if the color is changed
    }

    public IEnumerator DestroyGameObject(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
