using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public enum State { Idle, Chasing, Attacking }
    State currentState = State.Idle;

    public int lifepoints = 1;

    public Transform target;
    public float detectionRadius = 10;

    //Movement
    NavMeshAgent navMeshAgent;

    //Attack & Chasing
    public float attackDistance = 3.5f;
    public float maxFollowDistance = 100f;
    public float attackTimer = 3f; //time between each attack
    public float attackSpeed = 1f; //how fast the attack is

    float attackTime = 0;
    float targetRadius;


    public bool isRanged = false;
    public bool isTrigger = false;
    public GameObject trigger;

    public GameObject projectile;

    private void Start()
    {
        navMeshAgent = this.gameObject.GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        targetRadius = target.GetComponent<SphereCollider>().radius;
       

        if (isRanged && projectile == null)
        {
            Debug.Log("Projectile not set in " + gameObject.name);
        }

        if (navMeshAgent == null)
        {
            Debug.Log("navMeshAgent not set in: EnemyController- " + gameObject.name);
        }
    }

    private void Update()
    {
        if (currentState == State.Idle && (target.position - transform.position).magnitude <= detectionRadius)
        {
            currentState = State.Chasing;
        }
        else if (currentState == State.Chasing && navMeshAgent != null)
        {
            SetTargetDestination();

            if ((target.position - transform.position).magnitude >= maxFollowDistance)
            {
                Debug.Log("Object out of maxFollowDistance: State Idle " + gameObject.name);
                currentState = State.Idle;
            }
        }

        //time to attack && distance check
        if (currentState == State.Chasing && (Time.time > attackTime) && (target.position - transform.position).magnitude <= attackDistance)
        {
            //Debug.Log("Attack Start");
            attackTime = Time.time + attackTimer;

            if (!isRanged)
            {
                StartCoroutine(Attack());
            }
            else if (isRanged)
            {
                StartCoroutine(RangedAttack());
            }
        }

    }



    void SetTargetDestination()
    {
        //Follow the Player to his position + attackDistance
        if (target != null && currentState == State.Chasing)
        {
            Vector3 dirOffset = ((target.position - transform.position).normalized) * (attackDistance - 0.5f);
            Vector3 targetDestination = target.position - dirOffset;

            navMeshAgent.SetDestination(targetDestination);
        }
    }

    private IEnumerator Attack()
    {
        //Coroutine for Attacking, while attacking the enemy does not follow the player
        currentState = State.Attacking;
        navMeshAgent.enabled = false;

        FindObjectOfType<AudioController>().Play("Charge");

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = target.position - ((target.position - transform.position).normalized) * targetRadius; //-attack offset by playeradius

        float progress = 0;

        while (progress <= 1)
        {
            progress += Time.deltaTime * attackSpeed;
            float interpolation = ((-progress * progress) + progress) * 4; // -x^2 +x

            transform.position = Vector3.Lerp(startPosition, targetPosition, interpolation);
            yield return null;
        }

        currentState = State.Chasing;
        navMeshAgent.enabled = true;
    }

    private IEnumerator RangedAttack()
    {
        //Coroutine for Ranged- Attacking, while attacking the enemy does not follow the player
        currentState = State.Attacking;
        navMeshAgent.enabled = false;

        FindObjectOfType<AudioController>().Play("EnemyShot");

        Debug.Log("ranged attack start");
        //create projectile:
        GameObject pr = Instantiate(projectile, transform.position, transform.rotation);
        Destroy(pr, 6f);

        //manip rb:
        Vector3 shootDir = (target.transform.position - transform.position).normalized;

        Rigidbody pRB = pr.GetComponent<Rigidbody>();
        pRB.AddForce(shootDir * attackSpeed, ForceMode.Impulse);

        yield return null;
        currentState = State.Chasing;
        navMeshAgent.enabled = true;
    }

    public void SubLifePoint()
    {
        lifepoints--;
        EnemyDeath(); //checks lifepoints
    }

    public void SubLifePoint(int i)
    {
        lifepoints -= i;
        EnemyDeath();
    }

    private void EnemyDeath()
    {
        //called whenever the lifepoints fall to 0
        if (lifepoints <= 0)
        {
            currentState = State.Idle;
            Destroy(gameObject);

            if(isTrigger)
            {
                trigger.SetActive(true);
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerProjectile")
        {
            Debug.Log("ENEMY HIT");
            SubLifePoint();
        }
        else if (other.gameObject.tag == "Player")
        {
            Debug.Log("ENEMY HIT");
            SubLifePoint();
        }

        //SubLifePoint(); //TESTING - LATER CALLED WHEN HIT
    }
}
