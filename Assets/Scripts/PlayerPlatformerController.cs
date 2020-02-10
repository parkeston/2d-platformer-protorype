using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPlatformerController : PhysicsObject { //derived from physicsObject

    public float maxSpeed = 7f;
    public float jumpTakeOffSpeed = 7f;
    public GameObject vCamera;
    public Slider healthSlider;

    private Animator animator;
    private bool facingRight = true;
    public float jumpRate = 1f; //rate of jumping, to not abuse bunnyhoping and not to break a jump animation
    private float nextJump = 0.0f; //time of next jump
    private int comboPoints = 0; //to check what attack animation to play
    public float attackRate = 0.6f; //rate of attacks - how fast combopoints can be incremented,
    //to allow animator consequentially play all combos with it's duration and not to leap from 1 attack to the same attack (skipping 2 and 3)
    private float nextAttack = 0.0f; //time of next attack
    public float comboTimer = 0.6f; //time windows to perform a combo attack
    public int PlayerDamage { get; set; }
    private int Health
    {
        get { return health; }
        set
        {
            health = value;
            if (health <= 0)
                Die();
            else
                healthSlider.value = health;
        }
    }
    private  int health;
    private bool isHitted = false;
    private bool isAttacking = false;

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        PlayerDamage = 20;
        Health = GameManager.instance.playerHealth;
        base.Start();
    }


    protected override void Update()
    {
        CameraChange();
        targetVelocity = Vector2.zero;
        if(!isHitted && !isAttacking)
            ComputeVelocity();
        AttempAttack();
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;
        move.x = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Jump") && grounded && Time.time>nextJump)
        {
            nextJump = Time.time + jumpRate;
            animator.SetTrigger("jump");
            velocity.y = jumpTakeOffSpeed;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (velocity.y > 0)  //canceling jump in mid-air
                velocity.y = velocity.y * 0.5f;
        }
        if (move.x > 0.01f && !facingRight)
            Flip();
        else if (move.x < 0.0f && facingRight)
            Flip();
        animator.SetBool("grounded", grounded);
        animator.SetFloat("speedX", Mathf.Abs(velocity.x) / maxSpeed);
        targetVelocity = move * maxSpeed; //horizontal movement
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void AttempAttack()
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0); // 0 - base layer of animator
        if (Input.GetButton("Fire1") && (currentState.IsName("idle")) && Time.time>nextAttack)
        {
            isAttacking = true;
            nextAttack = Time.time + attackRate;
            comboPoints++;
            animator.SetInteger("attackCounter", comboPoints);
            animator.SetTrigger("attack");
            if (comboPoints == 3)
                comboPoints = 0;
        }
        else if (Time.time > nextAttack + comboTimer)
            comboPoints = 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Enemy") && !isHitted)
        {
            Health -= other.gameObject.GetComponentInParent<EnemyAI>().EnemyDamage;
            isHitted = true;
            animator.SetTrigger("hit");
            Debug.Log("Player was hitted!");
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Spikes") && grounded)
            Die();
    }

    void HittedOver()
    {
        isHitted = false;
        isAttacking = false;
    }

    void AttackOver()
    {
        isAttacking = false;
    }

    void Die()
    {
        healthSlider.fillRect.gameObject.SetActive(false);
        animator.SetTrigger("dead");
        enabled = false;
        gameObject.GetComponent<Collider2D>().enabled = false;
    }

    void CameraChange()
    {
        if (Input.GetAxis("Vertical") == -1 && grounded)
            vCamera.SetActive(true);
        else
            vCamera.SetActive(false);
    }

   /* void OnDisable()
    {
        if (Health > 0)
            GameManager.instance.playerHealth = Health;
    }*/ 
    // обдумать сохранение хп при переходе между сценами и т.п
}
