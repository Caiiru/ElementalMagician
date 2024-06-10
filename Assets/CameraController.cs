using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    public Transform player; // Referência ao transform do jogador
    public Vector3 offset; // Distância da câmera em relação ao jogador
    public float smoothSpeed = 0.125f; // Velocidade de suavização do movimento

    void LateUpdate()
    {
        // Posição desejada da câmera
        Vector3 desiredPosition = player.position + offset;
        // Posição suavizada da câmera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        // Atualiza a posição da câmera
        transform.position = smoothedPosition;
    }
}
