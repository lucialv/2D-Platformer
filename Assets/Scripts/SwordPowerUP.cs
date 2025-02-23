using UnityEngine;

public class SwordPowerUP : MonoBehaviour
{
    private PowerUPManager powerUPManager;

    private void Start()
    {
        powerUPManager = GameObject.FindGameObjectWithTag("PowerUPManager").GetComponent<PowerUPManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            powerUPManager.ActivateSwordPowerUP();
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

}
