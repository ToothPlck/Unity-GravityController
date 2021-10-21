using UnityEngine;

[RequireComponent(typeof(Movement))]
public class PlayerAnimations : MonoBehaviour
{

    private Movement playerMovement;
    Animator animator;
    int isWalkingHash;
    int isCrouchingHash;
    int isJumpingHash;
    int isFallingHash;

    public bool hey;

    private void Awake()
    {
        playerMovement = GetComponent<Movement>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        isWalkingHash = Animator.StringToHash("isWalking");
        isCrouchingHash = Animator.StringToHash("isCrouching");
        isJumpingHash = Animator.StringToHash("isJumping");
        isFallingHash = Animator.StringToHash("isFalling");
    }

    // Update is called once per frame
    void Update()
    {
        //if (hey)
        //    animator.SetBool(isCrouchingHash, true);
        //if (!hey)
        //    animator.SetBool(isCrouchingHash, false);

        if (playerMovement.IsFalling)
        {
            animator.SetBool(isFallingHash, true);
        }

        if (!playerMovement.IsFalling)
        {
            animator.SetBool(isFallingHash, false);
            animator.SetBool(isJumpingHash, false);
        }


        if (playerMovement.IsCrouching)
            animator.SetBool(isCrouchingHash, true);
        if (!playerMovement.IsCrouching)
            animator.SetBool(isCrouchingHash, false);

        if (playerMovement.IsJumping)
            animator.SetBool(isJumpingHash, true);

    }
}
