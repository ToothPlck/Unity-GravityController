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
    private bool crouchActionTriggered;

    [Header("Check player grounded state")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.3f;

    [Header("Player movement")]
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float rotationSpeed = 5.0f;
    //[SerializeField] private Vector3 playerCrouchingScale;
    [SerializeField] private Vector3 playerCrouchingColliderCenter;
    [SerializeField] private float playerCrouchingColliderHeight;
    private float normalMoveSpeed;
    private float normalJumpheight;

    [Header("Player movement multipliers")]
    [SerializeField] private float JumpBoost = 25f;
    [SerializeField] private float SpeedBoost = 50f;
    [SerializeField] private float MudSpeed = 1f;

    private Rigidbody playerRigidbody;
    private CapsuleCollider playerCollider;
    private Transform cameraMainTransform;
    private PlayerGravity playerGravity;

    //Player states
    [SerializeField] private bool onGround;
    [SerializeField] private bool isWalking;
    [SerializeField] private bool isCrouching;
    [SerializeField] private bool isCrouchWalking;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isFalling;

    public bool OnGround => onGround;
    public bool IsWalking => isWalking;
    public bool IsCrouching => isCrouching;
    public bool IsCrouchWalking => isCrouchWalking;
    public bool IsJumping => isJumping;
    public bool IsFalling => isFalling;

    //private Vector3 playerDefaultScale;
    private Vector3 playerDefaultColliderCenter;
    private float playerDefaultColliderHeight;
    private Vector3 velocity;


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
        playerCollider = GetComponent<CapsuleCollider>();
        //cameraMainTransform = Camera.main.transform;
        playerGravity = transform.GetComponent<PlayerGravity>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //playerDefaultScale = transform.localScale;
        playerDefaultColliderCenter = playerCollider.center;
        playerDefaultColliderHeight = playerCollider.height;

        normalJumpheight = jumpHeight;
        normalMoveSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {

        onGround = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        isJumping = false;

        if (onGround)
            isFalling = false;

        if (!onGround)
            isFalling = true;

        if (onGround && jumpControl.action.triggered && !isCrouching)
            PlayerJump();

        crouchControl.action.started += context =>
        {
            crouchActionTriggered = true;
        };

        crouchControl.action.canceled += context =>
        {
            crouchActionTriggered = false;
            PlayerCancelCrouch();
        };

        if (onGround && crouchActionTriggered)
            PlayerCrouch();
    }

    private void FixedUpdate()
    {

    }

    void PlayerJump()
    {
        isJumping = true;
        playerRigidbody.AddForce(-playerGravity.GravityDirection * jumpHeight, ForceMode.Impulse);
    }

    void PlayerCrouch()
    {
        isCrouching = true;
        playerCollider.height = playerCrouchingColliderHeight;
        playerCollider.center = playerCrouchingColliderCenter;

    }

    void PlayerCancelCrouch()
    {
        isCrouching = false;
        playerCollider.height = playerDefaultColliderHeight;
        playerCollider.center = playerDefaultColliderCenter;
    }
}
