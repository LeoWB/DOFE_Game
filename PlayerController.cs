using System.Collections;
using System.Collections.Generic; 
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    BoxCollider2D bc; 

    public float maxFallSpeed;  

    [Header("Run")]
    [HideInInspector] public float xInput = 0f;
    public float maxRunSpeed;
    public float acceleration;
    public float deceleration;
    [Space]

    [Header("Jump")]
    public float jumpHeight;
    public float lowJumpHeight;
    public float fallSpeed;
    public float increasedFallSpeed;
    public float airControl;

    [Space]
    public LayerMask groundLayer;
    [SerializeField] bool onGround;

    [Space]
    public Vector2 groundCheckOffset;
    public Vector2 groundCheckSize;
    [Space]
    public float coyoteTime;
    float coyoteTimeCounter;
    [Space]
    public float jumpBuffer;
    float jumpBufferCounter;
    //[Space]
    //public float wallJumpTime;
    //float wallJumpTimeCounter;
    [Space]

    [Header("Wall Movement")]
    [SerializeField] bool onWall;
    [SerializeField] bool rightFootOnWall;
    [SerializeField] bool leftFootOnWall;

    [Space]
    public Vector2 wallCheckOffset;
    public Vector2 wallJumpCheckOffset;
    public Vector2 wallCheckSize;

    [Space]
    public float wallSlideMultiplier;

    void Start() {
        // Gets a reference to the components attatched to the player
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    void Update() {
        Grab();
        WallSlide();
        Jump();

        // Takes input for running and returns a value from 1 (right) to -1 (left)
        xInput = Input.GetAxis("Horizontal");
    }

    void FixedUpdate() {
        //if (wallJumpTimeCounter > 0) {return;}
        /*if (Math.Abs(rb.velocity.x) < Math.Abs(xInput) * maxRunSpeed) {
            // Increases the velocity by acceleration until the max velocity is reached
            rb.velocity += new Vector2((maxRunSpeed * xInput) / acceleration, 0) * Time.deltaTime;
        } else if ((Math.Abs(rb.velocity.x) > Math.Abs(xInput) * maxRunSpeed) && xInput == 0) {
            // Decreases the velocity by deceleration until velocity reaches 0 
            rb.velocity -= new Vector2(rb.velocity.x / (onGround ? deceleration : deceleration * 4), 0) * Time.deltaTime;
        } else {
            // Applies a velocity scaled by maxRunSpeed to the player depending on the direction of the input
            rb.velocity = new Vector2(xInput * maxRunSpeed, rb.velocity.y);
        }*/
        rb.velocity = new Vector2(xInput * maxRunSpeed, rb.velocity.y);

        if (maxFallSpeed < rb.velocity.y) {return;}
        rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
    } 

    void Jump() {
        // Checks whether the player is on the ground and if it is, replenishes coyote time, but if not, it starts to tick it down
        coyoteTimeCounter = onGround ?  coyoteTime :  coyoteTimeCounter - Time.deltaTime;
        // When player jumps, jump buffer counter is reset, if not, ticks it down slowly
        jumpBufferCounter = Input.GetButtonDown("Jump") ? jumpBuffer : jumpBufferCounter - Time.deltaTime;
        //wallJumpTimeCounter = rightFootOnWall || leftFootOnWall ? wallJumpTime : wallJumpTimeCounter - Time.deltaTime;
        // Draws a box to check whether the player is standing on objects on the ground layer
        onGround = Physics2D.OverlapBox((Vector2)transform.position + groundCheckOffset, groundCheckSize, 0f, groundLayer);
        // Draws a box to check whether the player is touching a wall 
        rightFootOnWall = (Physics2D.OverlapBox((Vector2)transform.position + wallJumpCheckOffset, wallCheckSize, 0f, groundLayer));
        leftFootOnWall = (Physics2D.OverlapBox((Vector2)transform.position + new Vector2(-wallJumpCheckOffset.x, wallJumpCheckOffset.y), wallCheckSize, 0f, groundLayer));
        
        // Increases gravity of player when falling down or when the jump button is let go mid-jump
        if (rb.velocity.y < 0 ) {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallSpeed - 1) * Time.deltaTime;
        } else if ((rb.velocity.y > 0) && (!Input.GetButton("Jump"))) {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpHeight - 1) * Time.deltaTime;
        }

        // Checks whether the player has recently left a platform (coyote time) or
        // ... has pressed the jump button just before they land (jump buffer)
        // If so, sets the y velocity to jumpHeight (jumps)
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0) {
            rb.velocity = new Vector2 (rb.velocity.x, jumpHeight);
            jumpBufferCounter = 0;
        } else if ((rightFootOnWall || leftFootOnWall) && Input.GetButtonDown("Jump")) {
            rb.velocity = new Vector2 (maxRunSpeed * (rightFootOnWall ? -1 : 1), jumpHeight);
        }    
        Debug.Log(rb.velocity);
    }

    void Grab() {
        // Draws a box to check whether the player is touching objects on the ground layer (grabbable layer)
        onWall = (Physics2D.OverlapBox((Vector2)transform.position + wallCheckOffset, wallCheckSize, 0f, groundLayer) || (Physics2D.OverlapBox((Vector2)transform.position + new Vector2(-wallCheckOffset.x, wallCheckOffset.y), wallCheckSize, 0f, groundLayer)));
        if (Input.GetKey(KeyCode.LeftShift) && onWall) {
            rb.velocity = new Vector2 (rb.velocity.x, 0);
        }
    }

    void WallSlide() {
        if (!(rightFootOnWall || leftFootOnWall)) {return;}
        if (Math.Sign(rb.velocity.y) == 1) {return;}
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * wallSlideMultiplier);
    }

    // Draws red debug boxes to help show the boxes detecting walls and ground
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + groundCheckOffset, groundCheckSize);
        Gizmos.DrawWireCube((Vector2)transform.position + wallCheckOffset, wallCheckSize);
        Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(-wallCheckOffset.x, wallCheckOffset.y), wallCheckSize);
        Gizmos.DrawWireCube((Vector2)transform.position + wallJumpCheckOffset, wallCheckSize);
        Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(-wallJumpCheckOffset.x, wallJumpCheckOffset.y), wallCheckSize);
    }
}