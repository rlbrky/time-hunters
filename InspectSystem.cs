using System;
using UnityEngine;

public class InspectSystem : MonoBehaviour
{
    [Header("Inspection System")] 
    public Transform inspectLocation;
    public Transform objToInspect;
    public float rotationSpeed = 100f;

    [Header("Canceled Scripts")] 
    public PlayerController playerSC;
    public CameraMovement camSC;

    private Vector3 prevMousePos;

    public Vector3 pickedUpPos;
    public Vector3 pickedUpRot;

    private void Awake()
    {
        camSC = Camera.main.GetComponent<CameraMovement>();
    }

    public void BeginInspection()
    {
        
        pickedUpRot = objToInspect.rotation.eulerAngles;
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        playerSC.enabled = false;
        camSC.enabled = false;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            prevMousePos = Input.mousePosition;
        }

        //Apply difference between mouse previous position and current to rotate the object.
        if (Input.GetMouseButton(0))
        {
            Vector3 deltaMousePos = Input.mousePosition - prevMousePos;
            float rotationX = deltaMousePos.y * rotationSpeed * Time.deltaTime;
            float rotationY = -deltaMousePos.x * rotationSpeed * Time.deltaTime;
            
            Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
            objToInspect.rotation = rotation * objToInspect.rotation;
            
            prevMousePos = Input.mousePosition;
        }

        //End inspection
        if (Input.GetKeyDown(KeyCode.O))
        {
            objToInspect.position = pickedUpPos;
            objToInspect.rotation = Quaternion.Euler(pickedUpRot);
            objToInspect = null;
            
            Cursor.lockState = CursorLockMode.Locked;
            
            playerSC.enabled = true;
            camSC.enabled = true;
            enabled = false;
        }
    }
}
