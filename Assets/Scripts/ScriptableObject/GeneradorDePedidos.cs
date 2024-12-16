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
            posicionesLibres.Add(i);  // Añadir todas las posiciones disponibles
        }
    }

    public void GenerarNuevoPedido()
    {
        if (posicionesLibres.Count == 0)
        {
            Debug.LogWarning("No hay posiciones libres para generar pedidos.");
            return;
        }

        // Selección aleatoria de pedido y posición
        pedidoActual = pedidosDisponibles[Random.Range(0, pedidosDisponibles.Length)];
        int indicePosicion = posicionesLibres[Random.Range(0, posicionesLibres.Count)];
        posicionesLibres.Remove(indicePosicion);  // Liberar la posición seleccionada

        // Instanciar cliente en la posición seleccionada
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

        Debug.Log($"Nuevo pedido generado: {pedidoActual.nombrePedido} en la posición {indicePosicion}");
    }

    public void PedidoEntregado()
    {
        if (clienteActual != null)
        {
            Destroy(clienteActual);  // Destruir el cliente después de entregar el pedido
        }

        // Liberar la posición y generar un nuevo pedido
        int posicionLiberada = System.Array.IndexOf(posicionesClientes, clienteActual.transform);
        posicionesLibres.Add(posicionLiberada);
        GenerarNuevoPedido();  // Generar un nuevo pedido y cliente
    }
}