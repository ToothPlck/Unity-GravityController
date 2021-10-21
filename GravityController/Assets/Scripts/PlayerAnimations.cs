using UnityEngine;

//[RequireComponent(typeof(Movement))]
public class PlayerAnimations : MonoBehaviour
{

    //private Movement playerMovement;
    Animator animator;
    int isWalkingHash;
    public bool hey;

    private void Awake()
    {
        //playerMovement = GetComponent<Movement>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        isWalkingHash = Animator.StringToHash("isWalking");
    }

    // Update is called once per frame
    void Update()
    {
        //print(playerMovement.OnGround);
        if (hey)
            animator.SetBool(isWalkingHash, true);
        if (!hey)
            animator.SetBool(isWalkingHash, false);
    }
}
