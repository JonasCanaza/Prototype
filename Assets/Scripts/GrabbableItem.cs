using UnityEngine;

public class GrabbableItem : MonoBehaviour
{
    [SerializeField] private float attractionSpeed = 15.0f;

    private Transform target;
    private bool isBeingAttracted;

    public void Grab(Transform target)
    {
        this.target = target;
        isBeingAttracted = true;
    }

    private void Update()
    {
        if (!isBeingAttracted)
        {
            return;
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            attractionSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            isBeingAttracted = false;

            Destroy(gameObject);
        }
    }
}