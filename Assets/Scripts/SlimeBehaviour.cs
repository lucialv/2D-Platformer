using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SlimeBehaviour : MonoBehaviour
{
    public Rigidbody2D rb;
    private Animator anim;
    private float speed = 0.6f;
    private SpriteRenderer sprite;
    private bool isDead = false;
    private bool isTakingDamage = false;
    private int health = 3;
    private float eyeHeight = 0.2f;
    private bool movingRight = true; // Dirección inicial del movimiento
    private BoxCollider2D hitBox;
    private float hitBoxOffsetX;
    public bool killedThePlayer = false;
    private AnimationClip dieClip;
    private AnimationClip attackClip;
    public LayerMask groundLayer;
    public LayerMask playerLayer; // Añadir una capa para detectar al jugador
    private Transform player; // Referencia al transform del jugador
    private float heightOffset; // Altura del colisionador
    private bool isAttacking = false;
    public BoxCollider2D attackCollider; // Referencia al collider del ataque
    private Vector2 attackDirection; // Dirección en la que se realiza el ataque

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        anim.SetBool("isWalking", true);
        sprite = GetComponent<SpriteRenderer>();
        hitBox = GetComponent<BoxCollider2D>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        heightOffset = GetComponent<Collider2D>().bounds.extents.y; // Altura del colisionador
        if (playerObj != null)
        {
            player = playerObj.transform; // Buscar al jugador
        }

        dieClip = anim.runtimeAnimatorController.animationClips.FirstOrDefault(clip => clip.name == "die");
        attackClip = anim.runtimeAnimatorController.animationClips.FirstOrDefault(clip => clip.name == "attack");

        attackCollider.enabled = false;
    }

    void Update()
    {
        if (killedThePlayer)
        {
            rb.velocity = new Vector2(0, 0);
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
        Vector2 raycastOriginForward = new Vector2(transform.position.x + (movingRight ? 0.1f : -0.1f), transform.position.y + heightOffset);
        RaycastHit2D hitWall = Physics2D.Raycast(raycastOriginForward, movingRight ? Vector2.right : Vector2.left, 0.2f, groundLayer);
        Debug.DrawRay(raycastOriginForward, movingRight ? Vector2.right * 0.1f : Vector2.left * 0.1f, Color.red);


        // Detectar si hay un precipicio usando un Raycast hacia abajo
        RaycastHit2D hitGround = Physics2D.Raycast(transform.position + new Vector3(movingRight ? 0.2f : -0.2f, 0.1f, 0), Vector2.down, 0.2f, groundLayer);
        Debug.DrawRay(transform.position + new Vector3(movingRight ? 0.2f : -0.2f, 0.1f, 0), Vector2.down * 0.2f, Color.blue);

        // Raycast para detectar al jugador a la altura de los "ojos"
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
    }

    void ChasePlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        // Detectar si hay un precipicio usando un Raycast hacia abajo
        RaycastHit2D hitGround = Physics2D.Raycast(transform.position + new Vector3(movingRight ? 0.2f : -0.2f, 0.1f, 0), Vector2.down, 0.2f, groundLayer);
        Debug.DrawRay(transform.position + new Vector3(movingRight ? 0.2f : -0.2f, 0.1f, 0), Vector2.down * 0.2f, Color.blue);
        if (distanceToPlayer <= 0.3f) // Ajusta el rango de ataque
        {
            //Stop movement
            rb.velocity = new Vector2(0, rb.velocity.y);

            HandleAttack();
        }
        else if (hitGround.collider == null)
        {
            // Si no hay suelo, detener el movimiento
            rb.velocity = new Vector2(0, rb.velocity.y);
            anim.SetBool("isRunning", false);
        }
        else
        {
            // Mueve al slime hacia el jugador
            movingRight = player.position.x > transform.position.x;
            sprite.flipX = !movingRight;

            rb.velocity = new Vector2(movingRight ? speed + 0.3f : -speed - 0.3f, rb.velocity.y);
            anim.SetBool("isRunning", true);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerAttack") && !isTakingDamage)
        {
            TakeDamage(PlayerManager.damage);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        StartCoroutine(TakingDamage());
        if (health <= 0)
        {
            StartCoroutine(WaitAndDie(0.5f));
        }
    }

    private void HandleAttack()
    {
        isAttacking = true;
        anim.SetTrigger("Attack");
        anim.SetBool("isAttacking", isAttacking);

        // Determinar la dirección de ataque (izquierda o derecha)
        attackDirection = sprite.flipX ? Vector2.left : Vector2.right;
        StartCoroutine(ResetAttack());
        StartCoroutine(AttackDelay());

    }
    private IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(0.1f);
        attackCollider.enabled = true;

        attackCollider.offset = new Vector2(attackDirection.x * 0.15f, 0.15f); // Ajusta el offset según la dirección
    }
    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(attackClip.length);

        isAttacking = false;
        anim.SetBool("isAttacking", isAttacking);

        // Desactivar el área de ataque al finalizar la animación
        attackCollider.enabled = false;
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
