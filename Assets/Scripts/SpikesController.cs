using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesController : MonoBehaviour
{
    public enum State { Idle, Chasing, Attacking }
    State currentState = State.Idle;

    public int lifepoints = 1;

    public PlayerController target;

    public float detectionRadius = 15f;
    public float maxFollowDistance = 25f;

    //spikes creation:
    public GameObject prefab;
    float tbetweenSpikes = 0.3f;

    //attack
    public float attackTimer = 10f;
    float attackTime = 0;


    void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        //InstanciateAtPosition(this.transform.position);

    }

    void Update()
    {
        if (currentState == State.Idle && (target.transform.position - transform.position).magnitude <= detectionRadius)
        {
            currentState = State.Chasing;
        }
        if (currentState == State.Chasing && (target.transform.position - transform.position).magnitude >= maxFollowDistance)
        {
            Debug.Log("Object out of maxFollowDistance: State Idle " + gameObject.name);
            currentState = State.Idle;
        }
        //time to attack && distance check
        if (currentState == State.Chasing && (Time.time > attackTime) && (target.transform.position - transform.position).magnitude <= detectionRadius)
        {
            //Debug.Log("Attack Start");
            attackTime = Time.time + attackTimer;
            StartCoroutine(Attack(target.transform.position));
        }
    }

    void InstanciateAtPosition(Vector3 position)
    {
        //offset position by half y scaling for correct positioning
        position.y = transform.position.y - (transform.localScale.y / 2);
        var spike = Instantiate(prefab, position, this.transform.rotation);
        Destroy(spike, 1.0f);
    }

    private IEnumerator Attack(Vector3 attackPosition)
    {
        Debug.Log("Start Attack in " + gameObject.name);
        State lastState = currentState;
        currentState = State.Attacking;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = attackPosition;
        float progress = 0;
        while (progress <= 1)
        {
            progress += Time.deltaTime * 10;

            InstanciateAtPosition(Vector3.Lerp(startPosition, attackPosition, progress));
            FindObjectOfType<AudioController>().Play("EnemyShot");
            yield return new WaitForSeconds(tbetweenSpikes);
        }

        currentState = lastState;
        Debug.Log("End Attack in " + gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerProjectile")
        {
            subLifePoint();
        }
    }
   
    private void subLifePoint()
    {
        lifepoints--;

        if(lifepoints <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
