using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine;

[RequireComponent(typeof(PlayerGravity))]
public class Movement : MonoBehaviour
{
    //Get references to ipnut actions
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
    private Vector3 playerDefaultColliderCenter;
    private float playerDefaultColliderHeight;
    private Vector3 velocity;

    [Header("Player movement multipliers")]
    [SerializeField] private float JumpBoost = 25f;
    [SerializeField] private float SpeedBoost = 50f;
    [SerializeField] private float MudSpeed = 1f;

    [Header("Player states - For demonstration")]
    [SerializeField] private bool onGround;
    [SerializeField] private bool isWalking;
    [SerializeField] private bool isCrouching;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isFalling;

    //make reference to the player states for the animation controller
    public bool IsWalking => isWalking;
    public bool IsCrouching => isCrouching;
    public bool IsJumping => isJumping;
    public bool IsFalling => isFalling;

    private Rigidbody playerRigidbody;
    private CapsuleCollider playerCollider;
    public Transform cameraMainTransform;
    private PlayerGravity playerGravity;

    // OnEnable is called when the active game object (player) is enabled
    private void OnEnable()
    {
        //start actively monitoring all recieving inputs from devices that matches any bindings associated with the actions
        movementControl.action.Enable();
        jumpControl.action.Enable();
        crouchControl.action.Enable();
    }

    // OnDisable is called when the active game object (player) is disabled
    private void OnDisable()
    {
        //disable the input actions
        movementControl.action.Disable();
        jumpControl.action.Disable();
        crouchControl.action.Disable();
    }

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        playerGravity = transform.GetComponent<PlayerGravity>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //make a reference to the players default collider height and center of the collider.
        //Useful for the crouching function where the height needs to be reduced and the center needs to be altered
        playerDefaultColliderCenter = playerCollider.center;
        playerDefaultColliderHeight = playerCollider.height;

        //make a reference to the players default movement speed and jump height.
        //useful for handling the variable changes on movement and jump multiplier areas
        normalJumpheight = jumpHeight;
        normalMoveSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //Check whether the player is grounded (standing, walking, crouching, etc. in a surface with the 'Ground' layermask)
        onGround = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        //set players jumping state (useful for animations)
        isJumping = false;

        //set players falling states (useful for animations)
        if (onGround)
            isFalling = false;
        if (!onGround)
            isFalling = true;

        //call jump action only if the player is grounded, not crouching, and the key binding for the jump action is pressed
        if (onGround && jumpControl.action.triggered && !isCrouching)
            PlayerJump();

        //check whether the key binding for the 'crouch' action (interaction:hold) has been/is being pressed. 
        crouchControl.action.started += context =>
        {
            crouchActionTriggered = true;
        };

        //check whether the key binding for the 'crouch' action (interaction:hold) has been released
        crouchControl.action.canceled += context =>
        {
            crouchActionTriggered = false;
            PlayerCancelCrouch();
        };

        //call crouch action only if the player is grounded and the key binding for the crouch action os being actively pressed
        if (onGround && crouchActionTriggered)
            PlayerCrouch();
    }

    // FixedUpdate is framerate independant and is called once, zero, or multiple times per frame, depending on the required physics calculations.
    private void FixedUpdate()
    {
        PlayerMove();
    }

    void PlayerMove()
    {
        //read the inputs for the move action mapped to WASD and arrow keys on keyboard and the left stick on game pads.
        Vector2 movement = movementControl.action.ReadValue<Vector2>().normalized;
        Vector3 move = new Vector3(movement.x, 0, movement.y);

        //move.magnitue returns the length of the 'move' Vector3. (length of the Vector3 : {x*x + y*y + z*z})
        //set players walking states depending on the inputs (useful for animations)
        if (move.magnitude > 0.1f)
            isWalking = true;
        else
            isWalking = false;

        velocity = Vector3.zero;
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