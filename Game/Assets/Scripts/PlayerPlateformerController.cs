using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class PlayerPlateformerController : PhysicsObject
{
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
	public override void HurtTrigger()
	{
		animator.SetTrigger("HurtTrigger");
		AudioManager.instance.PlaySFX("hurt");
	}
	protected override void ComputeVelocity()
	{
		Vector2 move = Vector2.zero;
		move.x = Input.GetAxis("Horizontal");
		bool flipSprite = (spriteRenderer.flipX ? (move.x > 0.01f) : (move.x < -0.01f));
		if (flipSprite)
		{
			spriteRenderer.flipX = !spriteRenderer.flipX;
		}

		targetVelocity = move * maxSpeed;

		if (isRunning)
		{
			targetVelocity *= runSpeedModifier;
			PlayRunningSound();
		}
        else if (Input.GetKeyUp(KeyCode.LeftShift))
		{
            audioSource.Stop();
        }
		if (Input.GetButtonDown("Jump") && isGrounded)
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
		if (move.x != 0 && isRunning == false && isGrounded == true)
		{
			PlayWalkingSound();
		}
		else if (move.x == 0 && velocity.y == 0)
		{
			audioSource.Stop();
		}
		UpdateAnimator();
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
		animator.SetFloat("yVelocity", velocity.y);
		animator.SetFloat("xVelocity", Mathf.Abs(targetVelocity.x));
	}
}

