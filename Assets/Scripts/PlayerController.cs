using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public enum State { Iso, Follow }
    State currentState = State.Follow;

    //lifepoints-
    public GameController gameController;

    //movement:
    public float MovementSpeed = 4f; //called in movement to change the amout of force applied
    static Vector3 lastDirection;
    public float maxMoveSpeed = 10f;
    private Vector3 multForce = new Vector3(40, 1, 40);

    //dashing
    private bool dashing = false;
    public float dashCD = 2; //when changing - change the particlesys.lengt!
    float dashStrength = 250f;

    //jumping:
    public float jumpHeight = 4f;
    public float gravityInc = 3f;
    public float maxJumpVelocity = 10f;

    //attacking
    public GameObject playerProjectile;
    public float attackSpeed = 14f;
    public float timebetweenAttacks = 0.4f;
    bool attacking = false;

    //physics
    Rigidbody rb;

    //particle
    public ParticleSystem hitParticleSystem;
    public ParticleSystem dashParticleSystem;

    //collision:
    private bool isGrounded = false;
    Collider collider;

    //camera for inline movement:
    Transform cameraTrans;//instanciated in inline view

    //checkpoint and death:
    public Vector3 lastCheckPoint;
    public Vector3 startPoint;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        cameraTrans = Camera.main.transform; //assign cameratrans
        collider = GetComponent<SphereCollider>();
        hitParticleSystem.Stop();
        dashParticleSystem.Stop();



        if (playerProjectile == null)
            Debug.Log("projectile not set in " + gameObject.name);
    }

    private void FixedUpdate()
    {
        //just before physics calc:
        //move adds force to rb
        //inlineView keeps the added Input Force base on Camera Directions
        //movement applies basic movements horizontal + vertical
        if (currentState == State.Follow)
        {
            Move(InlineView(Movement()), MovementSpeed);
        }
        else if (currentState == State.Iso)//not inline with camera movement
        {
            Move((Movement()), MovementSpeed);
        }

        Dashing();
        Attack();

        if (isGrounded)//can only jump on ground objects
        {
            rb.AddForce(Jump() * jumpHeight, ForceMode.Impulse);
        }
        if (!isGrounded)
        {
            FasterFalling(); //while falling
        }
        if (!isGrounded && transform.position.y < -1000)
        {
            print("deathreset: too low!");
            ResetToStartOrLastCheckpoint();
        }
    }

    private void LateUpdate()
    {
        CameraStateChanger();
    }

    private Vector3 Movement()
    {

        //calculates Basic Movement -Input= Axis
        //normalizes it to get only direction
        float moveX = Input.GetAxis("Horizontal");
        //float moveY = Input.GetAxis("Jump");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 mMove = new Vector3(moveX, 0, moveZ);

        if (mMove.magnitude > 1)
        {
            mMove.Normalize();
        }
        return mMove;
    }

    private Vector3 Jump()
    {
        //handles player jumps:
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FindObjectOfType<AudioController>().Play("Jump");
        }

        float moveY = Input.GetAxis("Jump");
        Vector3 mMove = new Vector3(0, moveY * jumpHeight, 0); //y to jump up
        return mMove;
    }

    private void FasterFalling()
    {
        //Gravity Control:
        //increases the falling speed of the player to make a jump feel more fluid
        //2 states - low jump and high jump when holding jump

        //the code inside this method fasterFalling() is from https://www.youtube.com/watch?v=7KiK0Aqtmzc&t=4s, "Better Jumping with 4 Lines of Code"

        if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            //print("fasterfalling no hold");
            rb.velocity += Vector3.up * Physics.gravity.y * (gravityInc * 2) * Time.deltaTime; //faster falling
        }
        else if (rb.velocity.y < 0)
        {
            //print("fasterfalling hold");
            rb.velocity += Vector3.up * Physics.gravity.y * (gravityInc - 1) * Time.deltaTime; //faster falling
        }
    }

    private Vector3 LimitMaxSpeed(Vector3 moveSpeed)
    {
        //basic idea from https://answers.unity.com/questions/683158/how-to-limit-speed-of-a-rigidbody.html

        if (moveSpeed.magnitude >= maxMoveSpeed)
        {
            moveSpeed = moveSpeed.normalized * maxMoveSpeed;
        }

        //+limit maxvelocity
        //keep old velocity
        var velocityYcurrent = rb.velocity.y;

        //cap speed without affecting y velocity
        var velocityLimit = rb.velocity.normalized * maxMoveSpeed; ;
        velocityLimit.y = velocityYcurrent;

        //limit max velocity
        if (rb.velocity.magnitude > maxMoveSpeed)
        {
            rb.velocity = velocityLimit;
        }

        return moveSpeed;
    }

    public void Move(Vector3 direction, float strength)
    {
        LimitMaxSpeed(direction);
        rb.AddForce(direction * strength, ForceMode.Impulse);
    }

    private Vector3 InlineView(Vector3 movementVector)
    {
        //bring movementVector inline with Camera view:
        Vector3 dir = cameraTrans.TransformDirection(movementVector);
        dir.y = 0; //no impact on jumping
        return dir;
    }

    private void CameraStateChanger()
    {
        //changes the player to move inline with the camera or not iso/ follow camera
        if (Input.GetKeyDown("-"))
        {
            if (currentState == State.Follow)
            {
                currentState = State.Iso;
            }
            else if (currentState == State.Iso)
            {
                currentState = State.Follow;
            }
            Debug.Log("PlayerController: CameraStateChanger: " + currentState.ToString());
        }
    }

    private void Dashing()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !dashing)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        dashing = true;
        dashParticleSystem.Play();

        FindObjectOfType<AudioController>().Play("Dash");


        Vector3 dashDirection = InlineView(Movement()).normalized;
        if (dashDirection.magnitude <= 0)
        {
            dashDirection = cameraTrans.forward;
            dashDirection.y = 0;
        }

        rb.AddForce(dashDirection * dashStrength, ForceMode.Impulse);

        yield return new WaitForSeconds(dashCD);
        dashing = false;
    }
    void OnCollisionEnter(Collision collision)
    {
        //parts of contact points taken from https://answers.unity.com/questions/281430/how-to-specify-a-a-rigid-body-colliding-with-the-b.html
        var contact = collision.contacts[0].normal;

        //when entering collission with Ground tagged objects and when contact point is on the bottom side
        if (collision.gameObject.tag == "Ground" && contact.y > 0)
        {
            isGrounded = true;
        }

        if (collision.gameObject.tag == "Stone")
        {
            Debug.Log("Stone hit by Player");
            gameController.SubLifePoint(1);
        }

    }

    private void OnCollisionStay(Collision collision)
    {
        //parts of contact points taken from https://answers.unity.com/questions/281430/how-to-specify-a-a-rigid-body-colliding-with-the-b.html
        var contact = collision.contacts[0].normal;

        if (collision.gameObject.tag == "Ground" && contact.y > 0)
        {
            isGrounded = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {


        if (gameController == null)
        {
            Debug.Log("instance of gameController not attached to gameobject: " + gameObject.name);
        }
        else
        {
            if (other.gameObject.tag == "Projectile")
            {
                hitParticleSystem.Play();
                gameController.SubLifePoint(1);
                Destroy(other.gameObject, 0);
                //Debug.Log("DESTROY other: " + other.name);
                //Debug.Log("PARTICLE EMIT" + particleSystem.name);
            }
            else if (other.gameObject.tag == "Enemy")
            {
                hitParticleSystem.Play();
                gameController.SubLifePoint(1);
            }
        }
    }

    public void SetPosition(Vector3 position)
    {
        Debug.Log("Set Position Player +" + position.ToString());

        transform.position = position;
    }

    public void instantDeathZoneEnter()
    {
        //triggered by DeathZoneController:
        Debug.Log("DEATHZONE INSTANT DEATH");
        ResetToStartOrLastCheckpoint();
    }

    public void ResetToStartOrLastCheckpoint()
    {
        this.SetPosition(lastCheckPoint);
    }

    private void Attack()
    {
        if ((Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Tab)) && !attacking)
        {
            StartCoroutine(RangedAttack());
        }
    }
    private IEnumerator RangedAttack()
    {
        attacking = true;
        //Coroutine for Ranged- Attacking

        FindObjectOfType<AudioController>().Play("PlayerShot");
        Debug.Log("RangedAttack");

        //create projectile:
        Vector3 attackDirection = cameraTrans.forward;
        attackDirection.y = 0;


        GameObject pr = Instantiate(playerProjectile, transform.position, Quaternion.Euler(attackDirection));
        Destroy(pr, 6f);

        //manip rb:

        Rigidbody pRB = pr.GetComponent<Rigidbody>();
        pRB.AddForce(attackDirection * attackSpeed, ForceMode.Impulse);

        yield return new WaitForSeconds(timebetweenAttacks);
        attacking = false;
    }
}
