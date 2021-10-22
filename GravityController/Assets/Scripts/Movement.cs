using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine;

[RequireComponent(typeof(PlayerGravity))]
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
    [SerializeField] private float crouchMoveSpeed = 1.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float rotationSpeed = 5.0f;
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
    public Transform cameraMainTransform;
    private PlayerGravity playerGravity;

    //Player states
    [Header("Player states - For demonstration")]
    [SerializeField] private bool onGround;
    [SerializeField] private bool isWalking;
    [SerializeField] private bool isCrouching;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isFalling;

    public bool OnGround => onGround;
    public bool IsWalking => isWalking;
    public bool IsCrouching => isCrouching;
    public bool IsJumping => isJumping;
    public bool IsFalling => isFalling;

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
        playerGravity = transform.GetComponent<PlayerGravity>();
    }

    // Start is called before the first frame update
    void Start()
    {
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
        PlayerMove();
    }

    void PlayerMove()
    {
        Vector2 movement = movementControl.action.ReadValue<Vector2>().normalized;
        Vector3 move = new Vector3(movement.x, 0, movement.y);

        if (move.magnitude > 0.1f)
            isWalking = true;
        else
            isWalking = false;

        velocity = Vector3.zero;
        //velocity += Vector3.ProjectOnPlane(cameraMainTransform.right, transform.up).normalized * move.x;
        velocity += Vector3.ProjectOnPlane(cameraMainTransform.forward, transform.up).normalized * move.z;

        if (velocity.magnitude > 1f)
            velocity.Normalize();

        playerRigidbody.MovePosition(playerRigidbody.position + velocity * (moveSpeed * Time.fixedDeltaTime));

        Quaternion rotateTowards = Quaternion.Euler(0f, move.x * (rotationSpeed * Time.fixedDeltaTime), 0f);
        Quaternion rotatePlayer = Quaternion.Slerp(playerRigidbody.rotation, playerRigidbody.rotation * rotateTowards, Time.fixedDeltaTime * 3f);
        playerRigidbody.MoveRotation(rotatePlayer);
    }

    void PlayerJump()
    {
        isJumping = true;
        playerRigidbody.AddForce(-playerGravity.GravityDirection * jumpHeight, ForceMode.Impulse);
    }

    void PlayerCrouch()
    {
        isCrouching = true;
        moveSpeed = crouchMoveSpeed;
        playerCollider.height = playerCrouchingColliderHeight;
        playerCollider.center = playerCrouchingColliderCenter;

    }

    void PlayerCancelCrouch()
    {
        isCrouching = false;
        moveSpeed = normalMoveSpeed;
        playerCollider.height = playerDefaultColliderHeight;
        playerCollider.center = playerDefaultColliderCenter;
    }

    private void OnCollisionStay(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "SpeedBoost":
                moveSpeed = SpeedBoost;
                jumpHeight = normalJumpheight;
                break;
            case "Mud":
                moveSpeed = MudSpeed;
                jumpHeight = normalJumpheight;
                break;
            case "JumpPad":
                jumpHeight = JumpBoost;
                moveSpeed = normalMoveSpeed;
                break;
            default:
                moveSpeed = normalMoveSpeed;
                jumpHeight = normalJumpheight;
                break;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        moveSpeed = normalMoveSpeed;
        jumpHeight = normalJumpheight;
    }

}
//overlapping gravity areas has a major problem...fix that shit.
//walking on slopes and stairs.
//inverse gravity..for tricking purposes :B