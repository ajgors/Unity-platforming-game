using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _193257_193435_193271
{
	public class PlayerController : MonoBehaviour
	{
		[Header("Movement parameters")]
		[Range(0.01f, 20.0f)][SerializeField] private float moveSpeed = 0.1f;

		[Space(10)]
		[Range(0.01f, 20.0f)][SerializeField] private float jumpForce = 3f;

		[Space(10)]
		[Range(0.01f, 20.0f)][SerializeField] private float highJumpForce = 3f;

		//player
		private Vector2 startPosition;
		private Rigidbody2D rigidBody;

		bool isCollisionEnabled = true;
		bool isPlayerMovementEnabled = true;

		int keysNumber = 3;
		int keysFound = 0;

		//for ground detection
		public LayerMask groundLayer;
		private const float rayLength = 1.5f;

		//animations
		private Animator animator;
		private bool isWalking;
		private bool isFacingRight = true;

		//dashing
		private bool isDashing = false;
		private bool canDash = true;
		public float dashingPower = 20f;
		private float dashingTime = 0.3f;
		private float dashingCooldown = 1.2f;
		private bool dashed = false;

		//jumping
		private float coyoteTime = 0.2f;
		private float coyoteTimeCounter;
		public float maxJumpTime = 0.2f; // Maximum time for short jump
		private float jumpTime = 0f;
		private bool canDoubleJump = false;
		private bool isMaxJumpUsed = true;
		private bool wasGrounded = true;

		//corner correction
		[Header("Corner Correction Variables")]
		[SerializeField] private float topRaycastLength;
		[SerializeField] private Vector3 edgeRaycastOffset;
		[SerializeField] private Vector3 innerRaycastOffset;
		[SerializeField] private LayerMask cornerCorrectLayer;

		//wall jumping and wall sliding
		private bool isWallSliding;
		[SerializeField] private float wallSlidingSpeed = 2f;
		private bool isWallJumping;
		private float wallJumpingDirection;
		[SerializeField] private float wallJumpingTime = 0.35f;
		private float wallJumpingCounter;
		[SerializeField] private float wallJumpingDuration = 0.4f;
		[SerializeField] private float wallJumpingPower;
		[SerializeField] private Transform wallCheck;
		[SerializeField] private LayerMask wallLayer;

		public float slopeThreshold = 45f;  // The maximum slope angle the player can walk on

		public float fallMultiplier = 2.5f;

		public float maxFallSpeed = -10f;

		//particle effects
		public ParticleSystem dust;

		void Awake()
		{
			startPosition = transform.position;
			rigidBody = GetComponent<Rigidbody2D>();
			animator = GetComponent<Animator>();
		}

		void DetectSlope()
		{
			RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.3f, this.groundLayer);

			if (hit.collider != null)
			{
				// Calculate the slope angle using the normal of the hit surface
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

				if (slopeAngle > 0 && slopeAngle <= slopeThreshold)
				{
					//Debug.Log("Slope detected with angle: " + slopeAngle);

					if (Input.GetKey(KeyCode.Space))
					{
						rigidBody.bodyType = RigidbodyType2D.Dynamic;
					}
					else if (!Input.anyKey || (Input.anyKey && Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0))
					{
						// Make the Rigidbody static if there is no input
						rigidBody.bodyType = RigidbodyType2D.Static;
						rigidBody.velocity = new Vector2(0, 0);
						//rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
						animator.SetBool("isFalling", false);
					}
					else
					{
						// Make the Rigidbody dynamic if there is input
						rigidBody.bodyType = RigidbodyType2D.Dynamic;
						//rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
					}
				}
			}
			else
			{
				rigidBody.bodyType = RigidbodyType2D.Dynamic;
			}
		}

		// Update is called once per frame
		void Update()
		{
			/*//if dashing, don't do anything else
			if (isDashing)
			{
				return;
			}
			isWalking = false; //remove walking 

			//apply gravity multiplier when falling
			if (rigidBody.velocity.y < 0 && !dashed)
			{
				rigidBody.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
			}

			// Check if the downward velocity exceeds the maximum fall speed
			if (rigidBody.velocity.y < maxFallSpeed)
			{
				rigidBody.velocity = new Vector2(rigidBody.velocity.x, maxFallSpeed);
			}

			//update coyote time
			if (isGrounded())
			{
				if (dashed) dashed = false;
				//play landing sound when player lands
				if (!wasGrounded && rigidBody.velocity.y <= 0)
				{
					AudioManager.instance.PlaySFX(SoundEffect.landingSound);
				}

				coyoteTimeCounter = coyoteTime;
				if (!Input.GetButton("Jump"))
				{
					canDoubleJump = false;
				}
			}
			else
			{
				coyoteTimeCounter -= Time.deltaTime;
			}
			wasGrounded = isGrounded();
*/
			handleInput();

			/*wallSlide();
			wallJump();*/

			setAnimation();
			/*
						cornerCorrect();

						DetectSlope();*/
		}

		void cornerCorrect()
		{
			if (CheckCornerCollisions())
			{
				CornerCorrect(rigidBody.velocity.y);
			}
		}


		//function to handle input
		void handleInput()
		{
			//hud input
			if (GameManager.instance.currentGameState == GameState.GS_PAUSEMENU)
			{
				if (Input.GetKeyDown(KeyCode.Escape))
				{
					AudioManager.instance.GuiPlayResume();
					GameManager.instance.InGame();
				}
				EventSystem.current.SetSelectedGameObject(null);

				return;
			}
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (GameManager.instance.currentGameState != GameState.GS_PAUSEMENU)
				{
					AudioManager.instance.GuiPlayResume();
					GameManager.instance.PauseMenu();
				}
				EventSystem.current.SetSelectedGameObject(null);

				return;
			}


			//player input (disabled when dead);
			if (isPlayerMovementEnabled)
			{

				float horizontalInput = Input.GetAxis("Horizontal");
				float verticalInput = Input.GetAxis("Vertical");

				// Move the player based on input
				Vector2 movement = new Vector2(horizontalInput, 0.0f);
				rigidBody.velocity = new Vector2(movement.x * moveSpeed, rigidBody.velocity.y);

				// Check if the player is walking
				isWalking = Mathf.Abs(horizontalInput) > 0.1f;

				// Flip the player's sprite if necessary
				if (horizontalInput > 0 && !isFacingRight || horizontalInput < 0 && isFacingRight)
				{
					flip();
				}

				//handling jumping with double jump
				if (Input.GetButtonDown("Jump"))
				{
					if (coyoteTimeCounter > 0f || canDoubleJump)
					{
						if (dashed) dashed = false;
						//play jump sound from player sound source
						AudioManager.instance.PlaySFX(SoundEffect.jumpSound);
						CreateDust();
						isMaxJumpUsed = false;
						rigidBody.velocity = new Vector2(0, jumpForce);
						jumpTime = 0f;
						canDoubleJump = !canDoubleJump;
					}
				}

				//handling long jump
				if (Input.GetKey(KeyCode.Space) && !isMaxJumpUsed && !isWallSliding)
				{
					jumpTime += Time.deltaTime;
					if (jumpTime < maxJumpTime)
					{
						rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce);
					}
					else
					{
						rigidBody.velocity = new Vector2(rigidBody.velocity.x, highJumpForce);
						isMaxJumpUsed = true;
					}
				}

				if (Input.GetButtonUp("Jump")) //after releasing jump button, player can't jump again
				{
					isMaxJumpUsed = true;
					coyoteTimeCounter = 0f;
				}

				if (Input.GetKeyDown(KeyCode.LeftShift))
				{
					horizontalInput = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) ? 1f :
							(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) ? -1f : 0f);

					verticalInput = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) ? 1f :
										  (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) ? -1f : 0f);

					// Set the dash direction based on the input
					Vector2 dashDirection = new Vector2(horizontalInput, verticalInput);

					// Only dash if there's a non-zero direction
					if (dashDirection != Vector2.zero)
					{
						StartCoroutine(Dash(dashDirection));
					}
				}


			}
		}

		void CreateDust()
		{
			if (!dust.IsAlive())
			{
				dust.Play();
			}
		}

		//set animations
		void setAnimation()
		{
			//Debug.Log(isGrounded() && rigidBody.velocity.y <= 0);
			Debug.Log(rigidBody.velocity.y);
			animator.SetBool("isGrounded", isGrounded() && rigidBody.velocity.y <= 0);
			animator.SetBool("isWalking", isWalking);

			//set falling animation
			if (rigidBody.velocity.y < -0.5f)
			{
				animator.SetBool("isFalling", true);
			}
			else
			{
				animator.SetBool("isFalling", false);
			}
		}

		//function to handle collisions
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (isCollisionEnabled)
			{

				if (other.CompareTag("Cherry"))
				{
					AudioManager.instance.PlaySFX(SoundEffect.cherryPickSound);
					GameManager.instance.AddPoints(10);
					other.gameObject.SetActive(false);
				}
				else if (other.CompareTag("Key"))
				{
					AudioManager.instance.PlaySFX(SoundEffect.keyPickUpSound);
					keysFound += 1;
					GameManager.instance.AddKey(other.gameObject.name);
					other.gameObject.SetActive(false);

				}
				else if (other.CompareTag("Enemy"))
				{
					//if player boxCollider bottom y is > than player y, kill enemy
					BoxCollider2D playerCollider = GetComponent<BoxCollider2D>();

					//find highest point of enemy collider
					float y = float.MinValue;
					foreach (Collider2D collider in other.GetComponentsInChildren<Collider2D>())
					{
						if (collider.bounds.max.y > y)
						{
							y = collider.bounds.max.y;
						}
					}

					//if player bottom y collider + 0.45f > enemy highest y collider player beats enemy
					if (playerCollider.bounds.min.y + 0.45f > y)
					{
						//Bounce off enemy
						this.rigidBody.velocity = new Vector2(0, 0);
						rigidBody.velocity = new Vector2(rigidBody.velocity.x, highJumpForce);

						//Unlock ability to jump
						coyoteTimeCounter = float.PositiveInfinity;
						canDoubleJump = false;
					}
					else
					{
						Death();
					}
				}
				else if (other.CompareTag("Heart"))
				{
					GameManager.instance.AddHP();
				}
				else if (other.CompareTag("FallLevel"))
				{
					Death();
				}
				else if (other.CompareTag("MovingPlatform"))
				{
					transform.SetParent(other.transform);
				}
				else if (other.CompareTag("EndLevel") && keysFound == keysNumber)
				{
					GameManager.instance.AddPoints(100 * GameManager.instance.getHp());
					GameManager.instance.LevelCompleted();
				}
				else if (other.CompareTag("Spike"))
				{
					Death();
				}
				else if (other.CompareTag("CheckPoint"))
				{
					AudioManager.instance.PlaySFX(SoundEffect.checkPointSound);
					startPosition = other.transform.position;
					BoxCollider2D boxCollider = other.GetComponent<BoxCollider2D>();
					boxCollider.enabled = false;
				}
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.CompareTag("MovingPlatform"))
			{
				transform.SetParent(null);
			}
		}

		//flip player model
		private void flip()
		{
			CreateDust();
			isFacingRight = !isFacingRight;
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}

		//function to check if player is on ground
		public bool isGrounded()
		{
			if (isFacingRight)
			{
				Vector3 pos = this.transform.position;

				if (Physics2D.Raycast(pos, Vector2.down, rayLength, groundLayer.value))
				{
					return true;
				}

				pos.x += 0.3f;
				if (Physics2D.Raycast(pos, Vector2.down, rayLength, groundLayer.value))
				{
					return true;
				}

				pos = this.transform.position;
				pos.x -= 0.45f;

				if (Physics2D.Raycast(pos, Vector2.down, rayLength, groundLayer.value))
				{
					return true;
				}
			}
			else
			{
				Vector3 pos = this.transform.position;

				if (Physics2D.Raycast(pos, Vector2.down, rayLength, groundLayer.value))
				{
					return true;
				}

				pos.x += 0.45f;
				if (Physics2D.Raycast(pos, Vector2.down, rayLength, groundLayer.value))
				{
					return true;
				}

				pos = this.transform.position;
				pos.x -= 0.3f;

				if (Physics2D.Raycast(pos, Vector2.down, rayLength, groundLayer.value))
				{
					return true;
				}
			}

			return false;
		}

		private bool isWalled()
		{
			Collider2D overlapCollider = Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);

			// Check if there is an overlap and the collider has the "Wall" tag
			return overlapCollider != null && overlapCollider.CompareTag("Wall");
		}

		private void wallSlide()
		{
			if (isWalled() && !isGrounded())
			{
				isWallSliding = true;
				canDoubleJump = false; // disable double jump when wall sliding
				rigidBody.velocity = new Vector2(rigidBody.velocity.x, Mathf.Clamp(rigidBody.velocity.y, -wallSlidingSpeed, float.MaxValue));
			}
			else
			{
				isWallSliding = false;
			}
		}

		private void wallJump()
		{
			if (isWallSliding)
			{
				isWallJumping = false;
				wallJumpingDirection = -transform.localScale.x;
				wallJumpingCounter = wallJumpingTime;

				CancelInvoke(nameof(StopWallJumping));
			}
			else
			{
				wallJumpingCounter -= Time.deltaTime;
			}

			if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
			{
				AudioManager.instance.PlaySFX(SoundEffect.jumpSound);
				isWallJumping = true;
				animator.SetBool("isGrounded", false);
				isWallJumping = true;
				rigidBody.velocity = new Vector2(wallJumpingDirection * wallJumpingPower, highJumpForce);
				wallJumpingCounter = 0f;

				if (transform.localScale.x != wallJumpingDirection)
				{
					flip();
				}

				Invoke(nameof(StopWallJumping), wallJumpingDuration);
			}
		}

		private void StopWallJumping()
		{
			if (!isDashing)
			{
				rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
			}
			isWallJumping = false;
		}

		IEnumerator Dash(Vector2 direction)
		{
			if (canDash)
			{
				rigidBody.velocity = Vector2.zero;
				AudioManager.instance.PlaySFX(SoundEffect.dashSound);
				canDoubleJump = false;
				if (isGrounded())
				{
					canDoubleJump = !canDoubleJump;
				}
				float orgGravity = rigidBody.gravityScale;
				rigidBody.gravityScale = 0f;
				canDash = false;
				isDashing = true;
				dashed = true;

				// Calculate the dash velocity based on the input direction
				Vector2 dashVelocity;
				Vector2 dashDirection = direction.normalized;

				if (dashDirection == Vector2.up)
				{
					dashVelocity = Vector2.up * dashingPower / 1.5f;
				}
				else if (dashDirection == Vector2.down)
				{
					dashVelocity = Vector2.down * dashingPower / 1.5f;
				}
				else
				{
					dashVelocity = dashDirection * dashingPower / 1.2f;
				}

				rigidBody.velocity = dashVelocity;

				yield return new WaitForSeconds(dashingTime);

				rigidBody.gravityScale = orgGravity;
				rigidBody.velocity = Vector2.zero;
				isDashing = false;

				yield return new WaitForSeconds(dashingCooldown);

				canDash = true;
			}
		}


		//function to handle player death
		void Death()
		{
			AudioManager.instance.PlaySFX(SoundEffect.playerDeathSound);

			//disable player movement and collisions
			isCollisionEnabled = false;
			isPlayerMovementEnabled = false;
			//lose hp
			GameManager.instance.LoseHP();
			//freeze player
			rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
			rigidBody.isKinematic = true;
			//play death animation
			animator.SetBool("isDead", true);
			StartCoroutine(KillOnAnimationEnd());
		}

		//function to handle player death animation and respawn
		private IEnumerator KillOnAnimationEnd()
		{
			yield return new WaitForSeconds(1.0f);
			//disable death animation 
			animator.SetBool("isDead", false);
			//unfreeze player and reset position
			rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
			rigidBody.isKinematic = false;
			rigidBody.velocity = new Vector2(0, 0);
			transform.position = startPosition;
			//reenable player movement and collisions
			isCollisionEnabled = true;
			isPlayerMovementEnabled = true;
		}

		//Raycast to check for corner collisions
		void CornerCorrect(float Yvelocity)
		{
			//Push player to the right
			RaycastHit2D hit = Physics2D.Raycast(transform.position - innerRaycastOffset + Vector3.up * topRaycastLength, Vector3.left, topRaycastLength, cornerCorrectLayer);
			if (hit.collider != null)
			{
				float newPos = Vector3.Distance(new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.up * topRaycastLength,
					transform.position - edgeRaycastOffset + Vector3.up * topRaycastLength);
				transform.position = new Vector3(transform.position.x + newPos, transform.position.y, transform.position.z);
				rigidBody.velocity = new Vector2(rigidBody.velocity.x, Yvelocity);
				return;
			}

			//Push player to the left
			hit = Physics2D.Raycast(transform.position + innerRaycastOffset + Vector3.up * topRaycastLength, Vector3.right, topRaycastLength, cornerCorrectLayer);
			if (hit.collider != null)
			{
				float newPos = Vector3.Distance(new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.up * topRaycastLength,
					transform.position + edgeRaycastOffset + Vector3.up * topRaycastLength);
				transform.position = new Vector3(transform.position.x - newPos, transform.position.y, transform.position.z);
				rigidBody.velocity = new Vector2(rigidBody.velocity.x, Yvelocity);
			}
		}

		//check for Corner Collisions
		private bool CheckCornerCollisions()
		{
			return Physics2D.Raycast(transform.position + edgeRaycastOffset, Vector2.up, topRaycastLength, cornerCorrectLayer) &&
								!Physics2D.Raycast(transform.position + innerRaycastOffset, Vector2.up, topRaycastLength, cornerCorrectLayer) ||
								Physics2D.Raycast(transform.position - edgeRaycastOffset, Vector2.up, topRaycastLength, cornerCorrectLayer) &&
								!Physics2D.Raycast(transform.position - innerRaycastOffset, Vector2.up, topRaycastLength, cornerCorrectLayer);
		}

		//Draw lines for debugging
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;

			//Corner Check
			Gizmos.DrawLine(transform.position + edgeRaycastOffset, transform.position + edgeRaycastOffset + Vector3.up * topRaycastLength);
			Gizmos.DrawLine(transform.position - edgeRaycastOffset, transform.position - edgeRaycastOffset + Vector3.up * topRaycastLength);
			Gizmos.DrawLine(transform.position + innerRaycastOffset, transform.position + innerRaycastOffset + Vector3.up * topRaycastLength);
			Gizmos.DrawLine(transform.position - innerRaycastOffset, transform.position - innerRaycastOffset + Vector3.up * topRaycastLength);

			//Corner Distance Check
			Gizmos.DrawLine(transform.position - innerRaycastOffset + Vector3.up * topRaycastLength,
							transform.position - innerRaycastOffset + Vector3.up * topRaycastLength + Vector3.left * topRaycastLength);
			Gizmos.DrawLine(transform.position + innerRaycastOffset + Vector3.up * topRaycastLength,
							transform.position + innerRaycastOffset + Vector3.up * topRaycastLength + Vector3.right * topRaycastLength);

			Gizmos.color = Color.red;

			if (isFacingRight)
			{
				DrawRaycastGizmos(transform.position);
				DrawRaycastGizmos(transform.position + new Vector3(0.3f, 0f, 0f));
				DrawRaycastGizmos(transform.position - new Vector3(0.45f, 0f, 0f));
			}
			else
			{
				DrawRaycastGizmos(transform.position);
				DrawRaycastGizmos(transform.position + new Vector3(0.45f, 0f, 0f));
				DrawRaycastGizmos(transform.position - new Vector3(0.3f, 0f, 0f));
			}
		}

		public void Bounce(float forceX, float forceY)
		{
			AudioManager.instance.PlaySFX(SoundEffect.jumpSound);
			rigidBody.velocity = new Vector2(forceX, forceY);
			canDoubleJump = true;
		}

		void DrawRaycastGizmos(Vector3 position)
		{
			Gizmos.DrawLine(position, position + Vector3.down * rayLength);
		}

		public bool IsFacingRight()
		{
			return isFacingRight;
		}

		public Vector2 GetPosition()
		{
			return rigidBody.position;
		}
	}
}