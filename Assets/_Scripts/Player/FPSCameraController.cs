using UnityEngine;

/// <summary>
/// Updated FPS Camera Controller that works with the Command Pattern input system
/// </summary>
public class FPSCameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float sensX = 100f;
    [SerializeField] private float sensY = 100f;
    [SerializeField] private Transform orientation;

    [Header("Constraints")]
    [SerializeField] private float minVerticalAngle = -90f;
    [SerializeField] private float maxVerticalAngle = 90f;

    [Header("Smoothing")]
    [SerializeField] private bool enableSmoothing = false;
    [SerializeField] private float smoothingFactor = 5f;

    private float xRotation;
    private float yRotation;
    private Vector2 currentLookInput;
    private Vector2 smoothedLookInput;

    void Start()
    {
        InitializeCamera();
    }

    void Update()
    {
        ProcessLookInput();
        ApplyCameraRotation();
    }

    /// <summary>
    /// Called by LookCommand to set look input
    /// </summary>
    public void SetLookInput(Vector2 lookInput)
    {
        currentLookInput = lookInput;
    }

    private void InitializeCamera()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize rotation values
        Vector3 currentRotation = transform.localEulerAngles;
        xRotation = currentRotation.x;
        yRotation = currentRotation.y;

        // Ensure orientation reference
        if (!orientation)
        {
            Debug.LogWarning("FPSCameraController: No orientation Transform assigned!");
        }
    }

    private void ProcessLookInput()
    {
        Vector2 targetInput = currentLookInput * Time.deltaTime;

        if (enableSmoothing)
        {
            // Smooth the input for more fluid camera movement
            smoothedLookInput = Vector2.Lerp(smoothedLookInput, targetInput, smoothingFactor * Time.deltaTime);
            targetInput = smoothedLookInput;
        }

        // Apply sensitivity
        float mouseX = targetInput.x * sensX;
        float mouseY = targetInput.y * sensY;

        // Update rotation values
        yRotation += mouseX;
        xRotation -= mouseY; // Inverted for natural feel

        // Clamp vertical rotation
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);
    }

    private void ApplyCameraRotation()
    {
        // Apply rotation to camera
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // Apply rotation to orientation (for movement direction)
        if (orientation)
        {
            orientation.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }

    /// <summary>
    /// Public method to set sensitivity at runtime
    /// </summary>
    public void SetSensitivity(float newSensX, float newSensY)
    {
        sensX = newSensX;
        sensY = newSensY;
    }

    /// <summary>
    /// Public method to get current look direction
    /// </summary>
    public Vector3 GetLookDirection()
    {
        return transform.forward;
    }

    /// <summary>
    /// Public method to enable/disable camera rotation
    /// </summary>
    public void SetCameraEnabled(bool enabled)
    {
        this.enabled = enabled;
        if (!enabled)
        {
            currentLookInput = Vector2.zero;
            smoothedLookInput = Vector2.zero;
        }
    }

    /// <summary>
    /// Reset camera rotation to default values
    /// </summary>
    public void ResetRotation()
    {
        xRotation = 0f;
        yRotation = 0f;
        currentLookInput = Vector2.zero;
        smoothedLookInput = Vector2.zero;

        transform.localRotation = Quaternion.identity;
        if (orientation)
        {
            orientation.localRotation = Quaternion.identity;
        }
    }
}
