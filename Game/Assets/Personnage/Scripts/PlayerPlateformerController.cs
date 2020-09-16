using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerPlateformerController : PhysicsObject
{
	public float maxSpeed = 7;
	public float jumpTakeOffSpeed = 7;


	// Start is called before the first frame update
	void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
	}

	protected override void ComputeVelocity()
	{
		Vector2 move = Vector2.zero;
		move.x = Input.GetAxis("Horizontal");
		if (Input.GetButtonDown("Jump") && grounded)
		{
			velocity.y = jumpTakeOffSpeed;
			isJumping = true;
		}
		else if (Input.GetButtonUp("Jump"))
		{
			isJumping = false;
			if (velocity.y > 0)
			{
				velocity.y = velocity.y * .5f;
			}
		}
		bool flipSprite = (spriteRenderer.flipX ? (move.x > 0.01f) : (move.x < 0.01f));
		if (flipSprite)
		{
			spriteRenderer.flipX = !spriteRenderer.flipX;
		}
		//animator.SetBool("grounded", grounded);
		//animator.SetFloat("xVelocity", Mathf.Abs(velocity.x) / maxSpeed);
		targetVelocity = move * maxSpeed;
        if (isRunning)
        {
			targetVelocity *= runSpeedModifier;
		}

		animator.SetFloat("xVelocity", Mathf.Abs(targetVelocity.x));
	}
}

