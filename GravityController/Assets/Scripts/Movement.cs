using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(PlayerGravity))]
public class Movement : MonoBehaviour
{
    //Get references to input actions
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
        //projectOnPlane projects a vector3 defined by a normal perpendicular to the surface, in this case the vector projected is the cameras forward, to the players normal
        velocity += Vector3.ProjectOnPlane(cameraMainTransform.forward, transform.up).normalized * move.z;

        if (velocity.magnitude > 1f)
            velocity.Normalize();

        //Apply the movement vector to the current position, which is multiplied by fixedDeltaTime and moveSpeed for a smooth movement
        playerRigidbody.MovePosition(playerRigidbody.position + velocity * (moveSpeed * Time.fixedDeltaTime));

        //set the rotation of the player around its Y axis
        Quaternion rotateTowards = Quaternion.Euler(0f, move.x * (rotationSpeed * Time.fixedDeltaTime), 0f);
        //interpolate between the players current rotation and the desired rotation (for smooth rotation)
        Quaternion rotatePlayer = Quaternion.Slerp(playerRigidbody.rotation, playerRigidbody.rotation * rotateTowards, Time.fixedDeltaTime * 3f);
        playerRigidbody.MoveRotation(rotatePlayer);
    }

    void PlayerJump()
    {
        //set players jumping state (useful for animations)
        isJumping = true;
        //add an instant force to the player in the countering direction of gravity (players up direction)
        playerRigidbody.AddForce(-playerGravity.GravityDirection * jumpHeight, ForceMode.Impulse);
    }

    void PlayerCrouch()
    {
        //set players crouching states (useful for animations)
        isCrouching = true;
        //change the players movement speed while crouching
        moveSpeed = crouchMoveSpeed;
        //decrease the height of the players collider and set the collider at the bottom of the players mesh
        playerCollider.height = playerCrouchingColliderHeight;
        playerCollider.center = playerCrouchingColliderCenter;

    }

    void PlayerCancelCrouch()
    {
        //reverse the actions done in the PlayerCrouch() function when the crouch button is released
        isCrouching = false;
        moveSpeed = normalMoveSpeed;
        playerCollider.height = playerDefaultColliderHeight;
        playerCollider.center = playerDefaultColliderCenter;
    }

    // OnCollisionStay is called once per frame for every collider/rigidbody that the players collider is touching/colliding with
    private void OnCollisionStay(Collision collision)
    {
        //adjust the players movement and jump variables depending on the tags of the colliders
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

    // OnCollisionExit is called when the players collider has stopped touching/colliding with another collider/rigidbody
    private void OnCollisionExit(Collision collision)
    {
        //revert any changes done to the players movement and jump variables
        moveSpeed = normalMoveSpeed;
        jumpHeight = normalJumpheight;
    }

}