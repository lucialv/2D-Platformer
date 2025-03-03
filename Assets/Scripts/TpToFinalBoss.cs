using UnityEngine;

public class TpToFinalBoss : MonoBehaviour
{
    [SerializeField]
    private GameObject tpPoint;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.position = tpPoint.transform.position;
        }

    }
}
