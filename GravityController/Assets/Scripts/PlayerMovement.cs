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
    }

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {

        //
        Debug.DrawRay(cameraMainTransform.position, cameraMainTransform.forward * 5, Color.magenta);

        Debug.DrawRay(transform.position, transform.forward * 5, Color.yellow);
        //


        onGround = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        Vector2 movement = movementControl.action.ReadValue<Vector2>();

        Vector3 move = new Vector3(movement.x, 0, movement.y);
        move = transform.forward * move.z + transform.right * move.x;

        playerRigidbody.MovePosition(playerRigidbody.position + move * (moveSpeed * Time.fixedDeltaTime));

        if(movement != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + cameraMainTransform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
    }
}
