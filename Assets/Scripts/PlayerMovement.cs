using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct JumpSettings {
    public float maxHorizontalDistance;
    public float minHeight, maxHeight;
    public float gravityFastFalling;

    public JumpSettings(float maxHorizontalDistance, float minHeight, float maxHeight, float gravityFastFalling) {
        this.maxHorizontalDistance = maxHorizontalDistance;
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
        this.gravityFastFalling = gravityFastFalling;
    }
}

[Serializable]
public struct GroundCheck {
    public Vector2 offset;
    public float radius;

    public GroundCheck(Vector2 offset, float radius) {
        this.offset = offset;
        this.radius = radius;
    }
}

public class PlayerMovement : MonoBehaviour {
    public float horizontalSpeed = 20.0f;

    public LayerMask whatIsGround;
    public JumpSettings jump = new JumpSettings(10.0f, 1.0f, 4.0f, 1.5f);
    public GroundCheck groundCheck = new GroundCheck(new Vector2(0.0f, -0.5f), 0.1f);

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

        // We need to take the gravity setting into account,
        // since we will change rbody.gravityScale
        physicsGravity = Physics2D.gravity.magnitude;
    }

    private void FixedUpdate() {
        // Thanks to https://youtu.be/hG9SzQxaCm8
        // Compute gravities dynamically (should be in Start() for release)
        jumpMaxHeightTime = jump.maxHorizontalDistance / 2.0f / horizontalSpeed;
        jumpSpeed = 2.0f * jump.maxHeight / jumpMaxHeightTime;
        gravityMaxHeight = jumpSpeed / jumpMaxHeightTime / physicsGravity;
        gravityMinHeight = jumpSpeed * jumpSpeed / (2.0f * jump.minHeight) / physicsGravity;

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
            rbody.gravityScale = jump.gravityFastFalling * gravityMaxHeight;
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
