using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Rover : MonoBehaviour
{
    [Header("Movement Settings")] [Tooltip("Base movement speed of the rover.")]
    public float baseSpeed = 5f;

    [Tooltip("How much the speed is affected by slope. Positive makes uphill slower and downhill faster.")] [Range(0f, 1f)]
    public float slopeSpeedFactor = 0.5f;

    [Tooltip("Maximum speed multiplier when going downhill.")] [Range(1f, 3f)]
    public float maxDownhillSpeedMultiplier = 2f;

    [Tooltip("Minimum speed multiplier when going uphill.")] [Range(0.1f, 1f)]
    public float minUphillSpeedMultiplier = 0.3f;

    [Tooltip("Downward force to keep the rover grounded.")]
    public float gravity = -9.81f;

    [Tooltip("Distance to cast downwards to sample the ground normal.")]
    public float groundCheckDistance = 0.2f;

    [Header("Turning Settings")] [Tooltip("How fast the rover turns (degrees per second).")]
    public float turnSpeed = 90f;

    [Tooltip("Should the rover turn slower when going uphill?")]
    public bool affectTurningWithSlope = true;

    [Header("Ground Alignment")] [Tooltip("How fast the rover aligns to the ground normal.")] [Range(1f, 20f)]
    public float alignmentSpeed = 5f;

    [Tooltip("Should the rover align to ground normal?")]
    public bool alignToGround = true;

    [Tooltip("Maximum angle the rover can tilt (degrees).")] [Range(0f, 90f)]
    public float maxTiltAngle = 45f;

    [Header("Physics Settings")] [Tooltip("How much the rover slides on steep slopes.")]
    public float slideThreshold = 45f;

    [Tooltip("Force applied when sliding on steep slopes.")]
    public float slideForce = 5f;

    private CharacterController controller;
    private float verticalVelocity = 0f;
    private Vector3 lastGroundNormal = Vector3.up;
    private float currentSpeedMultiplier = 1f;
    private Quaternion targetRotation;
    private float currentYRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        targetRotation = transform.rotation;
        currentYRotation = transform.eulerAngles.y;
    }

    void Update()
    {
        HandleGroundAlignment();
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        // 1) Read input
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (input.sqrMagnitude > 1f)
            input.Normalize();

        // 2) Transform input to world space (rover's local forward/right)
        Vector3 moveDir = transform.TransformDirection(input);

        // 3) Sample ground normal and calculate speed modifier
        currentSpeedMultiplier = 1f;
        Vector3 slideForceVector = Vector3.zero;

        if (IsGrounded())
        {
            RaycastHit hit;
            Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, controller.height / 2f + groundCheckDistance + 2))
            {
                lastGroundNormal = hit.normal;

                // Calculate slope angle
                float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);
                // Handle steep slopes with sliding
                if (slopeAngle > slideThreshold)
                {
                    Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;
                    slideForceVector = slideDirection * slideForce;
                }

                // Calculate speed modification based on slope direction
                if (moveDir.magnitude > 0.1f)
                {
                    // Project the slope direction onto the horizontal plane
                    Vector3 slopeDirection = new Vector3(-hit.normal.x, 0f, -hit.normal.z).normalized;

                    // Dot product: positive = uphill, negative = downhill
                    float slopeDot = Vector3.Dot(slopeDirection, moveDir.normalized);

                    // Calculate speed multiplier
                    if (slopeDot > 0) // Going uphill
                    {
                        currentSpeedMultiplier = Mathf.Lerp(1f, minUphillSpeedMultiplier, slopeDot * slopeSpeedFactor);
                    }
                    else // Going downhill
                    {
                        currentSpeedMultiplier = Mathf.Lerp(1f, maxDownhillSpeedMultiplier, -slopeDot * slopeSpeedFactor);
                    }
                }
            }

            // Small downward force to keep contact with ground
            verticalVelocity = -8f;
        }
        else
        {
            // Apply gravity when airborne
            verticalVelocity += gravity * Time.deltaTime;
        }

        // 4) Combine horizontal movement, slope effects, and vertical velocity
        Vector3 velocity = moveDir * baseSpeed * currentSpeedMultiplier;
        velocity += slideForceVector;
        velocity.y = verticalVelocity;

        // 5) Move the character controller
        controller.Move(velocity * Time.deltaTime);
    }

    private bool IsGrounded()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        return (Physics.Raycast(rayOrigin, Vector3.down, out _, controller.height / 2f + groundCheckDistance + 2));
    }

    void HandleRotation()
    {
        // Only rotate if there's horizontal input
        float horizontalInput = Input.GetAxis("Horizontal");
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            float turnSpeedModifier = affectTurningWithSlope ? currentSpeedMultiplier : 1f;
            float rotationAmount = horizontalInput * turnSpeed * turnSpeedModifier * Time.deltaTime;
            currentYRotation += rotationAmount;
        }
    }

    void HandleGroundAlignment()
    {
        if (!alignToGround)
        {
            // If alignment is disabled, just apply Y rotation
            transform.rotation = Quaternion.Euler(0, currentYRotation, 0);
            return;
        }

        // Calculate the target rotation based on ground normal
        Vector3 targetUp = lastGroundNormal;

        // Limit the tilt angle to prevent extreme tilting
        float tiltAngle = Vector3.Angle(Vector3.up, targetUp);
        if (tiltAngle > maxTiltAngle)
        {
            // Lerp between world up and ground normal to limit the tilt
            float lerpFactor = maxTiltAngle / tiltAngle;
            targetUp = Vector3.Lerp(Vector3.up, targetUp, lerpFactor).normalized;
        }

        // Calculate the target forward direction
        // Project the current forward direction onto the ground plane
        Vector3 currentForward = transform.forward;
        Vector3 targetForward = Vector3.ProjectOnPlane(currentForward, targetUp).normalized;

        // If the projected forward is too small, use the world forward projected instead
        if (targetForward.magnitude < 0.1f)
        {
            targetForward = Vector3.ProjectOnPlane(Vector3.forward, targetUp).normalized;
        }

        // Create the target rotation using the ground normal as up and projected forward
        targetRotation = Quaternion.LookRotation(targetForward, targetUp);

        // Apply the Y rotation from input
        Vector3 targetEuler = targetRotation.eulerAngles;
        targetEuler.y = currentYRotation;
        targetRotation = Quaternion.Euler(targetEuler);

        // Smoothly rotate towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, alignmentSpeed * Time.deltaTime);
    }

    // Optional: Visualize ground normal and slope direction in Scene view
    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            // Draw ground normal
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, lastGroundNormal * 2f);

            // Draw slope direction
            Vector3 slopeDirection = new Vector3(-lastGroundNormal.x, 0f, -lastGroundNormal.z).normalized;
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, slopeDirection * 2f);

            // Draw rover's up direction
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, transform.up * 2f);

            // Draw target up direction
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, lastGroundNormal * 1.5f);
        }
    }
}