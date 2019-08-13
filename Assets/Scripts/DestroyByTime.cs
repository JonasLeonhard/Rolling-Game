using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByTime : MonoBehaviour
{
    // the code in this Script is take from https://unity3d.com/learn/tutorials/projects/space-shooter/spawning-waves

    public float lifetime = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }


}
