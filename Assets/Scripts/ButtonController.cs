using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ButtonController : MonoBehaviour
{
    [SerializeField] private LayerMask activators;
    [SerializeField] private Door door;

    //Collider2D activationCollider;
    List<Collider2D> activatorColliderList = new List<Collider2D>();

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //if (activationCollider != null)
        //{
        //    return;
        //}

        //if (((1 << collider.gameObject.layer) & activators) != 0)
        //{
        //    activatorColliders.Add(collider);
        //    //activationCollider = collider;

        //    door.SetOpen(true);

        //    Debug.Log("Boton presionado!");
        //}

        if (((1 << collider.gameObject.layer) & activators) != 0)
        {
            activatorColliderList.Add(collider);
        }

        if (activatorColliderList.Count >= 1)
        {
            door.SetOpen(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        //if (activationCollider != collider)
        //{
        //    return;
        //}

        //if (((1 << collider.gameObject.layer) & activators) != 0)
        //{
        //    activatorColliders.Remove(collider);
        //    //activationCollider = null;

        //    door.SetOpen(false);

        //    Debug.Log("Boton sin presionar!");
        //}

        if (((1 << collider.gameObject.layer) & activators) != 0)
        {
            activatorColliderList.Remove(collider);
        }

        if (activatorColliderList.Count <= 0)
        {
            door.SetOpen(false);
        }
    }
}