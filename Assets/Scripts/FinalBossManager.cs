using UnityEngine;

public class FinalBossManager : MonoBehaviour
{
    public static bool isInFinalBoss = false;
    public static bool isFinalBossDead = false;

    [SerializeField]
    private GameObject finalBossUI;

    private SkeletonBoss skeletonBoss;

    private void Start()
    {
        finalBossUI.SetActive(false);
        skeletonBoss = FindObjectOfType<SkeletonBoss>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInFinalBoss = true;
            finalBossUI.SetActive(true);
        }
    }

    public void FinalBossDead()
    {
        isFinalBossDead = true;
        finalBossUI.SetActive(false);
    }

    public void RestartFinalBoss()
    {
        finalBossUI.SetActive(false);
        skeletonBoss.ResetBoss();
    }
}