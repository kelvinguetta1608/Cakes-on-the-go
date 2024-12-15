using UnityEngine;
using System.Collections.Generic;

public class GeneradorDePedidos : MonoBehaviour
{
    [SerializeField] private Pedido[] pedidosDisponibles;
    [SerializeField] private Transform[] posicionesClientes;  // Puntos de spawn para los clientes
    [SerializeField] private GameObject clientePrefab;  // Prefab del cliente

    private List<int> posicionesLibres;
    public Pedido pedidoActual;
    private GameObject clienteActual;  // Cliente que se ha instanciado

    private void Start()
    {
        InicializarPosicionesLibres();
        GenerarNuevoPedido();
    }

    private void InicializarPosicionesLibres()
    {
        posicionesLibres = new List<int>();
        for (int i = 0; i < posicionesClientes.Length; i++)
        {
            posicionesLibres.Add(i);  // A�adir todas las posiciones disponibles
        }
    }

    public void GenerarNuevoPedido()
    {
        if (posicionesLibres.Count == 0)
        {
            Debug.LogWarning("No hay posiciones libres para generar pedidos.");
            return;
        }

        // Selecci�n aleatoria de pedido y posici�n
        pedidoActual = pedidosDisponibles[Random.Range(0, pedidosDisponibles.Length)];
        int indicePosicion = posicionesLibres[Random.Range(0, posicionesLibres.Count)];
        posicionesLibres.Remove(indicePosicion);  // Liberar la posici�n seleccionada

        // Instanciar cliente en la posici�n seleccionada
        if (clienteActual != null)
        {
            Destroy(clienteActual);  // Destruir el cliente anterior si ya existe
        }

        clienteActual = Instantiate(clientePrefab, posicionesClientes[indicePosicion].position, Quaternion.identity);

        // Asignar el pedido al cliente
        Cliente clienteScript = clienteActual.GetComponent<Cliente>();
        if (clienteScript != null)
        {
            clienteScript.AsignarPedido(pedidoActual);
        }

        Debug.Log($"Nuevo pedido generado: {pedidoActual.nombrePedido} en la posici�n {indicePosicion}");
    }

    public void PedidoEntregado()
    {
        if (clienteActual != null)
        {
            int posicionLiberada = -1;
            for (int i = 0; i < posicionesClientes.Length; i++)
            {
                if (posicionesClientes[i].position == clienteActual.transform.position)
                {
                    posicionLiberada = i;
                    break;
                }
            }

            if (posicionLiberada >= 0)
            {
                posicionesLibres.Add(posicionLiberada); // Liberar posici�n
                Debug.Log($"Posici�n liberada: {posicionLiberada}");
            }

            Destroy(clienteActual); // Destruir el cliente despu�s de entregar el pedido
            clienteActual = null;   // Limpiar la referencia
        }

        GenerarNuevoPedido(); // Generar un nuevo cliente y pedido
    }

}