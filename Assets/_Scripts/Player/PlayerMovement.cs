


using System;
using System.Security.Cryptography;
using System.Transactions;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]private float moveSpeed = 5f;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float groundDrag = 5f;
    [SerializeField] private Transform orientation;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpCooldown = 0.25f;
    [SerializeField] private float airMultiplier = 0.4f;

    [Header("Crouching Settings")]
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private float crouchYScale;
    [SerializeField] private float startYScale;
    bool isCrouching = false;

    bool readyToJump = true;
    Vector2 _moveInput;
    Vector3 moveDirection;

    Rigidbody rb;

    [Header("Sliding Settings")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;

    public float slideYScale;


    private bool isSliding;

    public bool IsSliding { get => isSliding; set => isSliding = value; }



    [Header("GroundCheck")]
    public float playerHeight;
    public LayerMask groundLayer;
    public bool isGrounded;

    [Header("Slope Handling")]
    [SerializeField] private float maxSlopeAngle = 45f; // Maximum slope angle the player can walk on
    private RaycastHit slopeHit; // Used to detect slopes
    [SerializeField] private bool exitingSlope;

    public MovementState currentState;

    #region Getters and Setters
    public bool ReadyToJump { get => readyToJump; set => readyToJump = value; }
    public Vector2 MoveInput { get => _moveInput; set => _moveInput = value; }
    public bool IsGrounded { get => isGrounded; set => isGrounded = value; }
    public bool IsCrouching { get => isCrouching; set => isCrouching = value; }
    #endregion

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

    

    private void StateHandler()
    {

        switch (PlayerController.CurrentState)
        {
            case MovementState.Walking:
                moveSpeed = walkSpeed;
                break;
            case MovementState.Sprinting:
                moveSpeed = sprintSpeed;
                break;
            case MovementState.Crouching:
                moveSpeed = crouchSpeed;
                break;
            case MovementState.Air:
                
                break;
        }
        
    }
    

    private void MovePlayer(Vector2 moveVector)
    {
        moveDirection = orientation.forward * moveVector.y + orientation.right * moveVector.x;

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
        if (isSliding) return; // no limitar velocidad durante el slide
        if (OnSlope() && !exitingSlope)
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


    #region Public Methods

    public void Move(Vector2 moveVector)
    {
        //Movement Logic
        MovePlayer(moveVector);
    }

    public void TryJump()
    {
        // Jump logic
        if (readyToJump && isGrounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    public void Crouch()
    {
        // Crouch
        isCrouching = true;
        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse); // Add a small downward force to prevent clipping
    }

    public void UnCrouch()
    {
        isCrouching = false;
        // Uncrouch
        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
    }

    public void StartSlide()
    {
        if (isSliding) return;
        rb.linearDamping = 0f; // Disable drag during slide
        isSliding = true;
        transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse); // Add a small downward force to prevent clipping
        
        slideTimer = maxSlideTime;
    }

    public void SlidingMovement(Vector2 direction)
    {

        Vector3 inputDir = orientation.forward * direction.y + orientation.right * direction.x;
        inputDir.Normalize();
        Debug.Log("Input Direction: " + inputDir);

        //Normal Slide
        if (!OnSlope() || rb.linearVelocity.y > -0.1f)
        {
            rb.AddForce(inputDir * slideForce, ForceMode.Force);

            slideTimer -= Time.fixedDeltaTime;
            Debug.Log(slideTimer);
        }
        else
        {
            rb.AddForce(GetSlopMoveDirection(inputDir) * slideForce, ForceMode.Force);
        }

        if (slideTimer <= 0)
        {
            StopSlide();
        }
    }

    public void StopSlide()
    {
        rb.linearDamping = groundDrag; // Restore drag after sliding
        isSliding = false;
        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);

    }
    #endregion
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
