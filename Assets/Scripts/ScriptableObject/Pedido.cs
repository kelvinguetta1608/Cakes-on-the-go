using UnityEngine;

[CreateAssetMenu(fileName = "NuevoPedido", menuName = "Pedidos/Pedido")]
public class Pedido : ScriptableObject
{
    [Header("Detalles del Pedido")]
    public string nombrePedido;
    public Color colorLuz;
}
