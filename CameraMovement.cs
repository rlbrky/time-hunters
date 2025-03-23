using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    [SerializeField]
    public float sensitivity = 5.0f;
    [SerializeField]
    public float smoothing = 2.0f;
    // get the incremental value of mouse moving
    private Vector2 mouseLook;
    // smooth the mouse moving
    private Vector2 smoothV;

    public bool isHiding;
    [Header("Leaning")] 
    [SerializeField] private float leaningSmoothing = 10.0f;
    [SerializeField] private float leanMax = 15;
    [HideInInspector] public Transform leaningPoint;

    private float _leanDegree;
    
    void Update () 
    {
        if (!isHiding)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                _leanDegree = Mathf.Lerp(_leanDegree, leanMax, 1f / leaningSmoothing);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                _leanDegree = Mathf.Lerp(_leanDegree, -leanMax, 1f / leaningSmoothing);
            }
            else
            {
                _leanDegree = Mathf.Lerp(_leanDegree, 0, 1f / leaningSmoothing);
            }
        }  
        
        // md is mouse delta
        var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        // the interpolated float result between the two float values
        smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);
        // incrementally add to the camera look
        mouseLook += smoothV;

        mouseLook.y = Mathf.Clamp(mouseLook.y, -75.0f, 80.0f);
        
        //Leaning limits
        _leanDegree = Mathf.Clamp(_leanDegree, -leanMax, leanMax);
        leaningPoint.localRotation = Quaternion.Euler(0, mouseLook.x, _leanDegree);
        
        // vector3.right means the x-axis
        transform.localRotation = Quaternion.Euler(-mouseLook.y, 0, 0);
    }
}