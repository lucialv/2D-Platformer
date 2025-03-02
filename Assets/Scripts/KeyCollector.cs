
using UnityEngine;

public class KeyCollector : MonoBehaviour
{
    [SerializeField]
    private GameObject boxes;
    [SerializeField]
    private GameObject wind;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            BoxCollider2D boxCollider = boxes.GetComponent<BoxCollider2D>();
            boxCollider.enabled = true;
            Destroy(wind);
            Destroy(gameObject, 0.2f);
        }
    }
}
