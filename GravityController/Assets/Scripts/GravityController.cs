using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class GravityController : MonoBehaviour
{
    [SerializeField]
    private int priority;
    public int Priority => priority; //======================================================================================================== what is => and why we using it?

    // Start is called before the first frame update
    void Start()
    {
        //Set the trigger of the gravity objects collider if isTrigger = false
        transform.GetComponent<Collider>().isTrigger = true;
    }

    public abstract Vector3 GetGravityDirection(PlayerGravity playerGravity);

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerGravity playerGravity))
        {
            playerGravity.AddGravityArea(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out PlayerGravity playerGravity))
        {
            playerGravity.RemoveGravityArea(this);
        }
    }

}
