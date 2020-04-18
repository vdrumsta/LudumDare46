using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]private float movementSpeed;

    private CharacterController controller;
    private Vector3 velocityVector;
    private PlayerFeet feet;
    private PlayerArms arms;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        feet = GetComponentInChildren<PlayerFeet>();
        arms = GetComponentInChildren<PlayerArms>();
    }

    void Update()
    {
        Vector3 moveVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));


        if (feet.IsGrounded)
        {
            moveVector.y = 0;
        }
        else
        {
            velocityVector.y += Physics.gravity.y * Time.deltaTime;
        }

        controller.Move(moveVector * Time.deltaTime * movementSpeed);
        controller.Move(velocityVector * Time.deltaTime);

        if (moveVector != Vector3.zero)
        {
            transform.forward = moveVector;
        }
    }
}
