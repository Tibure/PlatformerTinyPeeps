using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    //Objet Unity
    ///////////////
    protected Rigidbody2D rb2d;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    ///////////////
    //Run variable
    ///////////////
    protected float runSpeedModifier = 1.5f;
    [SerializeField] protected bool isRunning = false;
    ///////////////
    //Jump variable
    ///////////////
    public float jumpTakeOffSpeed = 7;
    public float gravityModifier = 1f;
    [SerializeField] protected bool isJumping = false;
    ///////////////
    //Grounded Variable
    ///////////////
    [SerializeField] protected bool isGrounded = true;
    [SerializeField] protected LayerMask groundLayer;
    protected const float groundCheckRadius = 0.2f;
    [SerializeField] protected Transform groundCheckCollider;
    protected Vector2 groundNormal;
    public float minGroundNormalY = .65f;
    ///////////////
    //Velocity variable
    ///////////////
    public float maxSpeed = 7;
    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;
    protected Vector2 velocity;
    protected Vector2 targetVelocity;
    ///////////////
    // Wall Variables
    ///////////////
    protected bool isTouchingWall;
    protected float wallJumpVelocityX = 1f;
    protected float wallJumpVelocityY = 1f;
    protected bool isWallSliding;
    protected float wallSlidingSpeed = -0.5f;
    ///////////////
    private void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        targetVelocity = Vector2.zero;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRunning = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
        }
        GroundCheck();
        WallCheck();
        ComputeVelocity();
    }
    private void FixedUpdate()
    {
        velocity += gravityModifier * Physics2D.gravity * Time.fixedDeltaTime;
        velocity.x = targetVelocity.x;
        isGrounded = false;
        Vector2 deltaPosition = velocity * Time.deltaTime;
        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);
        Vector2 move = moveAlongGround * deltaPosition.x;
        Movement(move, false);
        move = Vector2.up * deltaPosition.y;
        Movement(move, true);
    }
    void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if (distance > minMoveDistance)
        {
            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();
            for (int i = 0; i < count; i++)
            {
                hitBufferList.Add(hitBuffer[i]);
            }
            for (int i = 0; i < hitBufferList.Count; i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal;
                if (currentNormal.y > minGroundNormalY)
                {
                    isGrounded = true;
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }
                float projection = Vector2.Dot(velocity, currentNormal);
                if (projection < 0)
                {
                    velocity = velocity - projection * currentNormal;
                }

                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }
        rb2d.position = rb2d.position + move.normalized * distance;
    }
    ////////////////////
    //Fonction Virtuelle
    protected virtual void ComputeVelocity()
    {

    }
    protected virtual void GroundCheck()
    {

    }
    public virtual void HurtTrigger()
    {

    }

    protected virtual void WallCheck()
    {

    }
}
