using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6.0f; // Velocidad de movimiento

    private CharacterController controller;

    void Start()
    {
        // Obtener el componente CharacterController
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Input del jugador para moverse (eje X y Z)
        float x = Input.GetAxis("Horizontal"); // Movimiento horizontal (A/D o flechas)
        float z = Input.GetAxis("Vertical");   // Movimiento vertical (W/S o flechas)

        // Calcular el movimiento en el plano
        Vector3 move = transform.right * x + transform.forward * z;

        // Mover al personaje
        controller.Move(move * speed * Time.deltaTime);
    }
}
