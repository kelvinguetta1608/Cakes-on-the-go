using UnityEngine;

public class PedidoManager : MonoBehaviour
{
    [SerializeField] private GameObject clientePrefab;  // Prefab del Cliente
    [SerializeField] private Transform[] spawnPoints;  // Puntos donde se crean clientes

    private int indexSpawn = 0;

    private void Start()
    {
        GenerarNuevoCliente();
    }

    // Genera un nuevo cliente y asigna un pedido
    public void GenerarNuevoCliente()
    {
        // Seleccionar un punto de spawn
        Transform spawnPoint = spawnPoints[indexSpawn % spawnPoints.Length];
        indexSpawn++;

        // Instanciar cliente en la escena
        GameObject nuevoCliente = Instantiate(clientePrefab, spawnPoint.position, Quaternion.identity);
        Cliente clienteScript = nuevoCliente.GetComponent<Cliente>();

        // Generar un nuevo pedido y asignarlo
        Pedido nuevoPedido = GenerarPedido();
        clienteScript.AsignarPedido(nuevoPedido);
    }

    // Método que genera un pedido aleatorio (puedes mejorarlo)
    private Pedido GenerarPedido()
    {
        Pedido pedido = new Pedido();
        pedido.nombrePedido = "Pedido " + Random.Range(1, 100);
        Debug.Log($"Nuevo Pedido Generado: {pedido.nombrePedido}");
        return pedido;
    }
}
