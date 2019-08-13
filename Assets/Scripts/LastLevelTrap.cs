using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastLevelTrap : MonoBehaviour
{

    public GameObject enemy1;
    public GameObject enemy2;
    public GameObject enemy3;
    public GameObject enemy4;
    public GameObject enemy5;
    public GameObject enemy6;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && allAssigned())
        {
            FindObjectOfType<AudioController>().Play("GameOver");
            enemy1.SetActive(true);
            enemy2.SetActive(true);
            enemy3.SetActive(true);
            enemy4.SetActive(true);
            enemy5.SetActive(true);
            enemy6.SetActive(true);
        }

    }

    private bool allAssigned()
    {
        return enemy1 != null && enemy2 != null && enemy3 != null && enemy4 != null && enemy5 != null && enemy6 != null;
    }
}
