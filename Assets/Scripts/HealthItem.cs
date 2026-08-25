using UnityEngine;

public class HealthItem : MonoBehaviour
{
    [Header("Configuración")]
    public int cantidadCuracion = 1;

    [Header("Efecto Visual (Opcional)")]
    public bool animarFlotacion = true;
    public float velocidadFlotacion = 4f;
    public float alturaFlotacion = 0.15f;

    private Vector2 posicionInicial;

    private void Start()
    {
        // Guardamos la posición donde spawneo para la animación
        posicionInicial = transform.position;
    }

    private void Update()
    {
        // Onda senoidal
        if (animarFlotacion)
        {
            float nuevaY = posicionInicial.y + Mathf.Sin(Time.time * velocidadFlotacion) * alturaFlotacion;
            transform.position = new Vector2(transform.position.x, nuevaY);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth saludJugador = collision.GetComponent<PlayerHealth>();

            if (saludJugador != null)
            {
                saludJugador.Curar(cantidadCuracion);

                // TODO: AudioSource pickup sound

                Destroy(gameObject);
            }
        }
    }
}