using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFollowCamera : MonoBehaviour
{
    public Transform cameraTransform;
    public float distanceFromCamera = 2.0f;
    // Start is called before the first frame update
    void Start()
    { 
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = cameraTransform.position + cameraTransform.forward * distanceFromCamera;
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
    }
}
