using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6.0f; // Velocidad de movimiento
    public float gravity = -9.81f; // Valor de la gravedad
    public float mouseSensitivity = 100.0f; // Sensibilidad del ratón
    public Animator animator; // Referencia al Animator
    public Transform cameraTransform; // Referencia a la cámara del jugador

    private CharacterController controller;
    private Vector3 velocity; // Almacena la velocidad del jugador (incluyendo la gravedad)
    private bool isGrounded; // Para comprobar si está en el suelo

    public float groundCheckDistance = 0.4f; // Distancia para verificar el suelo
    private float cameraPitch = 0.0f; // Para el control del eje vertical (mirar arriba/abajo)

    void Start()
    {
        // Bloquear el cursor en el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;

        // Obtener el componente CharacterController
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Comprobar si el jugador está en el suelo usando un Raycast
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

        // Resetear la velocidad en Y si está en el suelo
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Pequeño empuje hacia abajo para mantener contacto con el suelo
        }

        // Movimiento del ratón para controlar la rotación
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Controlar la rotación de la cámara en el eje vertical
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f); // Limitar la rotación vertical

        // Aplicar la rotación de la cámara y el personaje
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Input del jugador para moverse (eje X y Z)
        float x = Input.GetAxis("Horizontal"); // Movimiento horizontal (A/D o flechas)
        float z = Input.GetAxis("Vertical");   // Movimiento vertical (W/S o flechas)

        // Obtener la dirección relativa a la cámara
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Ignorar la inclinación de la cámara (solo plano XZ)
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // Calcular el movimiento relativo a la cámara
        Vector3 move = (forward * z + right * x).normalized;

        // Mover al personaje
        controller.Move(move * speed * Time.deltaTime);

        // Aplicar la gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Calcular la magnitud del movimiento
        float movementMagnitude = move.magnitude;

        // Actualizar el parámetro 'Speed' del Animator
        animator.SetFloat("Speed", movementMagnitude);
    }
}
