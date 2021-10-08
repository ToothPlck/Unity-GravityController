using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class PlayerGravity : MonoBehaviour
{
    public float gravityForce = 1500;
    public float rotationSpeedToGravity = 5f;
    private Rigidbody playerRigibody;
    private List<GravityController> gravityAreas;

    // Start is called before the first frame update
    void Start()
    {
        //Get the players rigidbody component
        playerRigibody = transform.GetComponent<Rigidbody>();
        gravityAreas = new List<GravityController>();
    }

    public Vector3 GravityDirection
    {
        get
        {
            if (gravityAreas.Count == 0)
                return Vector3.zero;
            gravityAreas.Sort((area1, area2) => area1.Priority.CompareTo(area2.Priority));
            return gravityAreas.Last().GetGravityDirection(this).normalized;
        }
    }

    // FixedUpdate is called once per physics update
    private void FixedUpdate()
    {
        playerRigibody.AddForce(GravityDirection * (gravityForce * Time.fixedDeltaTime), ForceMode.Acceleration);

        Quaternion playerUpRotation = Quaternion.FromToRotation(transform.up, -GravityDirection);
        Quaternion rotatePlayer = Quaternion.Slerp(playerRigibody.rotation, playerUpRotation * playerRigibody.rotation, Time.deltaTime * rotationSpeedToGravity);
        playerRigibody.MoveRotation(rotatePlayer);
    }

    public void AddGravityArea(GravityController gravity)
    {
        gravityAreas.Add(gravity);
    }

    public void RemoveGravityArea(GravityController gravity)
    {
        gravityAreas.Remove(gravity);
    }
}
