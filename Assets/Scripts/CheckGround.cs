using UnityEngine;

public class CheckGround : MonoBehaviour
{
    public static bool isGrounded;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
            Debug.Log("Grounded");
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
            Debug.Log("Not Grounded");
        }
    }
}
