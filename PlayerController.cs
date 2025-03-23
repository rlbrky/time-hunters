using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
    public NetworkVariable<Quaternion> Rotation = new NetworkVariable<Quaternion>();

    public GameObject inventoryGO;
    public Transform cameraParent;
    public Transform itemCarryPos;
    public Vector3 cameraPosition;
    public float speed = 10.0f;
    public bool isHiding;

    private Camera playerCamera;
    private CameraMovement cameraSC;
    private Rigidbody _rigidbody;
    private Vector3 moveDir;
    private float translation;
    private float straffe;
    
    [Header("Jump")]
    public float coyoteTime;
    public float jumpHeight;
    public int gravityMultiplier = 1;
    public float maxYSpeed = -10f;
    public float maxJumpForce = 10f;

    private float gravity;
    private float ySpeed = 0;
    private float? lastGroundedTime;
    private float? jumpPressedTime;
    
    [Header("Ground Check")]
    [SerializeField] private float playerHeight = 0.5f;
    [SerializeField] private float playerHeightPadding = 0.05f;
    [SerializeField] private LayerMask groundMask;
    private RaycastHit hitInfo;
    private bool isGrounded;
    
    [Header("Inspect System Variables")]
    public InspectSystem inspectSystem;
    [SerializeField] private LayerMask inspectLayer;
    [SerializeField] private float maxInspectDistance = 5f;
    
    [Header("Slope Mech")]
    [SerializeField] private float maxGroundAngle = 120;
    [SerializeField] private bool debug;
    [SerializeField] private LayerMask SlopeLayer;
    private RaycastHit slopeHit;
    private float groundAngle;

    //Inventory stuff
    public PickableItem itemToPickUp;
    
    private void Awake()
    {
        inspectSystem = GetComponent<InspectSystem>();
        inspectSystem.enabled = false;
        
        _rigidbody = GetComponent<Rigidbody>();
        playerCamera = Camera.main;
        
        playerCamera.transform.SetParent(cameraParent);
        cameraSC = playerCamera.GetComponent<CameraMovement>();
        cameraSC.enabled = true;
        cameraSC.leaningPoint = cameraParent;
        playerCamera.transform.localPosition = cameraPosition;
    }

    // Use this for initialization
    void Start () {
        // turn off the cursor
        Cursor.lockState = CursorLockMode.Locked;		
    }
	
    // Update is called once per frame
    void Update ()
    {
        if (isHiding) return;
        if (Input.GetKeyDown(KeyCode.O) && Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, maxInspectDistance, inspectLayer))
        {
            if (hitInfo.collider.CompareTag("Inspectable"))
            {
                inspectSystem.enabled = true;
                inspectSystem.pickedUpPos = hitInfo.transform.position;
                inspectSystem.objToInspect = hitInfo.transform;
                
                hitInfo.transform.position = inspectSystem.inspectLocation.position;
                inspectSystem.BeginInspection();
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryGO.activeSelf)
            {
                cameraSC.enabled = true;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                inventoryGO.SetActive(false);
            }
            else
            {
                cameraSC.enabled = false;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                inventoryGO.SetActive(true);
            }
        }
        
        gravity = Physics.gravity.y * gravityMultiplier;
        ySpeed += gravity * Time.deltaTime;
        if(ySpeed < maxYSpeed)
            ySpeed = maxYSpeed;
        if (ySpeed > maxJumpForce)
            ySpeed = maxJumpForce;

        CheckForGround();
        
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            jumpPressedTime = Time.time;
        }

        if (isGrounded)
        {
            lastGroundedTime = Time.time;
            ySpeed = 0;
        }
        
        if (Time.time - lastGroundedTime <= coyoteTime)
        {
            if (Time.time - jumpPressedTime <= coyoteTime)
            {
                isGrounded = false;
                ySpeed = Mathf.Sqrt(jumpHeight * -3f * gravity);
                jumpPressedTime = null;
                lastGroundedTime = null;
                //animator.SetTrigger("Jump");
            }
        }
        
        Vector3 camForward = playerCamera.transform.forward;
        Vector3 camRight = playerCamera.transform.right;
        
        camForward.y = 0;
        camRight.y = 0;
        
        translation = Input.GetAxis("Vertical") * speed;
        straffe = Input.GetAxis("Horizontal") * speed;

        Vector3 forwardRelative = translation * camForward;
        Vector3 rightRelative = straffe * camRight;
        
        moveDir = forwardRelative + rightRelative;

        _rigidbody.velocity = new Vector3(moveDir.x, ySpeed, moveDir.z);

        if (OnSlope())
        {
            Debug.Log("OnSlope");
            _rigidbody.velocity = GetSlopeMoveDir() * speed;
        }
        else
        {
            Debug.Log("Not on Slope");
        }
        
        if (Input.GetKeyDown("escape")) {
            // turn on the cursor
            Cursor.lockState = CursorLockMode.None;
        }

        if (itemToPickUp != null && Input.GetKeyDown(KeyCode.F))
        {
            itemToPickUp.PickUpItem();
            itemToPickUp = null;
        }
        
    }

    private void FixedUpdate()
    {
        // Update this player's position on other clients
        if (!IsLocalPlayer)
        {
            transform.position = Position.Value;
            transform.rotation = Rotation.Value;
            return;
        }

        SubmitPositionAndRotationServerRpc(OwnerClientId, transform.position, transform.rotation);
    }
    
    private void CheckForGround()
    {
        if (ySpeed > 0) return;

        if(Physics.Raycast(transform.position, -Vector3.up, out hitInfo, playerHeight + playerHeightPadding, groundMask))
        {
            if(Vector3.Distance(transform.position, hitInfo.point) < playerHeight)
            {
                transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.up * playerHeight, 5 * Time.deltaTime);
            }
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    public void Hide(bool hiding)
    {
        isHiding = hiding;
        if (hiding)
        {
            _rigidbody.velocity = Vector3.zero;
        }
    }
    
    private bool OnSlope()
    {
        Debug.DrawRay(transform.position, Vector3.down * (playerHeight + 0.3f), Color.red);
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight + 0.3f))
        {
            Debug.Log(slopeHit.collider.name);
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxGroundAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDir()
    {
        return Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized;
    }
    
    [ServerRpc(RequireOwnership = false)]
    void SubmitPositionAndRotationServerRpc(ulong clientId, Vector3 newPosition, Quaternion newRotation)
    {
//        Debug.Log("pos:" + newPosition + " clientId=" + clientId);
        Position.Value = newPosition;
        Rotation.Value = newRotation;
    }
}