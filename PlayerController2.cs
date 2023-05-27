using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    [SerializeField]private Transform groundCheckCollider;
    [SerializeField]private Transform overheadCheckCollider;
    [SerializeField]LayerMask groundLayer;
    [SerializeField]Collider2D standingCollider1;
   

    
    private float horizontalValue = 0f;
    private float slideSpeedModifier = 2f;
    private float slideCounter;
    private float slideCoolcounter;
    [SerializeField]private float groundCheckRaius = 0.2f;
    [SerializeField]private float overheadCheckRaius = 0.2f;
    private float crouchSpeedModifier = 0.5f;
    [SerializeField] private float yClampMin = 0f;
    [SerializeField] private float yClampMax = 0f;


    private bool facingRight = true;
    private bool isSliding = false;
    private bool isGrounded = false;
    private bool isJumping = false;
    private bool isCrouching = false;

    [SerializeField]private float movementSpeed = 10.0f;
    [SerializeField]private float slideLength = .5f;
    [SerializeField]private float slideCooldown = 1f;
    [SerializeField]private float jumpPower = 300f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("yVelocity", rb.velocity.y);

        horizontalValue = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(slideCoolcounter <=0 && slideCounter <= 0)
            {
                isSliding = true;
                slideCounter = slideLength;
            }
        }

        if(slideCounter > 0)
        {
            slideCounter -= Time.deltaTime;

            if(slideCounter <= 0)
            {
                isSliding = false;
                slideCoolcounter = slideCooldown;
            }
        }
       

        if (slideCoolcounter > 0)
        {
            slideCoolcounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            
            isJumping = true;
            
        }
        else if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }


        if (Input.GetKeyDown(KeyCode.C))
        { 
            isCrouching = true;
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            isCrouching = false;
        }



    }

    private void FixedUpdate()
    {
        GroundCheck();
        Move(horizontalValue, isJumping, isCrouching);

        if (isGrounded && isSliding)
        {
            standingCollider1.enabled = !isSliding;

        }

        if (!isSliding && !isCrouching)
        {
            if (Physics2D.OverlapCircle(overheadCheckCollider.position, overheadCheckRaius, groundLayer))
            {
                isCrouching = true;
            }


        }
         
        if (!isGrounded)
        {
            isSliding = false;
            isCrouching = false;
        }

       


    }


    private void GroundCheck()
    {
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRaius, groundLayer);
        if(colliders.Length > 0)
        {
            isGrounded = true;
        }

        anim.SetBool("isJumping", !isGrounded);
    }

    private void Move(float dir, bool jumpFlag, bool crouchFlag)
    {
        #region Jump,Crouch 
       
       
        if (isGrounded)
        {
            standingCollider1.enabled = !crouchFlag;

            if (crouchFlag)
            {
                jumpFlag = false;
                
            }
           

            if (jumpFlag)
            {
               isGrounded = false;
               rb.AddForce(new Vector2(0f, jumpPower));
            }
            
        }

        anim.SetBool("isCrouching", crouchFlag);
        
        #endregion

        #region Move and slide
        float xVal = movementSpeed * 100 * dir * Time.fixedDeltaTime ;
        if (isSliding)
        {
            xVal *= slideSpeedModifier;
        }
        if (crouchFlag)
        {
            xVal *= crouchSpeedModifier;
        }

        rb.velocity = new Vector2(xVal, Mathf.Clamp(rb.velocity.y, yClampMin, yClampMax ));

        if (facingRight && dir < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            facingRight = false;
        }
        else if (!facingRight && dir > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            facingRight = true;
        }
        
        anim.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
        #endregion
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheckCollider.position, groundCheckRaius);
        Gizmos.DrawWireSphere(overheadCheckCollider.position, overheadCheckRaius);
    }
}
