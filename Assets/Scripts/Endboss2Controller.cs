using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Endboss2Controller : MonoBehaviour
{

    public enum State { Idle, Chasing, Attacking }
    State currentState = State.Idle;

    public enum Phase { One, Two, Three }
    Phase currentPhase = Phase.One;

    public int lifepoints = 500;

    //Movement
    NavMeshAgent navMeshAgent;

    public Transform target;
    public float detectionRadius = 30;

    //Attack & Chasing
    float attackDistance = 30f;
    float stayAtDistance = 20f;
    float maxFollowDistance = 100f;
    float attackTimer = 3f; //time between each attack
    float attackSpeed = 1f; //how fast the attack is

    float attackTime = 0;


    GameController gameController;

    //phase 1:
    public GameObject projectile;
    Audio shot;

    //Attack & Chasing P1
    public float attackDistanceP1 = 30f;
    public float stayAtDistanceP1 = 20f;
    public float maxFollowDistanceP1 = 100f;
    public float attackTimerP1 = 3f; //time between each attack
    public float attackSpeedP1 = 15f; //how fast the attack is


    //phase 2:
    public float phase2Timer = 10f;
    public GameObject bossWindzone;
    public GameObject trampolin;
    bool coroutine = false;

    //Attack & Chasing P2
    public float attackDistanceP2 = 30f;
    public float stayAtDistanceP2 = 0f;
    public float maxFollowDistanceP2 = 100f;
    public float attackTimerP2 = 3f; //time between each attack
    public float attackSpeedP2 = 15f; //how fast the attack is

    void Start()
    {
        navMeshAgent = this.gameObject.GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        shot = FindObjectOfType<AudioController>().getAudio("EnemyShot");

        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        if (gameController == null)
        {
            Debug.Log("cannot find Object with Tag GameController in " + gameObject.name);
        }

    }

    // Update is called once per frame
    private void Update()
    {
        IdleChasingSwitch();
        PhaseCheck();
        Attack();
    }

    public void PhaseCheck()
    {
        if (currentPhase == Phase.One)
        {
            //Boss Values in Phase 1:
            attackDistance = attackDistanceP1;
            stayAtDistance = stayAtDistanceP1;
            maxFollowDistance = maxFollowDistanceP1;
            attackTimer = attackTimerP1;//time between each attack
            attackSpeed = attackSpeedP1;
        }
        else if (currentPhase == Phase.Two)
        {
            //Boss Values in Phase 2:
            attackDistance = attackDistanceP2;
            stayAtDistance = stayAtDistanceP2;
            maxFollowDistance = maxFollowDistanceP2;
            attackTimer = attackTimerP2;//time between each attack
            attackSpeed = attackSpeedP2;

            navMeshAgent.speed = 5f;

            //
            if(!coroutine)
            StartCoroutine(Phase2(phase2Timer));
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
            StartCoroutine(RangedAttack()); //attack phase 1
        }
        else if (currentPhase == Phase.Two && timeToAttack() && currentState != State.Attacking)
        {
            attackTime = Time.time + attackTimer;
            //StartCoroutine(Spikes(target.transform.position)); //attack phase 2
        }
    }

    private IEnumerator RangedAttack()
    {
        //Coroutine for Ranged- Attacking, while attacking the enemy does not follow the player
        currentState = State.Attacking;
        navMeshAgent.enabled = false;

        if (shot != null)
            shot.source.Play();

        Debug.Log("ranged attack start");
        //create projectile:
        GameObject pr = Instantiate(projectile, transform.position, transform.rotation);
        Destroy(pr, 6f);

        //manip rb:
        Vector3 shootDir = (target.transform.position - transform.position);
        shootDir.y = 0;
        shootDir = shootDir.normalized;

        Rigidbody pRB = pr.GetComponent<Rigidbody>();
        pRB.AddForce(shootDir * attackSpeed, ForceMode.Impulse);


        yield return null;
        currentState = State.Chasing;
        navMeshAgent.enabled = true;
    }

    private IEnumerator Phase2(float t)
    {
        //start phase 2
        yield return new WaitForSeconds(0.5f);
        coroutine = true;
        Debug.Log("PHASE 2 CORU. CHANGE TO PHASE 2");
        currentPhase = Phase.Two;
        bossWindzone.SetActive(true);
        trampolin.SetActive(false);
        yield return new WaitForSeconds(t);

        Debug.Log("PHASE 2 CORU. CHANGE BACK");
        //change phase back
        currentPhase = Phase.One;
        bossWindzone.SetActive(false);
        trampolin.SetActive(true);
        coroutine = false;


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

    public void SubLifePoints(int i)
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

    public void TriggerPhase2()
    {
        //called in Enboss2_TrampolinTriggerController, phase check starts coroutine
        currentPhase = Phase.Two;

    }
}
