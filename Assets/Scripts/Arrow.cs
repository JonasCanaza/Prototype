using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 20.0f;

    [Header("Capas (Layers)")]
    [SerializeField] private LayerMask enemigo;
    [SerializeField] private LayerMask pared;
    [SerializeField] private LayerMask jugador;

    private Rigidbody2D rb;
    private bool isIdle = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 shootDirection)
    {
        Vector2 direction = shootDirection.normalized;
        rb.linearVelocity = direction * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
    }

    // Usamos SOLO OnTriggerEnter2D.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Si la flecha ya está en el piso/pared, solo el jugador puede interactuar con ella
        if (isIdle)
        {
            if (((1 << collision.gameObject.layer) & jugador) != 0)
            {
                Debug.Log("Flecha recogida por el jugador");
                // A FUTURO: Acá podés sumarle +1 a la munición del jugador
                Destroy(gameObject);
            }
            return; // Cortamos acá para que no haga nada más si está tirada
        }

        // 2. Chequeamos si golpeó un ESCUDO
        EnemyShield escudo = collision.GetComponent<EnemyShield>();
        if (escudo != null)
        {
            escudo.RecibirAtaque(); // Le avisamos al escudo que lo golpearon
            Destroy(gameObject);    // La flecha se destruye
            return;
        }

        // 3. Chequeamos si golpeó un ENEMIGO directo (o sea, ya no tiene escudo)
        if (((1 << collision.gameObject.layer) & enemigo) != 0)
        {
            // Buscamos el script del enemigo (asumiendo que usamos FlyEnemy o similar)
            FlyEnemy enemigoScript = collision.GetComponent<FlyEnemy>();
            if (enemigoScript != null)
            {
                enemigoScript.RecibirAtaque(); // ¡AHORA SÍ LE HACEMOS DAÑO/LO MATAMOS!
            }

            Destroy(gameObject);
            return;
        }

        // 4. Chequeamos si golpeó una PARED
        if (((1 << collision.gameObject.layer) & pared) != 0)
        {
            // FRENAMOS la flecha para que se quede clavada
            rb.linearVelocity = Vector2.zero;
            isIdle = true;

            // Opcional: meter la flecha un poquito adentro de la pared para que se vea mejor
            // transform.position += (Vector3)rb.velocity.normalized * 0.2f; 
        }
    }
}