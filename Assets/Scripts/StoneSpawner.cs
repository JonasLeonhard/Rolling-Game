using System.Collections;
using UnityEngine;

public class StoneSpawner : MonoBehaviour
{
    public enum State { Idle, Spawning }
    State currentState = State.Idle;

    public GameObject prefab;
    public float timebetween = 3f;
    public float destroyAfter = 10f;

    void Update()
    {
        if (currentState == State.Idle)
        {
            StartCoroutine(Spawner());
        }
    }

    void InstanciateAtPosition(Vector3 position)
    {
        //Instanciate Stone

        var stone = Instantiate(prefab, position, Random.rotation);
        Destroy(stone, destroyAfter);
    }

    private IEnumerator Spawner()
    {
        currentState = State.Spawning;
        InstanciateAtPosition(transform.position);
        yield return new WaitForSeconds(timebetween);
        currentState = State.Idle;
    }
}
