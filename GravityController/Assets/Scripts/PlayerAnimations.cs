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
        //get the animator parameters
        isWalkingHash = Animator.StringToHash("isWalking");
        isCrouchingHash = Animator.StringToHash("isCrouching");
        isJumpingHash = Animator.StringToHash("isJumping");
        isFallingHash = Animator.StringToHash("isFalling");
    }

    // Update is called once per frame
    void Update()
    {
        //trigger player falling/!falling animation handling
        if (playerMovement.IsFalling)
            animator.SetBool(isFallingHash, true);

        if (!playerMovement.IsFalling)
        {
            animator.SetBool(isFallingHash, false);
            animator.SetBool(isJumpingHash, false);
        }

        //trigger player walking/!walking animation handling
        if (playerMovement.IsWalking && !playerMovement.IsCrouching)
            animator.SetBool(isWalkingHash, true);
        if (!playerMovement.IsWalking)
            animator.SetBool(isWalkingHash, false);

        //trigger player walking/!walking while crouching animation handling
        if (playerMovement.IsWalking && playerMovement.IsCrouching)
            animator.SetBool(isWalkingHash, true);
        if (!playerMovement.IsWalking && !playerMovement.IsCrouching)
            animator.SetBool(isWalkingHash, false);

        //trigger player crouching/!crouching animation handling
        if (playerMovement.IsCrouching)
            animator.SetBool(isCrouchingHash, true);
        if (!playerMovement.IsCrouching)
            animator.SetBool(isCrouchingHash, false);

        //trigger player jumping animation handling
        if (playerMovement.IsJumping)
            animator.SetBool(isJumpingHash, true);

    }
}
