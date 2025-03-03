using UnityEngine;
using System.Collections;

public class SkeletonBoss : MonoBehaviour
{
    private float maxHealth = 6f;
    private float currentHealth;

    public HealthBar healthBar;

    private float followSpeed = 0.7f;
    public Transform player;

    public float attackRange = 1.5f;
    public float spinDuration = 3f;
    public float attackDelay = 1.5f; // Cooldown entre ataques

    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    private bool canSpin = false;
    private bool isSpinning = false;
    private bool isAttacking = false;

    private bool facingRight = true;

    public BoxCollider2D swordCollider;
    public BoxCollider2D spinCollider;

    private bool isTakingDamage = false;

    private bool canAttack = true; // Controla el cooldown entre ataques

    private BoxCollider2D hitBox;

    private float hitBoxOffsetX;

    private Vector3 startPosition;

    private FinalBossManager finalBossManager;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.InitializeHealthBar(maxHealth);
        healthBar.SetHealth(currentHealth);

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        hitBox = GetComponent<BoxCollider2D>();
        hitBoxOffsetX = hitBox.offset.x;

        swordCollider.enabled = false;
        spinCollider.enabled = false;

        startPosition = transform.position;
    }

    void Update()
    {
        finalBossManager = GameObject.FindGameObjectWithTag("FinalBossManager").GetComponent<FinalBossManager>();

        if (FinalBossManager.isInFinalBoss && !FinalBossManager.isFinalBossDead)
        {
            if (currentHealth <= maxHealth / 2f)
            {
                canSpin = true;
            }

            if (!isAttacking && !isSpinning)
            {
                FollowPlayerOnGround();
                CheckForAttack();
            }
        }

    }

    void FollowPlayerOnGround()
    {
        if (player == null) return;

        Vector2 targetPosition = new Vector2(player.position.x, rb.position.y); // Solo sigue horizontalmente
        Vector2 direction = (targetPosition - rb.position).normalized;
        rb.velocity = new Vector2(direction.x * followSpeed, rb.velocity.y);

        animator.SetBool("isWalking", Mathf.Abs(rb.velocity.x) > 0.1f);

        FlipSprite(direction.x);
    }

    void FlipSprite(float directionX)
    {
        if (directionX > 0 && !facingRight)
        {
            facingRight = true;
            sprite.flipX = true;
            hitBox.offset = new Vector2(-hitBoxOffsetX, hitBox.offset.y);
        }
        else if (directionX < 0 && facingRight)
        {
            facingRight = false;
            sprite.flipX = false;

            hitBox.offset = new Vector2(hitBoxOffsetX, hitBox.offset.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack"))
        {
            TakeDamage(1);
        }
    }

    void CheckForAttack()
    {
        if (!canAttack) return; // Respeta cooldown

        float distanceToPlayer = Vector2.Distance(new Vector2(transform.position.x, player.position.y), player.position);

        if (distanceToPlayer <= attackRange)
        {
            if (canSpin && Random.value > 0.5f)
            {
                StartCoroutine(DoSpinAttack());
            }
            else
            {
                StartCoroutine(DoSwordAttack());
            }
        }
    }

    IEnumerator DoSwordAttack()
    {
        isAttacking = true;
        canAttack = false;

        animator.SetTrigger("Attack");
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", true);

        yield return new WaitForSeconds(0.2f);

        AdjustSwordCollider();
        swordCollider.enabled = true;

        yield return new WaitForSeconds(0.3f);

        swordCollider.enabled = false;

        EndAttack();
        yield return new WaitForSeconds(attackDelay); // Cooldown
        canAttack = true;
    }

    IEnumerator DoSpinAttack()
    {
        isSpinning = true;
        canAttack = false;

        animator.SetTrigger("Spin");
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", true);

        spinCollider.enabled = true;

        float spinTime = 0f;
        while (spinTime < spinDuration)
        {
            rb.velocity = new Vector2(facingRight ? followSpeed * 2f : -followSpeed * 2f, rb.velocity.y);
            spinTime += Time.deltaTime;

            yield return null;
        }

        spinCollider.enabled = false;

        EndAttack();
        yield return new WaitForSeconds(attackDelay); // Cooldown
        canAttack = true;
    }

    void AdjustSwordCollider()
    {
        if (facingRight)
        {
            swordCollider.offset = new Vector2(Mathf.Abs(swordCollider.offset.x), swordCollider.offset.y);
        }
        else
        {
            swordCollider.offset = new Vector2(-Mathf.Abs(swordCollider.offset.x), swordCollider.offset.y);
        }
    }

    void EndAttack()
    {
        isAttacking = false;
        isSpinning = false;
        animator.SetBool("isAttacking", false);
        animator.SetBool("isWalking", false);
        rb.velocity = Vector2.zero;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
            FinalBossManager.isFinalBossDead = true;
        }
        else
        {
            StartCoroutine(TakingDamage());
            healthBar.SetHealth(currentHealth);
        }
    }

    private IEnumerator TakingDamage()
    {
        rb.velocity = Vector2.zero;

        float direction = (transform.position.x < GameObject.FindGameObjectWithTag("Player").transform.position.x) ? -1f : 1f;
        Vector2 knockback = new Vector2(direction * 1f, 2f);
        rb.velocity = knockback;

        isTakingDamage = true;
        animator.SetBool("isTakingDamage", isTakingDamage);

        StartCoroutine(ApplyHitEffect());

        yield return new WaitForSeconds(0.5f);

        isTakingDamage = false;
        animator.SetBool("isTakingDamage", isTakingDamage);
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

    void Die()
    {
        rb.velocity = Vector2.zero;
        gameObject.SetActive(false);
        finalBossManager.FinalBossDead();
        GameOver gameOver = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameOver>();
        gameOver.ShowWinScreen();
    }

    public void ResetBoss()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
        transform.position = startPosition;
        canSpin = false;
    }
}
