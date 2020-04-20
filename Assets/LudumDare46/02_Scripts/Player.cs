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
    private Animator characterAnimator;

    private int isWalking_hash = Animator.StringToHash("IsWalking");
    private int pickUp_hash = Animator.StringToHash("PickUP");
    private int drop_hash = Animator.StringToHash("Drop");
    private int die_hash = Animator.StringToHash("Die");

    public bool isWalkingAnimation { get { return characterAnimator.GetBool(isWalking_hash); } set { characterAnimator.SetBool(isWalking_hash, value); } }
    public bool pickUpAnimation { get { return characterAnimator.GetBool(pickUp_hash); } set { characterAnimator.SetBool(pickUp_hash, value); } }
    public bool dropAnimation { get { return characterAnimator.GetBool(drop_hash); } set { characterAnimator.SetBool(drop_hash, value); } }
    public bool dieAnimation { get { return characterAnimator.GetBool(die_hash); } set { characterAnimator.SetBool(die_hash, value); } }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        feet = GetComponentInChildren<PlayerFeet>();
        arms = GetComponentInChildren<PlayerArms>();
        characterAnimator = GetComponentInChildren<Animator>();
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
            isWalkingAnimation = true;
            transform.forward = moveVector;
        }
        else if(isWalkingAnimation)
        {
            isWalkingAnimation = false;
        }
    }
}
