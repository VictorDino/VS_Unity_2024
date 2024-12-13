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

        // Verificar si el CharacterController está activo
        if (!controller.enabled)
        {
            return; // No ejecutar el movimiento si el CharacterController está desactivado
        }

        // Comprobar si está en el suelo usando un Raycast
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

        // Resetear la velocidad en Y si está en el suelo
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Obtener entradas del teclado
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Crear un vector de movimiento en el plano XZ
        Vector3 move = new Vector3(x, 0, z);

        if (move.magnitude > 0.1f)
        {
            // Rotar el jugador hacia la dirección del movimiento
            Vector3 moveDirection = move.normalized;
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        // Mover al jugador usando el CharacterController
        controller.Move(move * speed * Time.deltaTime);

        // Aplicar la gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Actualizar animación
        float movementMagnitude = move.magnitude;
        animator.SetFloat("Speed", movementMagnitude);
    }
}
