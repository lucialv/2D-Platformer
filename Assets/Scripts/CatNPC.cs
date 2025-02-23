using System.Collections;
using TMPro;
using UnityEngine;

public class CatNPC : MonoBehaviour
{
    [Header("Elementos UI")]
    public GameObject tooltip;         // Objeto UI que muestra "F to talk"
    public TextMeshPro dialogText;     // Componente Text para mostrar el mensaje

    [Header("Configuración del Diálogo")]
    public string fullText = "meow meow meow!";
    public float typingSpeed = 0.05f;    // Velocidad de aparición de cada letra

    private bool playerInRange = false;
    private bool dialogActive = false;

    void Start()
    {
        // Inicialmente desactivamos el tooltip.
        tooltip.SetActive(false);
    }

    // Se activa cuando el jugador entra en el área del trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            // Solo muestra el tooltip si no está activo un diálogo.
            if (!dialogActive)
                tooltip.SetActive(true);
        }
    }

    // Se activa cuando el jugador sale del área del trigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            tooltip.SetActive(false);
            playerInRange = false;
        }
    }

    void Update()
    {
        // Si el jugador está cerca y presiona la tecla F, se inicia el diálogo.
        if (playerInRange && !dialogActive && Input.GetKeyDown(KeyCode.F))
        {
            tooltip.SetActive(false);
            dialogActive = true;
            StartCoroutine(TypeSentence());
        }
    }

    // Coroutine para mostrar el texto letra por letra, esperar 2 segundos y luego ocultarlo.
    IEnumerator TypeSentence()
    {
        dialogText.text = "";
        foreach (char letter in fullText.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Espera 2 segundos con el diálogo completamente escrito
        yield return new WaitForSeconds(2f);

        // Limpia el texto y finaliza el diálogo
        dialogText.text = "";
        dialogActive = false;

        // Si el jugador sigue en rango, se vuelve a mostrar el tooltip.
        if (playerInRange)
        {
            tooltip.SetActive(true);
        }
    }
}
