using UnityEngine;

public class DeadZone : MonoBehaviour
{
    [SerializeField]
    private GameObject tpPoint;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Tp the player to the 0.0 position
            collision.gameObject.transform.position = tpPoint.transform.position;
            //Make the player take damage
            collision.gameObject.GetComponent<PlayerManager>().TakeDamage(1);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }

    }
}
