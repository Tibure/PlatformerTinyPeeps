using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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
    [SerializeField] protected LayerMask traversableGroundLayer;
    protected const float groundCheckRadius = 0.2f;
    protected Vector2 groundNormal;
    public float minGroundNormalY = .65f;
    ///////////////
    //Velocity variable
    ///////////////
    public float maxSpeed = 7;
    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;
    [SerializeField] protected Vector2 velocity;
    public Vector2 targetVelocity;
    ///////////////
    // Wall Variables
    ///////////////
    public bool isTouchingFront;
    public bool wallSliding;
    public float wallJumpSide;
    public bool isWallJumping;
    [SerializeField] protected float wallSlidingSpeed = 0.5f;
    ///////////////
    protected AudioSource audioSource;
    [SerializeField] protected AudioClip sfx_jump, sfx_hurt, sfx_running, sfx_walk, sfx_grappling, sfx_dash, sfx_errorCrossPlateform, sfx_crossPlateform;
    [SerializeField] protected bool DashSoundHasBeenPlayed = false;
    ///////////////
    protected bool isDashing;
    [SerializeField] protected float dashTime;
    [SerializeField] protected float dashSpeed;
    [SerializeField] protected float distanceBetweenImage;
    [SerializeField] protected float dashCooldown;
    protected float dashTimeLeft;
    protected float lastImageXPosition;
    protected float lastDash = -100f;
    [SerializeField] protected Text dashText;
    protected bool isDashInCoolDown;
    protected float timerDash;
    ///////////////
    protected bool canMove = true;
    protected bool canFlip = true;
    ///////////////
    [SerializeField] protected bool isGrappling = false;
    protected bool checkClick;
    protected DistanceJoint2D myDistanceJoint2D;
    protected LineRenderer myLineRenderer;
    protected RaycastHit2D myRaycast;
    protected Vector3 myMousePos;
    protected Vector3 posTempo;
    protected Camera myCamera;
    [SerializeField] protected Text grapplingText;
    protected bool isGrapplingInCoolDown;
    protected float timerGrappling;
    [SerializeField] protected float grapplingCooldown;
    ///////////////
    [SerializeField] protected Tilemap TraversableFloorTileMap;
    protected bool isCrossingPlateform = false;
    ///////////////
    [SerializeField] protected RuntimeAnimatorController bluePlayerAnimationController;
    [SerializeField] protected RuntimeAnimatorController whitePlayerAnimationController;


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
        audioSource = GetComponent<AudioSource>();
        myCamera = Camera.main;
        myDistanceJoint2D = GetComponent<DistanceJoint2D>();
        myLineRenderer = GetComponent<LineRenderer>();
        myDistanceJoint2D.enabled = false;
        checkClick = true;
        myLineRenderer.positionCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        if (timerDash < 1)
        {
            dashText.text = "✔";
            isDashInCoolDown = false;
        }
        else
        {
            timerDash -= Time.deltaTime;
            dashText.text = Mathf.Round(timerDash).ToString();
            isDashInCoolDown = true;
        }

        if (timerGrappling < 1)
        {
            grapplingText.text = "✔";
            UpdateGrapplin();
            isGrapplingInCoolDown = false;
        }
        else
        {
            timerGrappling -= Time.deltaTime;
            grapplingText.text = Mathf.Round(timerGrappling).ToString();
            isGrapplingInCoolDown = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRunning = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
        }
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (!isDashInCoolDown)
            {
                Dash();
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            CrossPlateform();
        }
        ComputeVelocity();
    }
    private void FixedUpdate()
    {
            if(wallSliding){
                velocity = new Vector2(velocity.x, Mathf.Clamp(velocity.y - 3, -wallSlidingSpeed, float.MaxValue));
            }
            else if (!isDashing && !isGrappling)
            {
                velocity += gravityModifier * Physics2D.gravity * Time.fixedDeltaTime;
            }
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
    private void Dash()
    {
        PlayDashSound();
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;
        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXPosition = rb2d.transform.position.x;
    }

     void isNotWallJumpingAnymore(){
         isWallJumping = false;
    }
    ////////////////////
    //Fonction Virtuelle
    protected virtual void CrossPlateform() { }
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
    protected virtual void UpdateAnimator()
    { }
    protected virtual void DrawGrapplinLine()
    { }
    protected virtual void UpdateMousePosition()
    { }
    protected virtual void UpdateGrapplin()
    { }
    protected virtual void PlayDashSound() { }
}
