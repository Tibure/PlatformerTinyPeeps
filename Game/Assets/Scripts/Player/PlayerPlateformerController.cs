using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class PlayerPlateformerController : PhysicsObject
{
	void Awake()
	{
		GameMaster gameMaster = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
		gameObject.transform.position = gameMaster.lastCheckPointPos;
	}
	protected override void ComputeVelocity()
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
	protected override void UpdateGrapplin()
	{
		UpdateMousePosition();

		int layerMask = 1 << 8;
		Debug.DrawLine(gameObject.transform.position, myMousePos);
		Vector3 distanceRaycast = myMousePos - gameObject.transform.position;
		distanceRaycast.z = 0;
		myRaycast = Physics2D.Raycast(gameObject.transform.position, distanceRaycast, distanceRaycast.magnitude, layerMask);
		if (myRaycast.collider == null)
		{
			layerMask = 1 << 13;
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
	protected override void DrawGrapplinLine()
	{
		if (myLineRenderer.positionCount <= 0)
		{
			return;
		}
		myLineRenderer.SetPosition(0, transform.position);
		myLineRenderer.SetPosition(1, myDistanceJoint2D.connectedAnchor);
	}
	protected override void UpdateMousePosition()
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
			if (dashTimeLeft <= 0 /*|| isTouchingWall*/)
			{
				DashSoundHasBeenPlayed = false;
				isDashing = false;
				canMove = true;
				canFlip = true;
				timerDash = dashCooldown;
			}
		}
	}
	protected override void CrossPlateform()
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
	protected override void WallCheck()
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
	protected override void GroundCheck()
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
	protected override void PlayDashSound()
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
	protected override void UpdateAnimator()
	{
		animator.SetBool("isSliding", wallSliding);
		animator.SetBool("isJumping", isJumping);
		animator.SetBool("isGrappling", isGrappling);
		animator.SetBool("isDashing", isDashing);
		animator.SetFloat("yVelocity", velocity.y);
		animator.SetFloat("xVelocity", Mathf.Abs(targetVelocity.x));
		
	}
	public override void HurtTrigger()
	{
		animator.SetTrigger("HurtTrigger");
		AudioManager.instance.PlaySFX("hurt");
	}
}

