using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolinController : MonoBehaviour {
    public float jumpForce = 10f;
    public ParticleSystem jumpParticles;

    private void Start()
    {
        jumpParticles.Stop();
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TrampolinController add Force: " + gameObject.name);
        other.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        jumpParticles.Play();
    }

}

