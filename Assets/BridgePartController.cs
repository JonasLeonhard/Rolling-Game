using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgePartController : MonoBehaviour
{
    bool falling = false;
    Rigidbody rb;
    public float waitTime = 1;
    public ParticleSystem bridgeParticle;

    public float despawnTime = 10;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && falling == false)
        {
            StartCoroutine(goDown());
        }
    }
    IEnumerator goDown()
    {
        falling = true; // can just fall once
        Instantiate(bridgeParticle, transform);

        float progress = 0;
        while (progress <= 1)
        {
            progress += Time.deltaTime;
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.01f, transform.position.z);
            yield return null;
            Debug.Log(progress);
        }
        yield return new WaitForSeconds(0.5f);
        Falling();
    }
    public void Falling()
    {
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce(Vector3.up * 2);

        Destroy(gameObject, despawnTime);
        Destroy(bridgeParticle , despawnTime);
    }
}
