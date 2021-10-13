using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerGravity))]
public class PlayerController : MonoBehaviour
{



    ////C# class generated by the new input system according to the selected keybindings
    //private InputsController inputActions;
    //private InputAction movement;
    //private Rigidbody playerRigidbody;
    //private PlayerGravity playerGravity;

    //[SerializeField] private bool isGrounded;
    //[SerializeField] private Transform groundCheckObject;
    //[SerializeField] private LayerMask groundCheckMask;
    //[SerializeField] private float groundCheckRadius = 0.3f;
    //[SerializeField] private float moveSpeed = 1.0f;
    //[SerializeField] private bool isRunning;

    //private void Awake()
    //{
    //    playerRigidbody = GetComponent<Rigidbody>();
    //    playerGravity = transform.GetComponent<PlayerGravity>();
    //    inputActions = new InputsController();
    //}

    //private void Start()
    //{
    //    playerRigidbody.freezeRotation = true;
    //}

    //private void OnEnable()
    //{
    //    inputActions.Player.Jump.started += DoJump;
    //    movement = inputActions.Player.Move;
    //    inputActions.Player.Enable();
    //}

    //private void DoJump(InputAction.CallbackContext obj)
    //{
    //    throw new NotImplementedException();
    //}

    //private void OnDisable()
    //{
    //    inputActions.Player.Jump.started -= DoJump;
    //    inputActions.Player.Disable();
    //}

    //private void Update()
    //{
    //    isGrounded = Physics.CheckSphere(groundCheckObject.position, groundCheckRadius, groundCheckMask);
    //}

    //private void FixedUpdate()
    //{
    //    //isRunning = 
    //    PlayerMovement();
    //}

    //void PlayerMovement()
    //{
    //    playerRigidbody.AddForce(movement.ReadValue<Vector2>().x * moveSpeed, 0, movement.ReadValue<Vector2>().y * moveSpeed, ForceMode.Acceleration);
    //}
}


/*
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private Transform _groundCheck;

    private bool isRunning;
    private bool isGrounded;
    private Rigidbody playerRigidbody;
    private Vector3 direction;
    private PlayerGravity playerGravity;

    public float verticalSpeed = 7f;
    public float localRotationSpeed = 1500f;
    private float groundCheckRadius = 0.3f;
    public float jumpForce = 500f;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = transform.GetComponent<Rigidbody>();
        playerGravity = transform.GetComponent<PlayerGravity>();
    }

    // Update is called once per frame
    void Update()
    {
        direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
        isGrounded = Physics.CheckSphere(_groundCheck.position, groundCheckRadius, _groundMask);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            playerRigidbody.AddForce(-playerGravity.GravityDirection * jumpForce, ForceMode.Impulse);
    }

    // FixedUpdate is called once per physics update
    private void FixedUpdate()
    {
        isRunning = direction.magnitude > 0.1f;

        if (isRunning)
        {
            Vector3 moveDirection = transform.forward * direction.z;
            playerRigidbody.MovePosition(playerRigidbody.position + moveDirection * (verticalSpeed * Time.fixedDeltaTime));

            Quaternion rotateTowards = Quaternion.Euler(0f, direction.x * (localRotationSpeed * Time.fixedDeltaTime), 0f);
            Quaternion rotatePlayer = Quaternion.Slerp(playerRigidbody.rotation, playerRigidbody.rotation * rotateTowards, Time.fixedDeltaTime * 3f);
            playerRigidbody.MoveRotation(rotatePlayer);
        }
    }
 */
