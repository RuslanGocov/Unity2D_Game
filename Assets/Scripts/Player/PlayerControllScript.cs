using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllScript : MonoBehaviour
{
    //Serialize Variables
    [SerializeField] private float jumpForce = 3.0f;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float rollForce = 5.0f; // Сила кувырка
    [SerializeField] private float wallSlodingSpeed = 10.0f;


    //Serialize Objects
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask wallLayer;


    //Variables
    private float moveDirection;

    private bool isRolling = false;

    private bool isWallSliding;

    //walljump
    private float wallSlidingSpeed = 2f;
    private float wallJumpCooldown;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(4f, 8f);


    //roll
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 12f;
    private float dashingTime = 0.1f;
    private float dashingCoolDown = 1f;

    // private bool isGround;


    //Objects
    private Rigidbody2D rb;
    private PlayerControll playerControls;
    private Animator animator;
    private BoxCollider2D boxCollider2D;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Awake()
    {
        playerControls = new PlayerControll();


        //A and D
        playerControls.Movement.RL.performed += context => Move_RL();
        //Space
        playerControls.Movement.Jump.performed += context => Jump();
        //Shift
        playerControls.Movement.Roll.started += context => StartCoroutine(Roll());

        playerControls.Movement.Punch.started += context => puncing();

        playerControls.Movement.Block.started += context => Block();
    }


    void Update()
    {
        if (isDashing)
        {
            return;
        }

        animator.SetBool("isGrounded", isGrounded());
        animator.SetBool("onWall", onWall());
        wallSlide();
        wallJump();
    }


    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        Move_RL();
    }


    public void Move_RL()
    {
        moveDirection = playerControls.Movement.RL.ReadValue<float>();

        rb.velocity = new Vector2(moveDirection * speed, rb.velocity.y);
        if (moveDirection > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (moveDirection < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (isGrounded())
        {
            animator.SetBool("run", moveDirection != 0);
        }
        else
        {
            animator.SetBool("run", false);
        }
    }

    private void Jump()
    {
        if (isGrounded())
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            animator.SetTrigger("jump");
        }
    }

    private IEnumerator Roll()
    {
        print("Roll");
        animator.SetBool("isDashing", true);
        canDash = false;
        isDashing = true;

        float originGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        yield return new WaitForSeconds(dashingTime);
        rb.gravityScale = originGravity;
        isDashing = false;
        animator.SetBool("isDashing", isDashing);
        yield return new WaitForSeconds(dashingCoolDown);
        canDash = true;
    }


    //Fight\\
    private void puncing()
    {
        if (!onWall())
        {
            animator.SetTrigger("punc");
        }
    }


    private void wallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;
            CancelInvoke(nameof(stopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
                animator.SetTrigger("jump");
            }

            animator.SetTrigger("jump");

            Invoke(nameof(stopWallJumping), wallJumpingDuration);
        }
    }

    private void wallSlide()
    {
        if (onWall() && !isGrounded() && moveDirection == 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlodingSpeed, float.MaxValue));
            print("wall slide");
        }
        else
        {
            isWallSliding = false;
            print("No wall slide");
        }
    }


    private void stopWallJumping()
    {
        isWallJumping = false;
    }


    private void Block()
    {
        animator.SetTrigger("block");
    }


    //Checking
    public bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0,
            new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }


    private bool isGrounded()
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0,
            Vector2.down, 0.1f, groundLayerMask);
        return raycastHit2D.collider != null;
    }


    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
}