using UnityEngine;
using UnityEngine.InputSystem;

public class Cliente : MonoBehaviour
{
    [SerializeField] private float distanciaRaycast = 10f;
    [SerializeField] private Color colorRaycast = Color.red;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private string teclaInteraccion = "Interact";

    private Pedido pedidoCliente;
    private PlayerInputActions playerInputActions;
    private InputAction interactAction;
    private GeneradorDePedidos generadorDePedidos; // Referencia al GeneradorDePedidos

    private void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        // Configura las acciones de entrada
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player2.Enable();

        // Encuentra el GeneradorDePedidos en la escena
        generadorDePedidos = FindObjectOfType<GeneradorDePedidos>();
    }

    private void Update()
    {
        LanzarRaycastHaciaArriba();
    }

    private void LanzarRaycastHaciaArriba()
    {
        Ray ray = new Ray(transform.position, Vector3.up);
        if (Physics.Raycast(ray, out RaycastHit hit, distanciaRaycast))
        {
            Debug.DrawLine(ray.origin, hit.point, colorRaycast);
            MostrarRaycastVisual(ray.origin, hit.point);
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * distanciaRaycast, colorRaycast);
            MostrarRaycastVisual(ray.origin, ray.origin + ray.direction * distanciaRaycast);
        }
    }

    private void MostrarRaycastVisual(Vector3 start, Vector3 end)
    {
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }
    }

    public void AsignarPedido(Pedido pedido)
    {
        pedidoCliente = pedido;
        Debug.Log($"Pedido asignado al cliente: {pedido.nombrePedido}");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Player2"))
        {
            if (other.CompareTag("Player"))
            {
                interactAction = playerInputActions.Player.Interact;
            }
            else if (other.CompareTag("Player2"))
            {
                interactAction = playerInputActions.Player2.Interact;
            }

            if (interactAction.triggered)
            {
                MotoController motoController = other.GetComponent<MotoController>();
                if (motoController != null && motoController.pedidoAsignado != null)
                {
                    if (VerificarPedido(motoController.pedidoAsignado))
                    {
                        Debug.Log("Pedido entregado correctamente.");
                        motoController.pedidoAsignado = null;

                        // Llamada al GeneradorDePedidos para continuar el flujo
                        if (generadorDePedidos != null)
                        {
                            generadorDePedidos.PedidoEntregado();
                        }

                        PedidoEntregado();
                    }
                    else
                    {
                        Debug.Log("Pedido incorrecto.");
                    }
                }
            }
        }
    }

    public bool VerificarPedido(Pedido pedidoJugador)
    {
        if (pedidoCliente == null || pedidoJugador == null)
        {
            Debug.LogWarning("Uno de los pedidos está vacío.");
            return false;
        }
        return pedidoCliente == pedidoJugador;
    }

    public void PedidoEntregado()
    {
        Debug.Log("El cliente ha recibido su pedido.");
        Destroy(gameObject); // Destruye el cliente actual
    }
}
