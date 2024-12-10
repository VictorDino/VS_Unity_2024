using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6.0f; 
    public float gravity = -9.81f; 
    public float rotationSpeed = 10.0f; 
    public Animator animator; 

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded; 

    public float groundCheckDistance = 0.4f;

    void Start()
    {
        // Obtener el componente CharacterController
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        
        float x = Input.GetAxis("Horizontal"); 
        float z = Input.GetAxis("Vertical");   

   
        Vector3 move = new Vector3(x, 0, z);

        if (move.magnitude > 0.1f)
        {
            Vector3 moveDirection = move.normalized; 
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up); 
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime); 
        }

        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        float movementMagnitude = move.magnitude;

        animator.SetFloat("Speed", movementMagnitude);
    }
}
