using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EndbossController : MonoBehaviour
{
    public enum State { Idle, Chasing, Attacking }
    State currentState = State.Idle;

    public enum Phase { One, Two, Three }
    Phase currentPhase = Phase.One;

    public int lifepoints = 500;

    public Transform target;
    public float detectionRadius = 10;

    //Movement
    NavMeshAgent navMeshAgent;

    //Attack & Chasing
    public float attackDistance = 3.5f;
    public float stayAtDistance = 0f;
    public float maxFollowDistance = 100f;
    public float attackTimer = 3f; //time between each attack
    public float attackSpeed = 1f; //how fast the attack is

    float attackTime = 0;
    float targetRadius;

    //phase 1:
    bool chargeHit = false;

    //phase 2:
    public int phase2hp = 100;
    public Transform midPoint;
    public GameObject prefab;
    public float tbetweenSpikes = 0.3f;
    bool spikeHit = false;
    bool spiking = false;

    GameController gameController;

    void Start()
    {
        navMeshAgent = this.gameObject.GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        targetRadius = target.GetComponent<SphereCollider>().radius;



        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        if (gameController == null)
        {
            Debug.Log("cannot find Object with Tag GameController in " + gameObject.name);
        }

    }

    private void Update()
    {
        IdleChasingSwitch();
        PhaseCheck();
        Attack();

    }

    public void PhaseCheck()
    {
        if (currentPhase == Phase.One && lifepoints <= phase2hp)
        {
            //Boss Values in Phase 2:
            currentPhase = Phase.Two;
            stayAtDistance = 30;
            attackDistance = 50;
            attackTimer = 5;
            navMeshAgent.speed = 2f;
        }
    }

    bool timeToAttack()
    {
        //time to attack && in attack distance? 
        return currentState == State.Chasing && (Time.time > attackTime) && (target.position - transform.position).magnitude <= attackDistance;
    }

    void Attack()
    {
        //Debug.Log("Life: " + lifepoints);
        //resets the attack timer

        if (currentPhase == Phase.One && timeToAttack() && currentState != State.Attacking)
        {
            attackTime = Time.time + attackTimer;
            StartCoroutine(Charge());
        }
        else if (currentPhase == Phase.Two && timeToAttack() && !spiking)
        {
            attackTime = Time.time + attackTimer;
            StartCoroutine(Spikes(target.transform.position));
        }
    }

    void IdleChasingSwitch()
    {
        /*
         Switches Boss between Idle(out of range) and Chasing(in range)
         When chasing, then set destination to player
         */
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
    }
    void SetTargetDestination()
    {
        //Follow the Player to his position + attackDistance
        if (target != null && currentState == State.Chasing)
        {
            Vector3 dirOffset = ((target.position - transform.position).normalized) * (stayAtDistance - 0.5f);
            Vector3 targetDestination = target.position - dirOffset;

            navMeshAgent.SetDestination(targetDestination);
        }
    }

    private IEnumerator Charge()
    {
        //Coroutine for Attacking, while attacking the enemy does not follow the player
        currentState = State.Attacking;
        navMeshAgent.enabled = false;
        FindObjectOfType<AudioController>().Play("Charge");
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = target.position;

        float progress = 0;

        while (progress <= 0.5 && !chargeHit)
        {
            progress += Time.deltaTime * attackSpeed;
            float interpolation = ((-progress * progress) + progress) * 4; // -x^2 +x

            transform.position = Vector3.Lerp(startPosition, targetPosition, interpolation);
            yield return null;
        }

        chargeHit = false;
        currentState = State.Chasing;
        navMeshAgent.enabled = true;
    }

    private IEnumerator Spikes(Vector3 attackPosition)
    {
        Debug.Log("Start Attack in " + gameObject.name);
        spiking = true;
        Vector3 startPosition = transform.position + (target.transform.position - transform.position).normalized * 4;

        //Vector3 lastPosition = transform.position;

        while (!spikeHit)
        {
            Vector3 goIndirection = (target.transform.position - startPosition).normalized;
            startPosition = startPosition + (goIndirection * 2);

            InstanciateAtPosition(startPosition);
            FindObjectOfType<AudioController>().Play("EnemyShot");

            yield return new WaitForSeconds(tbetweenSpikes);
        }

        spiking = false;
        //currentState = State.Chasing;
        Debug.Log("End Attack in " + gameObject.name);
    }

    void InstanciateAtPosition(Vector3 position)
    {
        //offset position by half y scaling for correct positioning
        position.y = transform.position.y - (transform.localScale.y / 2);
        var spike = Instantiate(prefab, position, this.transform.rotation);
        Destroy(spike, 1.0f);
    }


    private void SubLifePoints(int i)
    {
        lifepoints -= i;
        EnemyDeath(); //checks lifepoints
    }

    private void EnemyDeath()
    {
        //called whenever the lifepoints fall to 0
        if (lifepoints <= 0)
        {
            currentState = State.Idle;
            Destroy(gameObject);
            Debug.Log("Endboss EnemyDeath()");
            gameController.LevelEndPointTrigger();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "BossPillar")
        {
            Debug.Log("HIT Pillar");
            ChargeHitPillar(collision);
        }

    }

    private void ChargeHitPillar(Collision pillar)
    {
        if (currentState == State.Attacking)
        {
            chargeHit = true; //stops the charge
            SubLifePoints(100);
            pillar.gameObject.SetActive(false);
            FindObjectOfType<AudioController>().Play("Event");
        }
    }

   

    private void SpikeHitBoss()
    {
        spikeHit = true;
        SubLifePoints(100);
        FindObjectOfType<AudioController>().Play("Event");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerProjectile")
        {
            Debug.Log("ENEMY HIT");
            SubLifePoints(1);
        }
        else if (other.gameObject.tag == "WaterSpike")
        {
            SpikeHitBoss();
        }


    }

}
