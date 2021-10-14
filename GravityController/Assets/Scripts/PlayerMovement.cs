using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference movementControl;
    [SerializeField] private InputActionReference jumpControl;

    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    private Rigidbody playerRigidbody;
    private Vector3 playerVelocity;
    private bool onGround;
    [SerializeField] private float rotationSpeed = 4.0f;
    private Transform cameraMainTransform;
    private PlayerGravity playerGravity;

    private void OnEnable()
    {
        movementControl.action.Enable();
        jumpControl.action.Enable();
    }

    private void OnDisable()
    {
        movementControl.action.Disable();
        jumpControl.action.Disable();
    }

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        cameraMainTransform = Camera.main.transform;
        playerGravity = transform.GetComponent<PlayerGravity>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        ////
        ////Direction from the camera towards the player
        //Vector3 direction = transform.position - cameraMainTransform.position;
        //Debug.DrawRay(cameraMainTransform.position, direction * 5, Color.magenta);

        //Vector3 playerMoveDirection = new Vector3(direction.x, 0f, direction.z);
        ////playerMoveDirection.y = 0;
        ////Vector3 playerMoveDirection = direction + transform.position;
        //Debug.DrawRay(transform.position, playerMoveDirection * 5, Color.yellow);
        ////



        ////
        //int playerLayer = 1 << LayerMask.NameToLayer("Player");

        //Vector3 directionFromCameraToPlayer = transform.position - cameraMainTransform.position;
        //Physics.Raycast(cameraMainTransform.position, directionFromCameraToPlayer, out RaycastHit hit, 10, playerLayer);
        //Debug.DrawLine(cameraMainTransform.position, directionFromCameraToPlayer * 10, Color.green);
        //print(hit.collider.gameObject.tag);
        //Debug.DrawLine(transform.position, -hit.normal * 10, Color.red);
        ////




        onGround = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        Vector2 movement = movementControl.action.ReadValue<Vector2>();

        Vector3 move = new Vector3(movement.x, 0, movement.y);
        move = transform.forward * move.z + transform.right * move.x;

        playerRigidbody.MovePosition(playerRigidbody.position + move * (moveSpeed * Time.fixedDeltaTime));
    }
}
