using System.Collections;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    public static TeleportManager Instance { get; private set; }
    private bool isTeleporting;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        isTeleporting = false;
    }

    public void Teleport(GameObject player, Portal sourcePortal)
    {
        if (isTeleporting) return;

        Portal destPortal = sourcePortal.destinationPortal;

        StartCoroutine(TeleportCoroutine(player, sourcePortal, destPortal));
    }

    private IEnumerator TeleportCoroutine(GameObject player, Portal source, Portal destination)
    {
        isTeleporting = true;

        Animator anim = player.GetComponent<Animator>();
        anim.SetBool("isTeleporting", true);

        float yOffset = -0.1f;
        player.transform.position = destination.transform.position + new Vector3(0, yOffset, 0);

        anim.SetBool("isTeleporting", false);
        yield return new WaitForSeconds(0.5f);

        isTeleporting = false;
    }
}
