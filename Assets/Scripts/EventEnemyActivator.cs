using UnityEngine;

public class EventEnemyActivator : MonoBehaviour
{
    [SerializeField] private FlyEnemy[] enemies;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        ActivateEnemies();
    }

    private void ActivateEnemies()
    {
        foreach (FlyEnemy enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.Activate();
            }
        }
    }
}