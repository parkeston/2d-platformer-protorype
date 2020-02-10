using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : PhysicsObject {

    public Transform target;
    public float maxSpeed = 7f;
    public float chaseDistance = 10f;
    public float chaseTimer = 0.5f;
    public float attackRate = 0.5f;
    public float attackDelay = 0.5f;
    public int EnemyDamage { get; set; }

    private float nextChase = 0.0f;
    private Animator animator;
    private bool facingRight;
    private float nextAttack = 0.0f;
    private bool isAttacking = false;
    private int health;

    private const float MINDISTANCE = 0.2f;
    private const float PIVOTOFFSET = 0.1f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (transform.localScale.x < 0)
            facingRight = false;
        else
            facingRight = true;
    }

    protected override void Start()
    {
        health = 120;
        EnemyDamage = 15;
        base.Start();
    }


    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;
        if (Vector3.Distance(target.position, transform.position) <= chaseDistance && Time.time > nextChase)
        {
            float distance = GetDistance();
            if (distance <= MINDISTANCE)
            {
                nextChase = Time.time + chaseTimer;
                if (Time.time > nextAttack && !isAttacking)
                    StartCoroutine(Attack());
            }
            else if (!isAttacking)
                move.x = (target.position.x < transform.position.x) ? -1 : 1;
        }
        if (move.x > 0.0f && !facingRight)
            Flip();
        else if (move.x < 0.0f && facingRight)
            Flip();
        animator.SetFloat("speedX", Mathf.Abs(velocity.x) / maxSpeed);
        targetVelocity = move * maxSpeed;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    float GetDistance()
    {
        if ((target.position.x > transform.position.x && !facingRight) ||
            (target.position.x < transform.position.x && facingRight))
            return Mathf.Abs(target.position.x - transform.position.x);
        else
            return Mathf.Abs(target.position.x - transform.position.x) - PIVOTOFFSET;
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        nextAttack = Time.time + attackRate+attackDelay;
        yield return new WaitForSeconds(attackDelay);
        animator.SetTrigger("attack");
    }

    void AttackOver()
    {
        isAttacking = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        if (other.gameObject.CompareTag("Player") && currentState.IsName("idle"))
        {
            health -= other.gameObject.GetComponentInParent<PlayerPlatformerController>().PlayerDamage;
            if (health <= 0)
                Die();
            animator.SetTrigger("hit");
            Debug.Log("Hit!");
        }
    }

    void Die()
    {
        animator.SetTrigger("dead");
        enabled = false;
        gameObject.GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 15f);
    }
}
