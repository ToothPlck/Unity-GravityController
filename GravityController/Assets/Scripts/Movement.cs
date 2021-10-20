using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{

    [Header("Player input actions")]
    [SerializeField] private InputActionReference movementControl;
    [SerializeField] private InputActionReference jumpControl;
    [SerializeField] private InputActionReference crouchControl;

    [Header("Check player grounded state")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.3f;

    [Header("Player movement")]
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float rotationSpeed = 5.0f;
    [SerializeField] private Vector3 playerCrouchingScale;
    private float normalMoveSpeed;
    private float normalJumpheight;

    [Header("Player movement multipliers")]
    [SerializeField] private float JumpBoost = 25f;
    [SerializeField] private float SpeedBoost = 50f;
    [SerializeField] private float MudSpeed = 1f;

    private Rigidbody playerRigidbody;
    private Transform cameraMainTransform;
    private PlayerGravity playerGravity;

    private Vector3 velocity;
    private bool onGround;
    private bool isCrouching;
    private Vector3 playerDefaultScale;


    private void OnEnable()
    {
        movementControl.action.Enable();
        jumpControl.action.Enable();
        crouchControl.action.Enable();
    }

    private void OnDisable()
    {
        movementControl.action.Disable();
        jumpControl.action.Disable();
        crouchControl.action.Disable();
    }

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        //cameraMainTransform = Camera.main.transform;
        playerGravity = transform.GetComponent<PlayerGravity>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerDefaultScale = transform.localScale;

        normalJumpheight = jumpHeight;
        normalMoveSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        

        onGround = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        if (onGround && jumpControl.action.triggered && !isCrouching)
            PlayerJump();

        crouchControl.action.started += context =>
        {
            isCrouching = true;
        };

        crouchControl.action.canceled += context =>
        {
            isCrouching = false;
            PlayerCancelCrouch();
        };

        if (onGround && isCrouching)
            PlayerCrouch();
    }

    private void FixedUpdate()
    {
        
    }

    void PlayerJump()
    {
        playerRigidbody.AddForce(-playerGravity.GravityDirection * jumpHeight, ForceMode.Impulse);
    }

    void PlayerCrouch()
    {
        transform.localScale = playerCrouchingScale;
    }

    void PlayerCancelCrouch()
    {
        transform.localScale = playerDefaultScale;
    }
}
