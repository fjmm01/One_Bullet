using UnityEngine;
using UnityEngine.UIElements;

public class PlayerSliding : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform playerObj;
    private PlayerMovement playerMovement;

    [Header("Sliding Settings")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;

    public float slideYScale;
    private float startYScale;


    private float horizontalInput;
    private float verticalInput;


    private bool isSliding;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();

        startYScale = playerObj.localScale.y;
    }


    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Horizontal");

        if(playerMovement.currentState == PlayerMovement.MovementState.sprinting && Input.GetKeyDown(KeyCode.LeftControl))
        {
            StartSlide();
        }

        if(Input.GetKeyUp(KeyCode.LeftControl) && isSliding)
        {
            StopSlide();
        }
    }

    private void FixedUpdate()
    {
        if(isSliding)
        {
            SlidingMovement();
            
        }
    }
    private void StartSlide()
    {
        isSliding = true;
        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse); // Add a small downward force to prevent clipping

        slideTimer = maxSlideTime;
    }
    private void SlidingMovement()
    {
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
        inputDir.Normalize();

        //Normal Slide
        if(!playerMovement.OnSlope() || rb.linearVelocity.y > -0.1f)
        {
            rb.AddForce(inputDir * slideForce, ForceMode.Force);

            slideTimer -= Time.fixedDeltaTime;
        }
        else
        {
            rb.AddForce(playerMovement.GetSlopMoveDirection(inputDir) * slideForce, ForceMode.Force);
        }

        if (slideTimer <= 0)
        {
            StopSlide();
        }
    }

    private void StopSlide()
    {
        isSliding = false;
        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);

    }
}


