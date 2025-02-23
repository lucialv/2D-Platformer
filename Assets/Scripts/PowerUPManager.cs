using UnityEngine;
using System.Collections;

public class PowerUPManager : MonoBehaviour
{
    public void ActivateSwordPowerUP()
    {
        StartCoroutine(SwordPowerUP());
    }

    private IEnumerator SwordPowerUP()
    {
        PlayerManager.damage = 2;
        Debug.Log("Daño aumentado: " + PlayerManager.damage);
        yield return new WaitForSeconds(10);
        PlayerManager.damage = 1;
        Debug.Log("Daño restaurado: " + PlayerManager.damage);
    }
}
