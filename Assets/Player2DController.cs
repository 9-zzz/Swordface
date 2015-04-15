using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Player2DController : MonoBehaviour
{

  public LayerMask collisionMask;

  const float skinWidth = 0.015f; // Can't change once set.

  public int horizontalRayCount = 4;
  public int verticalRayCount = 4;

  float horizontalRaySpacing;
  float verticalRaySpacing;

  float maxClimbAngle = 80;
  float maxDescendAngle = 75;
   
  RayCastOrigins raycastOrigins;
  BoxCollider2D collider;

  public CollisionInfo collisions;

  void Start()
  {
    collider = GetComponent<BoxCollider2D>();
    CalculateRaySpacing(); // Only need to do once, was in Update.
  }

  public void Move(Vector3 velocity)
  {
    UpdateRaycastOrigins();
    collisions.Reset(); // Blank slate each time you move.
    collisions.velocityOld = velocity;

    if (velocity.y < 0) // For descending slopes.
			DescendSlope(ref velocity);

    if (velocity.x != 0)
      HorizontalCollisions(ref velocity);

    if (velocity.y != 0)
      VerticalCollisions(ref velocity); // Doesn't make a copy of it, any change in the method will change this.

    transform.Translate(velocity);
  }

  void HorizontalCollisions(ref Vector3 velocity)
  {
    float directionX = Mathf.Sign(velocity.x);
    float rayLength = Mathf.Abs(velocity.x) + skinWidth;

    for (int i = 0; i < horizontalRayCount; i++)
    {
      // Cool!
      Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
      rayOrigin += Vector2.up * (horizontalRaySpacing * i);
      RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

      Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

      if (hit)
      {
        float slopeAngle = Vector2.Angle(hit.normal, Vector2.up); // Angle of ray perpendicular to x move and global up line.

        if (i == 0 && slopeAngle <= maxClimbAngle) // We are climbing a slope.
        {
          //print(slopeAngle);
          if(collisions.descendingSlope)
          {
            collisions.descendingSlope = false; // We are actually not descending anymore if we are climbing whilst descednign.
            velocity = collisions.velocityOld;
          }
          float distanceToSlopeStart = 0;
          if (slopeAngle != collisions.slopeAnlgleOld) // Starting to Climb a new slope.
          {
            distanceToSlopeStart = hit.distance - skinWidth;
            velocity.x -= distanceToSlopeStart * directionX;
          }
          ClimbSlope(ref velocity, slopeAngle);
            velocity.x += distanceToSlopeStart * directionX;
        }

        if ((!collisions.climbingSlope) || (slopeAngle > maxClimbAngle))
        {
          velocity.x = (hit.distance - skinWidth) * directionX;
          rayLength = hit.distance; // So it stays at the closest collision.

          if(collisions.climbingSlope)
          {
            velocity.y = Mathf.Tan(collisions.slopeAngle* Mathf.Deg2Rad) * Mathf.Abs(velocity.x); // For hitting stuff while going on slope.
          }

          collisions.left = directionX == -1; // If we've hit something and we go left, then this is true.
          collisions.left = directionX == 1;
        }
      }
    }
  }


  void VerticalCollisions(ref Vector3 velocity)
  {
    float directionY = Mathf.Sign(velocity.y);
    float rayLength = Mathf.Abs(velocity.y) + skinWidth;

    for (int i = 0; i < verticalRayCount; i++)
    {
      // Cool!
      Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
      rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
      RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

      Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

      if (hit) // Hit something above us.
      {
        velocity.y = (hit.distance - skinWidth) * directionY;
        rayLength = hit.distance; // So it stays at the closest collision.

        if (collisions.climbingSlope) // Hit something above us and climbing slope recalculate the velocity.y.
        {
          velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
        }

        collisions.below = directionY == -1; // If we've hit something and we go up, then this is true.
        collisions.above = directionY == 1;

      }
    }

    if (collisions.climbingSlope) // For changes in slope angle while climbing slopes.
    {
      float directionX = Mathf.Sign(velocity.x);
      rayLength = Mathf.Abs(velocity.x) + skinWidth;
      Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y; // If left or if right. casting from new height.
      RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

      if (hit)
      {
        float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
        if (slopeAngle != collisions.slopeAngle) // If we hit a new slope.
        {
          velocity.x = (hit.distance - skinWidth) * directionX; // Same as with horizontal collisions.
          collisions.slopeAngle = slopeAngle;
        }
      }
    }
  }

  void ClimbSlope(ref Vector3 velocity, float slopeAngle)
  {
    float moveDistance = Mathf.Abs(velocity.x);
    float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

    if (velocity.y <= climbVelocityY) // Assume we are jumping.
    {
      velocity.y = climbVelocityY;
      velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
      collisions.below = true; // To be able to jump while climbing a slope.
      collisions.climbingSlope = true;
      collisions.slopeAngle = slopeAngle;
    }
  }

  void DescendSlope(ref Vector3 velocity)
  {
    float directionX = Mathf.Sign(velocity.x);
    // Cast a ray down at either bottom right corner or right for movement.
    Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
    // Don't know how far down or what angles it is, cast to infinity down.
    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask); 

    if (hit)
    {
      float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
      if (slopeAngle != 0 && slopeAngle <= maxDescendAngle) // Don't care about flat surfs.
      {
        if (Mathf.Sign(hit.normal.x) == directionX) // To see if hit and direction are the same, moving in the same dir.
        {
          if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)) // Check if close enough to the slope to matter.
          {
            float moveDistance = Mathf.Abs(velocity.x);
            float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance; // Same from climbing.
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            velocity.y -= descendVelocityY;

            collisions.slopeAngle = slopeAngle;
            collisions.descendingSlope = true;
            collisions.below = true; // Assume we are grounded.
          }
        }
      }
    }
  }

  void UpdateRaycastOrigins()
  {
    Bounds bounds = GetComponent<BoxCollider2D>().bounds;
    bounds.Expand(skinWidth * -2);

    raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
    raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
    raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
    raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
  }


  void CalculateRaySpacing()
  {
    Bounds bounds = GetComponent<BoxCollider2D>().bounds;
    bounds.Expand(skinWidth * -2);

    // Never want it lower than 2.
    horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
    verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

    // Spaced out by size and number of.
    horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
    verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
  }

  struct RayCastOrigins
  {
    public Vector2 topLeft, topRight;
    public Vector2 bottomLeft, bottomRight;
  }

  public struct CollisionInfo
  {
    public bool above, below, left, right;

    public bool climbingSlope;
    public bool descendingSlope;
    public float slopeAngle, slopeAnlgleOld; // Previous frame slopeAngle.
    public Vector3 velocityOld;

    public void Reset()
    {
      above = below = left = right = false;
      climbingSlope = false;
      descendingSlope = false;
      slopeAnlgleOld = slopeAngle;
      slopeAngle = 0;
    }
  }
}