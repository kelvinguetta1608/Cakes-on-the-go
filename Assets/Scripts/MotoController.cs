using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MotoController : MonoBehaviour, PlayerInputActions.IPlayerActions, PlayerInputActions.IPlayer2Actions
{
    // Parámetros de la moto
    public float maxSteerAngle = 30f;
    public float acceleration = 10f;
    public float reverseSpeedFactor = 0.5f;
    public float brakePower = 5f;
    public Transform frontWheel;
    public Transform rearWheel;

    private Rigidbody rb;
    private Vector2 movementInput;
    private float currentSteerAngle = 0f;
    private float currentSpeed = 0f;
    private float turnSpeed = 5f;
    public GameObject objetoActivable;

    [SerializeField] private float wheelRadius = 0.5f;

    private PlayerInputActions playerInputActions;

    // Variables de interacción
    public MotoController motoOponente; // Referencia a la moto opuesta
    private bool puedeInteractuar = true; // Controla si esta moto puede interactuar
    public Pedido pedidoAsignado;

    // Nueva variable para invertir controles
    private bool controlesInvertidos = false;

    private void Awake()
    {
        // Inicializar el sistema de entradas
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.SetCallbacks(this);
        playerInputActions.Player2.SetCallbacks(this);
    }

    public void AsignarPedido(Pedido pedido)
    {
        pedidoAsignado = pedido;
        Debug.Log("Pedido asignado: " + pedido.nombrePedido);
    }

    private void OnEnable()
    {
        if (CompareTag("Player"))
        {
            playerInputActions.Player.Enable();
        }
        else if (CompareTag("Player2"))
        {
            playerInputActions.Player2.Enable();
        }
    }

    private void OnDisable()
    {
        playerInputActions.Player.Disable();
        playerInputActions.Player2.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.centerOfMass = new Vector3(0, -0.5f, 0);
        }
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        HandleSteering();
        HandleAcceleration();
        HandleMovement();
    }

    private void HandleSteering()
    {
        float reverseMultiplier = currentSpeed < 0 ? -1 : 1;
        currentSteerAngle = movementInput.x * maxSteerAngle * reverseMultiplier;

        if (frontWheel != null)
        {
            frontWheel.localRotation = Quaternion.Euler(0, currentSteerAngle, 0);
        }
    }

    private void HandleAcceleration()
    {
        if (movementInput.y > 0)
        {
            currentSpeed += movementInput.y * acceleration * Time.fixedDeltaTime;
        }
        else if (movementInput.y < 0)
        {
            currentSpeed += movementInput.y * acceleration * reverseSpeedFactor * Time.fixedDeltaTime;
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, brakePower * Time.fixedDeltaTime);
        }

        currentSpeed = Mathf.Clamp(currentSpeed, -acceleration * reverseSpeedFactor, acceleration);
    }

    private void HandleMovement()
    {
        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            Vector3 movement = transform.forward * currentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);

            float turnAmount = currentSteerAngle * turnSpeed * Time.fixedDeltaTime;

            if (currentSpeed < 0)
            {
                turnAmount *= -1;
            }

            Quaternion turnRotation = Quaternion.Euler(0, turnAmount, 0);
            rb.MoveRotation(rb.rotation * turnRotation);
        }

        UpdateWheelRotations();
    }

    private void UpdateWheelRotations()
    {
        if (rearWheel != null)
        {
            float rotationAmount = (currentSpeed * Time.fixedDeltaTime) / (2 * Mathf.PI * wheelRadius) * 360f;
            rearWheel.localRotation *= Quaternion.Euler(rotationAmount, 0, 0);
        }

        if (frontWheel != null)
        {
            float rotationAmountX = (currentSpeed * Time.fixedDeltaTime) / (2 * Mathf.PI * wheelRadius) * 360f;
            Quaternion forwardRotation = Quaternion.Euler(rotationAmountX, 0, 0);
            Quaternion steeringRotation = Quaternion.Euler(0, currentSteerAngle, 0);

            frontWheel.localRotation = forwardRotation * steeringRotation;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // Ajusta la entrada según si los controles están invertidos
        Vector2 input = context.ReadValue<Vector2>();
        movementInput = controlesInvertidos ? new Vector2(-input.x, -input.y) : input;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // Acción no utilizada
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started && puedeInteractuar && motoOponente != null)
        {
            StartCoroutine(AfectarOponente());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pastel"))
        {
            if (objetoActivable != null)
            {
                objetoActivable.SetActive(true);
                Debug.Log("Objeto activado al entrar en contacto con Pastel.");
            }
        }
    }

    private IEnumerator AfectarOponente()
    {
        // Desactiva la capacidad de interactuar temporalmente
        puedeInteractuar = false;

        // Reduce la aceleración del oponente y activa controles invertidos
        float aceleracionOriginal = motoOponente.acceleration;
        motoOponente.acceleration *= 0.5f;
        motoOponente.controlesInvertidos = true;

        Debug.Log("Efecto aplicado: velocidad reducida y controles invertidos.");

        // Espera 5 segundos antes de restaurar los efectos
        yield return new WaitForSeconds(8f);

        motoOponente.acceleration = aceleracionOriginal;
        motoOponente.controlesInvertidos = false;
        Debug.Log("Efectos del oponente restaurados.");

        // Espera 10 segundos antes de permitir otra interacción
        yield return new WaitForSeconds(15f);
        puedeInteractuar = true;
    }
}
