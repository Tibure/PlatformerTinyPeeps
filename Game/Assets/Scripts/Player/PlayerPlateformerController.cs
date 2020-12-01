using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerPlateformerController : MonoBehaviour
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
	[SerializeField]
	protected AudioClip sfx_jump, sfx_hurt, sfx_running, sfx_walk, sfx_grappling, sfx_dash, sfx_death,
										 sfx_errorCrossPlateform, sfx_crossPlateform, sfx_change_player, sfx_easter_egg;
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
	public bool canMove = true;
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
		if (Input.GetAxis("Vertical") < 0)
		{
			CrossPlateform();
		}
		ComputeVelocity();
	}
	private void FixedUpdate()
	{
		if (wallSliding)
		{
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
	void isNotWallJumpingAnymore()
	{
		isWallJumping = false;
	}
	void Awake()
	{
		TransitionCamera();
		GameMaster gameMaster = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
		if (gameMaster.getPlayerColor() == Color.white)
		{
			gameObject.GetComponent<Animator>().runtimeAnimatorController = whitePlayerAnimationController;
		}
		else if (gameMaster.getPlayerColor() == Color.blue)
		{
			gameObject.GetComponent<Animator>().runtimeAnimatorController = bluePlayerAnimationController;
		}
		gameObject.transform.position = gameMaster.lastCheckPointPos;
	}
	public void TransitionCamera()
	{
		FindObjectOfType<CameraEffect>().StartCoroutineUnPixelisation();

	}
	protected void ComputeVelocity()
	{
		Vector2 move = Vector2.zero;
		move.x = Input.GetAxis("Horizontal");
		bool flipSprite = (spriteRenderer.flipX ? (move.x > 0.01f) : (move.x < -0.01f));
		DashCheck();
		WallCheck();
		if (flipSprite && canFlip)
		{
			spriteRenderer.flipX = !spriteRenderer.flipX;
		}
        if (canMove)
        {
			if(!isWallJumping)
			targetVelocity = move * maxSpeed;
			else{
			targetVelocity.x = Mathf.Clamp(targetVelocity.x + ((move.x * maxSpeed*2)*Time.deltaTime), -7, 7f);
			}

		}
		if (isRunning && canMove && isGrounded)
		{
			targetVelocity *= runSpeedModifier;
			PlayRunningSound();
		}
        else if (Input.GetKeyUp(KeyCode.LeftShift))
		{
            audioSource.Stop();
        }
		if (Input.GetButtonDown("Jump") && isGrounded && canMove)
		{
			velocity.y = jumpTakeOffSpeed;
			isJumping = true;
			PlayJumpSound();
		}else if(Input.GetButtonDown("Jump") && wallSliding){
			wallJumpSide = Input.GetAxisRaw("Horizontal");
			velocity.y = jumpTakeOffSpeed;
			targetVelocity.x = (-wallJumpSide*jumpTakeOffSpeed);
			wallSliding = false;
			isWallJumping = true;
			Invoke("isNotWallJumpingAnymore", (0.4f));//this will happen after 2 seconds
		}
		else if (Input.GetButtonUp("Jump"))
		{
			if (velocity.y > 0)
			{
				velocity.y = velocity.y * .5f;
			}
		} 
		if (velocity.x != 0 && !isRunning && !isDashing && isGrounded)
		{
			PlayWalkingSound();
		}
		else if (velocity.x == 0 || wallSliding)
		{
			audioSource.clip = null;
		}
		UpdateAnimator();
	}
	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Vector3 test = new Vector3(myRaycast.point.x, myRaycast.point.y, 35);
		Gizmos.DrawSphere(test, 1);
	}
	protected void UpdateGrapplin()
	{
		UpdateMousePosition();

		int layerMask = 1 << LayerMask.NameToLayer("Ground");
		Debug.DrawLine(gameObject.transform.position, myMousePos);
		Vector3 distanceRaycast = myMousePos - gameObject.transform.position;
		distanceRaycast.z = 0;
		myRaycast = Physics2D.Raycast(gameObject.transform.position, distanceRaycast, distanceRaycast.magnitude, layerMask);
		if (myRaycast.collider == null)
		{
			layerMask = 1 << LayerMask.NameToLayer("TraversableGround"); //Traversable Ground
			myRaycast = Physics2D.Raycast(gameObject.transform.position, distanceRaycast, distanceRaycast.magnitude, layerMask);
		}

		if (Input.GetMouseButtonDown(0) && checkClick && myRaycast.collider.gameObject.tag == "ground" && !isGrapplingInCoolDown)
		{
			PlayGrapplingSound();
			isGrappling = true;
			gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
			myDistanceJoint2D.enabled = true;
			myDistanceJoint2D.connectedAnchor = myRaycast.point;
			myLineRenderer.positionCount = 2;
			posTempo = myRaycast.point;
			checkClick = false;
			rb2d.velocity = Vector2.zero;
		}
		else if (Input.GetMouseButtonDown(0))
		{
			gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
			gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
			myDistanceJoint2D.enabled = false;
			myLineRenderer.positionCount = 0;
			checkClick = true;
			isGrappling = false;
			timerGrappling = grapplingCooldown;
			rb2d.velocity = Vector2.zero;
		}
		DrawGrapplinLine();
	}
	protected void DrawGrapplinLine()
	{
		if (myLineRenderer.positionCount <= 0)
		{
			return;
		}
		myLineRenderer.SetPosition(0, transform.position);
		myLineRenderer.SetPosition(1, myDistanceJoint2D.connectedAnchor);
	}
	protected void UpdateMousePosition()
	{
		myMousePos = myCamera.ScreenToWorldPoint(Input.mousePosition);
	}
	private void DashCheck()
	{
		if (isDashing)
		{
			
			if (dashTimeLeft > 0)
			{
				canMove = false;
				isJumping = false;
				canFlip = false;
				if (spriteRenderer.flipX)
				{
					targetVelocity = new Vector2(-dashSpeed, 0);
				}
				else
				{
					targetVelocity = new Vector2(dashSpeed, 0);
				}
				
				dashTimeLeft -= Time.deltaTime;

				if (Mathf.Abs(rb2d.transform.position.x - lastImageXPosition) > distanceBetweenImage)
				{
					PlayerAfterImagePool.Instance.GetFromPool();
					lastImageXPosition = rb2d.transform.position.x;
				}
			}
			if (dashTimeLeft <= 0 || isTouchingFront)
			{
				DashSoundHasBeenPlayed = false;
				isDashing = false;
				canMove = true;
				canFlip = true;
				timerDash = dashCooldown;
			}
		}
	}
	protected void CrossPlateform()
	{
		Vector2 GroundCheckLocation = new Vector2(rb2d.position.x, (rb2d.position.y - 0.7f));
		Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheckLocation, groundCheckRadius, traversableGroundLayer);
		if (colliders.Length > 0)
		{
			PlayCrossPlateformSound();
			isCrossingPlateform = true;
			TraversableFloorTileMap.GetComponent<TilemapCollider2D>().enabled = false;
			Invoke("SetColliderTraversableFloor", 0.65f);
		}
		else 
		{
			PlayCrossPlateformErrorSound();
		}
	}
	private void SetColliderTraversableFloor()
	{
		TraversableFloorTileMap.GetComponent<TilemapCollider2D>().enabled = true;
		isCrossingPlateform = false;
	}
	protected void WallCheck()
	{	
		float inputSide = Input.GetAxisRaw("Horizontal");
		float flipValue = inputSide == -1 ? -0.4f : 0.4f;

		Vector2 frontCheckLocation = new Vector2(rb2d.position.x + flipValue , rb2d.position.y);

		 isTouchingFront = Physics2D.OverlapCircle(frontCheckLocation, groundCheckRadius, groundLayer);
		 if( isTouchingFront == true && isGrounded == false && inputSide != 0f && inputSide != wallJumpSide && !isCrossingPlateform){
			 wallSliding = true;
			 isJumping = false;
		 }else{
			 wallSliding = false;
		 }
	}
	protected void GroundCheck()
	{
		isGrounded = false;
		Vector2 GroundCheckLocation = new Vector2(rb2d.position.x, (rb2d.position.y - 0.7f));
		Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheckLocation, groundCheckRadius, groundLayer);
		if (colliders.Length > 0)
		{
			isGrounded = true;
			wallJumpSide = 0f;
		}
		else
		{
			colliders = Physics2D.OverlapCircleAll(GroundCheckLocation, groundCheckRadius, traversableGroundLayer);
			if (colliders.Length > 0)
			{
				isGrounded = true;
			}
		}
		isJumping = !isGrounded;
		animator.SetBool("isJumping", isJumping);
	}
	private void PlayCrossPlateformSound()
	{
		audioSource.loop = false;
		audioSource.PlayOneShot(sfx_crossPlateform);
	}
	private void PlayCrossPlateformErrorSound()
	{
		audioSource.loop = false;
		audioSource.PlayOneShot(sfx_errorCrossPlateform);
	}
	private void PlayJumpSound()
	{
		audioSource.loop = false;
		audioSource.PlayOneShot(sfx_jump);
	}
	protected void PlayDashSound()
	{
		audioSource.loop = false;
		audioSource.PlayOneShot(sfx_dash);
	}
	private void PlayGrapplingSound()
	{
		audioSource.loop = false;
		audioSource.PlayOneShot(sfx_grappling);
		audioSource.clip = null;
	}
	private void PlayRunningSound()
	{
		if (!audioSource.isPlaying)
		{
			audioSource.loop = true;
			audioSource.clip = sfx_running;	
			audioSource.Play();
		}
	}
	private void PlayWalkingSound()
	{
		if (!audioSource.isPlaying)
		{
			audioSource.loop = true;
			audioSource.clip = sfx_walk;
			audioSource.Play();
		}
	}
	public void PlayChangePlayerSound()
	{
		audioSource.loop = false;
		audioSource.PlayOneShot(sfx_change_player);
		audioSource.clip = null;
	}
	public void PlayDiscoveringEasterEggSound()
	{
		audioSource.loop = false;
		audioSource.PlayOneShot(sfx_easter_egg);
		audioSource.clip = null;
	}
	public void PlayDeathSound()
	{
		audioSource.loop = false;
		audioSource.PlayOneShot(sfx_death);
		audioSource.clip = null;
	}
	protected void UpdateAnimator()
	{
		animator.SetBool("isSliding", wallSliding);
		animator.SetBool("isJumping", isJumping);
		animator.SetBool("isGrappling", isGrappling);
		animator.SetBool("isDashing", isDashing);
		animator.SetFloat("yVelocity", velocity.y);
		animator.SetFloat("xVelocity", Mathf.Abs(targetVelocity.x));
		
	}
	public void HurtTrigger()
	{
		animator.SetTrigger("HurtTrigger");
		AudioManager.instance.PlaySFX("hurt");
	}
}

