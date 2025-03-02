using System.Collections;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    private PlayerManager player;
    private Coroutine healingCoroutine;
    private bool isPlayerInside = false;

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && healingCoroutine == null)
        {
            isPlayerInside = true;
            healingCoroutine = StartCoroutine(HealOverTime());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInside = false;
            if (healingCoroutine != null)
            {
                StopCoroutine(healingCoroutine);
                healingCoroutine = null;
            }
        }
    }

    private IEnumerator HealOverTime()
    {
        while (isPlayerInside && player.GetCurrentHealth() != player.GetMaxHealth())
        {
            player.GainHealth(1);
            yield return new WaitForSeconds(1f);
        }
        healingCoroutine = null;  // Por si acaso
    }
}
