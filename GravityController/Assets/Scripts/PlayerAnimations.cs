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
        isWalkingHash = Animator.StringToHash("isWalking");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement.IsFalling)
            animator.SetBool(isFallingHash, true);

        if (!playerMovement.IsFalling)
        {
            animator.SetBool(isFallingHash, false);
            animator.SetBool(isJumpingHash, false);
        }


        if (playerMovement.IsWalking && !playerMovement.IsCrouching)
            animator.SetBool(isWalkingHash, true);
        if (!playerMovement.IsWalking)
            animator.SetBool(isWalkingHash, false);


        if (playerMovement.IsWalking && playerMovement.IsCrouching)
            animator.SetBool(isWalkingHash, true);
        if (!playerMovement.IsWalking && !playerMovement.IsCrouching)
            animator.SetBool(isWalkingHash, false);


        if (playerMovement.IsCrouching)
            animator.SetBool(isCrouchingHash, true);
        if (!playerMovement.IsCrouching)
            animator.SetBool(isCrouchingHash, false);



        if (playerMovement.IsJumping)
            animator.SetBool(isJumpingHash, true);

    }
}
