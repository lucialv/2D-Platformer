using UnityEngine;
using System.Collections;

public class EnemyFireballAttack : MonoBehaviour
{
    [Header("Fireball")]
    public GameObject fireballPrefab;          // Prefab de la bola de fuego
    public Transform firePoint;                // Punto de salida de la bola de fuego

    [SerializeField] private float fireballSpeed = 2.5f;   // Velocidad de la bola de fuego
    [SerializeField] private float minFireDelay = 0.85f;    // Tiempo mínimo entre disparos
    [SerializeField] private float maxFireDelay = 1.33f;    // Tiempo máximo entre disparos

    private float nextFireTime = 0f;           // Tiempo interno para controlar la cadencia de disparo
    private Animator animator;                 // Animator del enemigo

    private void Start()
    {
        // Obtenemos el Animator del enemigo
        animator = GetComponent<Animator>();

        // Inicializamos el tiempo para el primer disparo
        nextFireTime = Time.time + Random.Range(minFireDelay, maxFireDelay);
    }

    private void Update()
    {
        // Verificamos si ya pasó el tiempo para disparar
        if (Time.time >= nextFireTime)
        {
            StartCoroutine(FireballAttack());
            // Actualizamos el tiempo para el próximo disparo con un intervalo aleatorio
            nextFireTime = Time.time + Random.Range(minFireDelay, maxFireDelay);
        }
    }

    private void Fire()
    {
        // Instanciamos la bola de fuego en el firePoint
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);

        // Le damos velocidad a la bola de fuego
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.down * fireballSpeed;
        }
    }

    // IEnumerator para la animación del ataque con bola de fuego
    private IEnumerator FireballAttack()
    {
        animator.SetBool("isAttacking", true); // Activamos la animación de ataque
        yield return new WaitForSeconds(0.2f);
        Fire();
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("isAttacking", false); // Volvemos a la animación de idle
        yield return new WaitForSeconds(0.5f);
    }
}
