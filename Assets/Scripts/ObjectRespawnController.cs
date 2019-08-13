using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRespawnController : MonoBehaviour {

    Vector3 startPosition;

	void Start () {
        startPosition = this.transform.position;
	}

    public Vector3 GetStartPosition()
    {
        //used in DeathZone Enter, when falling off plattform
        return startPosition;
    }
}
