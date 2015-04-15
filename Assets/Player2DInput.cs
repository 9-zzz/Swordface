using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player2DController))] // Will automatically add the Player2DController.
public class Player2DInput : MonoBehaviour
{
  Player2DController controller;

  public float jumpHeight = 4;
  public float timeToJumpApex = 0.4f;
  float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
  public float moveSpeed = 6;

  float jumpVelocity;
  float gravity;
  float velocityXSmoothing;
  Vector3 velocity;

  //public bool facingRight = true;
  //public GameObject dustups;

  void Start()
  {
    controller = GetComponent<Player2DController>();

    gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
    jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    print("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);
  }

  void Update()
  {

    if (controller.collisions.above || controller.collisions.below) // Fixes accumulating gravity problem.
      velocity.y = 0;

    Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));

    //print(velocity.normalized);

    if (((Input.GetKeyDown(KeyCode.Space)) || (Input.GetKeyDown(KeyCode.UpArrow))) && controller.collisions.below)
    {
      velocity.y = jumpVelocity;
      //Instantiate(dustups, transform.position + (-15*Vector3.up), transform.rotation);
    }

    // ###### Short Hop
    // Maybe if only when falling? because this is constant on the y... bad.
    //if ((Input.GetKeyUp(KeyCode.Space) || (Input.GetKeyUp(KeyCode.UpArrow))) && (velocity.y != 0)) velocity.y /= 2;

    float targetVelocityX = input.x * moveSpeed;
    // For slower x smoothing in the air.
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
    velocity.y += gravity * Time.deltaTime; // Apply gravity.
    controller.Move(velocity * Time.deltaTime);

    // ###### Flip Dir
    /*
    if(input.x > 0 && !facingRight)
			Flip();
		else if(input.x < 0 && facingRight)
			Flip();
    */
  }

  // ###### Flip Dir
  void Flip ()
	{
		// Switch the way the player is labelled as facing.
		//facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

}