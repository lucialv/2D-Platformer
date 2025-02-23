using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyBehaviour : MonoBehaviour
{
    public Rigidbody2D rb;
    private Animator anim;
    private float speed = 0.6f;
    private SpriteRenderer sprite;
    private bool isDead = false;
    private bool isTakingDamage = false;
    private int health = 3;

    private bool movingRight = true; // Dirección inicial del movimiento
    private BoxCollider2D hitBox;
    private float hitBoxOffsetX;

    public bool killedThePlayer = false;

    private AnimationClip dieClip;

    public LayerMask groundLayer;
    public LayerMask playerLayer; // Añadir una capa para detectar al jugador

    private Transform player; // Referencia al transform del jugador

    private float eyeHeight = 0.2f; // Altura de los ojos del enemigo

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        anim.SetBool("isWalking", true);
        sprite = GetComponent<SpriteRenderer>();
        hitBox = GetComponent<BoxCollider2D>();
        hitBoxOffsetX = hitBox.offset.x;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform; // Buscar al jugador
        }

        dieClip = anim.runtimeAnimatorController.animationClips.FirstOrDefault(clip => clip.name == "dead");

    }

    void Update()
    {
        if (killedThePlayer)
        {
            rb.velocity = new Vector2(0, 0);
            anim.SetBool("isRunning", false);
            anim.SetBool("isWalking", false);
        }
        else if (!isTakingDamage && !isDead)
        {
            MoveEnemy();
        }
    }

    void MoveEnemy()
    {
        // Detectar si hay un muro usando un Raycast hacia adelante
        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, movingRight ? Vector2.right : Vector2.left, 0.2f, groundLayer);
        Debug.DrawRay(transform.position, movingRight ? Vector2.right * 0.2f : Vector2.left * 0.2f, Color.red);

        // Detectar si hay un precipicio usando un Raycast hacia abajo
        RaycastHit2D hitGround = Physics2D.Raycast(transform.position + new Vector3(movingRight ? 0.2f : -0.2f, 0, 0), Vector2.down, 0.2f, groundLayer);
        Debug.DrawRay(transform.position + new Vector3(movingRight ? 0.2f : -0.2f, 0, 0), Vector2.down * 0.2f, Color.blue);

        // Raycast para detectar al jugador a la altura de los ojos
        Vector2 raycastOrigin = new Vector2(transform.position.x, transform.position.y + eyeHeight); // Ajustar la posición de inicio
        RaycastHit2D hitPlayer = Physics2D.Raycast(raycastOrigin, movingRight ? Vector2.right : Vector2.left, 2f, playerLayer);
        Debug.DrawRay(raycastOrigin, movingRight ? Vector2.right * 2f : Vector2.left * 2f, Color.green);
        RaycastHit2D hitObstacle = new RaycastHit2D();
        if (player != null)
        {
            // Raycast para verificar si hay una pared entre el jugador y el enemigo
            Vector2 raycastToPlayer = player.position - transform.position;
            hitObstacle = Physics2D.Raycast(raycastOrigin, raycastToPlayer.normalized, raycastToPlayer.magnitude, groundLayer);
            Debug.DrawRay(raycastOrigin, raycastToPlayer.normalized * raycastToPlayer.magnitude, Color.yellow);
        }


        // Si la velocidad en el eje Y es mayor que un umbral (indica que está cayendo o saltando), no cambiar la dirección
        if (Mathf.Abs(rb.velocity.y) > 0.1f)
        {
            return; // Ignorar la lógica de giro si está en el aire
        }

        // Si el jugador es detectado y no hay obstáculos entre ellos, perseguirlo
        if (hitPlayer.collider != null && hitPlayer.collider.CompareTag("Player") && hitObstacle.collider == null && !killedThePlayer)
        {
            ChasePlayer();
        }
        else
        {
            anim.SetBool("isRunning", false); // Asegurarse de que el enemigo no está corriendo
            // Si hay un muro o no hay suelo (precipicio), cambiar la dirección
            if (hitWall.collider != null || hitGround.collider == null)
            {
                Flip();
            }

            // Mover en la dirección correcta
            if (movingRight)
            {
                rb.velocity = new Vector2(speed, rb.velocity.y); // Mantener la velocidad en Y

            }
            else
            {
                rb.velocity = new Vector2(-speed, rb.velocity.y); // Mantener la velocidad en Y
            }
        }

        // Verificar si la velocidad en el eje X es cercana a 0
        if (Mathf.Abs(rb.velocity.x) < 0.01f) // Utilizamos una tolerancia más baja
        {
            anim.SetBool("isWalking", false); // Detener la animación de caminar
        }
        else
        {
            anim.SetBool("isWalking", true); // Asegurarse de que la animación de caminar esté activa
        }
    }

    void Flip()
    {
        // Cambiar la dirección de movimiento y la animación
        movingRight = !movingRight;
        sprite.flipX = !sprite.flipX;
        // Change hitbox
        if (movingRight)
        {
            hitBox.offset = new Vector2(hitBoxOffsetX, hitBox.offset.y);
        }
        else
        {
            hitBox.offset = new Vector2(-hitBoxOffsetX, hitBox.offset.y);
        }
    }

    void ChasePlayer()
    {
        if (player == null)
        {
            return;
        }
        // Persigue al jugador si es detectado
        if (player != null)
        {
            // Si el jugador está a la derecha del enemigo
            if (player.position.x > transform.position.x)
            {
                movingRight = true;
                sprite.flipX = false;
            }
            // Si el jugador está a la izquierda del enemigo
            else
            {
                movingRight = false;
                sprite.flipX = true;
            }

            // Mover hacia el jugador
            rb.velocity = new Vector2(movingRight ? speed + 0.3f : -speed - 0.3f, rb.velocity.y);
            anim.SetBool("isRunning", true); // Asegurarse de que el enemigo está caminando
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack") && !isTakingDamage)
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        StartCoroutine(TakingDamage());
        if (health <= 0) {
            StartCoroutine(WaitAndDie(0.5f));
        }
    }

    private IEnumerator Die()
    {
        isDead = true;
        anim.SetBool("isDead", true);
        anim.SetTrigger("Die");
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(dieClip.length);
        Destroy(gameObject);
    }

    private IEnumerator WaitAndDie(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        StartCoroutine(Die());
    }

    private IEnumerator TakingDamage()
    {
        rb.velocity = Vector2.zero;

        // Pequeño retroceso
        float direction = (transform.position.x < GameObject.FindGameObjectWithTag("Player").transform.position.x) ? -1f : 1f;
        Vector2 knockback = new Vector2(direction * 1f, 2f);
        rb.velocity = knockback;

        isTakingDamage = true;
        anim.SetBool("isTakingDamage", true);
        anim.SetTrigger("Hit");

        StartCoroutine(ApplyHitEffect());

        yield return new WaitForSeconds(0.5f); // Espera el tiempo de la animación de golpe

        isTakingDamage = false;
        anim.SetBool("isTakingDamage", false);
    }

    private IEnumerator ApplyHitEffect()
    {
        for (int i = 0; i < 3; i++)
        {
            Color red = new Color(1f, 0.5f, 0.5f);
            sprite.color = red;
            yield return new WaitForSeconds(0.05f);
            sprite.color = Color.white;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
