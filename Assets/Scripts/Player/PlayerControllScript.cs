using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllScript : MonoBehaviour
{
    //Serialize Variables
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float rollForce = 5.0f; // Сила кувырка

    //Serialize Objects
    [SerializeField] private LayerMask groundLayerMask;


    //Variables
    private float moveDirection;
    private bool isRolling = false;
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
        playerControls.Movement.Roll.started += context => Roll();

        playerControls.Movement.Punch.started += context => puncing();
    }


    void Update()
    {
        animator.SetBool("isGrounded", isGrounded());
    }



    private void FixedUpdate()
    {
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


   
    private void Roll()
    {
        if (isGrounded() && !isRolling)
        {
            isRolling = true;
            animator.SetTrigger("roll"); // Убедитесь, что у вас есть соответствующая анимация

            // Определите направление кувырка
            Vector2 rollDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

            // Примените силу к игроку для выполнения кувырка
            rb.velocity = new Vector2(0, rb.velocity.y); // Остановите текущее горизонтальное движение
            rb.AddForce(rollDirection * rollForce, ForceMode2D.Impulse);

            // Время, через которое закончится кувырок
            Invoke("EndRoll", 0.5f);
        }
    }

    private void EndRoll()
    {
        isRolling = false;
        animator.SetBool("roll", isRolling);
    }

    private void puncing()
    {
        animator.SetTrigger("punc");
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