using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float velocity = 6.0f;
    [SerializeField] private bool isDiagonalMovement = true;

    [Header("Tongue / Dash")]
    [SerializeField] private float tongueRange = 5.0f;
    [SerializeField] private float dashSpeed = 15.0f;
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private LayerMask hazardsLayer;
    [SerializeField] private LayerMask grabbableLayer;

    [Header("Arrow")]
    [SerializeField] private Arrow arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;

    [Header("Booger")]
    [SerializeField] private Booger boogerPrefab;
    [SerializeField] private Transform boogerSpawnPoint;

    private Vector2 directionInput;
    private Rigidbody2D rb;
    private bool isDashing = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isDashing)
        {
            return;
        }

        float moverX = Input.GetAxisRaw("Horizontal");
        float moverY = Input.GetAxisRaw("Vertical");

        // MOVEMENT INPUT
        // Si el jugador se mueve en X, forzamos la Y a 0 para anular la diagonal.
        // If player moves in X, we force Y=0 to prevent diagonals.
        if (moverX != 0f && !isDiagonalMovement)
        {
            moverY = 0f;
        }

        directionInput = new Vector2(moverX, moverY);

        // Rotar la rana (0, 90, 180 o -90°)
        // Rotate frog(0, 90, 180 o -90°)
        if (directionInput != Vector2.zero)
        {
            float angle = Mathf.Atan2(directionInput.y, directionInput.x) * Mathf.Rad2Deg;
            angle = Mathf.Round(angle / 90.0f) * 90.0f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // SPECIAL INPUTS
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ShootTongue();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            ShootArrow();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ShootBooger();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isDiagonalMovement = !isDiagonalMovement;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();      
#endif
        }
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            rb.linearVelocity = directionInput * velocity;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check Hazard collision
        if (((1 << collision.gameObject.layer) & hazardsLayer) != 0)
        {
            // If not dashing, die
            if (!isDashing)
            {
                Debug.Log("DEATH.FELL INTO A TRAP >:)");
                ResetLevel();
            }
            else
            {
                // If dashing, invulnerable
                Debug.Log("INVINCIBLE");
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        BoxController box = collision.gameObject.GetComponent<BoxController>();

        if (box != null)
        {
            box.Push(directionInput);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        BoxController box = collision.gameObject.GetComponent<BoxController>();

        if (box != null)
        {
            box.StopPushing();
        }
    }

    private void ShootTongue()
    {
        Vector2 facingDirection = transform.right;

        // Combinamos ambas layers en un solo raycast
        LayerMask combinedMask = grappleLayer | grabbableLayer;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, facingDirection, tongueRange, combinedMask);

        if (hit.collider != null)
        {
            AudioSource audioAux = GetComponent<AudioSource>();
            if (audioAux != null) audioAux.Play();

            // Chequeamos qué tipo de objeto golpeamos en orden de prioridad
            EnemyShield shield = hit.collider.GetComponent<EnemyShield>();
            GrabbableItem grabbable = hit.collider.GetComponent<GrabbableItem>();

            if (shield != null)
            {
                // Es un escudo: lo arrancamos y lo traemos hacia el jugador
                Debug.Log("Tongue grabbed shield: " + hit.collider.name);
                StartCoroutine(PullShieldToPlayer(shield));
            }
            else if (grabbable != null)
            {
                // Es un objeto recolectable de tu compañero
                Debug.Log("Tongue grabbed item: " + hit.collider.name);
                grabbable.Grab(transform);
            }
            else
            {
                // Es un punto de grapple normal: el jugador dashea hacia él
                Debug.Log("Tongue hit: " + hit.collider.name);
                StartCoroutine(DashToTarget(hit.point));
            }
        }
        else
        {
            Debug.Log("Tongue didn't hit anything");
        }
    }

    private IEnumerator PullShieldToPlayer(EnemyShield shield)
    {
        // Reutilizamos isDashing para que el jugador se quede quieto mientras tira con la lengua
        isDashing = true;
        rb.linearVelocity = Vector2.zero;

        Transform shieldTransform = shield.transform;

        // Mientras el escudo exista y no haya llegado a la rana
        while (shieldTransform != null && Vector2.Distance(shieldTransform.position, transform.position) > 0.5f)
        {
            // Movemos físicamente el escudo hacia nosotros (reutilizo dashSpeed)
            shieldTransform.position = Vector2.MoveTowards(shieldTransform.position, transform.position, dashSpeed * Time.deltaTime);
            yield return null;
        }

        if (shield != null)
        {
            shield.ArrancarEscudo();
        }

        // Le devolvemos el control al jugador
        isDashing = false;
    }

    private IEnumerator DashToTarget(Vector2 target)
    {
        isDashing = true;
        rb.linearVelocity = Vector2.zero;

        while (Vector2.Distance(transform.position, target) > 0.5f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, dashSpeed * Time.deltaTime);
            yield return null;
        }

        isDashing = false;
    }

    private void ResetLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    private void OnDrawGizmosSelected()
    {
        // Total tongue range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, tongueRange);

        // Tongue current aim
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * tongueRange);
    }

    private void ShootArrow()
    {
        Arrow arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);

        arrow.Initialize(transform.right);
    }

    private void ShootBooger()
    {
        Booger arrow = Instantiate(boogerPrefab, boogerSpawnPoint.position, Quaternion.identity);

        arrow.Initialize(transform.right);
    }
}