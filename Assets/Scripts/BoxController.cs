using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BoxController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float pushSpeed = 0.05f;

    private Rigidbody2D rb;
    private Vector2 pushDirection;
    private bool isBeingPushed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (isBeingPushed)
        {
            rb.linearVelocity = pushDirection * pushSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void Push(Vector2 direction)
    {
        pushDirection = direction.normalized;
        isBeingPushed = true;
    }

    public void StopPushing()
    {
        isBeingPushed = false;
        rb.linearVelocity = Vector2.zero;
    }
}