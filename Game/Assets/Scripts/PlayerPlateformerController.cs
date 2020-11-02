using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class PlayerPlateformerController : PhysicsObject
{
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
			targetVelocity = move * maxSpeed;
		}
		if (isRunning && canMove)
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
		}
		else if (Input.GetButtonUp("Jump"))
		{
			if (velocity.y > 0)
			{
				velocity.y = velocity.y * .5f;
			}
		} 
		if (move.x != 0 && !isRunning && !isDashing && isGrounded)
		{
			PlayWalkingSound();
		}
		else if (move.x == 0 && velocity.y == 0)
		{
			audioSource.Stop();
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
		if (myRaycast.collider != null)
		{
			/* print(myRaycast.collider.gameObject.tag);
			print(myRaycast.collider.gameObject.layer); */
		}

		if (Input.GetMouseButtonDown(0) && checkClick && myRaycast.collider.gameObject.tag == "ground")
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
				isDashing = false;
				canMove = true;
				canFlip = true;
			}
		}
	}
	protected override void WallCheck()
	{
		float flipValue = spriteRenderer.flipX ? -0.4f : 0.4f ;

		Vector2 frontCheckLocation = new Vector2(rb2d.position.x +flipValue , rb2d.position.y);
		 isTouchingFront = Physics2D.OverlapCircle(frontCheckLocation, groundCheckRadius, groundLayer);
		 if( isTouchingFront == true && isGrounded == false && Input.GetAxisRaw("Horizontal") != 0f){
			 wallSliding = true;
			 print(wallSliding);
		 }else{
			 wallSliding = false;
		 }

		 if(wallSliding){
			 /* rb2d.velocity = new Vector2(rb2d.velocity.x, Mathf.Clamp(rb2d.velocity.y, -wallSlidingSpeed, float.MaxValue)); */
			 //apparament ne fonctionne pas...
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
		}
		isJumping = !isGrounded;
		animator.SetBool("isJumping", isJumping);
	}
	private void PlayJumpSound()
	{
		audioSource.loop = false;
		audioSource.clip = sfx_jump;
		audioSource.Play();
	}
	protected override void PlayDashSound()
	{
		print("coucou");
		audioSource.loop = false;
		audioSource.clip = sfx_dash;
		audioSource.Play();
	}
	private void PlayGrapplingSound()
	{
		audioSource.loop = false;
		audioSource.clip = sfx_grappling;
		audioSource.Play();
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

