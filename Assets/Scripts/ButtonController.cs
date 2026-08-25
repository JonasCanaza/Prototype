using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ButtonController : MonoBehaviour
{
    [SerializeField] private LayerMask activators;
    [SerializeField] private Door door;

    private bool isPressed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & activators) != 0)
        {
            isPressed = true;

            door.SetOpen(true);

            Debug.Log("Boton presionado!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & activators) != 0)
        {
            isPressed = false;

            door.SetOpen(false);

            Debug.Log("Boton sin presionar!");
        }
    }
}