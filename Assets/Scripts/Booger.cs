using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Booger : MonoBehaviour
{
    [SerializeField] private float speed = 20.0f;

    private Rigidbody2D rb;
    private Vector2 direction;

    [SerializeField] private LayerMask enemigo;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 shootDirection)
    {
        direction = shootDirection.normalized;

        rb.linearVelocity = direction * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & enemigo) != 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & enemigo) != 0)
        {
            Destroy(gameObject);
        }
    }
}