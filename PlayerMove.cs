using System.Collections;
using System.Collections.Generic; 
using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rb;
    BoxCollider2D bc;   

    [Header("Run")]
    float xInput = 0f;
    public float maxRunSpeed;
    public float acceleration;
    public float deceleration;
    [Space]

    [Header("Jump")]
    public float jumpHeight;
    public float lowJumpHeight;
    public float fallSpeed;
    public float airControl;

    [Space]
    public LayerMask groundLayer;
    [SerializeField] bool onGround;

    [Space]
    public Vector2 groundCheckOffset;
    public Vector2 groundCheckSize;
    public float coyoteTime;

    [Header("Grab")]
    [SerializeField] bool onWall;

    [Space]
    public Vector2 wallCheckOffset;
    public Vector2 wallCheckSize;

    void Start() {
        // Gets a reference to the components attatched to the player
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    void Update() {
        Jump();
        Grab();

        // Takes input for running and returns a value from 1 (right) to -1 (left)
        xInput =  Math.Sign(Input.GetAxisRaw("Horizontal"));
    }

    void FixedUpdate() {
        if(Math.Abs(rb.velocity.x) < Math.Abs(xInput) * maxRunSpeed) {
            //Debug.Log("accelerate");
            // Increases the velocity by acceleration until the max velocity is reached
            rb.velocity += new Vector2(acceleration * xInput, rb.velocity.y) * Time.deltaTime;
        } /*else if (Math.Abs(rb.velocity.x) > Math.Abs(xInput) * maxRunSpeed) {
             Debug.Log("decelerate");
            // Decreases the velocity by deceleration until velocity reaches 0
            rb.velocity -= new Vector2(deceleration * (rb.velocity.x / Math.Abs(rb.velocity.x)), rb.velocity.x) * Time.deltaTime;
        } */ else {
             //Debug.Log("constant");
            // Applies a velocity scaled by maxRunSpeed to the player depending on the direction of the input
            rb.velocity = new Vector2(xInput * maxRunSpeed, rb.velocity.y);
        }
    }

    void Jump() {
        // Checks whether the player is on the ground and if it is, replenishes coyote time, but if not, it starts to tick it down
        coyoteTime = onGround ?  0.1f :  coyoteTime - Time.deltaTime;
        // Draws a box to check whether the player is touching objects on the ground layer
        onGround = Physics2D.OverlapBox((Vector2)transform.position + groundCheckOffset, groundCheckSize, 0f, groundLayer);

        // Adds an upwards velocity to player when there is still valid coyote time and the jump button is pressed
        if (Input.GetButtonDown("Jump") && coyoteTime > 0) {
            rb.velocity = new Vector2 (rb.velocity.x, jumpHeight);
        }
        
        // Increases gravity of player when falling down or when the jump button is let go mid-jump
        if (rb.velocity.y < 0 ) {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallSpeed - 1) * Time.deltaTime;
        } else if (rb.velocity.y > 0 && !Input.GetButton("Jump")) {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpHeight - 1) * Time.deltaTime;
        }
    }

    void Grab() {
        onWall = (Physics2D.OverlapBox((Vector2)transform.position + wallCheckOffset, wallCheckSize, 0f, groundLayer) || (Physics2D.OverlapBox((Vector2)transform.position + new Vector2(-wallCheckOffset.x, wallCheckOffset.y), wallCheckSize, 0f, groundLayer)));
        if (Input.GetKey(KeyCode.LeftShift) && onWall) {
            rb.velocity = new Vector2 (rb.velocity.x, 0);
        }
    }

    // Draws a red debug box to match the one drawn by the ground check
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + groundCheckOffset, groundCheckSize);
        Gizmos.DrawWireCube((Vector2)transform.position + wallCheckOffset, wallCheckSize);
        Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(-wallCheckOffset.x, wallCheckOffset.y), wallCheckSize);
    }
}