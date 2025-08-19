


using System;
using System.Security.Cryptography;
using System.Transactions;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    private float moveSpeed = 5f;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float groundDrag = 5f;
    [SerializeField] private Transform orientation;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpCooldown = 0.25f;
    [SerializeField] private float airMultiplier = 0.4f;

    [Header("Crouching")]
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private float crouchYScale;
    [SerializeField] private float startYScale;

    bool readyToJump = true;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    [Header("GroundCheck")]
    public float playerHeight;
    public LayerMask groundLayer;
    public bool isGrounded;

    [Header("Slope Handling")]
    [SerializeField] private float maxSlopeAngle = 45f; // Maximum slope angle the player can walk on
    private RaycastHit slopeHit; // Used to detect slopes
    [SerializeField] private bool exitingSlope;


    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    public MovementState currentState;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevents the player from tipping over
        startYScale = transform.localScale.y; // Store the initial Y scale of the player
    }

    void Update()
    {
        //groundCheck
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, groundLayer);
        MyInput();
        SpeedControl();
        StateHandler();


        //Handle drag
        if (isGrounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0f;
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void StateHandler()
    {
        if(Input.GetKey(KeyCode.LeftControl))
        {
            currentState = MovementState.crouching;
            moveSpeed = crouchSpeed;
            
        }
        //Mode - Sprinting
        if (isGrounded && Input.GetKey(KeyCode.LeftShift))
        {
            currentState = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (isGrounded)
        {
            currentState = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            currentState = MovementState.air;
        }
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if(Input.GetKeyDown(KeyCode.Space) && readyToJump && isGrounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            // Crouch
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse); // Add a small downward force to prevent clipping
            
        }
        if(Input.GetKeyUp(KeyCode.LeftControl))
        {
            // Uncrouch
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //On Slope
        if(OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopMoveDirection(moveDirection) * moveSpeed * 20, ForceMode.Force);
            if(rb.linearVelocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80, ForceMode.Force); // Prevents the player from jumping off slopes
            }
        }
        if (isGrounded)
        { 
            rb.AddForce(moveDirection.normalized * moveSpeed * 10, ForceMode.Force);
        }
        else if(!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10 * airMultiplier, ForceMode.Force);
        }

        //turn gravity off when on slope
        rb.useGravity = !OnSlope();

    }

    private void SpeedControl()
    {

        if(OnSlope() && !exitingSlope)
        {
            if(rb.linearVelocity.magnitude > moveSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
            }
        }
        //Limit the speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
        
    }

    private void Jump()
    {
        exitingSlope = true;
        //reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}
