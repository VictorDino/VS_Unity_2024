using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // El personaje al que la cámara sigue
    public Vector3 offset = new Vector3(0, 5, -10); // Desplazamiento respecto al personaje
    public float smoothSpeed = 0.125f; // Velocidad de suavizado
    public bool lookAtTarget = true; // Si la cámara debe mirar al personaje

    void LateUpdate()
    {
        // Calcular la posición deseada
        Vector3 desiredPosition = target.position + offset;

        // Suavizar el movimiento de la cámara
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Establecer la posición de la cámara
        transform.position = smoothedPosition;

        // Si se desea, rotar la cámara para que mire al objetivo
        if (lookAtTarget)
        {
            transform.LookAt(target);
        }
    }
}
