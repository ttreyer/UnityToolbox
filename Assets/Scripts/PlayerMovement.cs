using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float horizontalSpeed = 20.0f;

    public float jumpPeakHorizontalDistance = 10.0f;
    public float jumpMinHeight = 1.0f;
    public float jumpMaxHeight = 4.0f;
    public float jumpGravityFastFalling = 1.5f;

    public LayerMask whatIsGround;

    // Add a CircleCollider2D to a child object of the Player
    // and use the offset and radius property to visually set
    // the ground check properties.
    public CircleCollider2D groundCheck;

    private Rigidbody2D rbody;
    private float physicsGravity;

    private float horizontalMovement;

    private float jumpSpeed;
    private float jumpMaxHeightTime;
    private float gravityMinHeight;
    private float gravityMaxHeight;

    private bool isGrounded;

    void Start() {
        rbody = GetComponent<Rigidbody2D>();

        // GroundCheck is only used for its values,
        // we can disable it once we're done configuring it in the editor.
        groundCheck.enabled = false;

        // We need to take the gravity setting into account,
        // since we will change rbody.gravityScale
        physicsGravity = Physics2D.gravity.magnitude;
    }

    private void FixedUpdate() {
        // Thanks to https://youtu.be/hG9SzQxaCm8
        // Compute gravities dynamically (should be in Start() for release)
        jumpMaxHeightTime = jumpPeakHorizontalDistance / horizontalSpeed;
        jumpSpeed = 2.0f * jumpMaxHeight / jumpMaxHeightTime;
        gravityMaxHeight = jumpSpeed / jumpMaxHeightTime / physicsGravity;
        gravityMinHeight = jumpSpeed * jumpSpeed / (2.0f * jumpMinHeight) / physicsGravity;

        // Ground Check
        isGrounded = Physics2D.OverlapCircle(
            transform.position + (Vector3)groundCheck.offset,
            groundCheck.radius,
            whatIsGround);

        // Set horizontal velocity
        rbody.velocity = new Vector2(horizontalSpeed * horizontalMovement, rbody.velocity.y);

        // Check direction
        bool facingRight = transform.localScale.x > 0.0f;
        if (facingRight && horizontalMovement < 0)
            Flip();
        else if (!facingRight && horizontalMovement > 0)
            Flip();

        // Activate falling gravity
        if (rbody.velocity.y < 0)
            rbody.gravityScale = jumpGravityFastFalling * gravityMaxHeight;
    }

    void Flip() {
        Vector3 flipX = transform.localScale;
        flipX.x *= -1.0f;
        transform.localScale = flipX;
    }

    void Update() {
        horizontalMovement = Input.GetAxis("Horizontal");

        if (isGrounded && Input.GetButtonDown("Jump")) {
            // Set a light gravity while Jump is down
            rbody.gravityScale = gravityMaxHeight;
            rbody.velocity = jumpSpeed * Vector2.up;
        } else if (Input.GetButtonUp("Jump")) {
            // Set a heavier gravity when Jump is released
            rbody.gravityScale = gravityMinHeight;
        }
    }
}
