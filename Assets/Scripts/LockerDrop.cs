using System.Collections;
using UnityEngine;
using System.Linq;

public class LockerDrop : MonoBehaviour
{
    [SerializeField]
    private int lockerNumber;
    [SerializeField]
    private GameObject locker;
    private Animator anim;
    private AnimationClip animationClip;
    private bool droped;
    private void Start()
    {
        droped = false;
        anim = locker.GetComponent<Animator>();
        animationClip = anim.runtimeAnimatorController.animationClips.FirstOrDefault(clip => clip.name == $"Drop{lockerNumber}");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !droped)
        {
            StartCoroutine(DropLocker(lockerNumber));
            droped = true;
        }
    }
    private IEnumerator DropLocker(int number)
    {
        anim.SetTrigger($"Drop{number}");
        yield return new WaitForSeconds(animationClip.length);
        Destroy(locker);
        if (number == 1)
        {
            BoxLock.boxLocked1 = false;
        }
        else if (number == 2)
        {
            BoxLock.boxLocked2 = false;
        }
        else
        {
            BoxLock.boxLocked3 = false;
        }
    }
}
