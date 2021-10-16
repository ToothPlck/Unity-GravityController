using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference movementControl;
    [SerializeField] private InputActionReference jumpControl;
    [SerializeField] private InputActionReference crouchControl;

    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;

    private float normalMoveSpeed;
    private float normalJumpheight;

    [SerializeField] private float JumpBoost = 25f;
    [SerializeField] private float SpeedBoost = 50f;
    [SerializeField] private float MudSpeed = 1f;

    private Rigidbody playerRigidbody;
    private Vector3 velocity;
    private bool onGround;
    private bool isCrouching;
    //[SerializeField] private float rotationSpeed = 4.0f;
    private Transform cameraMainTransform;
    private PlayerGravity playerGravity;
    private Vector3 playerDefaultScale;
    [SerializeField] private Vector3 playerCrouchingScale;

    //[Header("Player step climb:")]
    //[SerializeField] float stepHeight = 0.3f;
    //[SerializeField] float stepSmooth = 0.1f;

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
        cameraMainTransform = Camera.main.transform;
        playerGravity = transform.GetComponent<PlayerGravity>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody.freezeRotation = true;
        playerDefaultScale = transform.localScale;

        normalJumpheight = jumpHeight;
        normalMoveSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        MovementDirection();

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
        PlayerMove();
        //PlayerStepClimb();
    }

    void MovementDirection()
    {
        Vector2 movement = movementControl.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(movement.x, 0, movement.y);

        velocity = Vector3.zero;
        velocity += Vector3.ProjectOnPlane(cameraMainTransform.right, transform.up).normalized * move.x;
        velocity += Vector3.ProjectOnPlane(cameraMainTransform.forward, transform.up).normalized * move.z;

        if (velocity.magnitude > 1f)
            velocity.Normalize();
    }

    void PlayerMove()
    {
        playerRigidbody.MovePosition(playerRigidbody.position + velocity * (moveSpeed * Time.fixedDeltaTime));
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

    //void PlayerStepClimb()
    //{
    //    int stairsLayer = LayerMask.NameToLayer("Stairs");
    //    RaycastHit hitLower;
    //    float playerLowerRaycast = 0.9f;
    //    Vector3 lowerRayStartPoint = new Vector3(transform.position.x, transform.position.y - playerLowerRaycast, transform.position.z);
    //    Debug.DrawRay(lowerRayStartPoint, transform.forward * 5, Color.green);
    //    if (Physics.Raycast(lowerRayStartPoint, transform.forward, out hitLower, 10f, stairsLayer))
    //    {
    //        print("lowwer");
    //        Vector3 upperRayStartPoint = new Vector3(transform.position.x, transform.position.y - stepHeight, transform.position.z);
    //        Debug.DrawRay(upperRayStartPoint, transform.forward * 5, Color.red);
    //        RaycastHit hitUpper;
    //        if(!Physics.Raycast(upperRayStartPoint, transform.forward, out hitUpper, 0.5f, stairsLayer))
    //        {
    //            print("uppser");
    //            playerRigidbody.position -= new Vector3(0f, -stepSmooth, 0f);
    //        }
    //    }
    //}

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
