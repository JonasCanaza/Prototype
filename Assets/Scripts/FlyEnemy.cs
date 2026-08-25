using UnityEngine;

[RequireComponent (typeof(CircleCollider2D))]
public class FlyEnemy : MonoBehaviour
{
    // enum = menu desplegable en el Inspector de Unity
    public enum FlyType { Patrullera, Perseguidora }

    [Header("Configuración General")]
    public FlyType tipoMosca;
    public float velocidad = 3f;
    public GameObject healthItemPrefab; // El prefab del corazón/bicho que cura

    [Header("Solo para Mosca Patrullera (Línea o Cuadrado)")]
    [Tooltip("Arrastrá acá los Empty GameObjects que sirven como puntos de ruta")]
    public Transform[] waypoints;
    private int indiceWaypointActual = 0;

    [Header("Solo para Mosca Perseguidora")]
    private Transform player;

    [Header("Vida del enemigo")]
    [SerializeField] private int life = 3;
    [SerializeField] private LayerMask layerArrow;

    private void Start()
    {
        // Si es perseguidora, busca al jugador automáticamente al arrancar
        if (tipoMosca == FlyType.Perseguidora)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    private void Update()
    {
        if (tipoMosca == FlyType.Patrullera)
        {
            Patrullar();
        }
        else if (tipoMosca == FlyType.Perseguidora)
        {
            Perseguir();
        }
    }

    private void Patrullar()
    {
        // Si no le asignaste puntos en el inspector, no hace nada para evitar errores
        if (waypoints.Length == 0) return;

        Transform destino = waypoints[indiceWaypointActual];

        // Nos movemos hacia el waypoint actual
        transform.position = Vector2.MoveTowards(transform.position, destino.position, velocidad * Time.deltaTime);

        // Si llegamos muy cerca del waypoint, pasamos al siguiente
        if (Vector2.Distance(transform.position, destino.position) < 0.1f)
        {
            // Esta cuenta matemática hace que cuando llegue al último waypoint, vuelva al primero (0)
            indiceWaypointActual = (indiceWaypointActual + 1) % waypoints.Length;
        }
    }

    private void Perseguir()
    {
        if (player != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, velocidad * Time.deltaTime);
        }
    }

    // --- INTERACCIONES ---

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el Trigger toca al jugador, le hace daño
        if (collision.CompareTag("Player"))
        {
            Debug.Log("La mosca tocó al jugador: -1 HP");
            collision.GetComponent<PlayerHealth>().RecibirDanio(1);
        }

        if (((1 << collision.gameObject.layer) & layerArrow) != 0)
        {
            Debug.Log("Colision con la flecha");
            life--;

            if (life <= 0)
            {
                Morir();
            }
        }
    }

    // Este método es PÚBLICO para que la Flecha o la Lengua puedan llamarlo al impactar
    public void RecibirAtaque()
    {
        Morir();
    }

    private void Morir()
    {
        // Si tenemos asignado el item de vida, lo instanciamos en la posición donde muere la mosca
        if (healthItemPrefab != null)
        {
            Instantiate(healthItemPrefab, transform.position, Quaternion.identity);
        }

        // Destruimos la mosca
        Destroy(gameObject);
    }
}