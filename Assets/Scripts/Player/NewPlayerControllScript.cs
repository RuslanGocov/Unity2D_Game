using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewPlayerControllScript : MonoBehaviour
{
    
    //Serialize Fields
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float rollForce = 5.0f;
    
    //Variables
    private float moveDerection;
    
    //Objects
    private Rigidbody2D rbRigidbody2D;
    private Animator Animator;
    private PlayerControll playerControll;


    private void Start()
    {
        rbRigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        
    }

    private void Awake()
    {
        playerControll = new PlayerControll();



        playerControll.Movement.RL.performed += context => Movement_RL();
    }

    private void Update()
    {
        Movement_RL();
    }

    private void FixedUpdate()
    {
        Movement_RL();
    }


    private void Movement_RL()
    {
        moveDerection = playerControll.Movement.RL.ReadValue<float>();

        rbRigidbody2D.velocity = new Vector2(moveDerection * speed, rbRigidbody2D.velocity.y);

        if (moveDerection > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if(moveDerection < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        
        
    }
}
