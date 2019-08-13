using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneCollisionController : MonoBehaviour
{

    public ParticleSystem explosion;
    public float pushStrenght = 140f;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Stone")
        {
            Debug.Log("Stone HIT OnCollisionEnter in " + gameObject.name);

            PushAway(collision);
            Destroy();
        }

    }

    private void PushAway(Collision collision)
    {
        Vector3 push = (collision.transform.position - transform.position);
        push.y = 0;
        push = push.normalized;
        collision.rigidbody.AddForce(push * pushStrenght, ForceMode.Impulse);

    }
    private void Destroy()
    {
        Instantiate(explosion, transform.position, transform.rotation); //gets destroy with script destroy by time
        Destroy(gameObject, 0);
    }
}
