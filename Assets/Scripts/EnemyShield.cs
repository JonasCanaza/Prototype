using System.Collections;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    [Header("Configuración del Escudo")]
    [Tooltip("El enemigo principal que porta este escudo")]
    public GameObject enemigoPadre;

    [Tooltip("¿El escudo se regenera después de un tiempo?")]
    public bool seRegenera = false;
    public float tiempoRegeneracion = 5f;

    private Collider2D shieldCollider;
    private SpriteRenderer shieldSprite;

    private void Awake()
    {
        shieldCollider = GetComponent<CircleCollider2D>();
        shieldSprite = GetComponent<SpriteRenderer>();
    }

    // Método que llamará la flecha al impactar
    public void RecibirAtaque()
    {
        // Las flechas rebotan o se destruyen sin hacer daño
        Debug.Log("¡Ataque bloqueado por el escudo!");
    }

    // Método que llamará la lengua al impactar
    public void ArrancarEscudo()
    {
        Debug.Log("¡Escudo arrancado por la lengua!");

        // Desactivamos la colisión y la visual del escudo
        shieldCollider.enabled = false;
        shieldSprite.enabled = false;

        // Avisarle al padre que perdió el escudo para que cambie su comportamiento
        enemigoPadre.SendMessage("AlPerderEscudo", SendMessageOptions.DontRequireReceiver);

        if (seRegenera)
        {
            StartCoroutine(RegenerarEscudo());
        }
        else
        {
            // Si no se regenera, podemos destruir el objeto del escudo tras ser atraído
            Destroy(gameObject, 1f); // Damos 1 segundo para que la animación de la lengua termine
        }
    }

    private IEnumerator RegenerarEscudo()
    {
        yield return new WaitForSeconds(tiempoRegeneracion);

        // Reseteamos su posición relativa al enemigo padre (0,0,0)
        transform.localPosition = Vector3.zero;

        // Animación de regeneración
        shieldSprite.color = new Color(1f, 1f, 1f, 0.5f);
        shieldSprite.enabled = true;
        yield return new WaitForSeconds(0.5f);
        shieldSprite.color = Color.white;
        shieldCollider.enabled = true;
    }
}