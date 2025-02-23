using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal destinationPortal;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Vector2.Distance(collision.transform.position, transform.position) < 0.8f)
            {
                TeleportManager.Instance.Teleport(collision.gameObject, this);
            }
        }
    }
}
