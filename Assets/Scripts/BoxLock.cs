using UnityEngine;
using System.Collections;
using System.Linq;

public class BoxLock : MonoBehaviour
{
    public static bool boxLocked1 = true;
    public static bool boxLocked2 = true;
    public static bool boxLocked3 = true;
    [SerializeField]
    private int boxId;
    private Animator anim;
    private AnimationClip animationClip;
    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        animationClip = anim.runtimeAnimatorController.animationClips.FirstOrDefault(clip => clip.name == "Destroy");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack") && !boxLocked1 && boxId == 1)
        {
            StartCoroutine(DestroyBox());
        }
        else if (other.CompareTag("PlayerAttack") && !boxLocked2 && boxId == 2)
        {
            StartCoroutine(DestroyBox());
        }
        else if (other.CompareTag("PlayerAttack") && !boxLocked3 && boxId == 3)
        {
            StartCoroutine(DestroyBox());
        }
    }

    private IEnumerator DestroyBox()
    {
        anim.SetTrigger("Destroy");
        yield return new WaitForSeconds(animationClip.length);
        Destroy(gameObject);
    }
}
