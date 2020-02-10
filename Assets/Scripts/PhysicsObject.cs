using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour {

    public float minGroundNormalY = 0.65f;
    public float gravityModifier = 1f;

    protected Vector2 targetVelocity;
    protected bool grounded;
    protected Vector2 groundNormal;
    protected Vector2 velocity;
    protected Rigidbody2D body;
    protected ContactFilter2D contactFilter; // a set of parameters for filtering contact results.
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16]; //stores info for collisions
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;

    void OnEnable() // works when enabling script
    {
        body = GetComponent<Rigidbody2D>();
    }

	// Use this for initialization
	protected virtual void Start ()
    {
        contactFilter.useTriggers = false; // not check collisions with triggers
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer)); //setting layer mask for detecting collisons
        contactFilter.useLayerMask = true;
	}
	
	// Update is called once per frame
	protected virtual void Update ()
    {
        targetVelocity = Vector2.zero;
        ComputeVelocity();
	}

    protected virtual void ComputeVelocity()
    {

    }

    void FixedUpdate()
    {
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime; //increasing velocity while falling
        velocity.x = targetVelocity.x;
        grounded = false;
        Vector2 deltaPosition = velocity * Time.deltaTime;
        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x); //vector of moving perpendicular to ground normal
        Vector2 move = moveAlongGround * deltaPosition.x; //vector of moving on x-axis
        Movement(move, false); // horizontal movement
        move = Vector2.up*deltaPosition.y; //vector of moving on y-axis
        Movement(move, true); //vertical movement
    }

    void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude; //length of the move vector
        if(distance>minMoveDistance) //to not check collision every time, while standing 
        {
            int count = body.Cast(move, contactFilter,hitBuffer,distance+shellRadius); //casting collider in specific direction to check possible collision on next frame
            hitBufferList.Clear();
            for(int i = 0; i<count;i++) //automatically counting element of hitbuffer[]
            {
                hitBufferList.Add(hitBuffer[i]); //copying elements to the list
            }
            for(int i = 0;i<hitBufferList.Count;i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal;
                if(currentNormal.y>minGroundNormalY) //grounded
                {
                    grounded = true;
                    if(yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }
                float projection = Vector2.Dot(velocity, currentNormal); //произведение 2-ух векторов (косинус угла между ними?)
                if(projection<0) //векторы направлены в разные стороны
                {
                    velocity = velocity - projection * currentNormal; //cancel out a part of velocity that would be stopped by the collision
                    //colission with ceiling for example
                }
                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance; //??????
            }
        }
        body.position = body.position + move.normalized * distance;
    }
}
