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
			print(myRaycast.collider.gameObject.tag);
			print(myRaycast.collider.gameObject.layer);
		}

		if (Input.GetMouseButtonDown(0) && checkClick && myRaycast.collider.gameObject.tag == "ground")
		{
			isGrappling = true;
			gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
			myDistanceJoint2D.enabled = true;
			myDistanceJoint2D.connectedAnchor = myRaycast.point;
			myLineRenderer.positionCount = 2;
			posTempo = myRaycast.point;
			checkClick = false;
		}
		else if (Input.GetMouseButtonDown(0))
		{
			gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
			gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
			myDistanceJoint2D.enabled = false;
			myLineRenderer.positionCount = 0;
			checkClick = true;
			isGrappling = false;
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
		myLineRenderer.SetPosition(1, posTempo);
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
		/*
		 isTouchingWall = false;
		 Collider2d[] collider = Physics2D.OverlapCircleAll(wallCheckCollider.position, wallCheckRadius, groundLayer);
		 if(colliders.length > 0)
		 {
			 isTouchingWall = true;
		 }
		*/
	}

	protected override void GroundCheck()
	{
		isGrounded = false;
		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, groundLayer);
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

