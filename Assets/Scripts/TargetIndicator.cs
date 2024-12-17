using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    public Transform target; // El punto final al que queremos apuntar
    public Transform arrowFront; // Objeto auxiliar que indica el frente de la flecha
    public float rotationSpeed = 5f;

    void Update()
    {
        // Calculamos la dirección desde la flecha hasta el objetivo
        Vector3 directionToTarget = target.position - transform.position;

        // Calculamos la rotación necesaria para que el frente de la flecha apunte hacia el objetivo
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

        // Ajustamos la rotación teniendo en cuenta la orientación inicial del frente
        Quaternion adjustedRotation = targetRotation * Quaternion.Inverse(arrowFront.localRotation);

        // Aplicamos la rotación de forma suave usando Slerp
        transform.rotation = Quaternion.Slerp(transform.rotation, adjustedRotation, rotationSpeed * Time.deltaTime);
    }
}
