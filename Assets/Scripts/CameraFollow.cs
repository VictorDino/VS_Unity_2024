using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // El personaje al que la c�mara sigue
    public Vector3 offset = new Vector3(0, 5, -10); // Desplazamiento respecto al personaje
    public float smoothSpeed = 0.125f; // Velocidad de suavizado
    public bool lookAtTarget = true; // Si la c�mara debe mirar al personaje

    void LateUpdate()
    {
        // Calcular la posici�n deseada
        Vector3 desiredPosition = target.position + offset;

        // Suavizar el movimiento de la c�mara
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Establecer la posici�n de la c�mara
        transform.position = smoothedPosition;

        // Si se desea, rotar la c�mara para que mire al objetivo
        if (lookAtTarget)
        {
            transform.LookAt(target);
        }
    }
}
