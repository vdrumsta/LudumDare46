using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantRotate : MonoBehaviour
{
    public Transform target;

    // Angular speed in radians per sec.
    public float speed = 1.0f;

    public bool debug;

    // Update is called once per frame
    void Update()
    {
        // Get angle between 2 vectors
        //Mathf.Acos(Vector3.Dot(vector1.normalized, vector2.normalized));

        var playerRotation = transform.forward;
        playerRotation.y = 0;

        // Determine which direction to rotate towards
        Vector3 targetDirection = target.position - transform.position;
        targetDirection.y = 0;

        // The step size is equal to speed times frame time.
        float singleStep = speed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        if (debug)
        {
            // Draw a ray pointing at our target in
            Debug.DrawRay(transform.position, newDirection, Color.red);
        }

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);
    }
}
