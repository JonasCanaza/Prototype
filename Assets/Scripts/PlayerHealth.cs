using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Estadísticas")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Invulnerabilidad")]
    public float tiempoInvulnerable = 1.5f;
    private bool isInvulnerable = false;

    // Referencia al SpriteRenderer para hacer que la rana parpadee al recibir daño
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        // Inicializamos la vida al máximo al arrancar el nivel
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Este método es público para que los enemigos lo puedan llamar
    public void RecibirDanio(int cantidad)
    {
        // Si el jugador está en sus frames de invulnerabilidad (o haciendo el dash), ignoramos el daño
        if (isInvulnerable) return;

        currentHealth -= cantidad;
        Debug.Log("Daño recibido! Vida restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            Morir();
        }
        else
        {
            // Si no murió, activamos la invulnerabilidad y el parpadeo
            StartCoroutine(RutinaInvulnerabilidad());
        }
    }

    // Este método es público para que el item de curación lo pueda llamar
    public void Curar(int cantidad)
    {
        currentHealth += cantidad;

        // Evitamos que la vida supere el máximo
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log("¡Jugador curado! Vida actual: " + currentHealth);
    }

    private IEnumerator RutinaInvulnerabilidad()
    {
        isInvulnerable = true;

        // Efecto visual: hacemos que el sprite parpadee
        int parpadeos = 5;
        float tiempoParpadeo = tiempoInvulnerable / (parpadeos * 2);

        for (int i = 0; i < parpadeos; i++)
        {
            // Ponemos el sprite un poco transparente (rojo y verde al máximo, azul a la mitad, alpha a la mitad)
            spriteRenderer.color = new Color(1f, 0.5f, 0.5f, 0.5f);
            yield return new WaitForSeconds(tiempoParpadeo);

            // Lo volvemos a la normalidad
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(tiempoParpadeo);
        }

        // Terminó el tiempo, vuelve a ser vulnerable
        isInvulnerable = false;
    }

    private void Morir()
    {
        Debug.Log("El jugador ha muerto. Reiniciando nivel...");
        // Por ahora lo resolvemos igual que las trampas: reseteamos la escena
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}