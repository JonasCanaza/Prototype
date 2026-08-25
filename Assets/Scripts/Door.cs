using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Door : MonoBehaviour
{
    [SerializeField] private Color openColor = Color.green;
    [SerializeField] private Color closedColor = Color.red;

    private Collider2D doorCollider;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        doorCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void SetOpen(bool open)
    {
        doorCollider.enabled = !open;

        if (open)
        {
            spriteRenderer.color = openColor;
        }
        else
        {
            spriteRenderer.color = closedColor;
        }
    }
}