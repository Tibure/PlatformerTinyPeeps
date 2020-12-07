using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransition : MonoBehaviour
{
    public void Awake()
    {
        TransitionCamera();
    }
    public void TransitionCamera()
    {
        FindObjectOfType<CameraEffect>().StartCoroutineUnPixelisation();
    }
}
