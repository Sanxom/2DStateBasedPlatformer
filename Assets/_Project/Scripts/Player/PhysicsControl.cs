using System.Collections.Generic;
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

    private bool CheckGround()
    {
        hitInfoLeft = Physics2D.Raycast(leftGroundPoint.position, Vector2.down, groundRayLength, whatToDetect);
        hitInfoRight = Physics2D.Raycast(rightGroundPoint.position, Vector2.down, groundRayLength, whatToDetect);

        Debug.DrawRay(leftGroundPoint.position, new Vector3(0f, -groundRayLength, 0f), Color.red);
        Debug.DrawRay(rightGroundPoint.position, new Vector3(0f, -groundRayLength, 0f), Color.red);

        if (hitInfoLeft || hitInfoRight)
            return true;

        return false;
    }

    private void FixedUpdate()
    {
        isGrounded = CheckGround();
    }
}