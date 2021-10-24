using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class GravityController : MonoBehaviour
{
    [SerializeField] private int priority;
    public int Priority => priority; //Read the serialized private variable 'priority' given to the gravity collider in Unitys' inspector.

    // Start is called before the first frame update
    void Start()
    {
        //Set the trigger of the gravity objects collider if isTrigger = false
        transform.GetComponent<Collider>().isTrigger = true;
    }

    //abstract method to be implemented by other classes (Gravity, InverseGravity, etc.) to calculate to return the direction of gravity.
    public abstract Vector3 GetGravityDirection(PlayerGravity playerGravity);

    //Called when a player enters the gravity objects collider.
    private void OnTriggerEnter(Collider other)
    {
        //Get the PlayerGravitycomponent/class if it exists, from the collider (The player) which entered the gravity objects collider.
        if(other.TryGetComponent(out PlayerGravity playerGravity))
        {
            //Add this instance of the GravityController to the 'gravityAreas' list in the PlayerGravity class.
            playerGravity.AddGravityArea(this);
        }
    }

    //Called when a player exits the gravity objects collider.
    private void OnTriggerExit(Collider other)
    {
        //Get the PlayerGravitycomponent/class if it exists, from the collider (The player) which exited the gravity objects collider.
        if (other.TryGetComponent(out PlayerGravity playerGravity))
        {
            //Remove this instance of the GravityController to the 'gravityAreas' list in the PlayerGravity class.
            playerGravity.RemoveGravityArea(this);
        }
    }

}
