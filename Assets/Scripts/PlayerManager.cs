using System.Collections;
using UnityEngine;
using System.Linq;

public class PlayerManager : MonoBehaviour
{
    private float jumpForce = 4.2f;
    private float runSpeed = 1.8f;
    private float walkSpeed = 1.5f;

    private float knockbackForce = 2f;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isWalking;
    private bool isRunning;
    private bool isJumping;
    private bool isFalling;
    private bool isAttacking;
    private bool isDead;
    private SpriteRenderer sprite;
    private Color originalColor;
    private AnimationClip attackClip;
    private AnimationClip dieClip;
    private int health = 3;
    private readonly int maxHealth = 3;
    private bool isKnockback = false;
    public BoxCollider2D attackCollider; // Referencia al collider del ataque
    private Vector2 attackDirection; // Dirección en la que se realiza el ataque
    private HeartUI heartUI;
    private GameOver gameOver;
    public static int damage = 1;
    private FinalBossManager finalBossManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        originalColor = sprite.color;


        attackClip = anim.runtimeAnimatorController.animationClips.FirstOrDefault(clip => clip.name == "Attack");
        dieClip = anim.runtimeAnimatorController.animationClips.FirstOrDefault(clip => clip.name == "Die");

        gameOver = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameOver>();

        heartUI = GameObject.FindGameObjectWithTag("HeartUI").GetComponent<HeartUI>();

        finalBossManager = GameObject.FindGameObjectWithTag("FinalBossManager").GetComponent<FinalBossManager>();

