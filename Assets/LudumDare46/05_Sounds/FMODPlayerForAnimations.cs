using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

//This script have to be attached in the same object that have the animator
public class FMODPlayerForAnimations : MonoBehaviour
{
    // Start is called before the first frame update
    void PlaySound(string path)

    {   //Play the Sound when called by the animation
        FMODUnity.RuntimeManager.PlayOneShot(path, GetComponent<Transform>().position);
    }

}