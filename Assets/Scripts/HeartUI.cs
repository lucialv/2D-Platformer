using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    [SerializeField]
    private GameObject[] hearts;
    [SerializeField]
    private Sprite emptyHeart;
    [SerializeField]
    private Sprite fullHeart;
    void Start()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].GetComponent<Image>().sprite = fullHeart;
        }
    }

    public void UpdateHearts(int heartsCount)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < heartsCount)
            {
                hearts[i].GetComponent<Image>().sprite = fullHeart;
            }
            else
            {
                hearts[i].GetComponent<Image>().sprite = emptyHeart;
            }
        }
    }
}
