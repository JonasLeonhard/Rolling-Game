using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueFollowController : MonoBehaviour {
    public Transform target;
    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update () 
    {
        Vector3 lookpos = target.transform.position - transform.position;
        lookpos.y = 0;
        this.transform.rotation = Quaternion.LookRotation(lookpos, Vector3.up);
	}

}