        attackCollider.enabled = false; // Asegúrate de que esté desactivado al inicio
    }

    void FixedUpdate()
    {
        if (!isKnockback && !isDead)
        {
            HandleMovement();
        }
        if (!isDead)
        {
            HandleJumpAndFall();
        }
    }

    void Update()
    {
        if (!isDead)
        {
            HandleAttack();
        }

    }

    private void HandleMovement()
    {
        // Comprobar si ambas teclas están presionadas
        if (Input.GetKey("a") && Input.GetKey("d"))
        {
            // Si ambas teclas están presionadas, que no se mueva el personaje
            rb.velocity = new Vector2(0, rb.velocity.y);
            isWalking = false;
            anim.SetBool("isWalking", isWalking);
        }
        // Movimiento a la izquierda
        else if (Input.GetKey("a"))
        {
            rb.velocity = new Vector2(-walkSpeed, rb.velocity.y);
            sprite.flipX = true;
            isWalking = true;
            anim.SetBool("isWalking", isWalking);
        }
        // Movimiento a la derecha
        else if (Input.GetKey("d"))
        {
            rb.velocity = new Vector2(walkSpeed, rb.velocity.y);
            sprite.flipX = false;
            isWalking = true;
            anim.SetBool("isWalking", isWalking);
        }
        // Detener el movimiento
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            isWalking = false;
            anim.SetBool("isWalking", isWalking);
        }

        // Correr
        if (Input.GetKey("left shift") && isWalking)
        {
            rb.velocity = new Vector2(rb.velocity.x * runSpeed, rb.velocity.y);
            if (!isJumping && !isFalling)
            {
                isRunning = true;
                isWalking = false;
                anim.SetBool("isRunning", isRunning);
                anim.SetBool("isWalking", isWalking);
            }
        }
        else
        {
            isRunning = false;
            anim.SetBool("isRunning", isRunning);
        }
    }

    private void HandleJumpAndFall()
    {
        // Saltar
        if (Input.GetKey("space") && CheckGround.isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
            anim.SetBool("isJumping", isJumping);
            isFalling = false;
            anim.SetBool("isFalling", isFalling);
        }

        // Detectar si está subiendo o cayendo
        if (rb.velocity.y > 0 && !CheckGround.isGrounded && !isAttacking)
        {
            isJumping = true;
            isFalling = false;
            anim.SetBool("isJumping", isJumping);
            anim.SetBool("isFalling", isFalling);
        }
        else if (rb.velocity.y < 0 && !CheckGround.isGrounded && !isAttacking)
        {
            isJumping = false;
            isFalling = true;
            anim.SetBool("isJumping", isJumping);
            anim.SetBool("isFalling", isFalling);
        }
        // Si toca el suelo
        if (CheckGround.isGrounded)
        {
            isJumping = false;
            isFalling = false;
            anim.SetBool("isJumping", isJumping);
            anim.SetBool("isFalling", isFalling);
        }

    }

    private void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            isAttacking = true;
            anim.SetTrigger("Attack");
            anim.SetBool("isAttacking", isAttacking);

            // Determinar la dirección de ataque (izquierda o derecha)
            attackDirection = sprite.flipX ? Vector2.left : Vector2.right;
            StartCoroutine(ResetAttack());
            StartCoroutine(AttackDelay());
        }
    }
    private IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(0.2f);
        attackCollider.offset = new Vector2(attackDirection.x * 0.2f, 0);
        attackCollider.enabled = true;
    }
    private IEnumerator ResetAttack()
    {
        // Espera la duración de la animación
        if (attackClip != null)
        {
            // Esperar la duración del clip de ataque
            yield return new WaitForSeconds(attackClip.length);
        }
        isAttacking = false;
        anim.SetBool("isAttacking", isAttacking);

        // Desactivar el área de ataque al finalizar la animación
        attackCollider.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isDead || collision.gameObject.CompareTag("EnemyAttack") && !isDead)
        {
            TakeDamage(1, collision);
            rb.velocity = Vector2.zero;

            float direction = (transform.position.x < collision.transform.position.x) ? -1f : 1f;
            Vector2 knockback = new Vector2(direction * knockbackForce, 2f);
            rb.velocity = knockback;

            isKnockback = true;

            StartCoroutine(EndKnockback());
        }
    }

    private IEnumerator ApplyHitEffect()
    {
        for (int i = 0; i < 3; i++)
        {
            Color red = new Color(1f, 0.5f, 0.5f);
            sprite.color = red;
            yield return new WaitForSeconds(0.05f);
            sprite.color = originalColor;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator EndKnockback()
    {
        yield return new WaitForSeconds(0.2f);
        isKnockback = false;
    }

    private IEnumerator Die()
    {
        rb.velocity = Vector2.zero;
        anim.SetTrigger("Die");
        anim.SetBool("isDead", true);
        yield return new WaitForSeconds(dieClip.length);
        gameObject.SetActive(false);
        gameOver.timesDied++;
        gameOver.ShowGameOverScreen();
    }

    private IEnumerator WaitAndDie(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        StartCoroutine(Die());
    }

    public void TakeDamage(int damage, Collision2D collision = null)
    {
        health -= damage;
        heartUI.UpdateHearts(health);

        CameraShake cameraShake = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CameraShake>();
        cameraShake.ShakeCamera(0.5f, 0.2f);
        StartCoroutine(ApplyHitEffect());

        if (health <= 0)
        {
            if (collision != null && collision.gameObject.CompareTag("Enemy"))
            {
                if (collision.gameObject.GetComponent<EnemyBehaviour>() != null)
                {
                    collision.gameObject.GetComponent<EnemyBehaviour>().killedThePlayer = true;
                }
                else if (collision.gameObject.GetComponent<SlimeBehaviour>() != null)
                {
                    collision.gameObject.GetComponent<SlimeBehaviour>().killedThePlayer = true;
                }
            }

            knockbackForce = 1f;
            isDead = true;
            StartCoroutine(WaitAndDie(0.5f));
        }
    }

    public void GainHealth(int healthGiven)
    {
        health += healthGiven;
        heartUI.UpdateHearts(health);
    }
    public int GetCurrentHealth()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void ResetPlayer()
    {
        gameObject.SetActive(true);
        health = maxHealth;
        heartUI.UpdateHearts(health);
        isDead = false;
        FinalBossManager.isFinalBossDead = false;
        FinalBossManager.isInFinalBoss = false;
        finalBossManager.RestartFinalBoss();
    }
}
