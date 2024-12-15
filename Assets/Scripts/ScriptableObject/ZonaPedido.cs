using UnityEngine;

public class ZonaPedido : MonoBehaviour
{
    // Referencia al pedido que se le asignará al jugador
    [SerializeField] private Pedido pedido;

    private void OnTriggerEnter(Collider other)
    {
        // Verificamos si el objeto que entra es un jugador
        if (other.CompareTag("Player") || other.CompareTag("Player2"))
        {
            // Mostrar mensaje de que el jugador puede recoger el pedido
            Debug.Log("Jugador detectado, presiona E para recoger el pedido.");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Verificamos si el objeto es un jugador y si presiona "E"
        if ((other.CompareTag("Player") || other.CompareTag("Player2")))
        {
            // Mostrar un mensaje para verificar que estamos en la zona correctamente
            Debug.Log("Jugador en la zona de pedido.");

            // Obtener el MotoController del jugador
            MotoController motoController = other.GetComponent<MotoController>();

            if (motoController != null)
            {
                // Asignamos el pedido al MotoController
                motoController.AsignarPedido(pedido);
                Debug.Log("Pedido asignado al jugador: " + pedido.nombrePedido);
            }
            else
            {
                Debug.LogWarning("MotoController no encontrado en el jugador.");
            }
        }
    }

}
