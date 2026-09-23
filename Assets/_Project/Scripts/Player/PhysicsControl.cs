using UnityEngine;

public class PhysicsControl : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Ground")]
    public bool isGrounded;
    [SerializeField] private float groundRayLength;
    [SerializeField] private Transform leftGroundPoint;
    [SerializeField] private Transform rightGroundPoint;
    [SerializeField] private LayerMask whatToDetect;
    private RaycastHit2D hitInfoLeft;
    private RaycastHit2D hitInfoRight;

    [Header("Wall")]
    public bool isWallDetected;
    public bool isWallDetectedUpper;
    public bool isWallDetectedLower;
    public bool isSmallWallDetected;
    public bool didWallJump;
    [SerializeField] private float wallRayLength;
    [SerializeField] private Transform wallCheckPointUpper;
    [SerializeField] private Transform wallCheckPointLower;
    [SerializeField] private Transform smallWallCheckPoint;
    private RaycastHit2D hitInfoWallUpper;
    private RaycastHit2D hitInfoWallLower;
    private RaycastHit2D hitInfoSmallWall;

    [Header("Ceiling")]
    public bool isCeilingDetected;
    [SerializeField] private float ceilingRayLength;
    [SerializeField] private Transform ceilingCheckPointLeft;
    [SerializeField] private Transform ceilingCheckPointRight;
    private RaycastHit2D hitInfoCeilingLeft;
    private RaycastHit2D hitInfoCeilingRight;

    [Header("Dash")]
    public bool hasDashReset;
    private float gravityDefault;

    [Header("Jump")]
    public bool isInTheAir;

    [Header("CoyoteTime")]
    public float coyoteTimer;
    [SerializeField] private float coyoteSetTime;

    [Header("Colliders")]
    [SerializeField] private Collider2D standCollider;
    [SerializeField] private Collider2D crouchCollider;

    private void Awake()
    {
        coyoteTimer = coyoteSetTime;
    }

    private void Start()
    {
        gravityDefault = rb.gravityScale;
    }

    private void Update()
    {
        if (!isGrounded)
        {
            coyoteTimer -= Time.deltaTime;
        }
        else
        {
            coyoteTimer = coyoteSetTime;
        }
    }

    private void FixedUpdate()
    {
        isGrounded = CheckGround();
        isSmallWallDetected = CheckSmallWall();
        isWallDetected = CheckWall();
        isWallDetectedUpper = CheckWallUpper();
        isCeilingDetected = CheckCeiling();
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(leftGroundPoint.position, new Vector3(0f, -groundRayLength, 0f), Color.red);
        Debug.DrawRay(rightGroundPoint.position, new Vector3(0f, -groundRayLength, 0f), Color.red);

        Debug.DrawRay(wallCheckPointUpper.position, new(wallRayLength, 0f, 0f), Color.red);
        Debug.DrawRay(wallCheckPointLower.position, new(wallRayLength, 0f, 0f), Color.red);

        Debug.DrawRay(smallWallCheckPoint.position, new(wallRayLength, 0f, 0f), Color.red);

        Debug.DrawRay(ceilingCheckPointLeft.position, new(0f, ceilingRayLength, 0f), Color.red);
        Debug.DrawRay(ceilingCheckPointRight.position, new(0f, ceilingRayLength, 0f), Color.red);

    }

    public void DisableGravity() => rb.gravityScale = 0f;
    public void EnableGravity() => rb.gravityScale = gravityDefault;
    public void ResetVelocity() => rb.linearVelocity = Vector2.zero;

    public void EnableStandCollider()
    {
        standCollider.enabled = true;
        crouchCollider.enabled = false;
    }

    public void EnableCrouchCollider()
    {
        crouchCollider.enabled = true;
        standCollider.enabled = false;
    }

    private bool CheckGround()
    {
        hitInfoLeft = Physics2D.Raycast(leftGroundPoint.position, Vector2.down, groundRayLength, whatToDetect);
        hitInfoRight = Physics2D.Raycast(rightGroundPoint.position, Vector2.down, groundRayLength, whatToDetect);

        return hitInfoLeft || hitInfoRight;
    }

    private bool CheckWall()
    {
        hitInfoWallUpper = Physics2D.Raycast(wallCheckPointUpper.position, transform.right, wallRayLength, whatToDetect);
        hitInfoWallLower = Physics2D.Raycast(wallCheckPointLower.position, transform.right, wallRayLength, whatToDetect);

        isWallDetectedUpper = hitInfoWallUpper;
        isWallDetectedLower = hitInfoWallLower;

        return hitInfoWallUpper || hitInfoWallLower;
    }

    private bool CheckWallUpper()
    {
        hitInfoWallUpper = Physics2D.Raycast(wallCheckPointUpper.position, transform.right, wallRayLength, whatToDetect);
        isWallDetectedUpper = hitInfoWallUpper;
        return hitInfoWallUpper;
    }

    private bool CheckSmallWall()
    {
        hitInfoSmallWall = Physics2D.Raycast(smallWallCheckPoint.position, transform.right, wallRayLength, whatToDetect);

        return hitInfoSmallWall;
    }

    private bool CheckCeiling()
    {
        hitInfoCeilingLeft = Physics2D.Raycast(ceilingCheckPointLeft.position, transform.up, ceilingRayLength, whatToDetect);
        hitInfoCeilingRight = Physics2D.Raycast(ceilingCheckPointRight.position, transform.up, ceilingRayLength, whatToDetect);

        return hitInfoCeilingLeft || hitInfoCeilingRight;
    }

    public float GetGravity()
    {
        return gravityDefault;
    }
}