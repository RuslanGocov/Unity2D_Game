using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllScript : MonoBehaviour
{
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float rollForce = 10.0f; // Сила кувырка
    [SerializeField] private float rollDuration = 0.5f;

    private float moveDirection;
    private bool isGround;

    private bool isRight;
    private bool isRolling;


    private Rigidbody2D rb;
    private PlayerControll playerControls;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        playerControls = new PlayerControll();
        //A and D
        playerControls.Movement.RL.performed += context => Move_RL();
        //Space
        playerControls.Movement.Jump.performed += context => Jump();
        //Shift
       //playerControls.Movement.Roll.performed += context => RollDown();
    }


    void Update()
    {
        animator .SetBool("isGrounded", isGround);
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void FixedUpdate()
    {
        Move_RL();
    }


    public void Jump()
    {
        if (isGround)
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGround = true;
            
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGround = false;
            
        }
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

        if (isGround)
        {
            animator.SetBool("run", moveDirection != 0);
        }
        else
        {
            animator.SetBool("run", false);
        }
    }
    
    
    // public void RollDown()
    // {
    //     speed += 3.0f;
    // }


    
    
}